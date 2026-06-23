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

namespace Mutagen.Bethesda.Fallout3;

public partial class Cell
{
    [Flags]
    public enum MajorFlag
    {
        Persistent = 0x0000_0400,
        OffLimits = 0x0002_0000,
        CantWait = 0x0008_0000,
    }

    [Flags]
    public enum Flag
    {
        IsInteriorCell = 0x0001,
        HasWater = 0x0002,
        InvertFastTravelBehavior = 0x0004,
        NoLODWater = 0x0008,
        PublicPlace = 0x0020,
        HandChanged = 0x0040,
        BehaveLikeExterior = 0x0080,
    }

    [Flags]
    public enum LightingInheritFlag
    {
        AmbientColor = 0x0001,
        DirectionalColor = 0x0002,
        FogColor = 0x0004,
        FogNear = 0x0008,
        FogFar = 0x0010,
        DirectionalRotation = 0x0020,
        DirectionalFade = 0x0040,
        ClipDistance = 0x0080,
        FogPower = 0x0100,
    }

    [Flags]
    public enum LandFlag
    {
        HideQuad1 = 0x01,
        HideQuad2 = 0x02,
        HideQuad3 = 0x04,
        HideQuad4 = 0x08,
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
            obj.UnknownGroupData = BinaryPrimitives.ReadInt32LittleEndian(groupMeta.HeaderData.Slice(groupMeta.HeaderData.Length - 4));
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
                        groupType: GroupTypeEnum.CellPersistentChildren);
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
                        groupType: GroupTypeEnum.CellVisibleDistantChildren);
                    break;
                default:
                    // Skip unknown group types
                    break;
            }
        }
    }

    static void ParseTypical(
        MutagenFrame frame,
        ICellInternal obj,
        IList<IPlaced> coll,
        GroupTypeEnum groupType)
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
        switch (groupType)
        {
            case GroupTypeEnum.CellPersistentChildren:
                obj.PersistentTimestamp = BinaryPrimitives.ReadInt32LittleEndian(groupMeta.LastModifiedData);
                obj.PersistentUnknownGroupData = BinaryPrimitives.ReadInt32LittleEndian(groupMeta.HeaderData.Slice(groupMeta.HeaderData.Length - 4));
                break;
            case GroupTypeEnum.CellVisibleDistantChildren:
                obj.VisibleWhenDistantTimestamp = BinaryPrimitives.ReadInt32LittleEndian(groupMeta.LastModifiedData);
                obj.VisibleWhenDistantUnknownGroupData = BinaryPrimitives.ReadInt32LittleEndian(groupMeta.HeaderData.Slice(groupMeta.HeaderData.Length - 4));
                break;
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
                        case RecordTypeInts.PGRE:
                            placed = PlacedGrenade.CreateFromBinary(r);
                            return true;
                        case RecordTypeInts.PMIS:
                            placed = PlacedMissile.CreateFromBinary(r);
                            return true;
                        case RecordTypeInts.PBEA:
                            placed = PlacedBeam.CreateFromBinary(r);
                            return true;
                        case RecordTypeInts.SCOL:
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
        if (nextHeader.Equals(RecordTypes.NAVM))
        {
            obj.NavigationMeshes.Add(
                NavigationMesh.CreateFromBinary(
                    frame.SpawnWithLength(majorMeta.TotalLength),
                    translationParams: null));
            return true;
        }
        else if (nextHeader.Equals(RecordTypes.LAND))
        {
            if (obj.Landscape != null)
            {
                throw new ArgumentException("Had more than one landscape");
            }
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
        obj.TemporaryUnknownGroupData = BinaryPrimitives.ReadInt32LittleEndian(groupMeta.HeaderData.Slice(groupMeta.HeaderData.Length - 4));
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
                    case RecordTypeInts.PGRE:
                        placed = PlacedGrenade.CreateFromBinary(r);
                        return true;
                    case RecordTypeInts.PMIS:
                        placed = PlacedMissile.CreateFromBinary(r);
                        return true;
                    case RecordTypeInts.PBEA:
                        placed = PlacedBeam.CreateFromBinary(r);
                        return true;
                    case RecordTypeInts.SCOL:
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
            var navMeshes = obj.NavigationMeshes;
            var landscape = obj.Landscape;
            if ((obj.Persistent?.Count ?? 0) == 0
                && (obj.Temporary?.Count ?? 0) == 0
                && (obj.VisibleWhenDistant?.Count ?? 0) == 0
                && navMeshes.Count == 0
                && landscape == null) return;
            using (HeaderExport.Header(writer, RecordTypes.GRUP, ObjectType.Group))
            {
                FormKeyBinaryTranslation.Instance.Write(
                    writer,
                    obj);
                writer.Write((int)GroupTypeEnum.CellChildren);
                writer.Write(obj.Timestamp);
                writer.Write(obj.UnknownGroupData);
                if (obj.Persistent?.Count > 0)
                {
                    using (HeaderExport.Header(writer, RecordTypes.GRUP, ObjectType.Group))
                    {
                        FormKeyBinaryTranslation.Instance.Write(
                            writer,
                            obj);
                        writer.Write((int)GroupTypeEnum.CellPersistentChildren);
                        writer.Write(obj.PersistentTimestamp);
                        writer.Write(obj.PersistentUnknownGroupData);
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
                    || navMeshes.Count > 0
                    || landscape != null)
                {
                    using (HeaderExport.Header(writer, RecordTypes.GRUP, ObjectType.Group))
                    {
                        FormKeyBinaryTranslation.Instance.Write(
                            writer,
                            obj);
                        writer.Write((int)GroupTypeEnum.CellTemporaryChildren);
                        writer.Write(obj.TemporaryTimestamp);
                        writer.Write(obj.TemporaryUnknownGroupData);
                        landscape?.WriteToBinary(writer);
                        foreach (var navMesh in navMeshes)
                        {
                            navMesh.WriteToBinary(writer);
                        }
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
                        writer.Write(obj.VisibleWhenDistantUnknownGroupData);
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
            RecordTypes.REFR,
            RecordTypes.PGRE,
            RecordTypes.PMIS,
            RecordTypes.PBEA,
            RecordTypes.SCOL));

    private ReadOnlyMemorySlice<byte>? _grupData;

    public IReadOnlyList<INavigationMeshGetter> NavigationMeshes { get; private set; } = Array.Empty<INavigationMeshGetter>();

    public int UnknownGroupData => _grupData.HasValue ? BinaryPrimitives.ReadInt32LittleEndian(_grupData.Value.Slice(20)) : default;

    private int? _landscapeLocation;
    public ILandscapeGetter? Landscape
    {
        get
        {
            if (!_landscapeLocation.HasValue) return default;
            try
            {
                return LandscapeBinaryOverlay.LandscapeFactory(new OverlayStream(_grupData!.Value.Slice(_landscapeLocation!.Value), _package), _package);
            }
            catch (System.IO.InvalidDataException)
            {
                // Some LAND records have corrupt compression - skip them gracefully
                return default;
            }
        }
    }

    public int Timestamp => _grupData != null ? BinaryPrimitives.ReadInt32LittleEndian(_package.MetaData.Constants.GroupHeader(_grupData.Value).LastModifiedData) : 0;

    private int? _persistentLocation;
    public int PersistentTimestamp => _persistentLocation.HasValue ? BinaryPrimitives.ReadInt32LittleEndian(_package.MetaData.Constants.GroupHeader(_grupData!.Value.Slice(_persistentLocation.Value)).LastModifiedData) : 0;
    public int PersistentUnknownGroupData => _persistentLocation.HasValue ? BinaryPrimitives.ReadInt32LittleEndian(_grupData!.Value.Slice(_persistentLocation.Value + 20)) : 0;
    public IReadOnlyList<IPlacedGetter> Persistent { get; private set; } = Array.Empty<IPlacedGetter>();

    private int? _temporaryLocation;
    public int TemporaryTimestamp => _temporaryLocation.HasValue ? BinaryPrimitives.ReadInt32LittleEndian(_package.MetaData.Constants.GroupHeader(_grupData!.Value.Slice(_temporaryLocation.Value)).LastModifiedData) : 0;
    public int TemporaryUnknownGroupData => _temporaryLocation.HasValue ? BinaryPrimitives.ReadInt32LittleEndian(_grupData!.Value.Slice(_temporaryLocation.Value + 20)) : 0;
    public IReadOnlyList<IPlacedGetter> Temporary { get; private set; } = Array.Empty<IPlacedGetter>();

    private int? _visibleWhenDistantLocation;
    public int VisibleWhenDistantTimestamp => _visibleWhenDistantLocation.HasValue ? BinaryPrimitives.ReadInt32LittleEndian(_package.MetaData.Constants.GroupHeader(_grupData!.Value.Slice(_visibleWhenDistantLocation.Value)).LastModifiedData) : 0;
    public int VisibleWhenDistantUnknownGroupData => _visibleWhenDistantLocation.HasValue ? BinaryPrimitives.ReadInt32LittleEndian(_grupData!.Value.Slice(_visibleWhenDistantLocation.Value + 20)) : 0;
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
                            return PlacedCreatureBinaryOverlay.PlacedCreatureFactory(new OverlayStream(span, package), package);
                        case RecordTypeInts.ACHR:
                            return PlacedNpcBinaryOverlay.PlacedNpcFactory(new OverlayStream(span, package), package);
                        case RecordTypeInts.REFR:
                            return PlacedObjectBinaryOverlay.PlacedObjectFactory(new OverlayStream(span, package), package);
                        case RecordTypeInts.PGRE:
                            return PlacedGrenadeBinaryOverlay.PlacedGrenadeFactory(new OverlayStream(span, package), package);
                        case RecordTypeInts.PMIS:
                            return PlacedMissileBinaryOverlay.PlacedMissileFactory(new OverlayStream(span, package), package);
                        case RecordTypeInts.PBEA:
                            return PlacedBeamBinaryOverlay.PlacedBeamFactory(new OverlayStream(span, package), package);
                        case RecordTypeInts.SCOL:
                            return PlacedObjectBinaryOverlay.PlacedObjectFactory(new OverlayStream(span, package), package);
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
                                stream: new OverlayStream(contentSpan, _package),
                                triggers: TypicalPlacedTypes,
                                constants: stream.MetaData.Constants.MajorConstants,
                                skipHeader: false));
                        break;
                    }
                    case GroupTypeEnum.CellTemporaryChildren:
                    {
                        this._temporaryLocation = checked((int)subGroupLocation);
                        List<int> ret = new List<int>();
                        List<INavigationMeshGetter> navMeshAccumulator = new();
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
                                stream.Position += (int)majorMeta.TotalLength;
                            }
                            else
                            {
                                switch (recType.TypeInt)
                                {
                                    case RecordTypeInts.NAVM:
                                    {
                                        var navStartMem = stream.RemainingMemory;
                                        var navLocs = ParseRecordLocations(
                                            stream: stream,
                                            constants: _package.MetaData.Constants.MajorConstants,
                                            trigger: recType,
                                            skipHeader: false);
                                        foreach (var loc in navLocs)
                                        {
                                            navMeshAccumulator.Add(
                                                NavigationMeshBinaryOverlay.NavigationMeshFactory(
                                                    new OverlayStream(navStartMem.Slice(loc), _package),
                                                    _package));
                                        }
                                        break;
                                    }
                                    case RecordTypeInts.LAND:
                                        _landscapeLocation = checked((int)stream.Position);
                                        stream.Position += (int)majorMeta.TotalLength;
                                        break;
                                    default:
                                        // Skip unknown record types gracefully
                                        stream.Position += (int)majorMeta.TotalLength;
                                        break;
                                }
                            }
                        }
                        if (navMeshAccumulator.Count > 0)
                            this.NavigationMeshes = navMeshAccumulator;
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
                                stream: new OverlayStream(contentSpan, _package),
                                triggers: TypicalPlacedTypes,
                                constants: stream.MetaData.Constants.MajorConstants,
                                skipHeader: false));
                        break;
                    }
                    default:
                        // Skip unknown group types gracefully
                        stream.Position += checked((int)subGroupMeta.TotalLength);
                        break;
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
