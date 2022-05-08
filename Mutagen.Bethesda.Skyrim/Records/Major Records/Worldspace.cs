using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Records.Internals;
using Noggog;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using Mutagen.Bethesda.Plugins.Binary.Headers;

namespace Mutagen.Bethesda.Skyrim;

public partial class Worldspace
{
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

partial class WorldspaceBinaryCreateTranslation
{
    public static partial void CustomBinaryEndImport(MutagenFrame frame, IWorldspaceInternal obj)
    {
        try
        {
            if (!frame.Reader.TryReadGroupHeader(out var groupHeader)) return;
            if (groupHeader.GroupType == (int)GroupTypeEnum.WorldChildren)
            {
                obj.SubCellsTimestamp = BinaryPrimitives.ReadInt32LittleEndian(groupHeader.LastModifiedData);
                obj.SubCellsUnknown = BinaryPrimitives.ReadInt32LittleEndian(groupHeader.HeaderData.Slice(groupHeader.HeaderLength - 4));
                var formKey = FormKeyBinaryTranslation.Instance.Parse(groupHeader.ContainedRecordTypeData, frame.MetaData.MasterReferences!);
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
                                ListBinaryTranslation<WorldspaceBlock>.Instance.Parse(
                                    reader: frame,
                                    triggeringRecord: RecordTypes.GRUP,
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
        catch (Exception ex)
        {
            throw RecordException.Enrich(ex, obj);
        }
    }
}

partial class WorldspaceBinaryWriteTranslation
{
    public static partial void CustomBinaryEndExport(MutagenWriter writer, IWorldspaceGetter obj)
    {
        try
        {
            var topCell = obj.TopCell;
            var subCells = obj.SubCells;
            if (subCells?.Count == 0
                && topCell == null) return;
            using (HeaderExport.Header(writer, RecordTypes.GRUP, ObjectType.Group))
            {
                FormKeyBinaryTranslation.Instance.Write(
                    writer,
                    obj.FormKey);
                writer.Write((int)GroupTypeEnum.WorldChildren);
                writer.Write(obj.SubCellsTimestamp);
                writer.Write(obj.SubCellsUnknown);

                topCell?.WriteToBinary(writer);
                ListBinaryTranslation<IWorldspaceBlockGetter>.Instance.Write(
                    writer: writer,
                    items: subCells,
                    transl: (MutagenWriter subWriter, IWorldspaceBlockGetter subItem) =>
                    {
                        subItem.WriteToBinary(subWriter);
                    });
            }
        }
        catch (Exception ex)
        {
            throw RecordException.Enrich(ex, obj);
        }
    }
}

partial class WorldspaceBinaryOverlay
{
    private ReadOnlyMemorySlice<byte>? _grupData;

    private int? _TopCellLocation;
    public ICellGetter? TopCell => _TopCellLocation.HasValue ? CellBinaryOverlay.CellFactory(new OverlayStream(_grupData!.Value.Slice(_TopCellLocation!.Value), _package), _package, insideWorldspace: true) : default;

    public int SubCellsTimestamp => _grupData != null ? BinaryPrimitives.ReadInt32LittleEndian(_package.MetaData.Constants.GroupHeader(_grupData.Value).LastModifiedData) : 0;

    public IReadOnlyList<IWorldspaceBlockGetter> SubCells { get; private set; } = ListExt.Empty<IWorldspaceBlockGetter>();

    public int SubCellsUnknown => _grupData != null ? BinaryPrimitives.ReadInt32LittleEndian(_grupData.Value.Slice(20)) : 0;

    partial void CustomEnd(OverlayStream stream, int finalPos, int offset)
    {
        try
        {
            if (stream.Complete) return;
            if (!stream.TryGetGroupHeader(out var groupMeta) || groupMeta.GroupType != (int)GroupTypeEnum.WorldChildren) return;

            if (this.FormKey != FormKey.Factory(_package.MetaData.MasterReferences!, BinaryPrimitives.ReadUInt32LittleEndian(groupMeta.ContainedRecordTypeData)))
            {
                throw new ArgumentException("Cell children group did not match the FormID of the parent cell.");
            }

            this._grupData = stream.ReadMemory(checked((int)groupMeta.TotalLength));
            stream = new OverlayStream(this._grupData.Value, stream.MetaData);
            stream.Position += groupMeta.HeaderLength;

            for (int i = 0; i < 2; i++)
            {
                if (stream.Complete) return;
                var varMeta = stream.GetVariableHeader();
                switch (varMeta.RecordTypeInt)
                {
                    case RecordTypeInts.CELL:
                        this._TopCellLocation = checked((int)stream.Position);
                        stream.Position += checked((int)varMeta.TotalLength);
                        if (!stream.Complete)
                        {
                            var subCellGroup = stream.GetGroupHeader();
                            if (subCellGroup.IsGroup && subCellGroup.GroupType == (int)GroupTypeEnum.CellChildren)
                            {
                                stream.Position += checked((int)subCellGroup.TotalLength);
                            }
                        }
                        break;
                    case RecordTypeInts.GRUP:
                        this.SubCells = BinaryOverlayList.FactoryByArray<IWorldspaceBlockGetter>(
                            stream.RemainingMemory,
                            _package,
                            getter: (s, p) => WorldspaceBlockBinaryOverlay.WorldspaceBlockFactory(new OverlayStream(s, p), p),
                            locs: ParseRecordLocations(
                                stream: new OverlayStream(stream.RemainingMemory, _package),
                                trigger: RecordTypes.GRUP,
                                constants: GameConstants.Oblivion.GroupConstants,
                                skipHeader: false));
                        break;
                    default:
                        i = 2; // Break out
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            throw RecordException.Enrich(ex, this);
        }
    }
}