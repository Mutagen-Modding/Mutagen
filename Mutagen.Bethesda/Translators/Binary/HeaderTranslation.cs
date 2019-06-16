using Mutagen.Bethesda.Internals;
using Noggog;
using System;
using System.Buffers.Binary;
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
            IBinaryReadStream reader,
            RecordType expectedHeader,
            out long contentLength,
            long lengthLength)
        {
            if (TryGet(
                reader,
                expectedHeader,
                out contentLength,
                lengthLength))
            {
                reader.Position += Constants.HEADER_LENGTH + lengthLength;
                return true;
            }
            return false;
        }

        public static bool TryGet(
            IBinaryReadStream reader,
            RecordType expectedHeader,
            out long contentLength,
            long lengthLength)
        {
            if (reader.Remaining < Constants.HEADER_LENGTH + lengthLength)
            {
                contentLength = -1;
                return false;
            }
            var header = reader.GetInt32();
            if (expectedHeader.TypeInt != header)
            {
                contentLength = -1;
                return false;
            }
            switch (lengthLength)
            {
                case 1:
                    contentLength = reader.GetUInt8(offset: Constants.HEADER_LENGTH);
                    break;
                case 2:
                    contentLength = reader.GetUInt16(offset: Constants.HEADER_LENGTH);
                    break;
                case 4:
                    contentLength = reader.GetUInt32(offset: Constants.HEADER_LENGTH);
                    break;
                default:
                    throw new NotImplementedException();
            }
            return true;
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
            IMutagenReadStream reader,
            RecordType expectedHeader)
        {
            if (!TryParse(
                reader,
                expectedHeader,
                out var contentLength,
                reader.MetaData.MajorConstants.LengthLength))
            {
                throw new ArgumentException($"Expected header was not read in: {expectedHeader}");
            }
            return reader.Position + contentLength + reader.MetaData.MajorConstants.LengthAfterLength;
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
            var header = reader.GetInt32();
            var ret = new RecordType(header);
            ret = recordTypeConverter.ConvertToStandard(ret);
            return ret;
        }

        public static RecordType GetNextRecordType(
            IBinaryReadStream reader,
            out int contentLength,
            RecordTypeConverter recordTypeConverter = null)
        {
            var ret = new RecordType(reader.GetInt32());
            contentLength = GetContentLength(reader, Constants.RECORD_LENGTHLENGTH, Constants.HEADER_LENGTH);
            ret = recordTypeConverter.ConvertToStandard(ret);
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

        protected static int GetContentLength(
            IBinaryReadStream reader,
            int lengthLength,
            int offset)
        {
            switch (lengthLength)
            {
                case 1:
                    return reader.GetUInt8(offset);
                case 2:
                    return reader.GetUInt16(offset);
                case 4:
                    return (int)reader.GetUInt32(offset);
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
            IMutagenReadStream reader,
            out int contentLength,
            out long finalPos,
            bool hopGroup = true)
        {
            GroupRecordMeta groupMeta = reader.MetaData.GetGroup(reader);
            RecordType ret = groupMeta.RecordType;
            contentLength = checked((int)groupMeta.RecordLength);
            if (groupMeta.IsGroup)
            {
                if (hopGroup)
                {
                    ret = groupMeta.ContainedRecordType;
                }
                finalPos = reader.Position + groupMeta.TotalLength;
            }
            else
            {
                finalPos = reader.Position + Constants.RECORD_META_LENGTH + Constants.HEADER_LENGTH + contentLength;
            }
            return ret;
        }

        public static RecordType GetNextSubRecordType(
            IBinaryReadStream reader,
            out int contentLength,
            int offset = 0)
        {
            var ret = new RecordType(reader.GetInt32(offset));
            contentLength = GetContentLength(
                reader: reader,
                lengthLength: Constants.SUBRECORD_LENGTHLENGTH,
                offset: Constants.HEADER_LENGTH + offset);
            return ret;
        }

        public static ReadOnlyMemorySlice<byte> ExtractRecordMemory(ReadOnlyMemorySlice<byte> span, int loc, MetaDataConstants meta)
        {
            var majorMeta = meta.MajorRecord(span.Span.Slice(loc));
            return span.Slice(loc + majorMeta.HeaderLength, checked((int)majorMeta.RecordLength));
        }

        public static ReadOnlySpan<byte> ExtractSubrecordSpan(ReadOnlyMemorySlice<byte> span, int loc, MetaDataConstants meta)
        {
            var subMeta = meta.SubRecord(span.Span.Slice(loc));
            return span.Span.Slice(loc + subMeta.HeaderLength, checked((int)subMeta.RecordLength));
        }

        public static ReadOnlySpan<byte> ExtractRecordSpan(ReadOnlyMemorySlice<byte> span, int loc, MetaDataConstants meta)
        {
            var majorMeta = meta.MajorRecord(span.Span.Slice(loc));
            return span.Span.Slice(loc + majorMeta.HeaderLength, checked((int)majorMeta.RecordLength));
        }

        public static ReadOnlyMemorySlice<byte> ExtractSubrecordWrapperMemory(ReadOnlyMemorySlice<byte> span, MetaDataConstants meta)
        {
            var subMeta = meta.SubRecord(span.Span);
            return span.Slice(subMeta.HeaderLength, subMeta.RecordLength);
        }

        public static ReadOnlyMemorySlice<byte> ExtractRecordWrapperMemory(ReadOnlyMemorySlice<byte> span, MetaDataConstants meta)
        {
            var majorMeta = meta.MajorRecord(span.Span);
            var len = majorMeta.RecordLength;
            len += (byte)meta.MajorConstants.LengthAfterLength;
            return span.Slice(meta.MajorConstants.TypeAndLengthLength, checked((int)len));
        }

        public static ReadOnlyMemorySlice<byte> ExtractGroupWrapperMemory(ReadOnlyMemorySlice<byte> span, MetaDataConstants meta)
        {
            var groupMeta = meta.Group(span.Span);
            var len = groupMeta.ContentLength;
            len += (byte)meta.GroupConstants.LengthAfterLength;
            return span.Slice(meta.GroupConstants.TypeAndLengthLength, checked((int)len));
        }
    }
}
