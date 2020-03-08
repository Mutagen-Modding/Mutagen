using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Binary
{
    public ref struct ModHeader
    {
        public GameConstants Meta { get; }
        public ReadOnlySpan<byte> Span { get; }

        public ModHeader(GameConstants meta, ReadOnlySpan<byte> span)
        {
            this.Meta = meta;
            this.Span = span.Slice(0, meta.ModHeaderLength);
        }

        public GameMode GameMode => Meta.GameMode;
        public bool HasContent => this.Span.Length >= this.HeaderLength;
        public sbyte HeaderLength => Meta.ModHeaderLength;
        public RecordType RecordType => new RecordType(BinaryPrimitives.ReadInt32LittleEndian(this.Span.Slice(0, 4)));
        public uint RecordLength => BinaryPrimitives.ReadUInt32LittleEndian(this.Span.Slice(4, 4));
        public long TotalLength => this.HeaderLength + this.RecordLength;
    }
}
