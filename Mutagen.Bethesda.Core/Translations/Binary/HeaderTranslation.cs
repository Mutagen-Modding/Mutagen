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

        public static bool TryGetRecordType(
            IBinaryReadStream reader,
            RecordType expectedHeader)
        {
            if (reader.Remaining < Constants.HEADER_LENGTH)
            {
                return false;
            }
            var header = reader.GetInt32();
            if (expectedHeader.TypeInt != header)
            {
                return false;
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
            IMutagenReadStream reader,
            RecordType expectedHeader)
        {
            if (!TryParse(
                reader,
                expectedHeader,
                out var contentLength,
                reader.MetaData.SubConstants.LengthLength))
            {
                throw new ArgumentException($"Expected header was not read in: {expectedHeader}");
            }
            return reader.Position + contentLength;
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
            IMutagenReadStream reader,
            RecordType expectedHeader)
        {
            var ret = ParseSubrecord(
                reader,
                expectedHeader);
            reader.Position -= reader.MetaData.SubConstants.HeaderLength;
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
            RecordTypeConverter? recordTypeConverter = null)
        {
            var header = reader.GetInt32();
            var ret = new RecordType(header);
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
            IMutagenReadStream reader,
            out int contentLength)
        {
            return ReadNextRecordType(
                reader,
                reader.MetaData.MajorConstants.LengthLength,
                out contentLength);
        }

        public static RecordType ReadNextSubRecordType(
            IMutagenReadStream reader,
            out int contentLength)
        {
            return ReadNextRecordType(
                reader,
                reader.MetaData.SubConstants.LengthLength,
                out contentLength);
        }

        public static RecordType GetNextType(
            IMutagenReadStream reader,
            out int contentLength,
            out long finalPos,
            bool hopGroup = true)
        {
            GroupHeader groupMeta = reader.GetGroup();
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
                finalPos = reader.Position + reader.MetaData.MajorConstants.HeaderLength + contentLength;
            }
            return ret;
        }

        public static RecordType GetNextSubRecordType(
            IMutagenReadStream reader,
            out int contentLength,
            int offset = 0)
        {
            var ret = new RecordType(reader.GetInt32(offset));
            contentLength = GetContentLength(
                reader: reader,
                lengthLength: reader.MetaData.SubConstants.LengthLength,
                offset: Constants.HEADER_LENGTH + offset);
            return ret;
        }

        public static ReadOnlySpan<byte> ExtractSubrecordSpan(ReadOnlySpan<byte> span, int loc, GameConstants meta)
        {
            var subMeta = meta.SubRecord(span.Slice(loc));
            return span.Slice(loc + subMeta.HeaderLength, subMeta.ContentLength);
        }

        public static ReadOnlyMemorySlice<byte> ExtractSubrecordMemory(ReadOnlyMemorySlice<byte> span, int loc, GameConstants meta)
        {
            var subMeta = meta.SubRecord(span.Span.Slice(loc));
            return span.Slice(loc + subMeta.HeaderLength, subMeta.ContentLength);
        }

        public static ReadOnlyMemorySlice<byte> ExtractSubrecordMemory(ReadOnlyMemorySlice<byte> span, GameConstants meta)
        {
            var subMeta = meta.SubRecord(span.Span);
            return span.Slice(subMeta.HeaderLength, subMeta.ContentLength);
        }

        public static ReadOnlyMemorySlice<byte> ExtractRecordMemory(ReadOnlyMemorySlice<byte> span, GameConstants meta)
        {
            var majorMeta = meta.MajorRecord(span.Span);
            var len = majorMeta.ContentLength;
            len += (byte)meta.MajorConstants.LengthAfterLength;
            return span.Slice(meta.MajorConstants.TypeAndLengthLength, checked((int)len));
        }

        public static ReadOnlyMemorySlice<byte> ExtractGroupMemory(ReadOnlyMemorySlice<byte> span, GameConstants meta)
        {
            var groupMeta = meta.Group(span.Span);
            var len = groupMeta.ContentLength;
            len += (byte)meta.GroupConstants.LengthAfterLength;
            return span.Slice(meta.GroupConstants.TypeAndLengthLength, checked((int)len));
        }
    }
}
