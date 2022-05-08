using System.Buffers.Binary;
using Mutagen.Bethesda.Plugins.Meta;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Binary.Headers;

public static class HeaderConstructionMixIns
{ 
    public static ModHeader ModHeader(this GameConstants meta, ReadOnlyMemorySlice<byte> span) => new(meta, span);

    public static GroupHeader GroupHeader(this GameConstants meta, ReadOnlyMemorySlice<byte> span) => new(meta, span);

    public static GroupFrame Group(this GameConstants meta, ReadOnlyMemorySlice<byte> span) => new(meta, span);

    public static MajorRecordHeader MajorRecordHeader(this GameConstants meta, ReadOnlyMemorySlice<byte> span) => new(meta, span);

    public static MajorRecordHeaderWritable MajorRecordHeaderWritable(this GameConstants meta, Span<byte> span) => new(meta, span);

    public static MajorRecordFrame MajorRecord(this GameConstants meta, ReadOnlyMemorySlice<byte> span) => new(meta, span);

    public static SubrecordHeader SubrecordHeader(this GameConstants meta, ReadOnlyMemorySlice<byte> span) => new(meta, span);

    public static SubrecordHeader SubrecordHeader(this GameConstants meta, ReadOnlyMemorySlice<byte> span, RecordType targetType)
    {
        var header = new SubrecordHeader(meta, span);
        if (header.RecordType != targetType)
        {
            throw new ArgumentException($"Unexpected header type: {header.RecordType}");
        }
        return header;
    }

    public static bool TrySubrecordHeader(this GameConstants meta, ReadOnlyMemorySlice<byte> span, RecordType targetType, out SubrecordHeader header)
    {
        if (span.Length < meta.SubConstants.HeaderLength)
        {
            header = default;
            return false;
        }
        header = new SubrecordHeader(meta, span);
        if (header.RecordType != targetType)
        {
            header = default;
            return false;
        }
        return true;
    }

    public static SubrecordFrame Subrecord(this GameConstants meta, ReadOnlyMemorySlice<byte> span) => new(meta, span);

    public static SubrecordFrame Subrecord(this GameConstants meta, ReadOnlyMemorySlice<byte> span, RecordType targetType)
    {
        var header = new SubrecordHeader(meta, span);
        if (header.RecordType != targetType)
        {
            throw new ArgumentException($"Unexpected header type: {header.RecordType}");
        }
        return Binary.Headers.SubrecordFrame.Factory(header, span);
    }

    public static bool TrySubrecord(this GameConstants meta, ReadOnlyMemorySlice<byte> span, RecordType targetType, out SubrecordFrame frame)
    {
        if (span.Length < meta.SubConstants.HeaderLength)
        {
            frame = default;
            return false;
        }
        var header = new SubrecordHeader(meta, span);
        if (header.RecordType != targetType)
        {
            frame = default;
            return false;
        }
        frame = Binary.Headers.SubrecordFrame.Factory(header, span);
        return true;
    }

    public static VariableHeader VariableHeader(this GameConstants meta, ReadOnlyMemorySlice<byte> span)
    {
        RecordType rec = new RecordType(BinaryPrimitives.ReadInt32LittleEndian(span));
        if (rec == Internals.Constants.Group)
        {
            return meta.GroupConstants.VariableMeta(span);
        }
        else
        {
            return meta.MajorConstants.VariableMeta(span);
        }
    }
}