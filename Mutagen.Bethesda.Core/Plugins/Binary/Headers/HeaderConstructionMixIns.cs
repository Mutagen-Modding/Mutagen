using System.Buffers.Binary;
using Mutagen.Bethesda.Plugins.Meta;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Binary.Headers;

public static class HeaderConstructionMixIns
{ 
    public static ModHeader ModHeader(this GameConstants meta, ReadOnlyMemorySlice<byte> span) => new(meta, span);

    public static GroupHeader GroupHeader(this GameConstants meta, ReadOnlyMemorySlice<byte> span) => new(meta, span);

    public static bool TryGroupHeader(this GameConstants meta, ReadOnlyMemorySlice<byte> span, out GroupHeader header)
    {
        if (span.Length < meta.GroupConstants.HeaderLength)
        {
            header = default;
            return false;
        }
        header = new GroupHeader(meta, span);
        return header.IsGroup;
    }

    public static GroupFrame Group(this GameConstants meta, ReadOnlyMemorySlice<byte> span) => new(meta, span);

    public static bool TryGroup(this GameConstants meta, ReadOnlyMemorySlice<byte> span, out GroupFrame frame)
    {
        if (!meta.TryGroupHeader(span, out var header))
        {
            frame = default;
            return false;
        }

        frame = new GroupFrame(header, span);
        return true;
    }

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
            return meta.VariableHeader(span, ObjectType.Group);
        }
        else
        {
            return meta.VariableHeader(span, ObjectType.Record);
        }
    }

    public static VariablePinHeader VariableHeader(this GameConstants meta, ReadOnlyMemorySlice<byte> span, int loc)
    {
        span = span.Slice(loc);
        RecordType rec = new RecordType(BinaryPrimitives.ReadInt32LittleEndian(span));
        if (rec == Internals.Constants.Group)
        {
            return meta.VariableHeader(span, ObjectType.Group, loc);
        }
        else
        {
            return meta.VariableHeader(span, ObjectType.Record, loc);
        }
    }

    public static VariableHeader VariableHeader(this GameConstants meta, ReadOnlyMemorySlice<byte> span, ObjectType objectType) => new(meta, objectType, span);

    public static VariableHeader VariableHeader(this GameConstants meta, ReadOnlyMemorySlice<byte> span, RecordHeaderConstants headerConstants) => new(meta, headerConstants, span);

    public static VariablePinHeader VariableHeader(this GameConstants meta, ReadOnlyMemorySlice<byte> span, ObjectType objectType, int loc) => new(meta, objectType, span, loc);

    public static VariablePinHeader VariableHeader(this GameConstants meta, ReadOnlyMemorySlice<byte> span, RecordHeaderConstants headerConstants, int loc) => new(meta, headerConstants, span, loc);

    public static VariableHeader GetVariableHeader(this GameConstants meta, IBinaryReadStream stream,
        ObjectType objectType, int offset = 0)
    {
        return GetVariableHeader(meta, stream, meta.Constants(objectType), offset);
    }

    public static VariableHeader GetVariableHeader(this GameConstants meta, IBinaryReadStream stream, RecordHeaderConstants headerConstants, int offset = 0)
    {
        return new(meta, headerConstants, stream.GetMemory(headerConstants.HeaderLength, offset));
    }
    
    public static VariableHeader ReadVariableHeader(this GameConstants meta, IBinaryReadStream stream, ObjectType objectType)
    {
        return ReadVariableHeader(meta, stream, meta.Constants(objectType));
    }
    
    public static VariableHeader ReadVariableHeader(this GameConstants meta, IBinaryReadStream stream, RecordHeaderConstants headerConstants)
    {
        return new(meta, headerConstants, stream.ReadMemory(headerConstants.HeaderLength));
    }
    
    public static bool TryGetAsMajorRecord(this VariableHeader header, out MajorRecordFrame majorFrame)
    {
        if (header.HeaderConstants.ObjectType != ObjectType.Record)
        {
            majorFrame = default;
            return false;
        }

        majorFrame = new MajorRecordFrame(header.Constants, header.HeaderAndContentData);
        return true;
    }
    
    public static bool TryGetAsMajorRecord(this VariablePinHeader header, out MajorRecordPinFrame majorFrame)
    {
        if (header.HeaderConstants.ObjectType != ObjectType.Record)
        {
            majorFrame = default;
            return false;
        }

        majorFrame = new MajorRecordPinFrame(header.Constants, header.HeaderAndContentData, header.Location);
        return true;
    }

    public static bool TryGetAsGroup(this VariableHeader header, out GroupFrame group)
    {
        if (header.HeaderConstants.ObjectType != ObjectType.Group)
        {
            group = default;
            return false;
        }

        group = new GroupFrame(header.Constants, header.HeaderAndContentData);
        return true;
    }

    public static bool TryGetAsGroup(this VariablePinHeader header, out GroupPinFrame group)
    {
        if (header.HeaderConstants.ObjectType != ObjectType.Group)
        {
            group = default;
            return false;
        }

        group = new GroupPinFrame(header.Constants, header.HeaderAndContentData, header.Location);
        return true;
    }
}