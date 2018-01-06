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
            MutagenFrame frame,
            RecordType expectedHeader,
            out ContentLength contentLength,
            ContentLength lengthLength)
        {
            if (!frame.TryCheckUpcomingRead(Constants.HEADER_LENGTH))
            {
                contentLength = ContentLength.Invalid;
                return false;
            }
            var header = Encoding.ASCII.GetString(frame.Reader.ReadBytes(Constants.HEADER_LENGTH));
            if (!expectedHeader.Equals(header))
            {
                contentLength = ContentLength.Invalid;
                frame.Reader.Position -= Constants.HEADER_LENGTH;
                return false;
            }
            switch (lengthLength.Value)
            {
                case 1:
                    contentLength = new ContentLength(frame.Reader.ReadByte());
                    break;
                case 2:
                    contentLength = new ContentLength(frame.Reader.ReadInt16());
                    break;
                case 4:
                    contentLength = new ContentLength(frame.Reader.ReadInt32());
                    break;
                default:
                    throw new NotImplementedException();
            }
            return true;
        }

        public static bool TryGet(
            MutagenFrame frame,
            RecordType expectedHeader,
            out ContentLength contentLength,
            ContentLength lengthLength)
        {
            var ret = TryParse(
                frame,
                expectedHeader,
                out contentLength,
                lengthLength);
            if (ret)
            {
                frame.Position -= Constants.HEADER_LENGTH + lengthLength;
            }
            return ret;
        }

        public static ContentLength Parse(
            MutagenFrame frame,
            RecordType expectedHeader,
            ContentLength lengthLength)
        {
            if (!TryParse(
                frame,
                expectedHeader,
                out var contentLength,
                lengthLength))
            {
                throw new ArgumentException($"Expected header was not read in: {expectedHeader}");
            }
            return contentLength;
        }

        public static FileLocation ParseRecord(
            MutagenFrame frame,
            RecordType expectedHeader)
        {
            if (!TryParse(
                frame,
                expectedHeader,
                out var contentLength,
                Constants.RECORD_LENGTHLENGTH))
            {
                throw new ArgumentException($"Expected header was not read in: {expectedHeader}");
            }
            return frame.Reader.Position + contentLength + Constants.RECORD_HEADER_SKIP;
        }

        public static FileLocation ParseSubrecord(
            MutagenFrame frame,
            RecordType expectedHeader)
        {
            if (!TryParse(
                frame,
                expectedHeader,
                out var contentLength,
                Constants.SUBRECORD_LENGTHLENGTH))
            {
                throw new ArgumentException($"Expected header was not read in: {expectedHeader}");
            }
            return frame.Reader.Position + contentLength;
        }

        public static FileLocation ParseGroup(
            MutagenFrame frame)
        {
            if (!TryParse(
                frame,
                GRUP_HEADER,
                out var contentLength,
                Constants.RECORD_LENGTHLENGTH))
            {
                throw new ArgumentException($"Expected header was not read in: {GRUP_HEADER}");
            }
            return frame.Reader.Position + contentLength - Constants.HEADER_LENGTH - Constants.RECORD_LENGTHLENGTH;
        }

        public static bool TryParseRecordType(
            MutagenFrame frame,
            ObjectType type,
            RecordType expectedHeader)
        {
            ContentLength lengthLength;
            switch (type)
            {
                case ObjectType.Subrecord:
                    lengthLength = Constants.SUBRECORD_LENGTHLENGTH;
                    break;
                case ObjectType.Record:
                    lengthLength = Constants.RECORD_LENGTHLENGTH;
                    break;
                case ObjectType.Group:
                case ObjectType.Mod:
                default:
                    throw new ArgumentException();
            }
            if (TryParse(
                frame,
                expectedHeader,
                out var contentLength,
                lengthLength))
            {
                return true;
            }
            return false;
        }

        public static bool TryGetRecordType(
            MutagenFrame frame,
            ObjectType type,
            RecordType expectedHeader)
        {
            ContentLength lengthLength;
            switch (type)
            {
                case ObjectType.Subrecord:
                    lengthLength = Constants.SUBRECORD_LENGTHLENGTH;
                    break;
                case ObjectType.Record:
                    lengthLength = Constants.RECORD_LENGTHLENGTH;
                    break;
                case ObjectType.Group:
                case ObjectType.Mod:
                default:
                    throw new ArgumentException();
            }
            if (TryGet(
                frame,
                expectedHeader,
                out var contentLength,
                lengthLength))
            {
                return true;
            }
            return false;
        }

        public static FileLocation GetSubrecord(
            MutagenFrame frame,
            RecordType expectedHeader)
        {
            var ret = ParseSubrecord(
                frame,
                expectedHeader);
            frame.Reader.Position -= Constants.SUBRECORD_LENGTH;
            return ret;
        }

        public static RecordType ReadNextRecordType(
            MutagenFrame frame)
        {
            var header = Encoding.ASCII.GetString(frame.Reader.ReadBytes(Constants.HEADER_LENGTH));
            return new RecordType(header, validate: false);
        }

        public static RecordType GetNextRecordType(
            MutagenFrame frame,
            RecordTypeConverter recordTypeConverter = null)
        {
            var header = Encoding.ASCII.GetString(frame.Reader.ReadBytes(Constants.HEADER_LENGTH));
            var ret = new RecordType(header, validate: false);
            ret = recordTypeConverter.Convert(ret);
            frame.Position -= Constants.HEADER_LENGTH;
            return ret;
        }

        public static RecordType GetNextRecordType(
            MutagenFrame frame,
            out ContentLength contentLength,
            RecordTypeConverter recordTypeConverter = null)
        {
            var ret = ReadNextRecordType(frame, out contentLength);
            ret = recordTypeConverter.Convert(ret);
            frame.Position -= Constants.HEADER_LENGTH + Constants.RECORD_LENGTHLENGTH;
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
            MutagenFrame frame,
            ContentLength lengthLength,
            out ContentLength contentLength)
        {
            frame.CheckUpcomingRead(Constants.HEADER_LENGTH + lengthLength);
            var ret = ReadNextRecordType(frame);
            contentLength = ReadContentLength(frame.Reader, lengthLength);
            return ret;
        }

        public static RecordType ReadNextRecordType(
            MutagenFrame frame,
            out ContentLength contentLength)
        {
            return ReadNextRecordType(
                frame,
                Constants.RECORD_LENGTHLENGTH,
                out contentLength);
        }

        public static RecordType ReadNextSubRecordType(
            MutagenFrame frame,
            out ContentLength contentLength)
        {
            return ReadNextRecordType(
                frame,
                Constants.SUBRECORD_LENGTHLENGTH,
                out contentLength);
        }

        public static RecordType ReadNextType(
            MutagenFrame frame,
            out ContentLength contentLength)
        {
            frame.CheckUpcomingRead(Constants.HEADER_LENGTH + Constants.RECORD_LENGTHLENGTH);
            var ret = ReadNextRecordType(frame);
            contentLength = ReadContentLength(frame.Reader, Constants.RECORD_LENGTHLENGTH);
            if (ret.Equals(GRUP_HEADER))
            {
                return ReadNextRecordType(frame);
            }
            return ret;
        }

        public static RecordType GetNextType(
            MutagenFrame frame,
            out ContentLength contentLength,
            RecordTypeConverter recordTypeConverter = null)
        {
            frame.CheckUpcomingRead(Constants.HEADER_LENGTH + Constants.RECORD_LENGTHLENGTH);
            var ret = ReadNextRecordType(frame);
            ret = recordTypeConverter.Convert(ret);
            contentLength = ReadContentLength(frame.Reader, Constants.RECORD_LENGTHLENGTH);
            if (ret.Equals(GRUP_HEADER))
            {
                ret = GetNextRecordType(frame);
            }
            frame.Position -= Constants.HEADER_LENGTH + Constants.RECORD_LENGTHLENGTH;
            return ret;
        }

        public static RecordType GetNextSubRecordType(
            MutagenFrame frame,
            out ContentLength contentLength,
            RecordTypeConverter recordTypeConverter = null)
        {
            var ret = ReadNextRecordType(
                frame,
                Constants.SUBRECORD_LENGTHLENGTH,
                out contentLength);
            ret = recordTypeConverter.Convert(ret);
            frame.Reader.Position -= Constants.SUBRECORD_LENGTH;
            return ret;
        }
    }
}
