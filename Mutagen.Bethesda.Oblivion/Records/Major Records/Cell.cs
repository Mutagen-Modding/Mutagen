using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Loqui.Internal;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Constants;
using Mutagen.Bethesda.Internals;
using Noggog;

namespace Mutagen.Bethesda.Oblivion
{
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

    namespace Internals
    {
        public partial class CellBinaryCreateTranslation
        {
            static partial void CustomBinaryEndImport(MutagenFrame frame, ICellInternal obj)
            {
                try
                {
                    CustomBinaryEnd(frame, obj);
                }
                catch (Exception ex)
                {
                    throw RecordException.Enrich(ex, obj);
                }
            }

            private static void CustomBinaryEnd(MutagenFrame frame, ICellInternal obj)
            {
                if (frame.Reader.Complete) return;
                if (!frame.TryGetGroup(out var groupMeta)) return;
                var formKey = FormKey.Factory(frame.MetaData.MasterReferences!, BinaryPrimitives.ReadUInt32LittleEndian(groupMeta.ContainedRecordTypeData));
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
                    var persistGroupMeta = frame.GetGroup();
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
                var groupMeta = frame.ReadGroup();
                var formKey = FormKey.Factory(frame.MetaData.MasterReferences!, BinaryPrimitives.ReadUInt32LittleEndian(groupMeta.ContainedRecordTypeData));
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
                    Mutagen.Bethesda.Binary.ListBinaryTranslation<IPlaced>.Instance.Parse(
                        frame: frame,
                        transl: (MutagenFrame r, RecordType header, out IPlaced placed) =>
                        {
                            switch (header.TypeInt)
                            {
                                case 0x45524341: // "ACRE":
                                    placed = PlacedCreature.CreateFromBinary(r);
                                    return true;
                                case 0x52484341: //"ACHR":
                                    placed = PlacedNpc.CreateFromBinary(r);
                                    return true;
                                case 0x52464552: // "REFR":
                                    placed = PlacedObject.CreateFromBinary(r);
                                    return true;
                                default:
                                    throw new NotImplementedException();
                            }
                        }));
            }

            static bool ParseTemporaryOutliers(MutagenFrame frame, ICellInternal obj)
            {
                var majorMeta = frame.GetMajorRecord();
                var nextHeader = majorMeta.RecordType;
                if (nextHeader.Equals(RecordTypes.PGRD))
                {
                    obj.PathGrid = PathGrid.CreateFromBinary(
                        frame.SpawnWithLength(majorMeta.TotalLength),
                        recordTypeConverter: null);
                    return true;
                }
                else if (nextHeader.Equals(RecordTypes.LAND))
                {
                    obj.Landscape = Landscape.CreateFromBinary(
                        frame.SpawnWithLength(majorMeta.TotalLength),
                        recordTypeConverter: null);
                    return true;
                }
                return false;
            }

