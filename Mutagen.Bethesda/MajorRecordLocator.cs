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
            private Dictionary<FormID, FileSection> _fromFormIDs = new Dictionary<FormID, FileSection>();
            private SortedList<FileLocation, FormID> _fromStart = new SortedList<FileLocation, FormID>();
            private SortedList<FileLocation, FormID> _fromEnd = new SortedList<FileLocation, FormID>();
            public SortedSet<FileLocation> GrupLocations = new SortedSet<FileLocation>();

            public FileSection this[FormID id]
            {
                get => _fromFormIDs[id];
            }

            public void Add(FormID id, FileSection section)
            {
                this._fromFormIDs[id] = section;
                this._fromStart[section.Min] = id;
                this._fromEnd[section.Max] = id;
            }

            public bool TryGetSection(FormID id, out FileSection section)
            {
                return this._fromFormIDs.TryGetValue(id, out section);
            }

            public bool TryGetRecord(FileLocation loc, out FormID id)
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

            public bool TryGetRecords(FileSection section, out IEnumerable<FormID> ids)
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
        }

        public static FileLocations GetFileLocations(
            string filePath,
            params RecordType[] interestingTypes)
        {
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                return GetFileLocations(stream, (IEnumerable<RecordType>)interestingTypes);
            }
        }

        public static FileLocations GetFileLocations(
            string filePath,
            IEnumerable<RecordType> interestingTypes = null,
            IEnumerable<RecordType> uninterestingTypes = null)
        {
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                return GetFileLocations(stream, interestingTypes, uninterestingTypes);
            }
        }

        public static FileLocations GetFileLocations(
            Stream stream,
            params RecordType[] interestingTypes)
        {
            return GetFileLocations(stream, (IEnumerable<RecordType>)interestingTypes);
        }

        public static FileLocations GetFileLocations(
            Stream stream,
            IEnumerable<RecordType> interestingTypes = null,
            IEnumerable<RecordType> uninterestingTypes = null)
        {
            HashSet<RecordType> interestingSet;
            if (interestingTypes != null
                && interestingTypes.Any())
            {
                interestingSet = new HashSet<RecordType>(interestingTypes);
            }
            else
            {
                interestingSet = null;
            }

            HashSet<RecordType> uninterestingSet;
            if (uninterestingTypes != null
                && uninterestingTypes.Any())
            {
                uninterestingSet = new HashSet<RecordType>(uninterestingTypes);
            }
            else
            {
                uninterestingSet = null;
            }

            FileLocations ret = new FileLocations();
            using (var reader = new MutagenReader(stream))
            {
                // Skip header
                reader.Position += new ContentLength(4);
                var headerLen = new ContentLength(reader.ReadUInt32());
                reader.Position += new ContentLength(12);
                reader.Position += headerLen;

                while (!reader.Complete
                    && (interestingSet?.Count ?? 1) > 0)
                {
                    ParseTopLevelGRUP(
                        reader: reader,
                        fileLocs: ret,
                        interestingSet: interestingSet,
                        uninterestingSet: uninterestingSet);
                }
            }
            return ret;
        }

        private static void ParseTopLevelGRUP(
            MutagenReader reader,
            FileLocations fileLocs,
            HashSet<RecordType> interestingSet,
            HashSet<RecordType> uninterestingSet,
            RecordType? grupRecOverride = null,
            bool checkGrupType = true)
        {
            var grup = HeaderTranslation.ReadNextRecordType(reader);
            if (!grup.Equals(GRUP))
            {
                throw new ArgumentException();
            }
            fileLocs.GrupLocations.Add(reader.Position - 4);
            var grupLength = new ContentLength(reader.ReadUInt32());
            var grupRec = HeaderTranslation.ReadNextRecordType(reader);
            if (grupRecOverride != null)
            {
                grupRec = grupRecOverride.Value;
            }
            var grupType = EnumBinaryTranslation<GroupTypeEnum>.Instance.ParseValue(new MutagenFrame(reader, new ContentLength(4)));

            if ((!interestingSet?.Contains(grupRec) ?? false)
                || (uninterestingSet?.Contains(grupRec) ?? false))
            { // Skip
                reader.Position -= new ContentLength(16);
                reader.Position += grupLength;
                return;
            }

            interestingSet?.Remove(grupRec);

            reader.Position += new ContentLength(4);

            using (var frame = new MutagenFrame(reader, new FileLocation(reader.Position + grupLength - 20)))
            {
                while (!frame.Complete)
                {
                    var recordLocation = reader.Position;
                    var targetRec = HeaderTranslation.ReadNextRecordType(reader);
                    if (checkGrupType 
                        && !grupRec.Equals(targetRec))
                    {
                        if (targetRec.Type.Equals("GRUP"))
                        {
                            reader.Position -= 4;
                            HandleSubLevelGRUP(
                                frame: frame,
                                fileLocs: fileLocs,
                                recordType: grupRec,
                                interestingSet: interestingSet,
                                uninterestingSet: uninterestingSet);
                        }
                        else
                        {
                            throw new ArgumentException($"Target Record {targetRec} at {frame.Position} did not match its containing GRUP: {grupRec}");
                        }
                    }
                    var recLength = new ContentLength(reader.ReadUInt32());
                    reader.Position += new ContentLength(4); // Skip flags
                    var formID = FormID.Factory(reader.ReadBytes(4));
                    fileLocs.Add(formID, new FileSection(recordLocation, recordLocation + recLength + Constants.RECORD_LENGTH + Constants.RECORD_META_OFFSET - 1));
                    reader.Position += new ContentLength(4);
                    reader.Position += recLength;
                }
            }
        }

        private static void HandleSubLevelGRUP(
            MutagenFrame frame,
            FileLocations fileLocs,
            RecordType recordType,
            HashSet<RecordType> interestingSet,
            HashSet<RecordType> uninterestingSet)
        {
            var targetRec = HeaderTranslation.ReadNextRecordType(frame.Reader);
            if (!targetRec.Type.Equals("GRUP"))
            {
                throw new ArgumentException();
            }
            fileLocs.GrupLocations.Add(frame.Reader.Position - 4);
            var recLength = new ContentLength(frame.Reader.ReadUInt32());
            var grupRec = HeaderTranslation.ReadNextRecordType(frame.Reader);
            var grupType = EnumBinaryTranslation<GroupTypeEnum>.Instance.ParseValue(new MutagenFrame(frame.Reader, new ContentLength(4)));
            frame.Reader.Position -= 16;
            switch (grupType)
            {
                case GroupTypeEnum.InteriorCellBlock:
                case GroupTypeEnum.ExteriorCellBlock:
                    HandleCells(
                        frame: frame.Spawn(recLength),
                        fileLocs: fileLocs,
                        interestingSet: interestingSet,
                        uninterestingSet: uninterestingSet);
                    break;
                case GroupTypeEnum.CellChildren:
                    HandleCellSubchildren(
                        frame: frame.Spawn(recLength),
                        fileLocs: fileLocs,
                        interestingSet: interestingSet,
                        uninterestingSet: uninterestingSet);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private static void HandleCells(
            MutagenFrame frame,
            FileLocations fileLocs,
            HashSet<RecordType> interestingSet,
            HashSet<RecordType> uninterestingSet)
        {
            frame.Reader.Position += Constants.GRUP_LENGTH;
            while (!frame.Complete)
            {
                var targetRec = HeaderTranslation.ReadNextRecordType(frame.Reader);
                if (!targetRec.Type.Equals("GRUP"))
                {
                    throw new ArgumentException();
                }
                fileLocs.GrupLocations.Add(frame.Reader.Position - 4);
                var recLength = new ContentLength(frame.Reader.ReadUInt32());
                var grupRec = HeaderTranslation.ReadNextRecordType(frame.Reader);
                var grupType = EnumBinaryTranslation<GroupTypeEnum>.Instance.ParseValue(new MutagenFrame(frame.Reader, new ContentLength(4)));
                frame.Reader.Position -= 16;
                switch (grupType)
                {
                    case GroupTypeEnum.InteriorCellSubBlock:
                    case GroupTypeEnum.ExteriorCellSubBlock:
                        ParseTopLevelGRUP(
                            reader: frame.Reader,
                            fileLocs: fileLocs,
                            interestingSet: interestingSet,
                            uninterestingSet: uninterestingSet,
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
            HashSet<RecordType> interestingSet,
            HashSet<RecordType> uninterestingSet)
        {
            frame.Reader.Position += Constants.GRUP_LENGTH;
            while (!frame.Complete)
            {
                var targetRec = HeaderTranslation.ReadNextRecordType(frame.Reader);
                if (!targetRec.Type.Equals("GRUP"))
                {
                    throw new ArgumentException();
                }
                fileLocs.GrupLocations.Add(frame.Reader.Position - 4);
                var recLength = new ContentLength(frame.Reader.ReadUInt32());
                var grupRec = HeaderTranslation.ReadNextRecordType(frame.Reader);
                var grupType = EnumBinaryTranslation<GroupTypeEnum>.Instance.ParseValue(new MutagenFrame(frame.Reader, new ContentLength(4)));
                frame.Reader.Position -= 16;
                switch (grupType)
                {
                    case GroupTypeEnum.CellPersistentChildren:
                    case GroupTypeEnum.CellTemporaryChildren:
                    case GroupTypeEnum.CellVisibleDistantChildren:
                        ParseTopLevelGRUP(
                            reader: frame.Reader,
                            fileLocs: fileLocs,
                            interestingSet: interestingSet,
                            uninterestingSet: uninterestingSet,
                            checkGrupType: false);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }
    }
}
