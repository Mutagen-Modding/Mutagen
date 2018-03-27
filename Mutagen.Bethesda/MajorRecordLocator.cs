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
        public class FileLocations
        {
            private Dictionary<FormID, FileSection> _fromFormIDs = new Dictionary<FormID, FileSection>();
            private SortedList<FileLocation, FormID> _fromStart = new SortedList<FileLocation, FormID>();
            private SortedList<FileLocation, FormID> _fromEnd = new SortedList<FileLocation, FormID>();

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
                var gotEnd = _fromEnd.TryGetInDirectionIndex(
                    key: section.Max,
                    higher: true,
                    result: out var end);
                if (!gotStart || !gotEnd)
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
            FileLocations ret = new FileLocations();
            foreach (var rec in MajorRecordIterator.GetFileLocations(
                stream: stream,
                interestingTypes: interestingTypes,
                uninterestingTypes: uninterestingTypes))
            {

                ret.Add(rec.FormID, rec.Section);
            }
            return ret;
        }
    }
}
