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

namespace Mutagen.Bethesda.Oblivion;

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
}

partial class WorldspaceBinaryWriteTranslation
{
    public static partial void CustomBinaryEndExport(MutagenWriter writer, IWorldspaceGetter obj)
    {
        try
        {
            var road = obj.Road;
            var topCell = obj.TopCell;
            var subCells = obj.SubCells;
            if (subCells?.Count == 0
                && road == null
                && topCell == null) return;
            using (HeaderExport.Header(writer, RecordTypes.GRUP, ObjectType.Group))
            {
                FormKeyBinaryTranslation.Instance.Write(
                    writer,
                    obj);
                writer.Write((int)GroupTypeEnum.WorldChildren);
                writer.Write(obj.SubCellsTimestamp);

                road?.WriteToBinary(writer);
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
                var formKey = FormKeyBinaryTranslation.Instance.Parse(groupHeader.ContainedRecordTypeData, frame.MetaData.MasterReferences);
                if (formKey != obj.FormKey)
                {
                    throw new ArgumentException("Worldspace children group did not match the FormID of the parent worldspace.");
                }
            }
            else
            {
                frame.Reader.Position -= groupHeader.HeaderLength;
                return;
            }
            var subFrame = MutagenFrame.ByLength(frame.Reader, groupHeader.ContentLength);
            for (int i = 0; i < 3; i++)
            {
                if (subFrame.Complete) return;
                var subType = HeaderTranslation.GetNextSubrecordType(frame.Reader, out var subLen);
                switch (subType.TypeInt)
                {
                    case RecordTypeInts.ROAD:
                        obj.Road = Road.CreateFromBinary(subFrame);
                        break;
                    case RecordTypeInts.CELL:
                        obj.TopCell = Cell.CreateFromBinary(subFrame);
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
        catch (Exception ex)
        {
            throw RecordException.Enrich(ex, obj);
        }
    }   
}

partial class WorldspaceBinaryOverlay
{
    private ReadOnlyMemorySlice<byte>? _grupData;

    private int? _RoadLocation;
    public IRoadGetter? Road => _RoadLocation.HasValue ? RoadBinaryOverlay.RoadFactory(new OverlayStream(_grupData!.Value.Slice(_RoadLocation!.Value), _package), _package) : default;

    private int? _TopCellLocation;
    public ICellGetter? TopCell => _TopCellLocation.HasValue ? CellBinaryOverlay.CellFactory(new OverlayStream(_grupData!.Value.Slice(_TopCellLocation!.Value), _package), _package) : default;

    public int SubCellsTimestamp => _grupData != null ? BinaryPrimitives.ReadInt32LittleEndian(_package.MetaData.Constants.GroupHeader(_grupData.Value).LastModifiedData) : 0;

    public IReadOnlyList<IWorldspaceBlockGetter> SubCells { get; private set; } = Array.Empty<IWorldspaceBlockGetter>();

    partial void CustomEnd(OverlayStream stream, int finalPos, int offset)
    {
        try
        {
            if (stream.Complete) return;
            var groupMeta = stream.GetGroupHeader();
            if (!groupMeta.IsGroup || groupMeta.GroupType != (int)GroupTypeEnum.WorldChildren) return;

            if (this.FormKey != Mutagen.Bethesda.Plugins.FormKey.Factory(
                    _package.MetaData.MasterReferences, 
                    new FormID(BinaryPrimitives.ReadUInt32LittleEndian(groupMeta.ContainedRecordTypeData)),
                    reference: true))
            {
                throw new ArgumentException("Worldspace children group did not match the FormID of the parent cell.");
            }

            this._grupData = stream.ReadMemory(checked((int)groupMeta.TotalLength));
            stream = new OverlayStream(this._grupData.Value, stream.MetaData);
            stream.Position += groupMeta.HeaderLength;

            for (int i = 0; i < 3; i++)
            {
                if (stream.Complete) return;
                var varMeta = stream.GetVariableHeader(subRecords: false);
                switch (varMeta.RecordTypeInt)
                {
                    case RecordTypeInts.ROAD:
                        this._RoadLocation = checked((int)stream.Position);
                        stream.Position += checked((int)varMeta.TotalLength);
                        break;
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
                        i = 3; // Break out
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