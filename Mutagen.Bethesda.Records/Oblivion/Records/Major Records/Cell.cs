using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using CSharpExt.Rx;
using Loqui;
using Loqui.Internal;
using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Internals;
using Mutagen.Bethesda.Oblivion.Internals;
using Noggog;
using Noggog.Notifying;

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
        public partial class CellCommon
        {
            partial void PostDuplicate(Cell obj, Cell rhs, Func<FormKey> getNextFormKey, IList<(IMajorRecordCommon Record, FormKey OriginalFormKey)> duplicatedRecords)
            {
                if (rhs.PathGrid_IsSet
                    && rhs.PathGrid != null)
                {
                    obj.PathGrid = (PathGrid)rhs.PathGrid.Duplicate(getNextFormKey, duplicatedRecords);
                }
                if (rhs.Landscape_IsSet
                    && rhs.Landscape != null)
                {
                    obj.Landscape = (Landscape)rhs.Landscape.Duplicate(getNextFormKey, duplicatedRecords);
                }
                obj.Persistent.SetTo(rhs.Persistent.Select((i) => (IPlaced)i.Duplicate(getNextFormKey, duplicatedRecords)));
                obj.Temporary.SetTo(rhs.Temporary.Select((i) => (IPlaced)i.Duplicate(getNextFormKey, duplicatedRecords)));
                obj.VisibleWhenDistant.SetTo(rhs.VisibleWhenDistant.Select((i) => (IPlaced)i.Duplicate(getNextFormKey)));
            }
        }

        public partial class CellBinaryCreateTranslation
        {
            public static async Task CustomBinaryEndImport(MutagenFrame frame, ICellInternal obj, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
            {
                CustomBinaryEnd(frame, obj, masterReferences, errorMask);
            }

            private static void CustomBinaryEnd(MutagenFrame frame, ICellInternal obj, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
            {
                if (frame.Reader.Complete) return;
                var groupMeta = frame.MetaData.GetGroup(frame);
                if (!groupMeta.IsGroup) return;
                var formKey = FormKey.Factory(masterReferences, BinaryPrimitives.ReadUInt32LittleEndian(groupMeta.ContainedRecordTypeSpan));
                if (groupMeta.GroupType == (int)GroupTypeEnum.CellChildren)
                {
                    obj.Timestamp = groupMeta.LastModifiedSpan.ToArray();
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
                    var persistGroupMeta = frame.MetaData.GetGroup(frame);
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
                                fieldIndex: (int)Cell_FieldIndex.Persistent,
                                masterReferences: masterReferences,
                                coll: obj.Persistent,
                                errorMask: errorMask,
                                persistentParse: true);
                            break;
                        case GroupTypeEnum.CellTemporaryChildren:
                            ParseTemporary(
                                itemFrame,
                                obj,
                                masterReferences,
                                errorMask);
                            break;
                        case GroupTypeEnum.CellVisibleDistantChildren:
                            ParseTypical(
                                frame: itemFrame,
                                obj: obj,
                                fieldIndex: (int)Cell_FieldIndex.VisibleWhenDistant,
                                masterReferences: masterReferences,
                                coll: obj.VisibleWhenDistant,
                                errorMask: errorMask,
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
                int fieldIndex,
                MasterReferences masterReferences,
                ISetList<IPlaced> coll,
                ErrorMaskBuilder errorMask,
                bool persistentParse)
            {
                var groupMeta = frame.MetaData.ReadGroup(frame);
                var formKey = FormKey.Factory(masterReferences, BinaryPrimitives.ReadUInt32LittleEndian(groupMeta.ContainedRecordTypeSpan));
                if (formKey != obj.FormKey)
                {
                    throw new ArgumentException("Cell children group did not match the FormID of the parent cell.");
                }
                if (persistentParse)
                {
                    obj.PersistentTimestamp = groupMeta.LastModifiedSpan.ToArray();
                }
                else
                {
                    obj.VisibleWhenDistantTimestamp = groupMeta.LastModifiedSpan.ToArray();
                }
                Mutagen.Bethesda.Binary.ListBinaryTranslation<IPlaced>.Instance.ParseRepeatedItem(
                    frame: frame,
                    fieldIndex: fieldIndex,
                    item: coll,
                    lengthLength: frame.MetaData.MajorConstants.LengthLength,
                    errorMask: errorMask,
                    transl: (MutagenFrame r, RecordType header, out IPlaced placed, ErrorMaskBuilder errMaskInternal) =>
                    {
                        switch (header.TypeInt)
                        {
                            case 0x45524341: // "ACRE":
                                if (LoquiBinaryTranslation<PlacedCreature>.Instance.Parse(
                                        frame: r,
                                        item: out var placedCrea,
                                        masterReferences: masterReferences,
                                        errorMask: errMaskInternal))
                                {
                                    placed = placedCrea;
                                    return true;
                                }
                                break;
                            case 0x52484341: //"ACHR":
                                if (LoquiBinaryTranslation<PlacedNPC>.Instance.Parse(
                                        frame: r,
                                        item: out var placedNPC,
                                        masterReferences: masterReferences,
                                        errorMask: errMaskInternal))
                                {
                                    placed = placedNPC;
                                    return true;
                                }
                                break;
                            case 0x52464552: // "REFR":
                                if (LoquiBinaryTranslation<PlacedObject>.Instance.Parse(
                                        frame: r,
                                        item: out var placedObj,
                                        masterReferences: masterReferences,
                                        errorMask: errMaskInternal))
                                {
                                    placed = placedObj;
                                    return true;
                                }
                                break;
                            default:
                                throw new NotImplementedException();
                        }
                        placed = null;
                        return false;
                    }
                    );
            }

            static bool ParseTemporaryOutliers(MutagenFrame frame, ICellInternal obj, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
            {
                var majorMeta = frame.MetaData.GetMajorRecord(frame);
                var nextHeader = majorMeta.RecordType;
                if (nextHeader.Equals(PathGrid_Registration.PGRD_HEADER))
                {
                    using (errorMask.PushIndex((int)Cell_FieldIndex.PathGrid))
                    {
                        obj.PathGrid = PathGrid.CreateFromBinary(
                            frame.SpawnWithLength(majorMeta.TotalLength),
                            errorMask: errorMask,
                            masterReferences: masterReferences,
                            recordTypeConverter: null);
                    }
                    return true;
                }
                else if (nextHeader.Equals(Landscape_Registration.LAND_HEADER))
                {
                    using (errorMask.PushIndex((int)Cell_FieldIndex.Landscape))
                    {
                        obj.Landscape = Landscape.CreateFromBinary(
                            frame.SpawnWithLength(majorMeta.TotalLength),
                            errorMask: errorMask,
                            masterReferences: masterReferences,
                            recordTypeConverter: null);
                    }
                    return true;
                }
                return false;
            }

            static void ParseTemporary(MutagenFrame frame, ICellInternal obj, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
            {
                var groupMeta = frame.MetaData.ReadGroup(frame);
                var formKey = FormKey.Factory(masterReferences, BinaryPrimitives.ReadUInt32LittleEndian(groupMeta.ContainedRecordTypeSpan));
                if (formKey != obj.FormKey)
                {
                    throw new ArgumentException("Cell children group did not match the FormID of the parent cell.");
                }
                obj.TemporaryTimestamp = groupMeta.LastModifiedSpan.ToArray();
                Mutagen.Bethesda.Binary.ListBinaryTranslation<IPlaced>.Instance.ParseRepeatedItem(
                    frame: frame,
                    item: obj.Temporary,
                    fieldIndex: (int)Cell_FieldIndex.Persistent,
                    lengthLength: frame.MetaData.MajorConstants.LengthLength,
                    errorMask: errorMask,
                    transl: (MutagenFrame r, RecordType header, out IPlaced placed, ErrorMaskBuilder listSubMask) =>
                    {
                        switch (header.TypeInt)
                        {
                            case 0x45524341: // "ACRE":
                                if (LoquiBinaryTranslation<PlacedCreature>.Instance.Parse(
                                        frame: r,
                                        item: out var placedCrea,
                                        masterReferences: masterReferences,
                                        errorMask: errorMask))
                                {
                                    placed = placedCrea;
                                    return true;
                                }
                                break;
                            case 0x52484341: //"ACHR":
                                if (LoquiBinaryTranslation<PlacedNPC>.Instance.Parse(
                                        frame: r,
                                        item: out var placedNPC,
                                        masterReferences: masterReferences,
                                        errorMask: errorMask))
                                {
                                    placed = placedNPC;
                                    return true;
                                }
                                break;
                            case 0x52464552: // "REFR":
                                if (LoquiBinaryTranslation<PlacedObject>.Instance.Parse(
                                        frame: r,
                                        item: out var placedObj,
                                        masterReferences: masterReferences,
                                        errorMask: errorMask))
                                {
                                    placed = placedObj;
                                    return true;
                                }
                                break;
                            default:
                                if (ParseTemporaryOutliers(frame, obj, masterReferences, errorMask))
                                {
                                    listSubMask = null;
                                    placed = null;
                                    return false;
                                }
                                throw new NotImplementedException();
                        }
                        placed = null;
                        return false;
                    });
            }
        }

        public partial class CellBinaryWriteTranslation
        {
            static partial void CustomBinaryEndExport(MutagenWriter writer, ICellGetter obj, MasterReferences masterReferences, ErrorMaskBuilder errorMask)
            {
                if (obj.Persistent.Count == 0
                    && obj.Temporary.Count == 0
                    && obj.VisibleWhenDistant.Count == 0
                    && !obj.PathGrid_IsSet
                    && !obj.Landscape_IsSet) return;
                using (HeaderExport.ExportHeader(writer, Group_Registration.GRUP_HEADER, ObjectType.Group))
                {
                    FormKeyBinaryTranslation.Instance.Write(
                        writer,
                        obj.FormKey,
                        masterReferences);
                    writer.Write((int)GroupTypeEnum.CellChildren);
                    writer.Write(obj.Timestamp);
                    if (obj.Persistent.Count > 0)
                    {
                        using (HeaderExport.ExportHeader(writer, Group_Registration.GRUP_HEADER, ObjectType.Group))
                        {
                            FormKeyBinaryTranslation.Instance.Write(
                                writer,
                                obj.FormKey,
                                masterReferences);
                            writer.Write((int)GroupTypeEnum.CellPersistentChildren);
                            writer.Write(obj.PersistentTimestamp);
                            Mutagen.Bethesda.Binary.ListBinaryTranslation<IPlacedGetter>.Instance.Write(
                                writer: writer,
                                items: obj.Persistent,
                                fieldIndex: (int)Cell_FieldIndex.Persistent,
                                errorMask: errorMask,
                                transl: (r, item, subErr) =>
                                {
                                    item.WriteToBinary(
                                        r,
                                        masterReferences,
                                        errorMask);
                                });
                        }
                    }
                    if (obj.Temporary.Count > 0
                        || obj.PathGrid_IsSet
                        || obj.Landscape_IsSet)
                    {
                        using (HeaderExport.ExportHeader(writer, Group_Registration.GRUP_HEADER, ObjectType.Group))
                        {
                            FormKeyBinaryTranslation.Instance.Write(
                                writer,
                                obj.FormKey,
                                masterReferences);
                            writer.Write((int)GroupTypeEnum.CellTemporaryChildren);
                            writer.Write(obj.TemporaryTimestamp);
                            if (obj.Landscape_IsSet)
                            {
                                obj.Landscape.WriteToBinary(
                                    writer,
                                    masterReferences: masterReferences,
                                    errorMask: errorMask);
                            }
                            if (obj.PathGrid_IsSet)
                            {
                                obj.PathGrid.WriteToBinary(
                                    writer,
                                    masterReferences: masterReferences,
                                    errorMask: errorMask);
                            }
                            Mutagen.Bethesda.Binary.ListBinaryTranslation<IPlacedGetter>.Instance.Write(
                                writer: writer,
                                items: obj.Temporary,
                                fieldIndex: (int)Cell_FieldIndex.Temporary,
                                errorMask: errorMask,
                                transl: (r, item, subErr) =>
                                {
                                    item.WriteToBinary(
                                        r,
                                        masterReferences,
                                        errorMask);
                                });
                        }
                    }
                    if (obj.VisibleWhenDistant.Count > 0)
                    {
                        using (HeaderExport.ExportHeader(writer, Group_Registration.GRUP_HEADER, ObjectType.Group))
                        {
                            FormKeyBinaryTranslation.Instance.Write(
                                writer,
                                obj.FormKey,
                                masterReferences);
                            writer.Write((int)GroupTypeEnum.CellVisibleDistantChildren);
                            writer.Write(obj.VisibleWhenDistantTimestamp);
                            Mutagen.Bethesda.Binary.ListBinaryTranslation<IPlacedGetter>.Instance.Write(
                                writer: writer,
                                items: obj.VisibleWhenDistant,
                                fieldIndex: (int)Cell_FieldIndex.VisibleWhenDistant,
                                errorMask: errorMask,
                                transl: (r, item, subErr) =>
                                {
                                    item.WriteToBinary(
                                        r,
                                        masterReferences,
                                        errorMask);
                                });
                        }
                    }
                }
            }
        }

        public partial class CellBinaryOverlay
        {
            static readonly HashSet<RecordType> TypicalPlacedTypes = new HashSet<RecordType>()
            {
                Cell_Registration.ACHR_HEADER,
                Cell_Registration.ACRE_HEADER,
                Cell_Registration.REFR_HEADER
            };

            private ReadOnlyMemorySlice<byte>? _grupData;

            private int? _pathgridLocation;
            public bool PathGrid_IsSet => _pathgridLocation.HasValue;
            public IPathGridGetter PathGrid => PathGridBinaryOverlay.PathGridFactory(new BinaryMemoryReadStream(_grupData.Value.Slice(_pathgridLocation.Value)), _package);

            private int? _landscapeLocation;
            public bool Landscape_IsSet => _landscapeLocation.HasValue;
            public ILandscapeGetter Landscape => LandscapeBinaryOverlay.LandscapeFactory(new BinaryMemoryReadStream(_grupData.Value.Slice(_landscapeLocation.Value)), _package);

            public ReadOnlySpan<byte> Timestamp => _grupData != null ? _package.Meta.Group(_grupData.Value).LastModifiedSpan : UtilityTranslation.Zeros.Slice(0, 4);

            private int? _persistentLocation;
            public ReadOnlySpan<byte> PersistentTimestamp => _persistentLocation.HasValue ? _package.Meta.Group(_grupData.Value.Slice(_persistentLocation.Value)).LastModifiedSpan : UtilityTranslation.Zeros.Slice(0, 4);
            public IReadOnlySetList<IPlacedGetter> Persistent { get; private set; } = EmptySetList<IPlacedGetter>.Instance;

            private int? _temporaryLocation;
            public ReadOnlySpan<byte> TemporaryTimestamp => _temporaryLocation.HasValue ? _package.Meta.Group(_grupData.Value.Slice(_temporaryLocation.Value)).LastModifiedSpan : UtilityTranslation.Zeros.Slice(0, 4);
            public IReadOnlySetList<IPlacedGetter> Temporary { get; private set; } = EmptySetList<IPlacedGetter>.Instance;

            private int? _visibleWhenDistantLocation;
            public ReadOnlySpan<byte> VisibleWhenDistantTimestamp => _visibleWhenDistantLocation.HasValue ? _package.Meta.Group(_grupData.Value.Slice(_visibleWhenDistantLocation.Value)).LastModifiedSpan : UtilityTranslation.Zeros.Slice(0, 4);
            public IReadOnlySetList<IPlacedGetter> VisibleWhenDistant { get; private set; } = EmptySetList<IPlacedGetter>.Instance;

            public static int[] ParseRecordLocations(BinaryMemoryReadStream stream, BinaryOverlayFactoryPackage package)
            {
                List<int> ret = new List<int>();
                var startingPos = stream.Position;
                while (!stream.Complete)
                {
                    var cellMeta = package.Meta.GetMajorRecord(stream);
                    if (cellMeta.RecordType != Cell_Registration.CELL_HEADER) break;
                    ret.Add(stream.Position - startingPos);
                    stream.Position += (int)cellMeta.TotalLength;
                    if (stream.Complete) break;
                    var grupMeta = package.Meta.GetGroup(stream);
                    if (grupMeta.IsGroup && (grupMeta.GroupType == (int)GroupTypeEnum.CellChildren))
                    {
                        stream.Position += (int)grupMeta.TotalLength;
                    }
                }
                return ret.ToArray();
            }

            partial void CustomEnd(IBinaryReadStream stream, int finalPos, int _)
            {
                if (stream.Complete) return;
                var startPos = stream.Position;
                var groupMeta = this._package.Meta.GetGroup(stream);
                if (!groupMeta.IsGroup) return;
                var formKey = FormKey.Factory(_package.MasterReferences, BinaryPrimitives.ReadUInt32LittleEndian(groupMeta.ContainedRecordTypeSpan));
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
                stream = new BinaryMemoryReadStream(this._grupData.Value);
                stream.Position += groupMeta.HeaderLength;
                while (!stream.Complete)
                {
                    var subGroupLocation = stream.Position;
                    var subGroupMeta = this._package.Meta.ReadGroup(stream);
                    if (!subGroupMeta.IsGroup)
                    {
                        throw new ArgumentException();
                    }

                    IPlacedGetter TypicalGetter(
                        ReadOnlyMemorySlice<byte> span,
                        BinaryOverlayFactoryPackage package)
                    {
                        var majorMeta = package.Meta.MajorRecord(span);
                        switch (majorMeta.RecordType.TypeInt)
                        {
                            case 0x45524341: // "ACRE":
                                return PlacedCreatureBinaryOverlay.PlacedCreatureFactory(new BinaryMemoryReadStream(span), package);
                            case 0x52484341: // "ACHR":
                                return PlacedNPCBinaryOverlay.PlacedNPCFactory(new BinaryMemoryReadStream(span), package);
                            case 0x52464552: // "REFR":
                                return PlacedObjectBinaryOverlay.PlacedObjectFactory(new BinaryMemoryReadStream(span), package);
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
                                this.Persistent = BinaryOverlaySetList<IPlacedGetter>.FactoryByArray(
                                    contentSpan,
                                    _package,
                                    getter: TypicalGetter,
                                    locs: ParseRecordLocations(
                                        stream: new BinaryMemoryReadStream(contentSpan),
                                        finalPos: subGroupLocation + subGroupMeta.TotalLength,
                                        triggers: TypicalPlacedTypes,
                                        constants: MetaDataConstants.Oblivion.MajorConstants,
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
                                    var majorMeta = _package.Meta.GetMajorRecord(stream);
                                    var recType = majorMeta.RecordType;
                                    if (TypicalPlacedTypes.Contains(recType))
                                    {
                                        ret.Add(checked((int)(stream.Position - subStartPos)));
                                    }
                                    else
                                    {
                                        switch (recType.TypeInt)
                                        {
                                            case 0x44524750: // PGRD
                                                if (_pathgridLocation.HasValue)
                                                {
                                                    throw new ArgumentException("Second pathgrid parsed.");
                                                }
                                                _pathgridLocation = checked((int)stream.Position);
                                                break;
                                            case 0x444e414c: // LAND
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
                                this.Temporary = BinaryOverlaySetList<IPlacedGetter>.FactoryByArray(
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
                                this.VisibleWhenDistant = BinaryOverlaySetList<IPlacedGetter>.FactoryByArray(
                                    contentSpan,
                                    _package,
                                    getter: TypicalGetter,
                                    locs: ParseRecordLocations(
                                        stream: new BinaryMemoryReadStream(contentSpan),
                                        finalPos: subGroupLocation + subGroupMeta.TotalLength,
                                        triggers: TypicalPlacedTypes,
                                        constants: MetaDataConstants.Oblivion.MajorConstants,
                                        skipHeader: false));
                                break;
                            }
                        default:
                            throw new NotImplementedException();
                    }
                }
            }
        }
    }
}