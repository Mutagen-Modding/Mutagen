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
    public static class MajorRecordLocator
    {
        private readonly static RecordType GRUP = new RecordType("GRUP");

        public static Dictionary<RawFormID, FileLocation> GetFileLocations(
            Stream stream,
            params RecordType[] interestingTypes)
        {
            return GetFileLocations(stream, (IEnumerable<RecordType>)interestingTypes);
        }

        public static Dictionary<RawFormID, FileLocation> GetFileLocations(
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

            Dictionary<RawFormID, FileLocation> ret = new Dictionary<RawFormID, FileLocation>();
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
                            var formID = RawFormID.Factory(reader.ReadBytes(4));
                            ret[formID] = recordLocation;
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
