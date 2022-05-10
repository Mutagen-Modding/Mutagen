using System.Buffers.Binary;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Records.Internals;
using Noggog;

namespace Mutagen.Bethesda.Fallout4;

partial class Worldspace
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
    public static partial void FillBinaryObjectBoundsMinCustom(MutagenFrame frame, IWorldspaceInternal item)
    {
        item.ObjectBoundsMin = new P2Float(
            frame.ReadFloat() / 4096f,
            frame.ReadFloat() / 4096f);
    }

    public static partial void FillBinaryObjectBoundsMaxCustom(MutagenFrame frame, IWorldspaceInternal item)
    {
        item.ObjectBoundsMax = new P2Float(
            frame.ReadFloat() / 4096f,
            frame.ReadFloat() / 4096f);
    }

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
                        case RecordTypeInts.CELL:
                            if (LoquiBinaryTranslation<Cell>.Instance.Parse(subFrame, out var topCell))
                            {
                                obj.TopCell = topCell;
                            }
                            else
                            {
                                obj.TopCell = default;
                            }
                            break;
                        case RecordTypeInts.GRUP:
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
    public static partial void WriteBinaryObjectBoundsMinCustom(MutagenWriter writer, IWorldspaceGetter item)
    {
        var min = item.ObjectBoundsMin;
        writer.Write(min.X * 4096f);
        writer.Write(min.Y * 4096f);
    }

    public static partial void WriteBinaryObjectBoundsMaxCustom(MutagenWriter writer, IWorldspaceGetter item)
    {
        var max = item.ObjectBoundsMax;
        writer.Write(max.X * 4096f);
        writer.Write(max.Y * 4096f);
    }

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
    public partial P2Float GetObjectBoundsMinCustom() => _NAM0Location.HasValue
        ? new P2Float(
            _data.Slice(_NAM0Location.Value.Min).Float() / 4096f,
            _data.Slice(_NAM0Location.Value.Min + 4).Float() / 4096f)
        : default;

    public partial P2Float GetObjectBoundsMaxCustom() => _NAM9Location.HasValue
        ? new P2Float(
            _data.Slice(_NAM9Location.Value.Min).Float() / 4096f,
            _data.Slice(_NAM9Location.Value.Min + 4).Float() / 4096f)
        : default;
    
    private ReadOnlyMemorySlice<byte>? _grupData;

    private int? _topCellLocation;
    public ICellGetter? TopCell => _topCellLocation.HasValue ? CellBinaryOverlay.CellFactory(new OverlayStream(_grupData!.Value.Slice(_topCellLocation!.Value), _package), _package, insideWorldspace: true) : default;

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
                        this._topCellLocation = checked((int)stream.Position);
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
