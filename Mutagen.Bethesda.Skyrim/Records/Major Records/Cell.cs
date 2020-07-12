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
                if (!frame.TryGetGroup(out var groupMeta)) return;
                var formKey = FormKey.Factory(frame.MetaData.MasterReferences!, BinaryPrimitives.ReadUInt32LittleEndian(groupMeta.ContainedRecordTypeData));
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
                var formKey = FormKey.Factory(frame.MetaData.MasterReferences!, BinaryPrimitives.ReadUInt32LittleEndian(groupMeta.ContainedRecordTypeData));
                if (formKey != obj.FormKey)
                {
                    throw new ArgumentException("Cell children group did not match the FormID of the parent cell.");
                }
                obj.PersistentTimestamp = BinaryPrimitives.ReadInt32LittleEndian(groupMeta.LastModifiedData);
                obj.PersistentUnknownGroupData = BinaryPrimitives.ReadInt32LittleEndian(groupMeta.HeaderData.Slice(groupMeta.HeaderData.Length - 4));
                obj.Persistent.AddRange(
                    Mutagen.Bethesda.Binary.ListBinaryTranslation<IPlaced>.Instance.Parse(
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
                                    throw new NotImplementedException();
                            }
                        }));
            }

            static bool ParseTemporaryOutliers(MutagenFrame frame, ICellInternal obj)
            {
                var majorMeta = frame.GetMajorRecord();
                var nextHeader = majorMeta.RecordType;
                if (nextHeader.Equals(RecordTypes.NAVM))
                {
                    if (frame.MetaData.InWorldspace)
                    {
                        obj.NavigationMeshes.Add(
                            WorldspaceNavigationMesh.CreateFromBinary(
                                frame.SpawnWithLength(majorMeta.TotalLength),
                                recordTypeConverter: null));
                    }
                    else
                    {
                        obj.NavigationMeshes.Add(
                            CellNavigationMesh.CreateFromBinary(
                                frame.SpawnWithLength(majorMeta.TotalLength),
                                recordTypeConverter: null));
                    }
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
                obj.TemporaryUnknownGroupData = BinaryPrimitives.ReadInt32LittleEndian(groupMeta.HeaderData.Slice(groupMeta.HeaderData.Length - 4));
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
                using (HeaderExport.Header(writer, RecordTypes.GRUP, Mutagen.Bethesda.Binary.ObjectType.Group))
                {
                    FormKeyBinaryTranslation.Instance.Write(
                        writer,
                        obj.FormKey);
                    writer.Write((int)GroupTypeEnum.CellChildren);
                    writer.Write(obj.Timestamp);
                    writer.Write(obj.UnknownGroupData);
                    if (obj.Persistent?.Count > 0)
                    {
                        using (HeaderExport.Header(writer, RecordTypes.GRUP, Mutagen.Bethesda.Binary.ObjectType.Group))
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
                        using (HeaderExport.Header(writer, RecordTypes.GRUP, Mutagen.Bethesda.Binary.ObjectType.Group))
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

            static readonly ICollectionGetter<RecordType> TypicalPlacedTypes = new CollectionGetterWrapper<RecordType>(
                new HashSet<RecordType>()
                {
                    RecordTypes.ACHR,
                    RecordTypes.REFR,
                    RecordTypes.PARW,
                    RecordTypes.PBAR,
                    RecordTypes.PBEA,
                    RecordTypes.PCON,
                    RecordTypes.PFLA,
                    RecordTypes.PHZD,
                    RecordTypes.PMIS,
                    RecordTypes.PGRE,
                });

            internal bool InsideWorldspace;

            private ReadOnlyMemorySlice<byte>? _grupData;

            public IReadOnlyList<IANavigationMeshGetter> NavigationMeshes { get; private set; } = ListExt.Empty<IANavigationMeshGetter>();

            public int UnknownGroupData => _grupData.HasValue ? BinaryPrimitives.ReadInt32LittleEndian(_grupData.Value.Slice(20)) : default;

            private int? _landscapeLocation;
            public ILandscapeGetter? Landscape => _landscapeLocation.HasValue ? LandscapeBinaryOverlay.LandscapeFactory(new OverlayStream(_grupData!.Value.Slice(_landscapeLocation!.Value), _package), _package) : default;

            public int Timestamp => _grupData != null ? BinaryPrimitives.ReadInt32LittleEndian(_package.MetaData.Constants.Group(_grupData.Value).LastModifiedData) : 0;

            private int? _persistentLocation;
            public int PersistentTimestamp => _persistentLocation.HasValue ? BinaryPrimitives.ReadInt32LittleEndian(_package.MetaData.Constants.Group(_grupData!.Value.Slice(_persistentLocation.Value)).LastModifiedData) : 0;
            public IReadOnlyList<IPlacedGetter> Persistent { get; private set; } = ListExt.Empty<IPlacedGetter>();

            private int? _temporaryLocation;
            public int TemporaryTimestamp => _temporaryLocation.HasValue ? BinaryPrimitives.ReadInt32LittleEndian(_package.MetaData.Constants.Group(_grupData!.Value.Slice(_temporaryLocation.Value)).LastModifiedData) : 0;
            public IReadOnlyList<IPlacedGetter> Temporary { get; private set; } = ListExt.Empty<IPlacedGetter>();

            public int PersistentUnknownGroupData => _persistentLocation.HasValue ? BinaryPrimitives.ReadInt32LittleEndian(_grupData!.Value.Slice(_persistentLocation.Value + 20)) : 0;

            public int TemporaryUnknownGroupData => _temporaryLocation.HasValue ? BinaryPrimitives.ReadInt32LittleEndian(_grupData!.Value.Slice(_temporaryLocation.Value + 20)) : 0;

            int? _flagsLoc;

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
                    if (stream.TryGetGroup(out var groupMeta)
                        && groupMeta.GroupType == (int)GroupTypeEnum.CellChildren)
                    {
                        stream.Position += (int)groupMeta.TotalLength;
                    }
                }
                return ret.ToArray();
            }

            public static CellBinaryOverlay CellFactory(
                OverlayStream stream,
                BinaryOverlayFactoryPackage package,
                bool insideWorldspace)
            {
                var origStream = stream;
                stream = UtilityTranslation.DecompressStream(stream);
                var ret = new CellBinaryOverlay(
                    bytes: HeaderTranslation.ExtractRecordMemory(stream.RemainingMemory, package.MetaData.Constants),
                    package: package)
                {
                    InsideWorldspace = insideWorldspace
                };
                var finalPos = checked((int)(stream.Position + package.MetaData.Constants.MajorRecord(stream.RemainingMemory).TotalLength));
                int offset = stream.Position + package.MetaData.Constants.MajorConstants.TypeAndLengthLength;
                stream.Position += 0x10 + package.MetaData.Constants.MajorConstants.TypeAndLengthLength;
                ret.CustomFactoryEnd(
                    stream: stream,
                    finalPos: finalPos,
                    offset: offset);
                ret.FillSubrecordTypes(
                    stream: stream,
                    finalPos: finalPos,
                    offset: offset,
                    recordTypeConverter: null,
                    fill: ret.FillRecordType);
                ret.CustomEnd(
                    stream: origStream,
                    finalPos: stream.Length,
                    offset: offset);
                return ret;
            }

            partial void CustomEnd(OverlayStream stream, int finalPos, int _)
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
                finalPos = stream.Length;
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
                            case 0x52484341: // "ACHR":
                                return PlacedNpcBinaryOverlay.PlacedNpcFactory(new OverlayStream(span, package), package);
                            case 0x52464552: // "REFR":
                                return PlacedObjectBinaryOverlay.PlacedObjectFactory(new OverlayStream(span, package), package);
                            case 0x57524150: // "PARW":
                                return PlacedArrowBinaryOverlay.PlacedArrowFactory(new OverlayStream(span, package), package);
                            case 0x52414250: // "PBAR":
                                return PlacedBarrierBinaryOverlay.PlacedBarrierFactory(new OverlayStream(span, package), package);
                            case 0x41454250: // "PBEA":
                                return PlacedBeamBinaryOverlay.PlacedBeamFactory(new OverlayStream(span, package), package);
                            case 0x4E4F4350: // "PCON":
                                return PlacedConeBinaryOverlay.PlacedConeFactory(new OverlayStream(span, package), package);
                            case 0x414C4650: // "PFLA":
                                return PlacedFlameBinaryOverlay.PlacedFlameFactory(new OverlayStream(span, package), package);
                            case 0x445A4850: // "PHZD":
                                return PlacedHazardBinaryOverlay.PlacedHazardFactory(new OverlayStream(span, package), package);
                            case 0x53494D50: // "PMIS":
                                return PlacedMissileBinaryOverlay.PlacedMissileFactory(new OverlayStream(span, package), package);
                            case 0x45524750: // "PGRE":
                                return PlacedTrapBinaryOverlay.PlacedTrapFactory(new OverlayStream(span, package), package);
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
                                        stream.Position += (int)majorMeta.TotalLength;
                                    }
                                    else
                                    {
                                        switch (recType.TypeInt)
                                        {
                                            case 0x4D56414E: // NAVM
                                                if (this.InsideWorldspace)
                                                {
                                                    this.NavigationMeshes = BinaryOverlayList.FactoryByArray<IWorldspaceNavigationMeshGetter>(
                                                        mem: stream.RemainingMemory,
                                                        package: _package,
                                                        getter: (s, p) => WorldspaceNavigationMeshBinaryOverlay.WorldspaceNavigationMeshFactory(s, p),
                                                        locs: ParseRecordLocations(
                                                            stream: stream,
                                                            constants: _package.MetaData.Constants.MajorConstants,
                                                            trigger: recType,
                                                            skipHeader: false));
                                                }
                                                else
                                                {
                                                    this.NavigationMeshes = BinaryOverlayList.FactoryByArray<ICellNavigationMeshGetter>(
                                                        mem: stream.RemainingMemory,
                                                        package: _package,
                                                        getter: (s, p) => CellNavigationMeshBinaryOverlay.CellNavigationMeshFactory(s, p),
                                                        locs: ParseRecordLocations(
                                                            stream: stream,
                                                            constants: _package.MetaData.Constants.MajorConstants,
                                                            trigger: recType,
                                                            skipHeader: false));
                                                }
                                                break;
                                            case 0x444e414c: // LAND
                                                if (_landscapeLocation.HasValue)
                                                {
                                                    throw new ArgumentException("Second landscape parsed.");
                                                }
                                                _landscapeLocation = checked((int)stream.Position);
                                                stream.Position += (int)majorMeta.TotalLength;
                                                break;
                                            default:
                                                throw new NotImplementedException();
                                        }
                                    }
                                }
                                this.Temporary = BinaryOverlayList.FactoryByArray<IPlacedGetter>(
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

            partial void FlagsCustomParse(OverlayStream stream, long finalPos, int offset)
            {
                _flagsLoc = (stream.Position - offset);
            }

            public Cell.Flag GetFlagsCustom()
            {
                if (!_flagsLoc.HasValue) return default(Cell.Flag);
                var subHeader = _package.MetaData.Constants.SubrecordFrame(_data.Slice(_flagsLoc.Value));
                switch (subHeader.Content.Length)
                {
                    case 1:
                        return (Cell.Flag)subHeader.Content[0];
                    case 2:
                        return (Cell.Flag)BinaryPrimitives.ReadUInt16LittleEndian(subHeader.Content);
                    default:
                        throw new NotImplementedException();
                }
            }
        }
    }
}
