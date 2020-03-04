using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Loqui;
using Loqui.Internal;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Internals;
using Mutagen.Bethesda.Oblivion.Internals;
using Noggog;
using Noggog.Utility;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class Worldspace
    {
        [Flags]
        public enum Flag
        {
            SmallWorld = 0x01,
            CantFastTravel = 0x02,
            OblivionWorldspace = 0x04,
            NoLODWater = 0x10,
        }

        private static readonly Worldspace.TranslationMask XmlFolderTranslation = new Worldspace.TranslationMask(true)
        {
            SubCells = new MaskItem<bool, WorldspaceBlock.TranslationMask?>(false, null),
            Road = new MaskItem<bool, Road.TranslationMask?>(false, null),
            TopCell = new MaskItem<bool, Cell.TranslationMask?>(false, null),
        };
        private static readonly TranslationCrystal XmlFolderTranslationCrystal = XmlFolderTranslation.GetCrystal();

        private static readonly WorldspaceBlock.TranslationMask BlockXmlFolderTranslation = new WorldspaceBlock.TranslationMask(true)
        {
            Items = new MaskItem<bool, WorldspaceSubBlock.TranslationMask?>(false, null),
            BlockNumberX = false,
            BlockNumberY = false,
        };
        private static readonly TranslationCrystal BlockXmlFolderTranslationCrystal = BlockXmlFolderTranslation.GetCrystal();

        private static readonly WorldspaceSubBlock.TranslationMask SubBlockXmlFolderTranslation = new WorldspaceSubBlock.TranslationMask(true)
        {
            BlockNumberX = false,
            BlockNumberY = false,
        };
        private static readonly TranslationCrystal SubBlockXmlFolderTranslationCrystal = SubBlockXmlFolderTranslation.GetCrystal();

        public static async Task<TryGet<Worldspace>> TryCreateXmFolder(
            DirectoryPath dir,
            ErrorMaskBuilder? errorMask)
        {
            var path = Path.Combine(dir.Path, $"{nameof(Worldspace)}.xml");
            if (!File.Exists(path))
            {
                return TryGet<Worldspace>.Failure;
            }
            var worldspaceNode = XDocument.Load(path).Root;
            var ret = CreateFromXml(
                node: worldspaceNode,
                errorMask: errorMask,
                translationMask: XmlFolderTranslationCrystal);
            var usingOffsetLenNode = worldspaceNode.Element("UsingOffsetLength");
            if (usingOffsetLenNode != null && usingOffsetLenNode.TryGetAttribute("value", out bool val))
            {
                ret.UsingOffsetLength = val;
            }
            var roadPath = Path.Combine(dir.Path, $"{nameof(Road)}.xml");
            if (File.Exists(roadPath))
            {
                ret.Road = Road.CreateFromXml(
                    roadPath,
                    translationMask: null,
                    errorMask: errorMask);
            }
            var topCellpath = Path.Combine(dir.Path, $"{nameof(TopCell)}.xml");
            if (File.Exists(topCellpath))
            {
                ret.TopCell = Cell.CreateFromXml(
                    topCellpath,
                    translationMask: null,
                    errorMask: errorMask);
            }
            var subCellsDir = new DirectoryPath(Path.Combine(dir.Path, $"SubCells"));
            if (!subCellsDir.Exists) return TryGet<Worldspace>.Succeed(ret);

            bool TryGetIndices(string str, out short x, out short y)
            {
                var split = str.Split(',');
                x = -1;
                y = -1;
                if (split.Length != 2) return false;
                int prefixIndex = split[0].IndexOf(" - (");
                if (prefixIndex == -1) return false;
                if (!split[1].EndsWith(")")) return false;
                split[0] = split[0].Substring(prefixIndex + 4).Trim();
                split[1] = split[1].Substring(0, split[1].Length - 1).Trim();
                if (split[0].Length < 2 || split[1].Length < 2) return false;
                if (!split[0].EndsWith("X")) return false;
                if (!split[1].EndsWith("Y")) return false;
                return short.TryParse(split[0].Substring(0, split[0].Length - 1), out x)
                    && short.TryParse(split[1].Substring(0, split[1].Length - 1), out y);
            }

            List<Task<WorldspaceBlock>> tasks = new List<Task<WorldspaceBlock>>();
            foreach (var blockDir in subCellsDir.EnumerateDirectories(includeSelf: false, recursive: false)
                .SelectWhere(d =>
                {
                    if (Mutagen.Bethesda.FolderTranslation.TryGetItemIndex(d.Name, out var index))
                    {
                        return TryGet<(int Index, DirectoryPath Dir)>.Succeed((index, d));
                    }
                    return TryGet<(int Index, DirectoryPath Dir)>.Failure;
                })
                .OrderBy(d => d.Index)
                .Select(d => d.Dir))
            {
                if (!TryGetIndices(blockDir.Name, out var blockX, out var blockY)) continue;
                tasks.Add(Task.Run(async () =>
                {
                    WorldspaceBlock wb = WorldspaceBlock.CreateFromXml(
                        path: Path.Combine(blockDir.Path, "Group.xml"),
                        errorMask: errorMask,
                        translationMask: BlockXmlFolderTranslation);
                    wb.BlockNumberX = blockX;
                    wb.BlockNumberY = blockY;

                    List<Task<WorldspaceSubBlock>> subTasks = new List<Task<WorldspaceSubBlock>>();
                    foreach (var subBlockFile in blockDir.EnumerateFiles()
                        .SelectWhere(d =>
                        {
                            if (Mutagen.Bethesda.FolderTranslation.TryGetItemIndex(d.Name, out var index))
                            {
                                return TryGet<(int Index, FilePath File)>.Succeed((index, d));
                            }
                            return TryGet<(int Index, FilePath File)>.Failure;
                        })
                        .OrderBy(d => d.Index)
                        .Select(d => d.File))
                    {
                        if (!TryGetIndices(subBlockFile.NameWithoutExtension, out var subBlockX, out var subBlockY)) continue;
                        subTasks.Add(Task.Run(() =>
                        {
                            WorldspaceSubBlock wsb = WorldspaceSubBlock.CreateFromXml(
                                path: subBlockFile.Path,
                                errorMask: errorMask,
                                translationMask: SubBlockXmlFolderTranslation);
                            wsb.BlockNumberX = subBlockX;
                            wsb.BlockNumberY = subBlockY;
                            return wsb;
                        }));
                    }
                    var subBlocks = await Task.WhenAll(subTasks).ConfigureAwait(false);
                    wb.Items = subBlocks.ToExtendedList();
                    return wb;
                }));
            }
            var blocks = await Task.WhenAll(tasks).ConfigureAwait(false);
            ret.SubCells = blocks.ToExtendedList();
            return TryGet<Worldspace>.Succeed(ret);
        }

        public override async Task WriteToXmlFolder(
            DirectoryPath dir, 
            string name, 
            XElement node, 
            int counter,
            ErrorMaskBuilder? errorMask)
        {
            dir = new DirectoryPath(Path.Combine(dir.Path, FolderTranslation.GetFileString(this, counter)));
            dir.Create();

            var worldspaceNode = new XElement("topnode");
            this.WriteToXml(
                name: name,
                node: worldspaceNode,
                errorMask: errorMask,
                translationMask: XmlFolderTranslationCrystal);
            worldspaceNode.Elements().First().SaveIfChanged(Path.Combine(dir.Path, $"{nameof(Worldspace)}.xml"));
            if (this.Road.TryGet(out var road))
            {
                road.WriteToXml(
                    path: Path.Combine(dir.Path, $"{nameof(Road)}.xml"),
                    errorMask: errorMask,
                    translationMask: null);
            }
            if (this.TopCell.TryGet(out var topCell))
            {
                topCell.WriteToXml(
                    path: Path.Combine(dir.Path, $"{nameof(TopCell)}.xml"),
                    errorMask: errorMask,
                    translationMask: null);
            }
            int blockCount = 0;
            List<Task> blockTasks = new List<Task>();
            foreach (var block in this.SubCells.TryIterate())
            {
                int blockStamp = blockCount++;
                blockTasks.Add(Task.Run(async () =>
                {
                    List<Task> subBlockTasks = new List<Task>();
                    var blockDir = new DirectoryPath(Path.Combine(dir.Path, $"SubCells/{blockStamp} - ({block.BlockNumberX}X, {block.BlockNumberY}Y)/"));
                    blockDir.Create();
                    int subBlockCount = 0;
                    block.WriteToXml(
                        Path.Combine(blockDir.Path, "Group.xml"),
                        errorMask: errorMask,
                        translationMask: BlockXmlFolderTranslationCrystal);
                    foreach (var subBlock in block.Items.TryIterate())
                    {
                        int subBlockStamp = subBlockCount++;
                        subBlockTasks.Add(Task.Run(() =>
                        {
                            subBlock.WriteToXml(
                                path: Path.Combine(blockDir.Path, $"{subBlockStamp} - ({subBlock.BlockNumberX}X, {subBlock.BlockNumberY}Y).xml"),
                                translationMask: SubBlockXmlFolderTranslationCrystal,
                                errorMask: errorMask);
                        }));
                    }
                    await Task.WhenAll(subBlockTasks).ConfigureAwait(false);
                }));
            }
            await Task.WhenAll(blockTasks).ConfigureAwait(false);
        }
    }

    namespace Internals
    {
        public partial class WorldspaceCommon
        {
            private static WorldspaceBlock.TranslationMask duplicateBlockCopyMask = new WorldspaceBlock.TranslationMask(true)
            {
                Items = new MaskItem<bool, WorldspaceSubBlock.TranslationMask?>(false, default)
            };

            private static WorldspaceSubBlock.TranslationMask duplicateSubBlockCopyMask = new WorldspaceSubBlock.TranslationMask(true)
            {
                Items = new MaskItem<bool, Cell.TranslationMask?>(false, default)
            };

            partial void PostDuplicate(Worldspace obj, Worldspace rhs, Func<FormKey> getNextFormKey, IList<(IMajorRecordCommon Record, FormKey OriginalFormKey)>? duplicatedRecords)
            {
                if (rhs.Road.TryGet(out var road))
                {
                    obj.Road = (Road)road.Duplicate(getNextFormKey, duplicatedRecords);
                }
                if (rhs.TopCell.TryGet(out var topCell))
                {
                    obj.TopCell = (Cell)topCell.Duplicate(getNextFormKey, duplicatedRecords);
                }
                obj.SubCells = rhs.SubCells.Select((block) =>
                {
                    var blockRet = new WorldspaceBlock();
                    blockRet.DeepCopyIn(block, duplicateBlockCopyMask);
                    blockRet.Items = block.Items.Select((subBlock) =>
                    {
                        var subBlockRet = new WorldspaceSubBlock();
                        subBlockRet.DeepCopyIn(subBlock, duplicateSubBlockCopyMask);
                        subBlockRet.Items = subBlock.Items.Select(c => (Cell)c.Duplicate(getNextFormKey, duplicatedRecords))
                            .ToExtendedList();
                        return subBlockRet;
                    }).ToExtendedList();

                    return blockRet;
                }).ToExtendedList();
            }
        }

        public partial class WorldspaceBinaryWriteTranslation
        {
            static partial void WriteBinaryOffsetLengthCustom(MutagenWriter writer, IWorldspaceGetter item, MasterReferences masterReferences)
            {
                if (!item.OffsetData.TryGet(out var offset)) return;
                if (!item.UsingOffsetLength) return;
                using (HeaderExport.ExportSubRecordHeader(writer, Worldspace_Registration.XXXX_HEADER))
                {
                    writer.Write(offset.Length);
                }
                writer.Write(Worldspace_Registration.OFST_HEADER.Type);
                writer.WriteZeros(2);
                writer.Write(offset);
            }

            static partial void WriteBinaryOffsetDataCustom(MutagenWriter writer, IWorldspaceGetter item, MasterReferences masterReferences)
            {
                if (item.UsingOffsetLength) return;
                if (!item.OffsetData.TryGet(out var offset)) return;
                using (HeaderExport.ExportSubRecordHeader(writer, Worldspace_Registration.OFST_HEADER))
                {
                    ByteArrayBinaryTranslation.Instance.Write(writer, offset);
                }
            }

            static partial void CustomBinaryEndExport(MutagenWriter writer, IWorldspaceGetter obj, MasterReferences masterReferences)
            {
                var road = obj.Road;
                var topCell = obj.TopCell;
                var subCells = obj.SubCells;
                if (subCells?.Count == 0
                    && road != null
                    && topCell != null) return;
                using (HeaderExport.ExportHeader(writer, Group_Registration.GRUP_HEADER, ObjectType.Group))
                {
                    FormKeyBinaryTranslation.Instance.Write(
                        writer,
                        obj.FormKey,
                        masterReferences);
                    writer.Write((int)GroupTypeEnum.WorldChildren);
                    writer.Write(obj.SubCellsTimestamp);

                    if (road != null)
                    {
                        road.WriteToBinary(
                            writer,
                            masterReferences: masterReferences);
                    }
                    if (topCell != null)
                    {
                        topCell.WriteToBinary(
                            writer,
                            masterReferences: masterReferences);
                    }
                    Mutagen.Bethesda.Binary.ListBinaryTranslation<IWorldspaceBlockGetter>.Instance.Write(
                        writer: writer,
                        items: subCells,
                        transl: (MutagenWriter subWriter, IWorldspaceBlockGetter subItem) =>
                        {
                            subItem.WriteToBinary(
                                writer: subWriter,
                                masterReferences: masterReferences);
                        });
                }
            }
        }

        public partial class WorldspaceBinaryCreateTranslation
        {
            static partial void FillBinaryOffsetLengthCustom(MutagenFrame frame, IWorldspaceInternal item, MasterReferences masterReferences)
            {
                item.UsingOffsetLength = true;
                var xxxxMeta = frame.MetaData.ReadSubRecord(frame);
                if (xxxxMeta.RecordType != Worldspace_Registration.XXXX_HEADER
                    || xxxxMeta.RecordLength != 4)
                {
                    throw new ArgumentException();
                }
                var contentLen = frame.Reader.ReadInt32();
                var ofstMeta = frame.MetaData.ReadSubRecord(frame);
                if (ofstMeta.RecordType != Worldspace_Registration.OFST_HEADER
                    || ofstMeta.RecordLength != 0)
                {
                    throw new ArgumentException();
                }
                item.OffsetData = frame.Reader.ReadBytes(contentLen);
            }

            static partial void FillBinaryOffsetDataCustom(MutagenFrame frame, IWorldspaceInternal item, MasterReferences masterReferences)
            {
                if (item.UsingOffsetLength) return;
                if (!HeaderTranslation.ReadNextSubRecordType(frame.Reader, out var len).Equals(Worldspace_Registration.OFST_HEADER))
                {
                    throw new ArgumentException();
                }
                item.OffsetData = frame.Reader.ReadBytes(len);
            }

            public static async Task CustomBinaryEndImport(MutagenFrame frame, IWorldspaceInternal obj, MasterReferences masterReferences)
            {
                if (frame.Reader.Complete) return;
                var next = HeaderTranslation.GetNextType(
                    reader: frame.Reader,
                    contentLength: out var len,
                    finalPos: out var _,
                    hopGroup: false);
                if (!next.Equals(Group_Registration.GRUP_HEADER)) return;
                frame.Reader.Position += 8;
                var formKey = FormKey.Factory(masterReferences, frame.Reader.ReadUInt32());
                var grupType = (GroupTypeEnum)frame.Reader.ReadInt32();
                if (grupType == GroupTypeEnum.WorldChildren)
                {
                    obj.SubCellsTimestamp = frame.Reader.ReadBytes(4);
                    if (formKey != obj.FormKey)
                    {
                        throw new ArgumentException("Cell children group did not match the FormID of the parent worldspace.");
                    }
                }
                else
                {
                    frame.Reader.Position -= 16;
                    return;
                }
                var subFrame = MutagenFrame.ByLength(frame.Reader, len - 20);
                for (int i = 0; i < 3; i++)
                {
                    if (subFrame.Complete) return;
                    var subType = HeaderTranslation.GetNextSubRecordType(frame.Reader, out var subLen);
                    switch (subType.TypeInt)
                    {
                        case 0x44414F52: // "ROAD":
                            if (LoquiBinaryTranslation<Road>.Instance.Parse(
                                frame: subFrame,
                                item: out var road,
                                masterReferences: masterReferences))
                            {
                                obj.Road = road;
                            }
                            else
                            {
                                obj.Road = default;
                            }
                            break;
                        case 0x4C4C4543: // "CELL":
                            var topCell = await LoquiBinaryAsyncTranslation<Cell>.Instance.Parse(
                                frame: subFrame,
                                masterReferences: masterReferences).ConfigureAwait(false);
                            if (topCell.Succeeded)
                            {
                                obj.TopCell = topCell.Value;
                            }
                            else
                            {
                                obj.TopCell = default;
                            }
                            break;
                        case 0x50555247: // "GRUP":
                            obj.SubCells = new ExtendedList<WorldspaceBlock>(
                                await Mutagen.Bethesda.Binary.ListAsyncBinaryTranslation<WorldspaceBlock>.Instance.ParseRepeatedItem(
                                    frame: frame,
                                    triggeringRecord: Worldspace_Registration.GRUP_HEADER,
                                    lengthLength: frame.MetaData.MajorConstants.LengthLength,
                                    transl: (MutagenFrame r) =>
                                    {
                                        return LoquiBinaryAsyncTranslation<WorldspaceBlock>.Instance.Parse(
                                            frame: r,
                                            masterReferences: masterReferences);
                                    }).ConfigureAwait(false));
                            break;
                        default:
                            return;
                    }
                }
            }   
        }

        public partial class WorldspaceBinaryOverlay
        {
            private ReadOnlyMemorySlice<byte>? _grupData;

            private int? _RoadLocation;
            public IRoadGetter? Road => _RoadLocation.HasValue ? RoadBinaryOverlay.RoadFactory(new BinaryMemoryReadStream(_grupData!.Value.Slice(_RoadLocation!.Value)), _package) : default;

            private int? _TopCellLocation;
            public ICellGetter? TopCell => _TopCellLocation.HasValue ? CellBinaryOverlay.CellFactory(new BinaryMemoryReadStream(_grupData!.Value.Slice(_TopCellLocation!.Value)), _package) : default;

            public ReadOnlyMemorySlice<byte> SubCellsTimestamp => _grupData != null ? _package.Meta.Group(_grupData.Value).LastModifiedSpan.ToArray() : UtilityTranslation.Zeros.Slice(0, 4);

            public IReadOnlyList<IWorldspaceBlockGetter>? SubCells { get; private set; }

            private int? _OffsetLengthLocation;
            public bool UsingOffsetLength => this._OffsetLengthLocation.HasValue;

            private int? _OffsetDataLocation;
            bool GetOffsetDataIsSetCustom() => this._OffsetDataLocation.HasValue;

            partial void OffsetDataCustomParse(BinaryMemoryReadStream stream, long finalPos, int offset)
            {
                _OffsetDataLocation = (ushort)(stream.Position - offset);
                if (this.UsingOffsetLength)
                {
                    var offsetLenFrame = _package.Meta.SubRecordFrame(_data.Slice(_OffsetLengthLocation!.Value));
                    stream.Position += checked((int)(_package.Meta.SubConstants.HeaderLength + BinaryPrimitives.ReadUInt32LittleEndian(offsetLenFrame.Content)));
                }
            }

            ReadOnlyMemorySlice<byte>? GetOffsetDataCustom()
            {
                if (this.UsingOffsetLength)
                {
                    var lenFrame = this._package.Meta.SubRecordFrame(_data.Slice(_OffsetLengthLocation!.Value));
                    var len = BinaryPrimitives.ReadInt32LittleEndian(lenFrame.Content);
                    return _data.Slice(_OffsetDataLocation!.Value + this._package.Meta.SubConstants.HeaderLength, len);
                }
                else
                {
                    var spanFrame = this._package.Meta.SubRecordFrame(this._data.Slice(_OffsetDataLocation!.Value));
                    return spanFrame.Content.ToArray();
                }
            }

            partial void OffsetLengthCustomParse(BinaryMemoryReadStream stream, int offset)
            {
                this._OffsetLengthLocation = stream.Position - offset;
            }

            partial void CustomEnd(IBinaryReadStream stream, int finalPos, int offset)
            {
                if (stream.Complete) return;
                var groupMeta = this._package.Meta.GetGroup(stream);
                if (!groupMeta.IsGroup || groupMeta.GroupType != (int)GroupTypeEnum.WorldChildren) return;

                if (this.FormKey != FormKey.Factory(_package.MasterReferences, BinaryPrimitives.ReadUInt32LittleEndian(groupMeta.ContainedRecordTypeSpan)))
                {
                    throw new ArgumentException("Cell children group did not match the FormID of the parent cell.");
                }

                this._grupData = stream.ReadMemory(checked((int)groupMeta.TotalLength));
                stream = new BinaryMemoryReadStream(this._grupData.Value);
                stream.Position += groupMeta.HeaderLength;

                for (int i = 0; i < 3; i++)
                {
                    if (stream.Complete) return;
                    var varMeta = _package.Meta.GetNextRecordVariableMeta(stream);
                    switch (varMeta.RecordTypeInt)
                    {
                        case 0x44414F52: // "ROAD":
                            this._RoadLocation = checked((int)stream.Position);
                            stream.Position += checked((int)varMeta.TotalLength);
                            break;
                        case 0x4C4C4543: // "CELL":
                            this._TopCellLocation = checked((int)stream.Position);
                            stream.Position += checked((int)varMeta.TotalLength);
                            if (!stream.Complete)
                            {
                                var subCellGroup = this._package.Meta.GetGroup(stream);
                                if (subCellGroup.IsGroup && subCellGroup.GroupType == (int)GroupTypeEnum.CellChildren)
                                {
                                    stream.Position += checked((int)subCellGroup.TotalLength);
                                }
                            }
                            break;
                        case 0x50555247: // "GRUP":
                            this.SubCells = BinaryOverlaySetList<IWorldspaceBlockGetter>.FactoryByArray(
                                stream.RemainingMemory,
                                _package,
                                getter: (s, p) => WorldspaceBlockBinaryOverlay.WorldspaceBlockFactory(new BinaryMemoryReadStream(s), p),
                                locs: ParseRecordLocations(
                                    stream: new BinaryMemoryReadStream(stream.RemainingMemory),
                                    finalPos: stream.Length,
                                    trigger: WorldspaceBlock_Registration.TRIGGERING_RECORD_TYPE,
                                    constants: GameConstants.Oblivion.GroupConstants,
                                    skipHeader: false));
                            break;
                        default:
                            i = 3; // Break out
                            break;
                    }
                }
            }
        }
    }
}
