using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Internals;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Records.Internals;
using Noggog;

namespace Mutagen.Bethesda.Oblivion;

public partial class Cell
{
    [Flags]
    public enum Flag
    {
        IsInteriorCell = 0x0001,
        HasWater = 0x0002,
        InvertFastTravelBehavior = 0x0004,
        ForceHideLand = 0x0008,
        PublicPlace = 0x0020,
        HandChanged = 0x0040,
        BehaveLikeExteriod = 0x0080,
    }
}

partial class CellBinaryCreateTranslation
{
    public static partial void CustomBinaryEndImport(MutagenFrame frame, ICellInternal obj)
    {
        try
        {
            CustomBinaryEnd(frame, obj);
        }
        catch (Exception ex)
        {
            RecordException.EnrichAndThrow(ex, obj);
            throw;
        }
    }

    private static void CustomBinaryEnd(MutagenFrame frame, ICellInternal obj)
    {
        if (frame.Reader.Complete) return;
        if (!frame.TryGetGroupHeader(out var groupMeta)) return;
        var formKey = FormKey.Factory(
            frame.MetaData.MasterReferences,
            new FormID(BinaryPrimitives.ReadUInt32LittleEndian(groupMeta.ContainedRecordTypeData)),
            reference: true);
        if (groupMeta.GroupType == (int)GroupTypeEnum.CellChildren)
        {
            obj.Timestamp = BinaryPrimitives.ReadInt32LittleEndian(groupMeta.LastModifiedData);
            frame.Position += groupMeta.HeaderLength;
            if (formKey != obj.FormKey)
            {
                throw new ArgumentException("Cell children group did not match the FormID of the parent cell.");
            }
        }
        else
        {
            return;
        }
        var subFrame = frame.SpawnWithLength(groupMeta.ContentLength);
        while (!subFrame.Complete)
        {
            var persistGroupMeta = frame.GetGroupHeader();
            if (!persistGroupMeta.IsGroup)
            {
                throw new ArgumentException();
            }
            GroupTypeEnum type = (GroupTypeEnum)persistGroupMeta.GroupType;
            var itemFrame = frame.SpawnWithLength(persistGroupMeta.TotalLength);
            switch (type)
            {
                case GroupTypeEnum.CellPersistentChildren:
                    ParseTypical(
                        frame: itemFrame,
                        obj: obj,
                        coll: obj.Persistent,
                        persistentParse: true);
                    break;
                case GroupTypeEnum.CellTemporaryChildren:
                    ParseTemporary(
                        itemFrame,
                        obj);
                    break;
                case GroupTypeEnum.CellVisibleDistantChildren:
                    ParseTypical(
                        frame: itemFrame,
                        obj: obj,
                        coll: obj.VisibleWhenDistant,
                        persistentParse: false);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }

    static void ParseTypical(
        MutagenFrame frame,
        ICellInternal obj,
        IList<IPlaced> coll,
        bool persistentParse)
    {
        var groupMeta = frame.ReadGroupHeader();
        var formKey = FormKey.Factory(
            frame.MetaData.MasterReferences, 
            new FormID(BinaryPrimitives.ReadUInt32LittleEndian(groupMeta.ContainedRecordTypeData)),
            reference: true);
        if (formKey != obj.FormKey)
        {
            throw new ArgumentException("Cell children group did not match the FormID of the parent cell.");
        }
        if (persistentParse)
        {
            obj.PersistentTimestamp = groupMeta.LastModifiedData.Int32();
        }
        else
        {
            obj.VisibleWhenDistantTimestamp = groupMeta.LastModifiedData.Int32();
        }
        coll.AddRange(
            ListBinaryTranslation<IPlaced>.Instance.Parse(
                reader: frame,
                transl: (MutagenFrame r, RecordType header, [MaybeNullWhen(false)] out IPlaced placed) =>
                {
                    switch (header.TypeInt)
                    {
                        case RecordTypeInts.ACRE:
                            placed = PlacedCreature.CreateFromBinary(r);
                            return true;
                        case RecordTypeInts.ACHR:
                            placed = PlacedNpc.CreateFromBinary(r);
                            return true;
                        case RecordTypeInts.REFR:
                            placed = PlacedObject.CreateFromBinary(r);
                            return true;
                        default:
                            throw new NotImplementedException();
                    }
                }));
    }

    static bool ParseTemporaryOutliers(MutagenFrame frame, ICellInternal obj)
    {
        var majorMeta = frame.GetMajorRecordHeader();
        var nextHeader = majorMeta.RecordType;
        if (nextHeader.Equals(RecordTypes.PGRD))
        {
            obj.PathGrid = PathGrid.CreateFromBinary(
                frame.SpawnWithLength(majorMeta.TotalLength),
                translationParams: null);
            return true;
        }
        else if (nextHeader.Equals(RecordTypes.LAND))
        {
            obj.Landscape = Landscape.CreateFromBinary(
                frame.SpawnWithLength(majorMeta.TotalLength),
                translationParams: null);
            return true;
        }
        return false;
    }

    static void ParseTemporary(MutagenFrame frame, ICellInternal obj)
    {
        var groupMeta = frame.ReadGroupHeader();
        var formKey = FormKey.Factory(
            frame.MetaData.MasterReferences, 
            new FormID(BinaryPrimitives.ReadUInt32LittleEndian(groupMeta.ContainedRecordTypeData)),
            reference: true);
        if (formKey != obj.FormKey)
        {
            throw new ArgumentException("Cell children group did not match the FormID of the parent cell.");
        }
        obj.TemporaryTimestamp = BinaryPrimitives.ReadInt32LittleEndian(groupMeta.LastModifiedData);
        var items = ListBinaryTranslation<IPlaced>.Instance.Parse(
            reader: frame,
            transl: (MutagenFrame r, RecordType header, [MaybeNullWhen(false)] out IPlaced placed) =>
            {
                switch (header.TypeInt)
                {
                    case RecordTypeInts.ACRE:
                        placed = PlacedCreature.CreateFromBinary(r);
                        return true;
                    case RecordTypeInts.ACHR:
                        placed = PlacedNpc.CreateFromBinary(r);
                        return true;
                    case RecordTypeInts.REFR:
                        placed = PlacedObject.CreateFromBinary(r);
                        return true;
                    default:
                        if (ParseTemporaryOutliers(frame, obj))
                        {
                            placed = null!;
                            return false;
                        }
                        throw new NotImplementedException();
                }
                placed = null!;
                return false;
            });
        obj.Temporary.SetTo(new ExtendedList<IPlaced>(items));
    }
}

partial class CellBinaryWriteTranslation
{
    public static partial void CustomBinaryEndExport(MutagenWriter writer, ICellGetter obj)
    {
        try
        {
            var pathGrid = obj.PathGrid;
            var landscape = obj.Landscape;
            if ((obj.Persistent?.Count ?? 0) == 0
                && (obj.Temporary?.Count ?? 0) == 0
                && (obj.VisibleWhenDistant?.Count ?? 0) == 0
                && pathGrid == null
                && landscape == null) return;
            using (HeaderExport.Header(writer, RecordTypes.GRUP, ObjectType.Group))
            {
                FormKeyBinaryTranslation.Instance.Write(
                    writer,
                    obj);
                writer.Write((int)GroupTypeEnum.CellChildren);
                writer.Write(obj.Timestamp);
                if (obj.Persistent?.Count > 0)
                {
                    using (HeaderExport.Header(writer, RecordTypes.GRUP, ObjectType.Group))
                    {
                        FormKeyBinaryTranslation.Instance.Write(
                            writer,
                            obj);
                        writer.Write((int)GroupTypeEnum.CellPersistentChildren);
                        writer.Write(obj.PersistentTimestamp);
                        ListBinaryTranslation<IPlacedGetter>.Instance.Write(
                            writer: writer,
                            items: obj.Persistent,
                            transl: (r, item) =>
                            {
                                item.WriteToBinary(r);
                            });
                    }
                }
                if (obj.Temporary?.Count > 0
                    || pathGrid != null
                    || landscape != null)
                {
                    using (HeaderExport.Header(writer, RecordTypes.GRUP, ObjectType.Group))
                    {
                        FormKeyBinaryTranslation.Instance.Write(
                            writer,
                            obj);
                        writer.Write((int)GroupTypeEnum.CellTemporaryChildren);
                        writer.Write(obj.TemporaryTimestamp);
                        landscape?.WriteToBinary(writer);
                        pathGrid?.WriteToBinary(writer);
                        if (obj.Temporary != null)
                        {
                            ListBinaryTranslation<IPlacedGetter>.Instance.Write(
                                writer: writer,
                                items: obj.Temporary,
                                transl: (r, item) =>
                                {
                                    item.WriteToBinary(r);
                                });
                        }
                    }
                }
                if (obj.VisibleWhenDistant?.Count > 0)
                {
                    using (HeaderExport.Header(writer, RecordTypes.GRUP, ObjectType.Group))
                    {
                        FormKeyBinaryTranslation.Instance.Write(
                            writer,
                            obj);
                        writer.Write((int)GroupTypeEnum.CellVisibleDistantChildren);
                        writer.Write(obj.VisibleWhenDistantTimestamp);
                        ListBinaryTranslation<IPlacedGetter>.Instance.Write(
                            writer: writer,
                            items: obj.VisibleWhenDistant,
                            transl: (r, item) =>
                            {
                                item.WriteToBinary(r);
                            });
                    }
                }
            }
        }
        catch (Exception ex)
        {
            RecordException.EnrichAndThrow(ex, obj);
            throw;
        }
    }
}

partial class CellBinaryOverlay
{
    static readonly RecordTriggerSpecs TypicalPlacedTypes = new RecordTriggerSpecs(
        RecordCollection.Factory(
            RecordTypes.ACHR,
            RecordTypes.ACRE,
            RecordTypes.REFR));

    private ReadOnlyMemorySlice<byte>? _grupData;

    private int? _pathgridLocation;
    public IPathGridGetter? PathGrid => _pathgridLocation.HasValue ? PathGridBinaryOverlay.PathGridFactory(new OverlayStream(_grupData!.Value.Slice(_pathgridLocation!.Value), _package), _package, default) : default;

    private int? _landscapeLocation;
    public ILandscapeGetter? Landscape => _landscapeLocation.HasValue ? LandscapeBinaryOverlay.LandscapeFactory(new OverlayStream(_grupData!.Value.Slice(_landscapeLocation!.Value), _package), _package, default) : default;

    public int Timestamp => _grupData != null ? BinaryPrimitives.ReadInt32LittleEndian(_package.MetaData.Constants.GroupHeader(_grupData.Value).LastModifiedData) : 0;

    private int? _persistentLocation;
    public int PersistentTimestamp => _persistentLocation.HasValue ? BinaryPrimitives.ReadInt32LittleEndian(_package.MetaData.Constants.GroupHeader(_grupData!.Value.Slice(_persistentLocation.Value)).LastModifiedData) : 0;
    public IReadOnlyList<IPlacedGetter> Persistent { get; private set; } = Array.Empty<IPlacedGetter>();

    private int? _temporaryLocation;
    public int TemporaryTimestamp => _temporaryLocation.HasValue ? BinaryPrimitives.ReadInt32LittleEndian(_package.MetaData.Constants.GroupHeader(_grupData!.Value.Slice(_temporaryLocation.Value)).LastModifiedData) : 0;
    public IReadOnlyList<IPlacedGetter> Temporary { get; private set; } = Array.Empty<IPlacedGetter>();

    private int? _visibleWhenDistantLocation;
    public int VisibleWhenDistantTimestamp => _visibleWhenDistantLocation.HasValue ? BinaryPrimitives.ReadInt32LittleEndian(_package.MetaData.Constants.GroupHeader(_grupData!.Value.Slice(_visibleWhenDistantLocation.Value)).LastModifiedData) : 0;
    public IReadOnlyList<IPlacedGetter> VisibleWhenDistant { get; private set; } = Array.Empty<IPlacedGetter>();

    public static int[] ParseRecordLocations(OverlayStream stream, BinaryOverlayFactoryPackage package)
    {
        List<int> ret = new List<int>();
        var startingPos = stream.Position;
        while (!stream.Complete)
        {
            var cellMeta = stream.GetMajorRecordHeader();
            if (cellMeta.RecordType != RecordTypes.CELL) break;
            ret.Add(stream.Position - startingPos);
            stream.Position += (int)cellMeta.TotalLength;
            if (stream.Complete) break;
            while (stream.TryGetGroupHeader(out var groupMeta)
                   && groupMeta.GroupType == (int)GroupTypeEnum.CellChildren)
            {
                stream.Position += (int)groupMeta.TotalLength;
            }
        }
        return ret.ToArray();
    }

    partial void CustomEnd(OverlayStream stream, int finalPos, int _)
    {
        try
        {
            if (stream.Complete) return;
            var startPos = stream.Position;
            if (!stream.TryGetGroupHeader(out var groupMeta)) return;
            var formKey = FormKey.Factory(
                _package.MetaData.MasterReferences, 
                new FormID(BinaryPrimitives.ReadUInt32LittleEndian(groupMeta.ContainedRecordTypeData)),
                reference: true);
            if (groupMeta.GroupType == (int)GroupTypeEnum.CellChildren)
            {
                if (formKey != this.FormKey)
                {
                    throw new ArgumentException("Cell children group did not match the FormID of the parent cell.");
                }
            }
            else
            {
                return;
            }
            this._grupData = stream.ReadMemory(checked((int)groupMeta.TotalLength));
            stream = new OverlayStream(this._grupData.Value, stream.MetaData);
            stream.Position += groupMeta.HeaderLength;
            while (!stream.Complete)
            {
                var subGroupLocation = stream.Position;
                var subGroupMeta = stream.ReadGroupHeader();
                if (!subGroupMeta.IsGroup)
                {
                    throw new ArgumentException();
                }

                IPlacedGetter TypicalGetter(
                    ReadOnlyMemorySlice<byte> span,
                    BinaryOverlayFactoryPackage package)
                {
                    var majorMeta = package.MetaData.Constants.MajorRecordHeader(span);
                    switch (majorMeta.RecordType.TypeInt)
                    {
                        case RecordTypeInts.ACRE:
                            return PlacedCreatureBinaryOverlay.PlacedCreatureFactory(new OverlayStream(span, stream.MetaData), package, default);
                        case RecordTypeInts.ACHR:
                            return PlacedNpcBinaryOverlay.PlacedNpcFactory(new OverlayStream(span, stream.MetaData), package, default);
                        case RecordTypeInts.REFR:
                            return PlacedObjectBinaryOverlay.PlacedObjectFactory(new OverlayStream(span, stream.MetaData), package, default);
                        default:
                            throw new NotImplementedException();
                    }
                }

                GroupTypeEnum type = (GroupTypeEnum)subGroupMeta.GroupType;
                switch (type)
                {
                    case GroupTypeEnum.CellPersistentChildren:
                    {
                        this._persistentLocation = checked((int)subGroupLocation);
                        var contentSpan = stream.ReadMemory(checked((int)subGroupMeta.ContentLength));
                        this.Persistent = BinaryOverlayList.FactoryByArray<IPlacedGetter>(
                            contentSpan,
                            _package,
                            getter: TypicalGetter,
                            locs: ParseLocationsRecordPerTrigger(
                                stream: new OverlayStream(contentSpan, stream.MetaData),
                                triggers: TypicalPlacedTypes,
                                constants: GameConstants.Oblivion.MajorConstants,
                                skipHeader: false,
                                translationParams: default));
                        break;
                    }
                    case GroupTypeEnum.CellTemporaryChildren:
                    {
                        this._temporaryLocation = checked((int)subGroupLocation);
                        List<int> ret = new List<int>();
                        var subStartPos = stream.Position;
                        var endPos = stream.Position + subGroupMeta.ContentLength;
                        var contentSpan = stream.GetMemory(checked((int)subGroupMeta.ContentLength));
                        while (stream.Position < endPos)
                        {
                            var majorMeta = stream.GetMajorRecordHeader();
                            var recType = majorMeta.RecordType;
                            if (TypicalPlacedTypes.TriggeringRecordTypes.Contains(recType))
                            {
                                ret.Add(checked((int)(stream.Position - subStartPos)));
                            }
                            else
                            {
                                switch (recType.TypeInt)
                                {
                                    case RecordTypeInts.PGRD:
                                        _pathgridLocation = checked((int)stream.Position);
                                        break;
                                    case RecordTypeInts.LAND:
                                        _landscapeLocation = checked((int)stream.Position);
                                        break;
                                    default:
                                        break;
                                }
                            }
                            stream.Position += (int)majorMeta.TotalLength;
                        }
                        this.Temporary = BinaryOverlayList.FactoryByArray<IPlacedGetter>(
                            contentSpan,
                            _package,
                            getter: TypicalGetter,
                            locs: ret.ToArray());
                        break;
                    }
                    case GroupTypeEnum.CellVisibleDistantChildren:
                    {
                        this._visibleWhenDistantLocation = checked((int)subGroupLocation);
                        var contentSpan = stream.ReadMemory(checked((int)subGroupMeta.ContentLength));
                        this.VisibleWhenDistant = BinaryOverlayList.FactoryByArray<IPlacedGetter>(
                            contentSpan,
                            _package,
                            getter: TypicalGetter,
                            locs: ParseLocationsRecordPerTrigger(
                                stream: new OverlayStream(contentSpan, stream.MetaData),
                                triggers: TypicalPlacedTypes,
                                constants: GameConstants.Oblivion.MajorConstants,
                                skipHeader: false,
                                translationParams: default));
                        break;
                    }
                    default:
                        throw new NotImplementedException();
                }
            }
        }
        catch (Exception ex)
        {
            RecordException.EnrichAndThrow(ex, this);
            throw;
        }
    }
}
