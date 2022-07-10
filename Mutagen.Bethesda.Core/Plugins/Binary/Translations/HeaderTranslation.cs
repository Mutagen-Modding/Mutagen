using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Internals;
using Mutagen.Bethesda.Plugins.Meta;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Binary.Translations;

public static class HeaderTranslation
{
    public static bool TryParse<TReader>(
        TReader reader,
        RecordType expectedHeader,
        out long contentLength,
        long lengthLength)
        where TReader : IBinaryReadStream
    {
        if (TryGet(
            reader,
            expectedHeader,
            out contentLength,
            lengthLength))
        {
            reader.Position += Constants.HeaderLength + lengthLength;
            return true;
        }
        return false;
    }

    public static bool TryGet<TReader>(
        TReader reader,
        RecordType expectedHeader,
        out long contentLength,
        long lengthLength)
        where TReader : IBinaryReadStream
    {
        if (reader.Remaining < Constants.HeaderLength + lengthLength)
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
                contentLength = reader.GetUInt8(offset: Constants.HeaderLength);
                break;
            case 2:
                contentLength = reader.GetUInt16(offset: Constants.HeaderLength);
                break;
            case 4:
                contentLength = reader.GetUInt32(offset: Constants.HeaderLength);
                break;
            default:
                throw new NotImplementedException();
        }
        return true;
    }

    public static bool TryGetRecordType<TReader>(
        TReader reader,
        RecordType expectedHeader)
        where TReader : IBinaryReadStream
    {
        if (reader.Remaining < Constants.HeaderLength)
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

    public static long Parse<TReader>(
        TReader reader,
        RecordType expectedHeader,
        int lengthLength)
        where TReader : IBinaryReadStream
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

    public static long ParseRecord<TReader>(
        TReader reader,
        RecordType expectedHeader)
        where TReader : IMutagenReadStream
    {
        if (!TryParse(
            reader,
            expectedHeader,
            out var contentLength,
            reader.MetaData.Constants.MajorConstants.LengthLength))
        {
            throw new ArgumentException($"Expected header was not read in: {expectedHeader}");
        }
        return reader.Position + contentLength + reader.MetaData.Constants.MajorConstants.LengthAfterLength;
    }

    public static long ParseRecord<TReader>(TReader reader)
        where TReader : IMutagenReadStream
    {
        reader.Position += 4;
        var len = checked((int)reader.ReadUInt32());
        return reader.Position + len + reader.MetaData.Constants.MajorConstants.LengthAfterLength;
    }

    public static long ParseSubrecord<TReader>(
        TReader reader,
        RecordType expectedHeader,
        int? lengthOverride = null)
        where TReader : IMutagenReadStream
    {
        if (!TryParse(
            reader,
            expectedHeader,
            out var contentLength,
            reader.MetaData.Constants.SubConstants.LengthLength))
        {
            throw new ArgumentException($"Expected header was not read in: {expectedHeader}");
        }
        return reader.Position + (lengthOverride ?? contentLength);
    }

    public static bool TryParseRecordType<TReader>(
        TReader reader,
        int lengthLength,
        RecordType expectedHeader)
        where TReader : IBinaryReadStream
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

    public static bool TryGetRecordType<TReader>(
        TReader reader,
        int lengthLength,
        out long contentLength,
        RecordType expectedHeader)
        where TReader : IBinaryReadStream
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

    public static long GetSubrecord<TReader>(
        TReader reader,
        RecordType expectedHeader)
        where TReader : IMutagenReadStream
    {
        var ret = ParseSubrecord(
            reader,
            expectedHeader);
        reader.Position -= reader.MetaData.Constants.SubConstants.HeaderLength;
        return ret;
    }

    public static RecordType ReadNextRecordType<TReader>(
        TReader reader)
        where TReader : IBinaryReadStream
    {
        var header = reader.ReadInt32();
        return new RecordType(header);
    }

    public static RecordType GetNextRecordType<TReader>(
        TReader reader,
        RecordTypeConverter? recordTypeConverter = null)
        where TReader : IBinaryReadStream
    {
        var header = reader.GetInt32();
        var ret = new RecordType(header);
        ret = recordTypeConverter.ConvertToStandard(ret);
        return ret;
    }

    private static int ReadContentLength<TReader>(
        TReader reader,
        int lengthLength)
        where TReader : IBinaryReadStream
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

    private static int GetContentLength<TReader>(
        TReader reader,
        int lengthLength,
        int offset)
        where TReader : IBinaryReadStream
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

    public static RecordType ReadNextRecordType<TReader>(
        TReader reader,
        int lengthLength,
        out int contentLength)
        where TReader : IBinaryReadStream
    {
        var ret = ReadNextRecordType(reader);
        contentLength = ReadContentLength(reader, lengthLength);
        return ret;
    }

    public static RecordType ReadNextRecordType<TReader>(
        TReader reader,
        out int contentLength)
        where TReader : IMutagenReadStream
    {
        return ReadNextRecordType(
            reader,
            reader.MetaData.Constants.MajorConstants.LengthLength,
            out contentLength);
    }

    public static RecordType ReadNextSubrecordType<TReader>(
        TReader reader,
        out int contentLength)
        where TReader : IMutagenReadStream
    {
        return ReadNextRecordType(
            reader,
            reader.MetaData.Constants.SubConstants.LengthLength,
            out contentLength);
    }

    public static RecordType GetNextType<TReader>(
        TReader reader,
        out int contentLength,
        out long finalPos,
        bool hopGroup = true)
        where TReader : IMutagenReadStream
    {
        GroupHeader groupMeta = reader.GetGroupHeader();
        RecordType ret = groupMeta.RecordType;
        contentLength = checked((int)groupMeta.TotalLength);
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
            finalPos = reader.Position + reader.MetaData.Constants.MajorConstants.HeaderLength + contentLength;
        }
        return ret;
    }

    public static RecordType GetNextSubrecordType<TReader>(
        TReader reader,
        out int contentLength,
        int offset = 0)
        where TReader : IMutagenReadStream
    {
        var ret = new RecordType(reader.GetInt32(offset));
        contentLength = GetContentLength(
            reader: reader,
            lengthLength: reader.MetaData.Constants.SubConstants.LengthLength,
            offset: Constants.HeaderLength + offset);
        return ret;
    }

    public static ReadOnlyMemorySlice<byte> ExtractSubrecordMemory(ReadOnlyMemorySlice<byte> span, int loc, GameConstants meta)
    {
        var subMeta = meta.SubrecordHeader(span.Slice(loc));
        return span.Slice(loc + subMeta.HeaderLength, subMeta.ContentLength);
    }

    public static ReadOnlyMemorySlice<byte> ExtractSubrecordMemory(ReadOnlyMemorySlice<byte> span, GameConstants meta, TypedParseParams translationParams = default)
    {
        var subMeta = meta.SubrecordHeader(span);
        return span.Slice(subMeta.HeaderLength, translationParams.LengthOverride ?? subMeta.ContentLength);
    }

    public static ReadOnlyMemorySlice<byte> ExtractRecordMemory(ReadOnlyMemorySlice<byte> span, GameConstants meta)
    {
        var majorMeta = meta.MajorRecordHeader(span);
        var len = majorMeta.ContentLength;
        len += (byte)meta.MajorConstants.LengthAfterLength;
        return span.Slice(meta.MajorConstants.TypeAndLengthLength, checked((int)len));
    }

    public static ReadOnlyMemorySlice<byte> ExtractGroupMemory(ReadOnlyMemorySlice<byte> span, GameConstants meta)
    {
        var groupMeta = meta.GroupHeader(span);
        var len = groupMeta.ContentLength;
        len += (byte)meta.GroupConstants.LengthAfterLength;
        return span.Slice(meta.GroupConstants.TypeAndLengthLength, checked((int)len));
    }
}
