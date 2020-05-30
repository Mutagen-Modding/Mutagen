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
    public partial class Cell
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
            Persistent = 0x0000_0400,
            OffLimits = 0x0002_0000,
            CantWait = 0x0008_0000,
        }

        [Flags]
        public enum Flag
        {
            IsInteriorCell = 0x0001,
            HasWater = 0x0002,
            CantTravelFromHere = 0x0004,
            NoLodWater = 0x0008,
            PublicArea = 0x0020,
            HandChanged = 0x0040,
            ShowSky = 0x0080,
            UseSkyLighting = 0x0100,
        }
    }


    namespace Internals
    {
        public partial class CellCommon
        {
            partial void PostDuplicate(Cell obj, Cell rhs, Func<FormKey> getNextFormKey, IList<(IMajorRecordCommon Record, FormKey OriginalFormKey)>? duplicatedRecords)
            {
                obj.Temporary.SetTo(new ExtendedList<IPlaced>(rhs.Temporary.Select((i) => (IPlaced)i.Duplicate(getNextFormKey, duplicatedRecords))));
            }
        }

        public partial class CellBinaryCreateTranslation
        {
            static partial void CustomBinaryEndImport(MutagenFrame frame, ICellInternal obj)
            {
                CustomBinaryEnd(frame, obj);
            }

            private static void CustomBinaryEnd(MutagenFrame frame, ICellInternal obj)
            {
                if (frame.Reader.Complete) return;
                var groupMeta = frame.GetGroup();
                if (!groupMeta.IsGroup) return;
                var formKey = FormKey.Factory(frame.MetaData.MasterReferences!, BinaryPrimitives.ReadUInt32LittleEndian(groupMeta.ContainedRecordTypeSpan));
                if (groupMeta.GroupType == (int)GroupTypeEnum.CellChildren)
                {
                    obj.Timestamp = BinaryPrimitives.ReadInt32LittleEndian(groupMeta.LastModifiedSpan);
                    obj.UnknownGroupData = BinaryPrimitives.ReadInt32LittleEndian(groupMeta.Span.Slice(groupMeta.Span.Length - 4));
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
                        case GroupTypeEnum.CellTemporaryChildren:
                            ParseTemporary(
                                itemFrame,
                                obj);
                            break;
                        case GroupTypeEnum.CellPersistentChildren:
                            ParseTypical(
                                itemFrame,
                                obj);
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }
            }

            static void ParseTypical(
                MutagenFrame frame,
                ICellInternal obj)
            {
                var groupMeta = frame.ReadGroup();
                var formKey = FormKey.Factory(frame.MetaData.MasterReferences!, BinaryPrimitives.ReadUInt32LittleEndian(groupMeta.ContainedRecordTypeSpan));
                if (formKey != obj.FormKey)
                {
                    throw new ArgumentException("Cell children group did not match the FormID of the parent cell.");
                }
                obj.PersistentTimestamp = BinaryPrimitives.ReadInt32LittleEndian(groupMeta.LastModifiedSpan);
                obj.PersistentUnknownGroupData = BinaryPrimitives.ReadInt32LittleEndian(groupMeta.Span.Slice(groupMeta.Span.Length - 4));
                obj.Persistent.AddRange(
                    Mutagen.Bethesda.Binary.ListBinaryTranslation<IPlaced>.Instance.Parse(
                        frame: frame,
                        transl: (MutagenFrame r, RecordType header, out IPlaced placed) =>
                        {
                            switch (header.TypeInt)
                            {
                                case 0x52484341: //"ACHR":
                                    if (LoquiBinaryTranslation<PlacedNpc>.Instance.Parse(
                                            frame: r,
                                            item: out var placedNPC))
                                    {
                                        placed = placedNPC;
                                        return true;
                                    }
                                    break;
                                case 0x52464552: // "REFR":
                                    if (LoquiBinaryTranslation<PlacedObject>.Instance.Parse(
                                            frame: r,
                                            item: out var placedObj))
                                    {
                                        placed = placedObj;
                                        return true;
                                    }
                                    break;
                                default:
                                    throw new NotImplementedException();
                            }
                            placed = null!;
                            return false;
                        }));
            }

            static bool ParseTemporaryOutliers(MutagenFrame frame, ICellInternal obj)
            {
                var majorMeta = frame.GetMajorRecord();
                var nextHeader = majorMeta.RecordType;
                if (nextHeader.Equals(ANavigationMesh_Registration.NAVM_HEADER))
                {
                    obj.NavigationMeshes.Add(
                        CellNavigationMesh.CreateFromBinary(
                            frame.SpawnWithLength(majorMeta.TotalLength),
                            recordTypeConverter: null));
                    return true;
                }
                else if (nextHeader.Equals(Landscape_Registration.LAND_HEADER))
                {
                    if (obj.Landscape != null)
                    {
                        throw new ArgumentException("Had more than one landscape");
                    }
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
                var formKey = FormKey.Factory(frame.MetaData.MasterReferences!, BinaryPrimitives.ReadUInt32LittleEndian(groupMeta.ContainedRecordTypeSpan));
                if (formKey != obj.FormKey)
                {
                    throw new ArgumentException("Cell children group did not match the FormID of the parent cell.");
                }
                obj.TemporaryTimestamp = BinaryPrimitives.ReadInt32LittleEndian(groupMeta.LastModifiedSpan);
                obj.TemporaryUnknownGroupData = BinaryPrimitives.ReadInt32LittleEndian(groupMeta.Span.Slice(groupMeta.Span.Length - 4));
                var items = Mutagen.Bethesda.Binary.ListBinaryTranslation<IPlaced>.Instance.Parse(
                    frame: frame,
                    transl: (MutagenFrame r, RecordType header, out IPlaced placed) =>
                    {
                        switch (header.TypeInt)
                        {
                            case 0x52484341: //"ACHR":
                                placed = PlacedNpc.CreateFromBinary(r);
                                return true;
                            case 0x52464552: // "REFR":
                                placed = PlacedObject.CreateFromBinary(r);
                                return true;
                            case 0x57524150: // "PARW":
                                placed = PlacedArrow.CreateFromBinary(r);
                                return true;
                            case 0x52414250: // "PBAR":
                                placed = PlacedBarrier.CreateFromBinary(r);
                                return true;
                            case 0x41454250: // "PBEA":
                                placed = PlacedBeam.CreateFromBinary(r);
                                return true;
                            case 0x4E4F4350: // "PCON":
                                placed = PlacedCone.CreateFromBinary(r);
                                return true;
                            case 0x414C4650: // "PFLA":
                                placed = PlacedFlame.CreateFromBinary(r);
                                return true;
                            case 0x445A4850: // "PHZD":
                                placed = PlacedHazard.CreateFromBinary(r);
                                return true;
                            case 0x53494D50: // "PMIS":
                                placed = PlacedMissile.CreateFromBinary(r);
                                return true;
                            case 0x45524750: // "PGRE":
                                placed = PlacedTrap.CreateFromBinary(r);
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

        public partial class CellBinaryWriteTranslation
        {
            static partial void CustomBinaryEndExport(MutagenWriter writer, ICellGetter obj)
            {
                var navMeshes = obj.NavigationMeshes;
                var landscape = obj.Landscape;
                if ((obj.Persistent?.Count ?? 0) == 0
                    && (obj.Temporary?.Count ?? 0) == 0
                    && navMeshes.Count == 0
                    && landscape == null) return;
                using (HeaderExport.ExportHeader(writer, Group_Registration.GRUP_HEADER, Mutagen.Bethesda.Binary.ObjectType.Group))
                {
                    FormKeyBinaryTranslation.Instance.Write(
                        writer,
                        obj.FormKey);
                    writer.Write((int)GroupTypeEnum.CellChildren);
                    writer.Write(obj.Timestamp);
                    writer.Write(obj.UnknownGroupData);
                    if (obj.Persistent?.Count > 0)
                    {
                        using (HeaderExport.ExportHeader(writer, Group_Registration.GRUP_HEADER, Mutagen.Bethesda.Binary.ObjectType.Group))
                        {
                            FormKeyBinaryTranslation.Instance.Write(
                                writer,
                                obj.FormKey);
                            writer.Write((int)GroupTypeEnum.CellPersistentChildren);
                            writer.Write(obj.PersistentTimestamp);
                            writer.Write(obj.PersistentUnknownGroupData);
                            Mutagen.Bethesda.Binary.ListBinaryTranslation<IPlacedGetter>.Instance.Write(
                                writer: writer,
                                items: obj.Persistent,
                                transl: WritePlaced);
                        }
                    }
                    if (obj.Temporary?.Count > 0
                        || navMeshes.Count > 0
                        || landscape != null)
                    {
                        using (HeaderExport.ExportHeader(writer, Group_Registration.GRUP_HEADER, Mutagen.Bethesda.Binary.ObjectType.Group))
                        {
                            FormKeyBinaryTranslation.Instance.Write(
                                writer,
                                obj.FormKey);
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
                                Mutagen.Bethesda.Binary.ListBinaryTranslation<IPlacedGetter>.Instance.Write(
                                    writer: writer,
                                    items: obj.Temporary,
                                    transl: WritePlaced);
                            }
                        }
                    }
                }
            }

            static void WritePlaced(MutagenWriter writer, IPlacedGetter placed)
            {
                placed.WriteToBinary(writer);
            }
        }

        public partial class CellBinaryOverlay
        {
            #region Interfaces
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            string INamedRequiredGetter.Name => this.Name?.String ?? string.Empty;
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            string? INamedGetter.Name => this.Name?.String;
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            TranslatedString ITranslatedNamedRequiredGetter.Name => this.Name ?? string.Empty;
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            #endregion

            static readonly HashSet<RecordType> TypicalPlacedTypes = new HashSet<RecordType>()
            {
                Cell_Registration.ACHR_HEADER,
                Cell_Registration.REFR_HEADER
            };

            private ReadOnlyMemorySlice<byte>? _grupData;

            private int? _navigationMeshLocation;
            public IReadOnlyList<ICellNavigationMeshGetter> NavigationMeshes => throw new NotImplementedException();

            private int? _landscapeLocation;
            public ILandscapeGetter? Landscape => _landscapeLocation.HasValue ? LandscapeBinaryOverlay.LandscapeFactory(new BinaryMemoryReadStream(_grupData!.Value.Slice(_landscapeLocation!.Value)), _package) : default;

            public int Timestamp => _grupData != null ? BinaryPrimitives.ReadInt32LittleEndian(_package.MetaData.Constants.Group(_grupData.Value).LastModifiedSpan) : 0;

            private int? _persistentLocation;
            public int PersistentTimestamp => _persistentLocation.HasValue ? BinaryPrimitives.ReadInt32LittleEndian(_package.MetaData.Constants.Group(_grupData!.Value.Slice(_persistentLocation.Value)).LastModifiedSpan) : 0;
            public IReadOnlyList<IPlacedGetter> Persistent { get; private set; } = ListExt.Empty<IPlacedGetter>();

            private int? _temporaryLocation;
            public int TemporaryTimestamp => _temporaryLocation.HasValue ? BinaryPrimitives.ReadInt32LittleEndian(_package.MetaData.Constants.Group(_grupData!.Value.Slice(_temporaryLocation.Value)).LastModifiedSpan) : 0;
            public IReadOnlyList<IPlacedGetter> Temporary { get; private set; } = ListExt.Empty<IPlacedGetter>();

            private int? _visibleWhenDistantLocation;
            public int VisibleWhenDistantTimestamp => _visibleWhenDistantLocation.HasValue ? BinaryPrimitives.ReadInt32LittleEndian(_package.MetaData.Constants.Group(_grupData!.Value.Slice(_visibleWhenDistantLocation.Value)).LastModifiedSpan) : 0;
            public IReadOnlyList<IPlacedGetter> VisibleWhenDistant { get; private set; } = ListExt.Empty<IPlacedGetter>();

            public int UnknownGroupData => throw new NotImplementedException();

            public int PersistentUnknownGroupData => throw new NotImplementedException();

            public int TemporaryUnknownGroupData => throw new NotImplementedException();

            public static int[] ParseRecordLocations(BinaryMemoryReadStream stream, BinaryOverlayFactoryPackage package)
            {
                List<int> ret = new List<int>();
                var startingPos = stream.Position;
                while (!stream.Complete)
                {
                    var cellMeta = package.MetaData.Constants.GetMajorRecord(stream);
                    if (cellMeta.RecordType != Cell_Registration.CELL_HEADER) break;
                    ret.Add(stream.Position - startingPos);
                    stream.Position += (int)cellMeta.TotalLength;
                    if (stream.Complete) break;
                    var grupMeta = package.MetaData.Constants.GetGroup(stream);
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
                var groupMeta = this._package.MetaData.Constants.GetGroup(stream);
                if (!groupMeta.IsGroup) return;
                var formKey = FormKey.Factory(_package.MetaData.MasterReferences!, BinaryPrimitives.ReadUInt32LittleEndian(groupMeta.ContainedRecordTypeSpan));
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
                    var subGroupMeta = this._package.MetaData.Constants.ReadGroup(stream);
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
                            case 0x52484341: // "ACHR":
                                return PlacedNpcBinaryOverlay.PlacedNpcFactory(new BinaryMemoryReadStream(span), package);
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
                                    var majorMeta = _package.MetaData.Constants.GetMajorRecord(stream);
                                    var recType = majorMeta.RecordType;
                                    if (TypicalPlacedTypes.Contains(recType))
                                    {
                                        ret.Add(checked((int)(stream.Position - subStartPos)));
                                    }
                                    else
                                    {
                                        switch (recType.TypeInt)
                                        {
                                            case 0x4D56414E: // NAVM
                                                if (_navigationMeshLocation.HasValue)
                                                {
                                                    throw new ArgumentException("Second navmesh parsed.");
                                                }
                                                _navigationMeshLocation = checked((int)stream.Position);
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
                        default:
                            throw new NotImplementedException();
                    }
                }
            }
        }
    }
}
