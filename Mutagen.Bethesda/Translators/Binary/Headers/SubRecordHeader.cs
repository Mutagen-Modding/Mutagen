using Noggog;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Binary
{
    public ref struct SubRecordHeader
    {
        public GameConstants Meta { get; }
        public ReadOnlySpan<byte> Span { get; }

        public SubRecordHeader(GameConstants meta, ReadOnlySpan<byte> span)
        {
            this.Meta = meta;
            this.Span = span.Slice(0, meta.SubConstants.HeaderLength);
        }

        public GameMode GameMode => Meta.GameMode;
        public sbyte HeaderLength => Meta.SubConstants.HeaderLength;
        public RecordType RecordType => new RecordType(this.RecordTypeInt);
        public int RecordTypeInt => BinaryPrimitives.ReadInt32LittleEndian(this.Span.Slice(0, 4));
        public ushort RecordLength => BinaryPrimitives.ReadUInt16LittleEndian(this.Span.Slice(4, 2));
        public int TotalLength => this.HeaderLength + this.RecordLength;
    }

    public ref struct SubRecordFrame
    {
        public SubRecordHeader Header { get; }
        public ReadOnlySpan<byte> Content { get; }

        public SubRecordFrame(GameConstants meta, ReadOnlySpan<byte> span)
        {
            this.Header = meta.SubRecord(span);
            this.Content = span.Slice(this.Header.HeaderLength, this.Header.RecordLength);
        }

        public SubRecordFrame(SubRecordHeader meta, ReadOnlySpan<byte> span)
        {
            this.Header = meta;
            this.Content = span;
        }
    }

    public ref struct SubRecordMemoryFrame
    {
        public SubRecordHeader Header { get; }
        public ReadOnlyMemorySlice<byte> Content { get; }

        public SubRecordMemoryFrame(GameConstants meta, ReadOnlyMemorySlice<byte> span)
        {
            this.Header = meta.SubRecord(span);
            this.Content = span.Slice(this.Header.HeaderLength, this.Header.RecordLength);
        }

        public SubRecordMemoryFrame(SubRecordHeader meta, ReadOnlyMemorySlice<byte> span)
        {
            this.Header = meta;
            this.Content = span;
        }
    }
}
