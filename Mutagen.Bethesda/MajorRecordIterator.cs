using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Internals;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    public static class MajorRecordIterator
    {
        private readonly static RecordType GRUP = new RecordType("GRUP");
        
        public static ICollection<(RecordType Type, FormID FormID, FileSection Section)> GetFileLocations(
            string filePath,
            IEnumerable<RecordType> interestingTypes = null,
            IEnumerable<RecordType> uninterestingTypes = null)
        {
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                return GetFileLocations(stream, interestingTypes, uninterestingTypes);
            }
        }

        public static ICollection<(RecordType Type, FormID FormID, FileSection Section)> GetFileLocations(
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

            List<(RecordType Type, FormID FormID, FileSection Section)> ret = new List<(RecordType Type, FormID FormID, FileSection Section)>();
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
                    var grup = HeaderTranslation.ReadNextRecordType(reader);
                    if (!grup.Equals(GRUP))
                    {
                        throw new ArgumentException();
                    }
                    var grupLength = new ContentLength(reader.ReadUInt32());
                    var grupRec = HeaderTranslation.ReadNextRecordType(reader);
                    var grupType = EnumBinaryTranslation<GroupTypeEnum>.Instance.ParseValue(new MutagenFrame(reader, new ContentLength(4)));

                    if ((!interestingSet?.Contains(grupRec) ?? false)
                        || (uninterestingSet?.Contains(grupRec) ?? false))
                    { // Skip
                        reader.Position -= new ContentLength(16);
                        reader.Position += grupLength;
                        continue;
                    }

                    interestingSet?.Remove(grupRec);

                    reader.Position += new ContentLength(4);

                    using (var frame = new MutagenFrame(reader, new FileLocation(reader.Position + grupLength - 20)))
                    {
                        while (!frame.Complete)
                        {
                            var recordLocation = reader.Position;
                            var targetRec = HeaderTranslation.ReadNextRecordType(reader);
                            var recLength = new ContentLength(reader.ReadUInt32());
                            if (!grupRec.Equals(targetRec))
                            {
                                throw new ArgumentException($"Target Record {targetRec} did not match its containing GRUP: {grupRec}");
                            }
                            reader.Position += new ContentLength(4); // Skip flags
                            var formID = FormID.Factory(reader.ReadBytes(4));
                            ret.Add((targetRec, formID, new FileSection(recordLocation, recordLocation + recLength - 1)));
                            reader.Position += new ContentLength(4);
                            reader.Position += recLength;
                        }
                    }
                }
            }
            return ret;
        }
    }
}
