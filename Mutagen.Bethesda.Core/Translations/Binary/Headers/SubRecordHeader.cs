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
        public ushort ContentLength => BinaryPrimitives.ReadUInt16LittleEndian(this.Span.Slice(4, 2));
        public int TotalLength => this.HeaderLength + this.ContentLength;
    }

    public ref struct SubRecordFrame
    {
        public SubRecordHeader Header { get; }
        public ReadOnlySpan<byte> AllData { get; }
        public ReadOnlySpan<byte> Content => AllData.Slice(this.Header.HeaderLength, checked((int)this.Header.ContentLength));

        public SubRecordFrame(GameConstants meta, ReadOnlySpan<byte> span)
        {
            this.Header = meta.SubRecord(span);
            this.AllData = span.Slice(0, checked((int)this.Header.TotalLength));
        }

        public SubRecordFrame(SubRecordHeader meta, ReadOnlySpan<byte> span)
        {
            this.Header = meta;
            this.AllData = span.Slice(0, checked((int)this.Header.TotalLength));
        }
    }

    public ref struct SubRecordMemoryFrame
    {
        public SubRecordHeader Header { get; }
        public ReadOnlyMemorySlice<byte> AllData { get; }
        public ReadOnlyMemorySlice<byte> Content => AllData.Slice(this.Header.HeaderLength, checked((int)this.Header.ContentLength));

        public SubRecordMemoryFrame(GameConstants meta, ReadOnlyMemorySlice<byte> span)
        {
            this.Header = meta.SubRecord(span);
            this.AllData = span.Slice(0, checked((int)this.Header.TotalLength));
        }

        public SubRecordMemoryFrame(SubRecordHeader meta, ReadOnlyMemorySlice<byte> span)
        {
            this.Header = meta;
            this.AllData = span.Slice(0, checked((int)this.Header.TotalLength));
        }
    }
}
