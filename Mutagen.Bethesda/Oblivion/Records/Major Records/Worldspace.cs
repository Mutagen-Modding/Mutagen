using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics;
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
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        String INamedRequiredGetter.Name => this.Name ?? string.Empty;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        String INamedRequired.Name
        {
            get => this.Name ?? string.Empty;
            set => this.Name = value;
        }

        [Flags]
        public enum Flag
        {
            SmallWorld = 0x01,
            CantFastTravel = 0x02,
            OblivionWorldspace = 0x04,
            NoLODWater = 0x10,
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
                obj.SubCells.SetTo(rhs.SubCells.Select((block) =>
                {
                    var blockRet = new WorldspaceBlock();
                    blockRet.DeepCopyIn(block, duplicateBlockCopyMask);
                    blockRet.Items.SetTo(block.Items.Select((subBlock) =>
                    {
                        var subBlockRet = new WorldspaceSubBlock();
                        subBlockRet.DeepCopyIn(subBlock, duplicateSubBlockCopyMask);
                        subBlockRet.Items.SetTo(subBlock.Items.Select(c => (Cell)c.Duplicate(getNextFormKey, duplicatedRecords)));
                        return subBlockRet;
                    }));

                    return blockRet;
                }));
            }
        }

        public partial class WorldspaceBinaryWriteTranslation
        {
            static partial void WriteBinaryOffsetLengthCustom(MutagenWriter writer, IWorldspaceGetter item)
            {
                if (!item.OffsetData.TryGet(out var offset)) return;
                if (!item.UsingOffsetLength) return;
                using (HeaderExport.ExportSubrecordHeader(writer, Worldspace_Registration.XXXX_HEADER))
                {
                    writer.Write(offset.Length);
                }
                writer.Write(Worldspace_Registration.OFST_HEADER.Type);
                writer.WriteZeros(2);
                writer.Write(offset);
            }

            static partial void WriteBinaryOffsetDataCustom(MutagenWriter writer, IWorldspaceGetter item)
            {
                if (item.UsingOffsetLength) return;
                if (!item.OffsetData.TryGet(out var offset)) return;
                using (HeaderExport.ExportSubrecordHeader(writer, Worldspace_Registration.OFST_HEADER))
                {
                    ByteArrayBinaryTranslation.Instance.Write(writer, offset);
                }
            }

            static partial void CustomBinaryEndExport(MutagenWriter writer, IWorldspaceGetter obj)
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
                        obj.FormKey);
                    writer.Write((int)GroupTypeEnum.WorldChildren);
                    writer.Write(obj.SubCellsTimestamp);

                    road?.WriteToBinary(writer);
                    topCell?.WriteToBinary(writer);
                    Mutagen.Bethesda.Binary.ListBinaryTranslation<IWorldspaceBlockGetter>.Instance.Write(
                        writer: writer,
                        items: subCells,
                        transl: (MutagenWriter subWriter, IWorldspaceBlockGetter subItem) =>
                        {
                            subItem.WriteToBinary(subWriter);
                        });
                }
            }
        }

        public partial class WorldspaceBinaryCreateTranslation
        {
            static partial void FillBinaryOffsetLengthCustom(MutagenFrame frame, IWorldspaceInternal item)
            {
                item.UsingOffsetLength = true;
                if (!frame.TryReadSubrecord(Worldspace_Registration.XXXX_HEADER, out var xxxxMeta)
                    || xxxxMeta.ContentLength != 4)
                {
                    throw new ArgumentException();
                }
                var contentLen = frame.Reader.ReadInt32();
                if (!frame.Reader.TryReadSubrecord(Worldspace_Registration.OFST_HEADER, out var ofstMeta)
                    || ofstMeta.ContentLength != 0)
                {
                    throw new ArgumentException();
                }
                item.OffsetData = frame.Reader.ReadBytes(contentLen);
            }

            static partial void FillBinaryOffsetDataCustom(MutagenFrame frame, IWorldspaceInternal item)
            {
                if (item.UsingOffsetLength) return;
                if (!HeaderTranslation.ReadNextSubrecordType(frame.Reader, out var len).Equals(Worldspace_Registration.OFST_HEADER))
                {
                    throw new ArgumentException();
                }
                item.OffsetData = frame.Reader.ReadBytes(len);
            }

            static partial void CustomBinaryEndImport(MutagenFrame frame, IWorldspaceInternal obj)
            {
                if (frame.Reader.Complete) return;
                var next = HeaderTranslation.GetNextType(
                    reader: frame.Reader,
                    contentLength: out var len,
                    finalPos: out var _,
                    hopGroup: false);
                if (!next.Equals(Group_Registration.GRUP_HEADER)) return;
                frame.Reader.Position += 8;
                var formKey = FormKey.Factory(frame.MasterReferences!, frame.Reader.ReadUInt32());
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
                    var subType = HeaderTranslation.GetNextSubrecordType(frame.Reader, out var subLen);
                    switch (subType.TypeInt)
                    {
                        case 0x44414F52: // "ROAD":
                            if (LoquiBinaryTranslation<Road>.Instance.Parse(
                                frame: subFrame,
                                item: out var road))
                            {
                                obj.Road = road;
                            }
                            else
                            {
                                obj.Road = default;
                            }
                            break;
                        case 0x4C4C4543: // "CELL":
                            if (LoquiBinaryTranslation<Cell>.Instance.Parse(subFrame, out var topCell))
                            {
                                obj.TopCell = topCell;
                            }
                            else
                            {
                                obj.TopCell = default;
                            }
                            break;
                        case 0x50555247: // "GRUP":
                            obj.SubCells.SetTo(
                                Mutagen.Bethesda.Binary.ListBinaryTranslation<WorldspaceBlock>.Instance.Parse(
                                    frame: frame,
                                    triggeringRecord: Worldspace_Registration.GRUP_HEADER,
                                    transl: LoquiBinaryTranslation<WorldspaceBlock>.Instance.Parse));
                            break;
                        default:
                            return;
                    }
                }
            }   
        }

        public partial class WorldspaceBinaryOverlay
        {
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            String INamedRequiredGetter.Name => this.Name ?? string.Empty;

            private ReadOnlyMemorySlice<byte>? _grupData;

            private int? _RoadLocation;
            public IRoadGetter? Road => _RoadLocation.HasValue ? RoadBinaryOverlay.RoadFactory(new BinaryMemoryReadStream(_grupData!.Value.Slice(_RoadLocation!.Value)), _package) : default;

            private int? _TopCellLocation;
            public ICellGetter? TopCell => _TopCellLocation.HasValue ? CellBinaryOverlay.CellFactory(new BinaryMemoryReadStream(_grupData!.Value.Slice(_TopCellLocation!.Value)), _package) : default;

            public ReadOnlyMemorySlice<byte> SubCellsTimestamp => _grupData != null ? _package.Meta.Group(_grupData.Value).LastModifiedSpan.ToArray() : UtilityTranslation.Zeros.Slice(0, 4);

            public IReadOnlyList<IWorldspaceBlockGetter> SubCells { get; private set; } = ListExt.Empty<IWorldspaceBlockGetter>();

            private int? _OffsetLengthLocation;
            public bool UsingOffsetLength => this._OffsetLengthLocation.HasValue;

            private int? _OffsetDataLocation;
            bool GetOffsetDataIsSetCustom() => this._OffsetDataLocation.HasValue;

            partial void OffsetDataCustomParse(BinaryMemoryReadStream stream, long finalPos, int offset)
            {
                _OffsetDataLocation = (ushort)(stream.Position - offset);
                if (this.UsingOffsetLength)
                {
                    var offsetLenFrame = _package.Meta.SubrecordFrame(_data.Slice(_OffsetLengthLocation!.Value));
                    stream.Position += checked((int)(_package.Meta.SubConstants.HeaderLength + BinaryPrimitives.ReadUInt32LittleEndian(offsetLenFrame.Content)));
                }
            }

            ReadOnlyMemorySlice<byte>? GetOffsetDataCustom()
            {
                if (!_OffsetDataLocation.HasValue) return null;
                if (this.UsingOffsetLength)
                {
                    var lenFrame = this._package.Meta.SubrecordFrame(_data.Slice(_OffsetLengthLocation!.Value));
                    var len = BinaryPrimitives.ReadInt32LittleEndian(lenFrame.Content);
                    return _data.Slice(_OffsetDataLocation.Value + this._package.Meta.SubConstants.HeaderLength, len);
                }
                else
                {
                    var spanFrame = this._package.Meta.SubrecordFrame(this._data.Slice(_OffsetDataLocation.Value));
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
                                    trigger: WorldspaceBlock_Registration.TriggeringRecordType,
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
