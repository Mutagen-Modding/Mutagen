using Loqui;
using Mutagen.Bethesda.Binary;
using Noggog;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class Worldspace
    {
        #region Interfaces
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        string INamedRequiredGetter.Name => this.Name?.String ?? string.Empty;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        string? INamedGetter.Name => this.Name?.String;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        TranslatedString ITranslatedNamedRequiredGetter.Name => this.Name ?? string.Empty;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        string INamedRequired.Name
        {
            get => this.Name?.String ?? string.Empty;
            set => this.Name = new TranslatedString(value);
        }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        TranslatedString ITranslatedNamedRequired.Name
        {
            get => this.Name ?? string.Empty;
            set => this.Name = value;
        }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        string? INamed.Name
        {
            get => this.Name?.String;
            set => this.Name = value == null ? null : new TranslatedString(value);
        }
        #endregion

        [Flags]
        public enum MajorFlag
        {
            CanNotWait = 0x0008_0000,
        }

        [Flags]
        public enum Flag
        {
            SmallWorld = 0x01,
            CannotFastTravel = 0x02,
            NoLodWater = 0x08,
            NoLandscape = 0x10,
            NoSky = 0x20,
            FixedDimensions = 0x40,
            NoGrass = 0x80,
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

        public partial class WorldspaceBinaryCreateTranslation
        {
            static partial void CustomBinaryEndImport(MutagenFrame frame, IWorldspaceInternal obj)
            {
                if (!frame.Reader.TryReadGroup(out var groupHeader)) return;
                if (groupHeader.GroupType == (int)GroupTypeEnum.WorldChildren)
                {
                    obj.SubCellsTimestamp = BinaryPrimitives.ReadInt32LittleEndian(groupHeader.LastModifiedSpan);
                    obj.SubCellsUnknown = BinaryPrimitives.ReadInt32LittleEndian(groupHeader.Span.Slice(groupHeader.HeaderLength - 4));
                    var formKey = FormKeyBinaryTranslation.Instance.Parse(groupHeader.ContainedRecordTypeSpan, frame.MetaData.MasterReferences!);
                    if (formKey != obj.FormKey)
                    {
                        throw new ArgumentException("Cell children group did not match the FormID of the parent worldspace.");
                    }
                }
                else
                {
                    frame.Reader.Position -= groupHeader.HeaderLength;
                    return;
                }
                frame.MetaData.InWorldspace = true;
                try
                {
                    var subFrame = MutagenFrame.ByLength(frame.Reader, groupHeader.ContentLength);
                    for (int i = 0; i < 2; i++)
                    {
                        if (subFrame.Complete) return;
                        var subType = HeaderTranslation.GetNextSubrecordType(frame.Reader, out var subLen);
                        switch (subType.TypeInt)
                        {
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
                finally
                {
                    frame.MetaData.InWorldspace = false;
                }
            }
        }

        public partial class WorldspaceBinaryWriteTranslation
        {
            static partial void CustomBinaryEndExport(MutagenWriter writer, IWorldspaceGetter obj)
            {
                var topCell = obj.TopCell;
                var subCells = obj.SubCells;
                if (subCells?.Count == 0
                    && topCell != null) return;
                using (HeaderExport.ExportHeader(writer, Group_Registration.GRUP_HEADER, Mutagen.Bethesda.Binary.ObjectType.Group))
                {
                    FormKeyBinaryTranslation.Instance.Write(
                        writer,
                        obj.FormKey);
                    writer.Write((int)GroupTypeEnum.WorldChildren);
                    writer.Write(obj.SubCellsTimestamp);
                    writer.Write(obj.SubCellsUnknown);

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

        public partial class WorldspaceBinaryOverlay
        {
            #region Interfaces
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            string INamedRequiredGetter.Name => this.Name?.String ?? string.Empty;
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            string? INamedGetter.Name => this.Name?.String;
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            TranslatedString ITranslatedNamedRequiredGetter.Name => this.Name ?? string.Empty;
            #endregion

            private ReadOnlyMemorySlice<byte>? _grupData;

            private int? _TopCellLocation;
            public ICellGetter? TopCell => _TopCellLocation.HasValue ? CellBinaryOverlay.CellFactory(new BinaryMemoryReadStream(_grupData!.Value.Slice(_TopCellLocation!.Value)), _package) : default;

            public int SubCellsTimestamp => _grupData != null ? BinaryPrimitives.ReadInt32LittleEndian(_package.MetaData.Constants.Group(_grupData.Value).LastModifiedSpan) : 0;

            public IReadOnlyList<IWorldspaceBlockGetter> SubCells { get; private set; } = ListExt.Empty<IWorldspaceBlockGetter>();

            public int SubCellsUnknown => throw new NotImplementedException();

            partial void CustomEnd(BinaryMemoryReadStream stream, int finalPos, int offset)
            {
                if (stream.Complete) return;
                var groupMeta = this._package.MetaData.Constants.GetGroup(stream);
                if (!groupMeta.IsGroup || groupMeta.GroupType != (int)GroupTypeEnum.WorldChildren) return;

                if (this.FormKey != FormKey.Factory(_package.MetaData.MasterReferences!, BinaryPrimitives.ReadUInt32LittleEndian(groupMeta.ContainedRecordTypeSpan)))
                {
                    throw new ArgumentException("Cell children group did not match the FormID of the parent cell.");
                }

                this._grupData = stream.ReadMemory(checked((int)groupMeta.TotalLength));
                stream = new BinaryMemoryReadStream(this._grupData.Value);
                stream.Position += groupMeta.HeaderLength;

                for (int i = 0; i < 2; i++)
                {
                    if (stream.Complete) return;
                    var varMeta = _package.MetaData.Constants.GetNextRecordVariableMeta(stream);
                    switch (varMeta.RecordTypeInt)
                    {
                        case 0x4C4C4543: // "CELL":
                            this._TopCellLocation = checked((int)stream.Position);
                            stream.Position += checked((int)varMeta.TotalLength);
                            if (!stream.Complete)
                            {
                                var subCellGroup = this._package.MetaData.Constants.GetGroup(stream);
                                if (subCellGroup.IsGroup && subCellGroup.GroupType == (int)GroupTypeEnum.CellChildren)
                                {
                                    stream.Position += checked((int)subCellGroup.TotalLength);
                                }
                            }
                            break;
                        case 0x50555247: // "GRUP":
                            this.SubCells = BinaryOverlayList<IWorldspaceBlockGetter>.FactoryByArray(
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
                            i = 2; // Break out
                            break;
                    }
                }
            }
        }
    }
}
