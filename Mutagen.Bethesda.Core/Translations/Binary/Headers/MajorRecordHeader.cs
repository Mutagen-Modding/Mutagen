using Mutagen.Bethesda.Binary;
using Noggog;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Binary
{
    public ref struct MajorRecordHeader
    {
        public GameConstants Meta { get; }
        public ReadOnlySpan<byte> Span { get; }

        public MajorRecordHeader(GameConstants meta, ReadOnlySpan<byte> span)
        {
            this.Meta = meta;
            this.Span = span.Slice(0, meta.MajorConstants.HeaderLength);
        }

        public GameMode GameMode => Meta.GameMode;
        public sbyte HeaderLength => Meta.MajorConstants.HeaderLength;
        public RecordType RecordType => new RecordType(BinaryPrimitives.ReadInt32LittleEndian(this.Span.Slice(0, 4)));
        public uint ContentLength => BinaryPrimitives.ReadUInt32LittleEndian(this.Span.Slice(4, 4));
        public int MajorRecordFlags => BinaryPrimitives.ReadInt32LittleEndian(this.Span.Slice(8, 4));
        public FormID FormID => FormID.Factory(BinaryPrimitives.ReadUInt32LittleEndian(this.Span.Slice(12, 4)));
        public long TotalLength => this.HeaderLength + this.ContentLength;
        public bool IsCompressed => (this.MajorRecordFlags & Mutagen.Bethesda.Constants.CompressedFlag) > 0;
    }

    public ref struct MajorRecordHeaderWritable
    {
        public GameConstants Meta { get; }
        public Span<byte> Span { get; }

        public MajorRecordHeaderWritable(GameConstants meta, Span<byte> span)
        {
            this.Meta = meta;
            this.Span = span.Slice(0, meta.MajorConstants.HeaderLength);
        }

        public GameMode GameMode => Meta.GameMode;
        public sbyte HeaderLength => Meta.MajorConstants.HeaderLength;
        public RecordType RecordType
        {
            get => new RecordType(BinaryPrimitives.ReadInt32LittleEndian(this.Span.Slice(0, 4)));
            set => BinaryPrimitives.WriteInt32LittleEndian(this.Span.Slice(0, 4), value.TypeInt);
        }
        public uint RecordLength
        {
            get => BinaryPrimitives.ReadUInt32LittleEndian(this.Span.Slice(4, 4));
            set => BinaryPrimitives.WriteUInt32LittleEndian(this.Span.Slice(4, 4), value);
        }
        public int MajorRecordFlags
        {
            get => BinaryPrimitives.ReadInt32LittleEndian(this.Span.Slice(8, 4));
            set => BinaryPrimitives.WriteInt32LittleEndian(this.Span.Slice(8, 4), value);
        }
        public FormID FormID
        {
            get => FormID.Factory(BinaryPrimitives.ReadUInt32LittleEndian(this.Span.Slice(12, 4)));
            set => BinaryPrimitives.WriteUInt32LittleEndian(this.Span.Slice(12, 4), value.Raw);
        }
        public long TotalLength => this.HeaderLength + this.RecordLength;
        public bool IsCompressed
        {
            get => (this.MajorRecordFlags & Mutagen.Bethesda.Constants.CompressedFlag) > 0;
            set
            {
                if (value)
                {
                    this.MajorRecordFlags |= Mutagen.Bethesda.Constants.CompressedFlag;
                }
                else
                {
                    this.MajorRecordFlags &= ~Mutagen.Bethesda.Constants.CompressedFlag;
                }
            }
        }
    }

    public ref struct MajorRecordFrame
    {
        public MajorRecordHeader Header { get; }
        public ReadOnlySpan<byte> HeaderAndContentData { get; }
        public ReadOnlySpan<byte> Content => HeaderAndContentData.Slice(this.Header.HeaderLength, checked((int)this.Header.ContentLength));

        public MajorRecordFrame(GameConstants meta, ReadOnlySpan<byte> span)
        {
            this.Header = meta.MajorRecord(span);
            this.HeaderAndContentData = span.Slice(0, checked((int)this.Header.TotalLength));
        }

        public MajorRecordFrame(MajorRecordHeader meta, ReadOnlySpan<byte> span)
        {
            this.Header = meta;
            this.HeaderAndContentData = span.Slice(0, checked((int)this.Header.TotalLength));
        }
    }

    public ref struct MajorRecordMemoryFrame
    {
        public MajorRecordHeader Header { get; }
        public ReadOnlyMemorySlice<byte> HeaderAndContentData { get; }
        public ReadOnlyMemorySlice<byte> Content => HeaderAndContentData.Slice(this.Header.HeaderLength, checked((int)this.Header.ContentLength));

        public MajorRecordMemoryFrame(GameConstants meta, ReadOnlyMemorySlice<byte> span)
        {
            this.Header = meta.MajorRecord(span);
            this.HeaderAndContentData = span.Slice(0, checked((int)this.Header.TotalLength));
        }

        public MajorRecordMemoryFrame(MajorRecordHeader meta, ReadOnlyMemorySlice<byte> span)
        {
            this.Header = meta;
            this.HeaderAndContentData = span.Slice(0, checked((int)this.Header.TotalLength));
        }
    }
}
