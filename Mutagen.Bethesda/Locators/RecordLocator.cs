using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Internals;
using Noggog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    public static class RecordLocator
    {
        #region Get File Locations
        internal class FileLocationConstructor
        {
            public Dictionary<FormID, (RangeInt64 Range, IEnumerable<long> GroupPositions, RecordType Record)> FromFormIDs = new Dictionary<FormID, (RangeInt64 Range, IEnumerable<long> GroupPositions, RecordType Record)>();
            public List<long> FromStartPositions = new List<long>();
            public List<long> FromEndPositions = new List<long>();
            public List<(FormID FormID, RecordType Record)> IDs = new List<(FormID FormID, RecordType Record)>();
            public List<long> GrupLocations = new List<long>();
            public FormID LastParsed;
            public long LastLoc;
            public MetaDataConstants MetaData { get; }
            public Func<IMutagenReadStream, RecordType, uint, bool> AdditionalCriteria;

            public FileLocationConstructor(MetaDataConstants metaData)
            {
                this.MetaData = metaData;
            }

            public void Add(
                FormID id,
                RecordType record,
                Stack<long> parentGrupLocations,
                RangeInt64 section)
            {
                var grupArr = parentGrupLocations.ToArray();
                this.FromFormIDs[id] = (section, grupArr, record);
                this.FromStartPositions.Add(section.Min);
                this.FromEndPositions.Add(section.Max);
                this.IDs.Add((id, record));
                this.LastParsed = id;
                this.LastLoc = section.Min;
            }
        }

        public class FileLocations
        {
            private readonly Dictionary<FormID, (RangeInt64 Range, IEnumerable<long> GroupPositions, RecordType Record)> _fromFormIDs;
            private readonly SortingListDictionary<long, (FormID FormID, RecordType Record)> _fromStart;
            private readonly SortingListDictionary<long, (FormID FormID, RecordType Record)> _fromEnd;
            public readonly SortingList<long> GrupLocations;
            public SortingListDictionary<long, (FormID FormID, RecordType Record)> ListedRecords => _fromStart;
            public RangeInt64 this[FormID id] => _fromFormIDs[id].Range;

            internal FileLocations(FileLocationConstructor constructor)
            {
                _fromFormIDs = constructor.FromFormIDs;
                _fromStart = SortingListDictionary<long, (FormID FormID, RecordType Record)>.Factory_Wrap_AssumeSorted(
                    constructor.FromStartPositions,
                    constructor.IDs);
                _fromEnd = SortingListDictionary<long, (FormID FormID, RecordType Record)>.Factory_Wrap_AssumeSorted(
                    constructor.FromEndPositions,
                    constructor.IDs);
                GrupLocations = SortingList<long>.Factory_Wrap_AssumeSorted(constructor.GrupLocations);
            }

            public bool TryGetSection(FormID id, out RangeInt64 section)
            {
                if (this._fromFormIDs.TryGetValue(id, out var item))
                {
                    section = item.Range;
                    return true;
                }
                section = default(RangeInt64);
                return false;
            }

            public bool TryGetRecord(long loc, out (FormID FormID, RecordType Record) record)
            {
                if (!_fromStart.TryGetInDirection(
                    key: loc,
                    higher: false,
                    result: out var lowerID))
                {
                    record = default;
                    return false;
                }
                if (!_fromEnd.TryGetInDirection(
                    key: loc,
                    higher: true,
                    result: out var higherID))
                {
                    record = default;
                    return false;
                }
                if (lowerID.Value.FormID != higherID.Value.FormID)
                {
                    record = default;
                    return false;
                }
                record = lowerID.Value;
                return true;
            }

            public bool TryGetRecords(RangeInt64 section, out IEnumerable<(FormID FormID, RecordType Record)> ids)
            {
                var gotStart = _fromStart.TryGetIndexInDirection(
                    key: section.Min,
                    higher: false,
                    result: out var start);
                var gotEndStart = _fromStart.TryGetIndexInDirection(
                    key: section.Max,
                    higher: true,
                    result: out var endStart);
                var gotEnd = _fromEnd.TryGetIndexInDirection(
                    key: section.Max,
                    higher: true,
                    result: out var end);
                if (!gotStart || !gotEnd || !gotEndStart)
                {
                    ids = null;
                    return false;
                }
                var endLocation = _fromEnd.Keys[end];
                var endStartLocation = _fromStart.Keys[endStart];
                if (endLocation > endStartLocation)
                {
                    ids = null;
                    return false;
                }
                var ret = new HashSet<(FormID FormID, RecordType Record)>();
                for (int i = start; i <= end; i++)
                {
                    ret.Add(_fromStart.Values[i]);
                }
                ids = ret;
                return true;
            }

            public IEnumerable<long> GetContainingGroupLocations(FormID id)
            {
                return _fromFormIDs[id].GroupPositions;
            }
        }

        public static FileLocations GetFileLocations(
            string filePath,
            MetaDataConstants meta,
            RecordInterest interest = null)
        {
            using (var stream = new MutagenBinaryReadStream(filePath, meta))
            {
                return GetFileLocations(stream, interest);
            }
        }

        private static void SkipHeader(IMutagenReadStream reader)
        {
            ModHeaderMeta headerMeta = reader.MetaData.GetHeader(reader);
            if (!headerMeta.HasContent)
            {
                reader.Position = reader.Length;
                return;
            }
            reader.Position += headerMeta.TotalLength;
        }

        public static FileLocations GetFileLocations(
            IMutagenReadStream reader,
            RecordInterest interest = null,
            Func<IMutagenReadStream, RecordType, uint, bool> additionalCriteria = null)
        {
            FileLocationConstructor ret = new FileLocationConstructor(reader.MetaData)
            {
                AdditionalCriteria = additionalCriteria,
            };
            SkipHeader(reader);

            HashSet<RecordType> remainingTypes = ((interest?.InterestingTypes?.Count ?? 0) <= 0) ? null : new HashSet<RecordType>(interest.InterestingTypes);
            Stack<long> grupPositions = new Stack<long>();
            while (!reader.Complete
                && (remainingTypes?.Count ?? 1) > 0)
            {
                var parsed = ParseTopLevelGRUP(
                    reader: reader,
                    fileLocs: ret,
                    interest: interest,
                    grupRecOverride: null,
                    checkOverallGrupType: true,
                    parentGroupLocations: grupPositions);
                if (parsed.HasValue)
                {
                    remainingTypes?.Remove(parsed.Value);
                }
            }
            return new FileLocations(ret);
        }

        private static RecordType? ParseTopLevelGRUP(
            IMutagenReadStream reader,
            FileLocationConstructor fileLocs,
            RecordInterest interest,
            Stack<long> parentGroupLocations,
            RecordType? grupRecOverride,
            bool checkOverallGrupType)
        {
            var grupLoc = reader.Position;
            GroupRecordMeta groupMeta = fileLocs.MetaData.GetGroup(reader);
            if (!groupMeta.IsGroup)
            {
                throw new ArgumentException();
            }
            fileLocs.GrupLocations.Add(reader.Position);
            var grupRec = grupRecOverride ?? groupMeta.ContainedRecordType;

            if (checkOverallGrupType
                && (!interest?.IsInterested(grupRec) ?? false))
            { // Skip
                reader.Position += groupMeta.TotalLength;
                return null;
            }

            reader.Position += groupMeta.HeaderLength;

            using (var frame = MutagenFrame.ByFinalPosition(reader, reader.Position + groupMeta.ContentLength))
            {
                while (!frame.Complete)
                {
                    MajorRecordMeta majorRecordMeta = fileLocs.MetaData.GetMajorRecord(frame.Reader);
                    var targetRec = majorRecordMeta.RecordType;
                    if (targetRec != grupRec)
                    {
                        if (IsSubLevelGRUP(fileLocs.MetaData.GetGroup(frame.Reader)))
                        {
                            parentGroupLocations.Push(grupLoc);
                            HandleSubLevelGRUP(
                                frame: frame,
                                fileLocs: fileLocs,
                                recordType: grupRec,
                                parentGroupLocations: parentGroupLocations,
                                interest: interest);
                            parentGroupLocations.Pop();
                            continue;
                        }
                        else if (checkOverallGrupType)
                        {
                            throw new ArgumentException($"Target Record {targetRec} at {frame.Position} did not match its containing GRUP: {grupRec}");
                        }
                    }
                    var recLength = majorRecordMeta.RecordLength;
                    if (fileLocs.AdditionalCriteria != null)
                    {
                        var pos = reader.Position;
                        if (!fileLocs.AdditionalCriteria(reader, targetRec, recLength))
                        {
                            reader.Position = pos + majorRecordMeta.TotalLength;
                            continue;
                        }
                        reader.Position = pos;
                    }
                    if (interest?.IsInterested(targetRec) ?? true)
                    {
                        parentGroupLocations.Push(grupLoc);
                        var pos = reader.Position;
                        fileLocs.Add(
                            id: majorRecordMeta.FormID,
                            record: targetRec,
                            parentGrupLocations: parentGroupLocations,
                            section: new RangeInt64(pos, pos + majorRecordMeta.TotalLength - 1));
                        parentGroupLocations.Pop();
                    }
                    reader.Position += majorRecordMeta.TotalLength;
                }
            }

            return grupRec;
        }

        private static bool IsSubLevelGRUP(GroupRecordMeta groupMeta)
        {
            if (!groupMeta.IsGroup)
            {
                return false;
            }
            var grupType = EnumExt<GroupTypeEnum>.Convert(groupMeta.GroupType);
            switch (grupType)
            {
                case GroupTypeEnum.InteriorCellBlock:
                case GroupTypeEnum.ExteriorCellBlock:
                case GroupTypeEnum.CellChildren:
                case GroupTypeEnum.WorldChildren:
                case GroupTypeEnum.TopicChildren:
                    return true;
                default:
                    return false;
            }
        }

        private static void HandleSubLevelGRUP(
            MutagenFrame frame,
            FileLocationConstructor fileLocs,
            RecordType recordType,
            RecordInterest interest,
            Stack<long> parentGroupLocations)
        {
            var grupLoc = frame.Position;
            GroupRecordMeta groupMeta = fileLocs.MetaData.GetGroup(frame.Reader);
            if (!groupMeta.IsGroup)
            {
                throw new DataMisalignedException("Group was not read in where expected: 0x" + (frame.Position - 4).ToString("X"));
            }
            fileLocs.GrupLocations.Add(grupLoc);
            switch (EnumExt<GroupTypeEnum>.Convert(groupMeta.GroupType))
            {
                case GroupTypeEnum.InteriorCellBlock:
                case GroupTypeEnum.ExteriorCellBlock:
                    parentGroupLocations.Push(grupLoc);
                    HandleCells(
                        frame: frame.SpawnWithLength(groupMeta.RecordLength),
                        fileLocs: fileLocs,
                        parentGroupLocations: parentGroupLocations,
                        interest: interest);
                    parentGroupLocations.Pop();
                    break;
                case GroupTypeEnum.CellChildren:
                    parentGroupLocations.Push(grupLoc);
                    HandleCellSubchildren(
                        frame: frame.SpawnWithLength(groupMeta.RecordLength),
                        fileLocs: fileLocs,
                        parentGroupLocations: parentGroupLocations,
                        interest: interest);
                    parentGroupLocations.Pop();
                    break;
                case GroupTypeEnum.WorldChildren:
                case GroupTypeEnum.TopicChildren:
                    ParseTopLevelGRUP(
                        reader: frame.Reader,
                        fileLocs: fileLocs,
                        interest: interest,
                        parentGroupLocations: parentGroupLocations,
                        checkOverallGrupType: false,
                        grupRecOverride: null);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private static void HandleCells(
            MutagenFrame frame,
            FileLocationConstructor fileLocs,
            RecordInterest interest,
            Stack<long> parentGroupLocations)
        {
            frame.Reader.Position += fileLocs.MetaData.GroupConstants.HeaderLength;
            while (!frame.Complete)
            {
                var grupLoc = frame.Position;
                var targetRec = HeaderTranslation.ReadNextRecordType(frame.Reader);
                if (!targetRec.Equals(Mutagen.Bethesda.Constants.GRUP))
                {
                    throw new ArgumentException();
                }
                fileLocs.GrupLocations.Add(grupLoc);
                var recLength = frame.Reader.ReadUInt32();
                var grupRec = HeaderTranslation.ReadNextRecordType(frame.Reader);
                var grupType = EnumBinaryTranslation<GroupTypeEnum>.Instance.ParseValue(MutagenFrame.ByLength(frame.Reader, 4));
                frame.Reader.Position -= 16;
                switch (grupType)
                {
                    case GroupTypeEnum.InteriorCellSubBlock:
                    case GroupTypeEnum.ExteriorCellSubBlock:
                        ParseTopLevelGRUP(
                            reader: frame.Reader,
                            fileLocs: fileLocs,
                            interest: interest,
                            parentGroupLocations: parentGroupLocations,
                            grupRecOverride: new RecordType("CELL"),
                            checkOverallGrupType: true);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        private static void HandleCellSubchildren(
            MutagenFrame frame,
            FileLocationConstructor fileLocs,
            RecordInterest interest,
            Stack<long> parentGroupLocations)
        {
            frame.Reader.Position += fileLocs.MetaData.GroupConstants.HeaderLength;
            while (!frame.Complete)
            {
                var grupLoc = frame.Position;
                var targetRec = HeaderTranslation.ReadNextRecordType(frame.Reader);
                if (!targetRec.Equals(Mutagen.Bethesda.Constants.GRUP))
                {
                    throw new ArgumentException();
                }
                fileLocs.GrupLocations.Add(grupLoc);
                var recLength = frame.Reader.ReadUInt32();
                var grupRec = HeaderTranslation.ReadNextRecordType(frame.Reader);
                var grupType = EnumBinaryTranslation<GroupTypeEnum>.Instance.ParseValue(MutagenFrame.ByLength(frame.Reader, 4));
                frame.Reader.Position -= 16;
                switch (grupType)
                {
                    case GroupTypeEnum.CellPersistentChildren:
                    case GroupTypeEnum.CellTemporaryChildren:
                    case GroupTypeEnum.CellVisibleDistantChildren:
                        ParseTopLevelGRUP(
                            reader: frame.Reader,
                            fileLocs: fileLocs,
                            interest: interest,
                            parentGroupLocations: parentGroupLocations,
                            grupRecOverride: null,
                            checkOverallGrupType: false);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }
        #endregion

        #region Base GRUP Iterator
        public static IEnumerable<KeyValuePair<RecordType, long>> IterateBaseGroupLocations(
            IMutagenReadStream reader)
        {
            SkipHeader(reader);
            while (!reader.Complete)
            {
                GroupRecordMeta groupMeta = reader.MetaData.GetGroup(reader);
                if (!groupMeta.IsGroup)
                {
                    throw new DataMisalignedException("Group was not read in where expected: 0x" + reader.Position.ToString("X"));
                }
                var pos = reader.Position + groupMeta.TotalLength;
                yield return new KeyValuePair<RecordType, long>(groupMeta.ContainedRecordType, reader.Position);
                reader.Position = pos;
            }
        }

        public static IEnumerable<(FormID FormID, long Position)> ParseTopLevelGRUP(
            IMutagenReadStream reader,
            GameMode gameMode,
            bool checkOverallGrupType = true)
        {
            var meta = MetaDataConstants.Get(gameMode);
            var groupMeta = meta.GetGroup(reader);
            var grupLoc = reader.Position;
            var targetRec = groupMeta.ContainedRecordType;
            if (!groupMeta.IsGroup)
            {
                throw new ArgumentException();
            }

            reader.Position += groupMeta.HeaderLength;

            using (var frame = MutagenFrame.ByFinalPosition(reader, reader.Position + groupMeta.ContentLength))
            {
                while (!frame.Complete)
                {
                    var recordLocation = reader.Position;
                    MajorRecordMeta majorMeta = meta.GetMajorRecord(reader);
                    if (majorMeta.RecordType != targetRec)
                    {
                        var subGroupMeta = meta.GetGroup(reader);
                        if (IsSubLevelGRUP(subGroupMeta))
                        {
                            reader.Position += subGroupMeta.TotalLength;
                            continue;
                        }
                        else if (checkOverallGrupType)
                        {
                            throw new ArgumentException($"Target Record {targetRec} at {frame.Position} did not match its containing GRUP: {subGroupMeta.ContainedRecordType}");
                        }
                    }
                    var formID = majorMeta.FormID;
                    var len = majorMeta.TotalLength;
                    yield return (
                        formID,
                        recordLocation);
                    reader.Position += len;
                }
            }
        }
        #endregion
    }
}
