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
    public static class MajorRecordLocator
    {
        private readonly static RecordType GRUP = new RecordType("GRUP");

        public class FileLocations
        {
            private Dictionary<FormID, RangeInt64> _fromFormIDs = new Dictionary<FormID, RangeInt64>();
            private SortedList<long, FormID> _fromStart = new SortedList<long, FormID>();
            private SortedList<long, FormID> _fromEnd = new SortedList<long, FormID>();
            private Dictionary<FormID, IEnumerable<long>> _parentGroupMapper = new Dictionary<FormID, IEnumerable<long>>();
            public SortedSet<long> GrupLocations = new SortedSet<long>();
            public SortedList<long, FormID> ListedRecords => _fromStart;
            public RangeInt64 this[FormID id] => _fromFormIDs[id];

            public void Add(
                FormID id,
                IEnumerable<long> parentGrupLocations,
                RangeInt64 section)
            {
                this._fromFormIDs[id] = section;
                this._fromStart[section.Min] = id;
                this._fromEnd[section.Max] = id;
                this._parentGroupMapper[id] = new HashSet<long>(parentGrupLocations);
            }

            public bool TryGetSection(FormID id, out RangeInt64 section)
            {
                return this._fromFormIDs.TryGetValue(id, out section);
            }

            public bool TryGetRecord(long loc, out FormID id)
            {
                if (!_fromStart.TryGetInDirection(
                    key: loc,
                    higher: false,
                    result: out var lowerID))
                {
                    id = default(FormID);
                    return false;
                }
                if (!_fromEnd.TryGetInDirection(
                    key: loc,
                    higher: true,
                    result: out var higherID))
                {
                    id = default(FormID);
                    return false;
                }
                if (lowerID.Value != higherID.Value)
                {
                    id = default(FormID);
                    return false;
                }
                id = lowerID.Value;
                return true;
            }

            public bool TryGetRecords(RangeInt64 section, out IEnumerable<FormID> ids)
            {
                var gotStart = _fromStart.TryGetInDirectionIndex(
                    key: section.Min,
                    higher: false,
                    result: out var start);
                var gotEndStart = _fromStart.TryGetInDirectionIndex(
                    key: section.Max,
                    higher: true,
                    result: out var endStart);
                var gotEnd = _fromEnd.TryGetInDirectionIndex(
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
                var ret = new HashSet<FormID>();
                for (int i = start; i <= end; i++)
                {
                    ret.Add(_fromStart.Values[i]);
                }
                ids = ret;
                return true;
            }

            public IEnumerable<long> GetContainingGroupLocations(FormID id)
            {
                return _parentGroupMapper[id];
            }
        }

        public static FileLocations GetFileLocations(
            string filePath,
            RecordInterest interest = null)
        {
            using (var stream = new BinaryReadStream(filePath))
            {
                return GetFileLocations(stream, interest);
            }
        }

        public static FileLocations GetFileLocations(
            BinaryReadStream reader,
            RecordInterest interest = null)
        {
            FileLocations ret = new FileLocations();
            // Skip header
            reader.Position += 4;
            var headerLen = reader.ReadUInt32();
            reader.Position += 12;
            reader.Position += headerLen;

            HashSet<RecordType> remainingTypes = (interest?.InterestingTypes?.Count <= 0) ? null : new HashSet<RecordType>(interest.InterestingTypes);

            while (!reader.Complete
                && (remainingTypes?.Count ?? 1) > 0)
            {
                var parsed = ParseTopLevelGRUP(
                    reader: reader,
                    fileLocs: ret,
                    interest: interest);
                if (parsed.HasValue)
                {
                    remainingTypes?.Remove(parsed.Value);
                }
            }
            return ret;
        }

        private static RecordType? ParseTopLevelGRUP(
            IBinaryStream reader,
            FileLocations fileLocs,
            RecordInterest interest,
            RecordType? grupRecOverride = null,
            bool checkOverallGrupType = true,
            IEnumerable<long> parentGroupLocations = null)
        {
            var grupLoc = reader.Position;
            var grup = HeaderTranslation.ReadNextRecordType(reader);
            if (!grup.Equals(GRUP))
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
                            HandleSubLevelGRUP(
                                frame: frame,
                                fileLocs: fileLocs,
                                recordType: grupRec,
                                parentGroupLocations: grupLoc.And(parentGroupLocations),
                                interest: interest);
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
                    var formID = FormID.Factory(reader.ReadBytes(4));
                    if (interest?.IsInterested(targetRec) ?? true)
                    {
                        fileLocs.Add(
                            id: formID,
                            parentGrupLocations: grupLoc.And(parentGroupLocations),
                            section: new RangeInt64(recordLocation, recordLocation + recLength + Constants.RECORD_LENGTH + Constants.RECORD_META_OFFSET - 1));
                    }
                    reader.Position += 4;
                    reader.Position += recLength;
                }
            }

            return grupRec;
        }

        private static bool IsSubLevelGRUP(
            IBinaryStream reader)
        {
            var targetRec = HeaderTranslation.ReadNextRecordType(reader);
            if (!targetRec.Type.Equals("GRUP"))
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
                    return true;
                default:
                    return false;
            }
        }

        private static void HandleSubLevelGRUP(
            MutagenFrame frame,
            FileLocations fileLocs,
            RecordType recordType,
            RecordInterest interest,
            IEnumerable<long> parentGroupLocations)
        {
            var grupLoc = frame.Position;
            var targetRec = HeaderTranslation.ReadNextRecordType(frame.Reader);
            if (!targetRec.Type.Equals("GRUP"))
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
                case GroupTypeEnum.InteriorCellBlock:
                case GroupTypeEnum.ExteriorCellBlock:
                    HandleCells(
                        frame: frame.SpawnWithLength(recLength),
                        fileLocs: fileLocs,
                        parentGroupLocations: grupLoc.And(parentGroupLocations),
                        interest: interest);
                    break;
                case GroupTypeEnum.CellChildren:
                    HandleCellSubchildren(
                        frame: frame.SpawnWithLength(recLength),
                        fileLocs: fileLocs,
                        parentGroupLocations: grupLoc.And(parentGroupLocations),
                        interest: interest);
                    break;
                case GroupTypeEnum.WorldChildren:
                    ParseTopLevelGRUP(
                        reader: frame.Reader,
                        fileLocs: fileLocs,
                        interest: interest,
                        parentGroupLocations: grupLoc.And(parentGroupLocations),
                        checkOverallGrupType: false);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private static void HandleCells(
            MutagenFrame frame,
            FileLocations fileLocs,
            RecordInterest interest,
            IEnumerable<long> parentGroupLocations)
        {
            frame.Reader.Position += Constants.GRUP_LENGTH;
            while (!frame.Complete)
            {
                var grupLoc = frame.Position;
                var targetRec = HeaderTranslation.ReadNextRecordType(frame.Reader);
                if (!targetRec.Type.Equals("GRUP"))
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
                            parentGroupLocations: grupLoc.And(parentGroupLocations),
                            grupRecOverride: new RecordType("CELL"));
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        private static void HandleCellSubchildren(
            MutagenFrame frame,
            FileLocations fileLocs,
            RecordInterest interest,
            IEnumerable<long> parentGroupLocations)
        {
            frame.Reader.Position += Constants.GRUP_LENGTH;
            while (!frame.Complete)
            {
                var grupLoc = frame.Position;
                var targetRec = HeaderTranslation.ReadNextRecordType(frame.Reader);
                if (!targetRec.Type.Equals("GRUP"))
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
                            parentGroupLocations: grupLoc.And(parentGroupLocations),
                            checkOverallGrupType: false);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }
    }
}
