using Mutagen.Bethesda.Internals;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Binary
{
    public class HeaderTranslation
    {
        public static readonly RecordType GRUP_HEADER = new RecordType("GRUP");

        public static bool TryParse(
            MutagenReader reader,
            RecordType expectedHeader,
            out ContentLength contentLength,
            ContentLength lengthLength)
        {
            if (!reader.TryCheckUpcomingRead(Constants.HEADER_LENGTH))
            {
                contentLength = ContentLength.Invalid;
                return false;
            }
            var header = Encoding.ASCII.GetString(reader.ReadBytes(Constants.HEADER_LENGTH));
            if (!expectedHeader.Equals(header))
            {
                contentLength = ContentLength.Invalid;
                reader.Position -= Constants.HEADER_LENGTH;
                return false;
            }
            switch (lengthLength.Value)
            {
                case 1:
                    contentLength = new ContentLength(reader.ReadByte());
                    break;
                case 2:
                    contentLength = new ContentLength(reader.ReadInt16());
                    break;
                case 4:
                    contentLength = new ContentLength(reader.ReadInt32());
                    break;
                default:
                    throw new NotImplementedException();
            }
            return true;
        }

        public static bool TryGet(
            MutagenReader reader,
            RecordType expectedHeader,
            out ContentLength contentLength,
            ContentLength lengthLength)
        {
            var ret = TryParse(
                reader,
                expectedHeader,
                out contentLength,
                lengthLength);
            if (ret)
            {
                reader.Position -= Constants.HEADER_LENGTH + lengthLength;
            }
            return ret;
        }

        public static ContentLength Parse(
            MutagenReader reader,
            RecordType expectedHeader,
            ContentLength lengthLength)
        {
            if (!TryParse(
                reader,
                expectedHeader,
                out var contentLength,
                lengthLength))
            {
                throw new ArgumentException($"Expected header was not read in: {expectedHeader}");
            }
            return contentLength;
        }

        public static FileLocation ParseRecord(
            MutagenReader reader,
            RecordType expectedHeader)
        {
            if (!TryParse(
                reader,
                expectedHeader,
                out var contentLength,
                Constants.RECORD_LENGTHLENGTH))
            {
                throw new ArgumentException($"Expected header was not read in: {expectedHeader}");
            }
            return reader.Position + contentLength + Constants.RECORD_META_SKIP;
        }

        public static FileLocation ParseSubrecord(
            MutagenReader reader,
            RecordType expectedHeader)
        {
            if (!TryParse(
                reader,
                expectedHeader,
                out var contentLength,
                Constants.SUBRECORD_LENGTHLENGTH))
            {
                throw new ArgumentException($"Expected header was not read in: {expectedHeader}");
            }
            return reader.Position + contentLength;
        }

        public static FileLocation ParseGroup(
            MutagenReader reader)
        {
            if (!TryParse(
                reader,
                GRUP_HEADER,
                out var contentLength,
                Constants.RECORD_LENGTHLENGTH))
            {
                throw new ArgumentException($"Expected header was not read in: {GRUP_HEADER}");
            }
            return reader.Position + contentLength - Constants.HEADER_LENGTH - Constants.RECORD_LENGTHLENGTH;
        }

        public static bool TryParseRecordType(
            MutagenReader reader,
            ContentLength lengthLength,
            RecordType expectedHeader)
        {
            if (TryParse(
                reader,
                expectedHeader,
                out var contentLength,
                lengthLength))
            {
                return true;
            }
            return false;
        }

        public static bool TryGetRecordType(
            MutagenReader reader,
            ContentLength lengthLength,
            RecordType expectedHeader)
        {
            if (TryGet(
                reader,
                expectedHeader,
                out var contentLength,
                lengthLength))
            {
                return true;
            }
            return false;
        }

        public static FileLocation GetSubrecord(
            MutagenReader reader,
            RecordType expectedHeader)
        {
            var ret = ParseSubrecord(
                reader,
                expectedHeader);
            reader.Position -= Constants.SUBRECORD_LENGTH;
            return ret;
        }

        public static RecordType ReadNextRecordType(
            MutagenReader reader)
        {
            var header = Encoding.ASCII.GetString(reader.ReadBytes(Constants.HEADER_LENGTH));
            return new RecordType(header, validate: false);
        }

        public static RecordType GetNextRecordType(
            MutagenReader reader,
            RecordTypeConverter recordTypeConverter = null)
        {
            var header = Encoding.ASCII.GetString(reader.ReadBytes(Constants.HEADER_LENGTH));
            var ret = new RecordType(header, validate: false);
            ret = recordTypeConverter.ConvertToStandard(ret);
            reader.Position -= Constants.HEADER_LENGTH;
            return ret;
        }

        public static RecordType GetNextRecordType(
            MutagenReader reader,
            out ContentLength contentLength,
            RecordTypeConverter recordTypeConverter = null)
        {
            var ret = ReadNextRecordType(reader, out contentLength);
            ret = recordTypeConverter.ConvertToStandard(ret);
            reader.Position -= Constants.HEADER_LENGTH + Constants.RECORD_LENGTHLENGTH;
            return ret;
        }

        protected static ContentLength ReadContentLength(
            MutagenReader reader,
            ContentLength lengthLength)
        {
            switch (lengthLength.Value)
            {
                case 1:
                    return new ContentLength(reader.ReadByte());
                case 2:
                    return new ContentLength(reader.ReadUInt16());
                case 4:
                    return new ContentLength((int)reader.ReadUInt32());
                default:
                    throw new NotImplementedException();
            }
        }

        public static RecordType ReadNextRecordType(
            MutagenReader reader,
            ContentLength lengthLength,
            out ContentLength contentLength)
        {
            reader.CheckUpcomingRead(Constants.HEADER_LENGTH + lengthLength);
            var ret = ReadNextRecordType(reader);
            contentLength = ReadContentLength(reader, lengthLength);
            return ret;
        }

        public static RecordType ReadNextRecordType(
            MutagenReader reader,
            out ContentLength contentLength)
        {
            return ReadNextRecordType(
                reader,
                Constants.RECORD_LENGTHLENGTH,
                out contentLength);
        }

        public static RecordType ReadNextSubRecordType(
            MutagenReader reader,
            out ContentLength contentLength)
        {
            return ReadNextRecordType(
                reader,
                Constants.SUBRECORD_LENGTHLENGTH,
                out contentLength);
        }

        public static RecordType ReadNextType(
            MutagenReader reader,
            out ContentLength contentLength)
        {
            reader.CheckUpcomingRead(Constants.HEADER_LENGTH + Constants.RECORD_LENGTHLENGTH);
            var ret = ReadNextRecordType(reader);
            contentLength = ReadContentLength(reader, Constants.RECORD_LENGTHLENGTH);
            if (ret.Equals(GRUP_HEADER))
            {
                return ReadNextRecordType(reader);
            }
            return ret;
        }

        public static RecordType GetNextType(
            MutagenReader reader,
            out ContentLength contentLength,
            RecordTypeConverter recordTypeConverter = null)
        {
            reader.CheckUpcomingRead(Constants.HEADER_LENGTH + Constants.RECORD_LENGTHLENGTH);
            var ret = ReadNextRecordType(reader);
            ret = recordTypeConverter.ConvertToStandard(ret);
            contentLength = ReadContentLength(reader, Constants.RECORD_LENGTHLENGTH);
            if (ret.Equals(GRUP_HEADER))
            {
                ret = GetNextRecordType(reader);
            }
            reader.Position -= Constants.HEADER_LENGTH + Constants.RECORD_LENGTHLENGTH;
            return ret;
        }

        public static RecordType GetNextSubRecordType(
            MutagenReader reader,
            out ContentLength contentLength,
            RecordTypeConverter recordTypeConverter = null)
        {
            var ret = ReadNextRecordType(
                reader,
                Constants.SUBRECORD_LENGTHLENGTH,
                out contentLength);
            ret = recordTypeConverter.ConvertToStandard(ret);
            reader.Position -= Constants.SUBRECORD_LENGTH;
            return ret;
        }
    }
}
