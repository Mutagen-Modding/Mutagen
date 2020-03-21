using Noggog;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Binary
{
    public ref struct SubrecordHeader
    {
        public GameConstants Meta { get; }
        public ReadOnlySpan<byte> Span { get; }

        public SubrecordHeader(GameConstants meta, ReadOnlySpan<byte> span)
        {
            this.Meta = meta;
            this.Span = span.Slice(0, meta.SubConstants.HeaderLength);
        }

        public GameMode GameMode => Meta.GameMode;
        public sbyte HeaderLength => Meta.SubConstants.HeaderLength;
        public RecordType RecordType => new RecordType(this.RecordTypeInt);
        public int RecordTypeInt => BinaryPrimitives.ReadInt32LittleEndian(this.Span.Slice(0, 4));
        public ushort ContentLength => BinaryPrimitives.ReadUInt16LittleEndian(this.Span.Slice(4, 2));
        public int TotalLength => this.HeaderLength + this.ContentLength;
    }

    public ref struct SubrecordFrame
    {
        public SubrecordHeader Header { get; }
        public ReadOnlySpan<byte> HeaderAndContentData { get; }
        public ReadOnlySpan<byte> Content => HeaderAndContentData.Slice(this.Header.HeaderLength, checked((int)this.Header.ContentLength));

        public SubrecordFrame(GameConstants meta, ReadOnlySpan<byte> span)
        {
            this.Header = meta.Subrecord(span);
            this.HeaderAndContentData = span.Slice(0, checked((int)this.Header.TotalLength));
        }

        public SubrecordFrame(SubrecordHeader meta, ReadOnlySpan<byte> span)
        {
            this.Header = meta;
            this.HeaderAndContentData = span.Slice(0, checked((int)this.Header.TotalLength));
        }
    }

    public ref struct SubrecordMemoryFrame
    {
        public SubrecordHeader Header { get; }
        public ReadOnlyMemorySlice<byte> HeaderAndContentData { get; }
        public ReadOnlyMemorySlice<byte> Content => HeaderAndContentData.Slice(this.Header.HeaderLength, checked((int)this.Header.ContentLength));

        public SubrecordMemoryFrame(GameConstants meta, ReadOnlyMemorySlice<byte> span)
        {
            this.Header = meta.Subrecord(span);
            this.HeaderAndContentData = span.Slice(0, checked((int)this.Header.TotalLength));
        }

        public SubrecordMemoryFrame(SubrecordHeader meta, ReadOnlyMemorySlice<byte> span)
        {
            this.Header = meta;
            this.HeaderAndContentData = span.Slice(0, checked((int)this.Header.TotalLength));
        }
    }
}
