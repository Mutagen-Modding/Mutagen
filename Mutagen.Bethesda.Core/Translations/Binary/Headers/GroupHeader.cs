using Noggog;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Binary
{
    public ref struct GroupHeader
    {
        public GameConstants Meta { get; }
        public ReadOnlySpan<byte> Span { get; }

        public GroupHeader(GameConstants meta, ReadOnlySpan<byte> span)
        {
            this.Meta = meta;
            this.Span = span.Slice(0, meta.GroupConstants.HeaderLength);
        }

        public GameMode GameMode => Meta.GameMode;
        public sbyte HeaderLength => Meta.GroupConstants.HeaderLength;
        public RecordType RecordType => new RecordType(BinaryPrimitives.ReadInt32LittleEndian(this.Span.Slice(0, 4)));
        public uint RecordLength => BinaryPrimitives.ReadUInt32LittleEndian(this.Span.Slice(4, 4));
        public ReadOnlySpan<byte> ContainedRecordTypeSpan => this.Span.Slice(8, 4);
        public RecordType ContainedRecordType => new RecordType(BinaryPrimitives.ReadInt32LittleEndian(this.ContainedRecordTypeSpan));
        public int GroupType => BinaryPrimitives.ReadInt32LittleEndian(this.Span.Slice(12, 4));
        public ReadOnlySpan<byte> LastModifiedSpan => this.Span.Slice(16, 4);
        public long TotalLength => this.RecordLength;
        public bool IsGroup => this.RecordType == Constants.Group;
        public uint ContentLength => checked((uint)(this.TotalLength - this.HeaderLength));
        public int TypeAndLengthLength => Meta.GroupConstants.TypeAndLengthLength;
        public bool IsTopLevel => this.GroupType == 0;
    }

    public ref struct GroupFrame
    {
        public GroupHeader Header { get; }
        public ReadOnlySpan<byte> HeaderAndContentData { get; }
        public ReadOnlySpan<byte> Content => HeaderAndContentData.Slice(this.Header.HeaderLength, checked((int)this.Header.RecordLength));

        public GroupFrame(GameConstants meta, ReadOnlySpan<byte> span)
        {
            this.Header = meta.Group(span);
            this.HeaderAndContentData = span.Slice(0, checked((int)this.Header.TotalLength));
        }

        public GroupFrame(GroupHeader meta, ReadOnlySpan<byte> span)
        {
            this.Header = meta;
            this.HeaderAndContentData = span.Slice(0, checked((int)this.Header.TotalLength));
        }
    }

    public ref struct GroupMemoryFrame
    {
        public GroupHeader Header { get; }
        public ReadOnlyMemorySlice<byte> HeaderAndContentData { get; }
        public ReadOnlySpan<byte> Content => HeaderAndContentData.Slice(this.Header.HeaderLength, checked((int)this.Header.RecordLength));

        public GroupMemoryFrame(GameConstants meta, ReadOnlyMemorySlice<byte> span)
        {
            this.Header = meta.Group(span);
            this.HeaderAndContentData = span.Slice(0, checked((int)this.Header.TotalLength));
        }

        public GroupMemoryFrame(GroupHeader meta, ReadOnlyMemorySlice<byte> span)
        {
            this.Header = meta;
            this.HeaderAndContentData = span.Slice(0, checked((int)this.Header.TotalLength));
        }
    }
}
