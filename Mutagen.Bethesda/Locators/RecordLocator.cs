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
            public Constants Constants { get; }
            public GameMode GameMode { get; }
            public Func<IMutagenReadStream, RecordType, uint, bool> AdditionalCriteria;

            public FileLocationConstructor(GameMode mode)
            {
                this.GameMode = mode;
                this.Constants = Constants.Get(mode);
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
            GameMode gameMode,
            RecordInterest interest = null)
        {
            using (var stream = new MutagenBinaryReadStream(filePath))
            {
                return GetFileLocations(stream, gameMode, interest);
            }
        }

        private static void SkipHeader(
            IMutagenReadStream reader)
        {
            reader.Position += 4;
            var headerLen = reader.ReadUInt32();
            reader.Position += 12;
            reader.Position += headerLen;
        }

        public static FileLocations GetFileLocations(
            MutagenBinaryReadStream reader,
            GameMode gameMode,
            RecordInterest interest = null,
            Func<IMutagenReadStream, RecordType, uint, bool> additionalCriteria = null)
        {
            FileLocationConstructor ret = new FileLocationConstructor(gameMode)
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
            var grup = HeaderTranslation.ReadNextRecordType(reader);
            if (!grup.Equals(Constants.GRUP))
            {
                throw new ArgumentException();
            }
            fileLocs.GrupLocations.Add(grupLoc);
            var grupLength = reader.ReadUInt32();
            var grupRec = HeaderTranslation.ReadNextRecordType(reader);
            if (grupRecOverride != null)
            {
                grupRec = grupRecOverride.Value;
            }
            var grupType = EnumBinaryTranslation<GroupTypeEnum>.Instance.ParseValue(MutagenFrame.ByLength(reader, 4));

            if (checkOverallGrupType
                && (!interest?.IsInterested(grupRec) ?? false))
            { // Skip
                reader.Position -= 16;
                reader.Position += grupLength;
                return null;
            }

            reader.Position += 4;

            using (var frame = MutagenFrame.ByFinalPosition(reader, reader.Position + grupLength - 20))
            {
                while (!frame.Complete)
                {
                    var recordLocation = reader.Position;
                    var targetRec = HeaderTranslation.ReadNextRecordType(reader);
                    if (!grupRec.Equals(targetRec))
                    {
                        reader.Position -= 4;
                        if (IsSubLevelGRUP(reader))
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
                        else if (!checkOverallGrupType)
                        {
                            reader.Position += 4;
                        }
                        else
                        {
                            throw new ArgumentException($"Target Record {targetRec} at {frame.Position} did not match its containing GRUP: {grupRec}");
                        }
                    }
                    var recLength = reader.ReadUInt32();
                    if (fileLocs.AdditionalCriteria != null)
                    {
                        var pos = reader.Position;
                        reader.Position -= 8;
                        if (!fileLocs.AdditionalCriteria(reader, targetRec, recLength))
                        {
                            reader.Position = pos + 12 + recLength;
                            continue;
                        }
                        reader.Position = pos;
                    }
                    reader.Position += 4; // Skip flags
                    var formID = FormID.Factory(reader.ReadUInt32());
                    if (interest?.IsInterested(targetRec) ?? true)
                    {
                        parentGroupLocations.Push(grupLoc);
                        fileLocs.Add(
                            id: formID,
                            record: targetRec,
                            parentGrupLocations: parentGroupLocations,
                            section: new RangeInt64(recordLocation, recordLocation + recLength + Constants.RECORD_LENGTH + Constants.RECORD_META_OFFSET - 1));
                        parentGroupLocations.Pop();
                    }
                    reader.Position += 4 + recLength;
                }
            }

            return grupRec;
        }

        private static bool IsSubLevelGRUP(
            IMutagenReadStream reader)
        {
            var targetRec = HeaderTranslation.ReadNextRecordType(reader);
            if (!targetRec.Equals(Group_Registration.GRUP_HEADER))
            {
                reader.Position -= 4;
                return false;
            }
            reader.Position += 8;
            var grupType = EnumBinaryTranslation<GroupTypeEnum>.Instance.ParseValue(MutagenFrame.ByLength(reader, 4));
            reader.Position -= 16;
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
            var targetRec = HeaderTranslation.ReadNextRecordType(frame.Reader);
            if (!targetRec.Equals(Group_Registration.GRUP_HEADER))
            {
                throw new DataMisalignedException("Group was not read in where expected: 0x" + (frame.Position - 4).ToString("X"));
            }
            fileLocs.GrupLocations.Add(grupLoc);
            var recLength = frame.Reader.ReadUInt32();
            var grupRec = HeaderTranslation.ReadNextRecordType(frame.Reader);
            var grupType = EnumBinaryTranslation<GroupTypeEnum>.Instance.ParseValue(MutagenFrame.ByLength(frame.Reader, 4));
            frame.Reader.Position -= 16;
            switch (grupType)
            {
                case GroupTypeEnum.InteriorCellBlock:
                case GroupTypeEnum.ExteriorCellBlock:
                    parentGroupLocations.Push(grupLoc);
                    HandleCells(
                        frame: frame.SpawnWithLength(recLength),
                        fileLocs: fileLocs,
                        parentGroupLocations: parentGroupLocations,
                        interest: interest);
                    parentGroupLocations.Pop();
                    break;
                case GroupTypeEnum.CellChildren:
                    parentGroupLocations.Push(grupLoc);
                    HandleCellSubchildren(
                        frame: frame.SpawnWithLength(recLength),
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
            frame.Reader.Position += Constants.GRUP_LENGTH;
            while (!frame.Complete)
            {
                var grupLoc = frame.Position;
                var targetRec = HeaderTranslation.ReadNextRecordType(frame.Reader);
                if (!targetRec.Equals(Group_Registration.GRUP_HEADER))
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
            frame.Reader.Position += Constants.GRUP_LENGTH;
            while (!frame.Complete)
            {
                var grupLoc = frame.Position;
                var targetRec = HeaderTranslation.ReadNextRecordType(frame.Reader);
                if (!targetRec.Equals(Group_Registration.GRUP_HEADER))
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
                var grupLoc = reader.Position;
                var grup = HeaderTranslation.ReadNextRecordType(reader);
                if (grup != Constants.GRUP)
                {
                    throw new DataMisalignedException("Group was not read in where expected: 0x" + (reader.Position - 4).ToString("X"));
                }
                var grupLength = reader.ReadUInt32();
                var recType = HeaderTranslation.ReadNextRecordType(reader);
                yield return new KeyValuePair<RecordType, long>(recType, grupLoc);
                reader.Position = grupLoc + grupLength;
            }
        }

        public static IEnumerable<(FormID FormID, long Position)> ParseTopLevelGRUP(
            IMutagenReadStream reader,
            bool checkOverallGrupType = true)
        {
            var grupLoc = reader.Position;
            var grup = HeaderTranslation.ReadNextRecordType(reader);
            if (!grup.Equals(Constants.GRUP))
            {
                throw new ArgumentException();
            }
            var grupLength = reader.ReadUInt32();
            var grupRec = HeaderTranslation.ReadNextRecordType(reader);
            var grupType = EnumBinaryTranslation<GroupTypeEnum>.Instance.ParseValue(MutagenFrame.ByLength(reader, 4));

            reader.Position += 4;

            using (var frame = MutagenFrame.ByFinalPosition(reader, reader.Position + grupLength - 20))
            {
                while (!frame.Complete)
                {
                    var recordLocation = reader.Position;
                    var targetRec = HeaderTranslation.ReadNextRecordType(reader);
                    if (!grupRec.Equals(targetRec))
                    {
                        reader.Position -= 4;
                        if (IsSubLevelGRUP(reader))
                        {
                            reader.Position += 4;
                            var subGrupLen = reader.ReadUInt32();
                            reader.Position += subGrupLen - 8;
                            continue;
                        }
                        else if (!checkOverallGrupType)
                        {
                            reader.Position += 4;
                        }
                        else
                        {
                            throw new ArgumentException($"Target Record {targetRec} at {frame.Position} did not match its containing GRUP: {grupRec}");
                        }
                    }
                    var recLength = reader.ReadUInt32();
                    reader.Position += 4; // Skip flags
                    var formID = FormID.Factory(reader.ReadUInt32());
                    yield return (
                        formID,
                        recordLocation);
                    reader.Position += 4 + recLength;
                }
            }
        }
        #endregion
    }
}