            static void ParseTemporary(MutagenFrame frame, ICellInternal obj)
            {
                var groupMeta = frame.ReadGroup();
                var formKey = FormKey.Factory(frame.MetaData.MasterReferences!, BinaryPrimitives.ReadUInt32LittleEndian(groupMeta.ContainedRecordTypeData));
                if (formKey != obj.FormKey)
                {
                    throw new ArgumentException("Cell children group did not match the FormID of the parent cell.");
                }
                obj.TemporaryTimestamp = BinaryPrimitives.ReadInt32LittleEndian(groupMeta.LastModifiedData);
                var items = Mutagen.Bethesda.Binary.ListBinaryTranslation<IPlaced>.Instance.Parse(
                    frame: frame,
                    transl: (MutagenFrame r, RecordType header, out IPlaced placed) =>
                    {
                        switch (header.TypeInt)
                        {
                            case 0x45524341: // "ACRE":
                                placed = PlacedCreature.CreateFromBinary(r);
                                return true;
                            case 0x52484341: //"ACHR":
                                placed = PlacedNpc.CreateFromBinary(r);
                                return true;
                            case 0x52464552: // "REFR":
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

        public partial class CellBinaryWriteTranslation
        {
            static partial void CustomBinaryEndExport(MutagenWriter writer, ICellGetter obj)
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
                            obj.FormKey);
                        writer.Write((int)GroupTypeEnum.CellChildren);
                        writer.Write(obj.Timestamp);
                        if (obj.Persistent?.Count > 0)
                        {
                            using (HeaderExport.Header(writer, RecordTypes.GRUP, ObjectType.Group))
                            {
                                FormKeyBinaryTranslation.Instance.Write(
                                    writer,
                                    obj.FormKey);
                                writer.Write((int)GroupTypeEnum.CellPersistentChildren);
                                writer.Write(obj.PersistentTimestamp);
                                Mutagen.Bethesda.Binary.ListBinaryTranslation<IPlacedGetter>.Instance.Write(
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
                                    obj.FormKey);
                                writer.Write((int)GroupTypeEnum.CellTemporaryChildren);
                                writer.Write(obj.TemporaryTimestamp);
                                landscape?.WriteToBinary(writer);
                                pathGrid?.WriteToBinary(writer);
                                if (obj.Temporary != null)
                                {
                                    Mutagen.Bethesda.Binary.ListBinaryTranslation<IPlacedGetter>.Instance.Write(
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
                                    obj.FormKey);
                                writer.Write((int)GroupTypeEnum.CellVisibleDistantChildren);
                                writer.Write(obj.VisibleWhenDistantTimestamp);
                                Mutagen.Bethesda.Binary.ListBinaryTranslation<IPlacedGetter>.Instance.Write(
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
                    throw RecordException.Enrich(ex, obj);
                }
            }
        }

        public partial class CellBinaryOverlay
        {
            static readonly ICollectionGetter<RecordType> TypicalPlacedTypes = new CollectionGetterWrapper<RecordType>(
                new HashSet<RecordType>()
                {
                    RecordTypes.ACHR,
                    RecordTypes.ACRE,
                    RecordTypes.REFR
                });

            private ReadOnlyMemorySlice<byte>? _grupData;

            private int? _pathgridLocation;
            public IPathGridGetter? PathGrid => _pathgridLocation.HasValue ? PathGridBinaryOverlay.PathGridFactory(new OverlayStream(_grupData!.Value.Slice(_pathgridLocation!.Value), _package), _package) : default;

            private int? _landscapeLocation;
            public ILandscapeGetter? Landscape => _landscapeLocation.HasValue ? LandscapeBinaryOverlay.LandscapeFactory(new OverlayStream(_grupData!.Value.Slice(_landscapeLocation!.Value), _package), _package) : default;

            public int Timestamp => _grupData != null ? BinaryPrimitives.ReadInt32LittleEndian(_package.MetaData.Constants.Group(_grupData.Value).LastModifiedData) : 0;

            private int? _persistentLocation;
            public int PersistentTimestamp => _persistentLocation.HasValue ? BinaryPrimitives.ReadInt32LittleEndian(_package.MetaData.Constants.Group(_grupData!.Value.Slice(_persistentLocation.Value)).LastModifiedData) : 0;
            public IReadOnlyList<IPlacedGetter> Persistent { get; private set; } = ListExt.Empty<IPlacedGetter>();

            private int? _temporaryLocation;
            public int TemporaryTimestamp => _temporaryLocation.HasValue ? BinaryPrimitives.ReadInt32LittleEndian(_package.MetaData.Constants.Group(_grupData!.Value.Slice(_temporaryLocation.Value)).LastModifiedData) : 0;
            public IReadOnlyList<IPlacedGetter> Temporary { get; private set; } = ListExt.Empty<IPlacedGetter>();

            private int? _visibleWhenDistantLocation;
            public int VisibleWhenDistantTimestamp => _visibleWhenDistantLocation.HasValue ? BinaryPrimitives.ReadInt32LittleEndian(_package.MetaData.Constants.Group(_grupData!.Value.Slice(_visibleWhenDistantLocation.Value)).LastModifiedData) : 0;
            public IReadOnlyList<IPlacedGetter> VisibleWhenDistant { get; private set; } = ListExt.Empty<IPlacedGetter>();

            public static int[] ParseRecordLocations(OverlayStream stream, BinaryOverlayFactoryPackage package)
            {
                List<int> ret = new List<int>();
                var startingPos = stream.Position;
                while (!stream.Complete)
                {
                    var cellMeta = stream.GetMajorRecord();
                    if (cellMeta.RecordType != RecordTypes.CELL) break;
                    ret.Add(stream.Position - startingPos);
                    stream.Position += (int)cellMeta.TotalLength;
                    if (stream.Complete) break;
                    if (!stream.TryGetGroup(out var groupFrame)) continue;
                    if (groupFrame.GroupType == (int)GroupTypeEnum.CellChildren)
                    {
                        stream.Position += (int)groupFrame.TotalLength;
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
                    if (!stream.TryGetGroup(out var groupMeta)) return;
                    var formKey = FormKey.Factory(_package.MetaData.MasterReferences!, BinaryPrimitives.ReadUInt32LittleEndian(groupMeta.ContainedRecordTypeData));
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
                        var subGroupMeta = stream.ReadGroup();
                        if (!subGroupMeta.IsGroup)
                        {
                            throw new ArgumentException();
                        }

                        IPlacedGetter TypicalGetter(
                            ReadOnlyMemorySlice<byte> span,
                            BinaryOverlayFactoryPackage package)
                        {
                            var majorMeta = package.MetaData.Constants.MajorRecord(span);
                            switch (majorMeta.RecordType.TypeInt)
                            {
                                case RecordTypeInts.ACRE:
                                    return PlacedCreatureBinaryOverlay.PlacedCreatureFactory(new OverlayStream(span, stream.MetaData), package);
                                case RecordTypeInts.ACHR:
                                    return PlacedNpcBinaryOverlay.PlacedNpcFactory(new OverlayStream(span, stream.MetaData), package);
                                case RecordTypeInts.REFR:
                                    return PlacedObjectBinaryOverlay.PlacedObjectFactory(new OverlayStream(span, stream.MetaData), package);
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
                                        locs: ParseRecordLocations(
                                            stream: new OverlayStream(contentSpan, stream.MetaData),
                                            triggers: TypicalPlacedTypes,
                                            constants: GameConstants.Oblivion.MajorConstants,
                                            skipHeader: false));
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
                                        var majorMeta = stream.GetMajorRecord();
                                        var recType = majorMeta.RecordType;
                                        if (TypicalPlacedTypes.Contains(recType))
                                        {
                                            ret.Add(checked((int)(stream.Position - subStartPos)));
                                        }
                                        else
                                        {
                                            switch (recType.TypeInt)
                                            {
                                                case RecordTypeInts.PGRD:
                                                    if (_pathgridLocation.HasValue)
                                                    {
                                                        throw new ArgumentException("Second pathgrid parsed.");
                                                    }
                                                    _pathgridLocation = checked((int)stream.Position);
                                                    break;
                                                case RecordTypeInts.LAND:
                                                    if (_landscapeLocation.HasValue)
                                                    {
                                                        throw new ArgumentException("Second landscape parsed.");
                                                    }
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
                                        locs: ParseRecordLocations(
                                            stream: new OverlayStream(contentSpan, stream.MetaData),
                                            triggers: TypicalPlacedTypes,
                                            constants: GameConstants.Oblivion.MajorConstants,
                                            skipHeader: false));
                                    break;
                                }
                            default:
                                throw new NotImplementedException();
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw RecordException.Enrich(ex, this);
                }
            }
        }
    }
}
