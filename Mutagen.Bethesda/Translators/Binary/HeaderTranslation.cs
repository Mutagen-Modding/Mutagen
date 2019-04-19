using Mutagen.Bethesda.Internals;
using Noggog;
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

        /// <summary>
        /// Parses span for the expected header, and retrieves the content length
        /// </summary>
        /// <param name="frame">Span to retreive data from</param>
        /// <param name="expectedHeader">Header to expect</param>
        /// <param name="contentLength">Out parameter for the content length</param>
        /// <param name="lengthLength">Byte length of the length data</param>
        /// <returns>True if success.  False if span is too small, or record header parsed in is not expected.</returns>
        public static bool TryGetContentLength(
            ReadOnlySpan<byte> frame,
            RecordType expectedHeader,
            out long contentLength,
            int lengthLength)
        {
            if (frame.Length < Constants.HEADER_LENGTH + lengthLength)
            {
                contentLength = -1;
                return false;
            }
            var header = frame.GetInt32();
            if (expectedHeader.TypeInt != header)
            {
                contentLength = -1;
                return false;
            }
            frame = frame.Slice(4);
            switch (lengthLength)
            {
                case 1:
                    contentLength = frame[0];
                    break;
                case 2:
                    contentLength = frame.GetUInt16();
                    break;
                case 4:
                    contentLength = checked((int)frame.GetUInt32());
                    break;
                default:
                    throw new NotImplementedException();
            }
            return true;
        }

        /// <summary>
        /// Parses span for the expected header, and retrieves the content length.
        /// Throws exception if span is too small, or record header parsed in is not expected.
        /// </summary>
        /// <param name="frame">Span to retreive data from</param>
        /// <param name="expectedHeader">Header to expect</param>
        /// <param name="lengthLength">Byte length of the length data</param>
        /// <returns>content length</returns>
        public static long GetContentLength(
            ReadOnlySpan<byte> frame,
            RecordType expectedHeader,
            int lengthLength)
        {
            if (!TryGetContentLength(
                frame,
                expectedHeader,
                out var contentLength,
                lengthLength))
            {
                throw new ArgumentException($"Expected header was not read in: {expectedHeader}");
            }
            return contentLength;
        }

        /// <summary>
        /// Parses a record header, and retrieves the final position of the content based on the input span
        /// Throws exception if span is too small, or record header parsed in is not expected.
        /// </summary>
        /// <param name="frame">Span to retreive data from</param>
        /// <param name="expectedHeader">Header to expect</param>
        /// <returns>Final position of the content relative to input span</returns>
        public static long GetFinalRecordPosition(
            ReadOnlySpan<byte> frame,
            RecordType expectedHeader)
        {
            if (!TryGetContentLength(
                frame,
                expectedHeader,
                out var contentLength,
                Constants.RECORD_LENGTHLENGTH))
            {
                throw new ArgumentException($"Expected header was not read in: {expectedHeader}");
            }
            return Constants.RECORD_LENGTH + contentLength + Constants.RECORD_META_SKIP;
        }

        /// <summary>
        /// Parses a sub record header, and retrieves the final position of the content based on the input span
        /// Throws exception if span is too small, or record header parsed in is not expected.
        /// </summary>
        /// <param name="frame">Span to retreive data from</param>
        /// <param name="expectedHeader">Header to expect</param>
        /// <returns>Final position of the content relative to input span</returns>
        public static long GetFinalSubrecordPosition(
            ReadOnlySpan<byte> frame,
            RecordType expectedHeader)
        {
            if (!TryGetContentLength(
                frame,
                expectedHeader,
                out var contentLength,
                Constants.SUBRECORD_LENGTHLENGTH))
            {
                throw new ArgumentException($"Expected header was not read in: {expectedHeader}");
            }
            return Constants.SUBRECORD_LENGTH + contentLength;
        }

        /// <summary>
        /// Parses a GRUP header, and retrieves the final position of the content based on the input span
        /// Throws exception if span is too small, or record header parsed in is not expected.
        /// </summary>
        /// <param name="frame">Span to retreive data from</param>
        /// <param name="expectedHeader">Header to expect</param>
        /// <returns>Final position of the content relative to input span</returns>
        public static long GetFinalGroupPosition(
            ReadOnlySpan<byte> frame)
        {
            if (!TryGetContentLength(
                frame,
                GRUP_HEADER,
                out var contentLength,
                Constants.RECORD_LENGTHLENGTH))
            {
                throw new ArgumentException($"Expected header was not read in: {GRUP_HEADER}");
            }
            return Constants.RECORD_LENGTH + contentLength - Constants.HEADER_LENGTH - Constants.RECORD_LENGTHLENGTH;
        }

        public static RecordType GetNextRecordType(
            ReadOnlySpan<byte> frame,
            RecordTypeConverter recordTypeConverter = null)
        {
            var ret = new RecordType(frame.GetInt32());
            ret = recordTypeConverter.ConvertToStandard(ret);
            return ret;
        }

        public static RecordType GetNextRecordType(
            ReadOnlySpan<byte> frame,
            out int contentLength,
            RecordTypeConverter recordTypeConverter = null)
        {
            if (frame.Length < Constants.RECORD_LENGTH)
            {
                throw new ArgumentException("Span was too short");
            }
            var header = new RecordType(frame.GetInt32());
            header = recordTypeConverter.ConvertToStandard(header);
            frame = frame.Slice(4);
            contentLength = checked((int)frame.GetUInt32());
            return header;
        }

        public static RecordType GetNextSubRecordType(
            ReadOnlySpan<byte> frame,
            out int contentLength,
            RecordTypeConverter recordTypeConverter = null)
        {
            if (frame.Length < Constants.RECORD_LENGTH)
            {
                throw new ArgumentException("Span was too short");
            }
            var header = new RecordType(frame.GetInt32());
            header = recordTypeConverter.ConvertToStandard(header);
            frame = frame.Slice(4);
            contentLength = frame.GetUInt16();
            return header;
        }

        public static RecordType GetNextType(
            ReadOnlySpan<byte> frame,
            out int contentLength,
            RecordTypeConverter recordTypeConverter = null,
            bool hopGroup = true)
        {
            var ret = GetNextRecordType(
                frame,
                out contentLength,
                recordTypeConverter);
            if (hopGroup && ret.Equals(GRUP_HEADER))
            {
                ret = GetNextRecordType(frame);
            }
            return ret;
        }

        #region IBinaryReadStream
        public static bool TryParse(
            IBinaryReadStream reader,
            RecordType expectedHeader,
            out long contentLength,
            long lengthLength)
        {
            if (reader.Remaining < Constants.HEADER_LENGTH)
            {
                contentLength = -1;
                return false;
            }
            var header = reader.ReadInt32();
            if (expectedHeader.TypeInt != header)
            {
                contentLength = -1;
                reader.Position -= Constants.HEADER_LENGTH;
                return false;
            }
            switch (lengthLength)
            {
                case 1:
                    contentLength = reader.ReadUInt8();
                    break;
                case 2:
                    contentLength = reader.ReadUInt16();
                    break;
                case 4:
                    contentLength = reader.ReadUInt32();
                    break;
                default:
                    throw new NotImplementedException();
            }
            return true;
        }

        public static bool TryGet(
            IBinaryReadStream reader,
            RecordType expectedHeader,
            out long contentLength,
            long lengthLength)
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

        public static long Parse(
            IBinaryReadStream reader,
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
            IBinaryReadStream reader,
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

        public static long ParseSubrecord(
            IBinaryReadStream reader,
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

        public static long ParseGroup(
            IBinaryReadStream reader)
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
            IBinaryReadStream reader,
            int lengthLength,
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
            IBinaryReadStream reader,
            int lengthLength,
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

        public static bool TryGetRecordType(
            IBinaryReadStream reader,
            int lengthLength,
            out long contentLength,
            RecordType expectedHeader)
        {
            if (TryGet(
                reader,
                expectedHeader,
                out contentLength,
                lengthLength))
            {
                return true;
            }
            return false;
        }

        public static long GetSubrecord(
            IBinaryReadStream reader,
            RecordType expectedHeader)
        {
            var ret = ParseSubrecord(
                reader,
                expectedHeader);
            reader.Position -= Constants.SUBRECORD_LENGTH;
            return ret;
        }

        public static RecordType ReadNextRecordType(
            IBinaryReadStream reader)
        {
            var header = reader.ReadInt32();
            return new RecordType(header);
        }

        public static RecordType GetNextRecordType(
            IBinaryReadStream reader,
            RecordTypeConverter recordTypeConverter = null)
        {
            var header = reader.ReadInt32();
            var ret = new RecordType(header);
            ret = recordTypeConverter.ConvertToStandard(ret);
            reader.Position -= Constants.HEADER_LENGTH;
            return ret;
        }

        public static RecordType GetNextRecordType(
            IBinaryReadStream reader,
            out int contentLength,
            RecordTypeConverter recordTypeConverter = null)
        {
            var ret = ReadNextRecordType(reader, out contentLength);
            ret = recordTypeConverter.ConvertToStandard(ret);
            reader.Position -= Constants.HEADER_LENGTH + Constants.RECORD_LENGTHLENGTH;
            return ret;
        }

        protected static int ReadContentLength(
            IBinaryReadStream reader,
            int lengthLength)
        {
            switch (lengthLength)
            {
                case 1:
                    return reader.ReadUInt8();
                case 2:
                    return reader.ReadUInt16();
                case 4:
                    return (int)reader.ReadUInt32();
                default:
                    throw new NotImplementedException();
            }
        }

        public static RecordType ReadNextRecordType(
            IBinaryReadStream reader,
            int lengthLength,
            out int contentLength)
        {
            var ret = ReadNextRecordType(reader);
            contentLength = ReadContentLength(reader, lengthLength);
            return ret;
        }

        public static RecordType ReadNextRecordType(
            IBinaryReadStream reader,
            out int contentLength)
        {
            return ReadNextRecordType(
                reader,
                Constants.RECORD_LENGTHLENGTH,
                out contentLength);
        }

        public static RecordType ReadNextSubRecordType(
            IBinaryReadStream reader,
            out int contentLength)
        {
            return ReadNextRecordType(
                reader,
                Constants.SUBRECORD_LENGTHLENGTH,
                out contentLength);
        }

        public static RecordType ReadNextType(
            IBinaryReadStream reader,
            out int contentLength)
        {
            var ret = ReadNextRecordType(reader);
            contentLength = ReadContentLength(reader, Constants.RECORD_LENGTHLENGTH);
            if (ret.Equals(GRUP_HEADER))
            {
                return ReadNextRecordType(reader);
            }
            return ret;
        }

        public static RecordType GetNextType(
            IBinaryReadStream reader,
            out int contentLength,
            RecordTypeConverter recordTypeConverter = null,
            bool hopGroup = true)
        {
            var ret = ReadNextRecordType(reader);
            ret = recordTypeConverter.ConvertToStandard(ret);
            contentLength = ReadContentLength(reader, Constants.RECORD_LENGTHLENGTH);
            if (hopGroup && ret.Equals(GRUP_HEADER))
            {
                ret = GetNextRecordType(reader);
            }
            reader.Position -= Constants.HEADER_LENGTH + Constants.RECORD_LENGTHLENGTH;
            return ret;
        }

        public static RecordType GetNextSubRecordType(
            IBinaryReadStream reader,
            out int contentLength,
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
        #endregion
    }
}
