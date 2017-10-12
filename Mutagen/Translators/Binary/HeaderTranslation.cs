using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Binary
{
    public class HeaderTranslation
    {
        public static bool TryParse(
            BinaryReader reader,
            RecordType expectedHeader,
            out int contentLength,
            int lengthLength)
        {
            var header = Encoding.ASCII.GetString(reader.ReadBytes(Constants.HEADER_LENGTH));
            if (!expectedHeader.Equals(header))
            {
                contentLength = 0;
                reader.BaseStream.Position -= Constants.HEADER_LENGTH;
                return false;
            }
            switch (lengthLength)
            {
                case 1:
                    contentLength = reader.ReadByte();
                    break;
                case 2:
                    contentLength = reader.ReadInt16();
                    break;
                case 4:
                    contentLength = reader.ReadInt32();
                    break;
                default:
                    throw new NotImplementedException();
            }
            return true;
        }

        public static int Parse(
            BinaryReader reader,
            RecordType expectedHeader,
            int lengthLength)
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

        public static long ParseRecord(
            BinaryReader reader,
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
            return reader.BaseStream.Position + contentLength + Constants.RECORD_HEADER_SKIP;
        }

        public static long ParseSubrecord(
            BinaryReader reader,
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
            return reader.BaseStream.Position + contentLength;
        }
        
        public static long ParseGroup(
            BinaryReader reader)
        {
            if (!TryParse(
                reader,
                Mutagen.Internals.Group_Registration.GRUP_HEADER,
                out var contentLength,
                Constants.RECORD_LENGTHLENGTH))
            {
                throw new ArgumentException($"Expected header was not read in: {Mutagen.Internals.Group_Registration.GRUP_HEADER}");
            }
            return reader.BaseStream.Position + contentLength - Constants.HEADER_LENGTH - Constants.RECORD_LENGTHLENGTH;
        }

        public static bool TryParseRecordType(
            BinaryReader reader,
            ObjectType type,
            RecordType expectedHeader)
        {
            int lengthLength;
            switch (type)
            {
                case ObjectType.Subrecord:
                    lengthLength = Constants.SUBRECORD_LENGTHLENGTH;
                    break;
                case ObjectType.Record:
                    lengthLength = Constants.RECORD_LENGTHLENGTH;
                    break;
                case ObjectType.Struct:
                case ObjectType.Group:
                case ObjectType.Mod:
                default:
                    throw new ArgumentException();
            }
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

        public static long GetSubrecord(
            BinaryReader reader,
            RecordType expectedHeader)
        {
            var ret = ParseSubrecord(
                reader,
                expectedHeader);
            reader.BaseStream.Position -= Constants.SUBRECORD_LENGTH;
            return ret;
        }

        public static RecordType ReadNextRecordType(
            BinaryReader reader)
        {
            var header = Encoding.ASCII.GetString(reader.ReadBytes(Constants.HEADER_LENGTH));
            return new RecordType(header, validate: false);
        }

        public static int ReadContentLength(
            BinaryReader reader,
            int lengthLength)
        {
            switch (lengthLength)
            {
                case 1:
                    return reader.ReadByte();
                case 2:
                    return reader.ReadUInt16();
                case 4:
                    return (int)reader.ReadUInt32();
                default:
                    throw new NotImplementedException();
            }
        }

        public static RecordType ReadNextRecordType(
            BinaryReader reader,
            int lengthLength,
            out int contentLength)
        {
            var ret = ReadNextRecordType(reader);
            contentLength = ReadContentLength(reader, lengthLength);
            return ret;
        }

        public static RecordType ReadNextRecordType(
            BinaryReader reader,
            out int contentLength)
        {
            return ReadNextRecordType(
                reader,
                Constants.RECORD_LENGTHLENGTH,
                out contentLength);
        }

        public static RecordType ReadNextSubRecordType(
            BinaryReader reader,
            out int contentLength)
        {
            return ReadNextRecordType(
                reader,
                Constants.SUBRECORD_LENGTHLENGTH,
                out contentLength);
        }

        public static RecordType ReadNextType(
            BinaryReader reader,
            out int contentLength)
        {
            var ret = ReadNextRecordType(reader);
            contentLength = ReadContentLength(reader, Constants.RECORD_LENGTHLENGTH);
            if (ret.Equals(Mutagen.Internals.Group_Registration.GRUP_HEADER))
            {
                return ReadNextRecordType(reader);
            }
            return ret;
        }

        public static RecordType GetNextSubRecordType(
            BinaryReader reader,
            out int contentLength)
        {
            var ret = ReadNextRecordType(
                reader,
                Constants.SUBRECORD_LENGTHLENGTH,
                out contentLength);
            reader.BaseStream.Position -= Constants.SUBRECORD_LENGTH;
            return ret;
        }
    }
}
