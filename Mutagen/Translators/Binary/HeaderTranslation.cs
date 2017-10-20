using Mutagen.Internals;
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
            MutagenFrame frame,
            RecordType expectedHeader,
            out ContentLength contentLength,
            ContentLength lengthLength)
        {
            if (!frame.TryCheckUpcomingRead(Constants.HEADER_LENGTH))
            {
                contentLength = -1;
                return false;
            }
            var header = Encoding.ASCII.GetString(frame.Reader.ReadBytes(Constants.HEADER_LENGTH));
            if (!expectedHeader.Equals(header))
            {
                contentLength = -1;
                frame.Reader.Position -= Constants.HEADER_LENGTH;
                return false;
            }
            switch (lengthLength.Length)
            {
                case 1:
                    contentLength = frame.Reader.ReadByte();
                    break;
                case 2:
                    contentLength = frame.Reader.ReadInt16();
                    break;
                case 4:
                    contentLength = frame.Reader.ReadInt32();
                    break;
                default:
                    throw new NotImplementedException();
            }
            return true;
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

        public static MutagenFrame ParseRecord(
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
            return frame.Spawn(frame.Reader.Position + contentLength + Constants.RECORD_HEADER_SKIP);
        }

        public static MutagenFrame ParseSubrecord(
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
            return frame.Spawn(frame.Reader.Position + contentLength);
        }
        
        public static MutagenFrame ParseGroup(
            MutagenFrame frame)
        {
            if (!TryParse(
                frame,
                Mutagen.Internals.Group_Registration.GRUP_HEADER,
                out var contentLength,
                Constants.RECORD_LENGTHLENGTH))
            {
                throw new ArgumentException($"Expected header was not read in: {Mutagen.Internals.Group_Registration.GRUP_HEADER}");
            }
            return frame.Spawn(frame.Reader.Position + contentLength - Constants.HEADER_LENGTH - Constants.RECORD_LENGTHLENGTH);
        }

        public static bool TryParseRecordType(
            MutagenFrame frame,
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
                frame,
                expectedHeader,
                out var contentLength,
                lengthLength))
            {
                return true;
            }
            return false;
        }

        public static MutagenFrame GetSubrecord(
            MutagenFrame frame,
            RecordType expectedHeader)
        {
            var ret = ParseSubrecord(
                frame,
                expectedHeader);
            frame.Reader.Position -= Constants.SUBRECORD_LENGTH;
            return ret;
        }

        protected static RecordType ReadNextRecordType(
            MutagenReader reader)
        {
            var header = Encoding.ASCII.GetString(reader.ReadBytes(Constants.HEADER_LENGTH));
            return new RecordType(header, validate: false);
        }

        protected static ContentLength ReadContentLength(
            MutagenReader reader,
            ContentLength lengthLength)
        {
            switch (lengthLength.Length)
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
            MutagenFrame frame,
            ContentLength lengthLength,
            out ContentLength contentLength)
        {
            frame.CheckUpcomingRead(Constants.HEADER_LENGTH + lengthLength);
            var ret = ReadNextRecordType(frame.Reader);
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
            var ret = ReadNextRecordType(frame.Reader);
            contentLength = ReadContentLength(frame.Reader, Constants.RECORD_LENGTHLENGTH);
            if (ret.Equals(Mutagen.Internals.Group_Registration.GRUP_HEADER))
            {
                return ReadNextRecordType(frame.Reader);
            }
            return ret;
        }

        public static RecordType GetNextSubRecordType(
            MutagenFrame frame,
            out ContentLength contentLength)
        {
            var ret = ReadNextRecordType(
                frame,
                Constants.SUBRECORD_LENGTHLENGTH,
                out contentLength);
            frame.Reader.Position -= Constants.SUBRECORD_LENGTH;
            return ret;
        }
    }
}
