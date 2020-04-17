using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Binary
{
    /// <summary>
    /// A ref struct that overlays on top of bytes that is able to retrive Mod header data on demand.
    /// </summary>
    public ref struct ModHeader
    {
        /// <summary>
        /// Game metadata to use as reference for alignment
        /// </summary>
        public GameConstants Meta { get; }
        
        /// <summary>
        /// Bytes overlaid onto
        /// </summary>
        public ReadOnlySpan<byte> Span { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="meta">Game metadata to use as reference for alignment</param>
        /// <param name="span">Span to overlay on, aligned to the start of the Group's header</param>
        public ModHeader(GameConstants meta, ReadOnlySpan<byte> span)
        {
            this.Meta = meta;
            this.Span = span.Slice(0, meta.ModHeaderLength);
        }

        /// <summary>
        /// GameMode associated with header
        /// </summary>
        public GameMode GameMode => Meta.GameMode;
        public bool HasContent => this.Span.Length >= this.HeaderLength;
        /// <summary>
        /// The length that the header itself takes
        /// </summary>
        public sbyte HeaderLength => Meta.ModHeaderLength;
        
        /// <summary>
        /// RecordType of the Mod header.
        /// </summary>
        public RecordType RecordType => new RecordType(BinaryPrimitives.ReadInt32LittleEndian(this.Span.Slice(0, 4)));
        
        /// <summary>
        /// The length explicitly contained in the length bytes of the header
        /// Note that for Mod headers, this is equivalent to ContentLength
        /// </summary>
        public uint RecordLength => BinaryPrimitives.ReadUInt32LittleEndian(this.Span.Slice(4, 4));
        
        /// <summary>
        /// The length of the content, excluding the header bytes.
        /// </summary>
        public uint ContentLength => BinaryPrimitives.ReadUInt32LittleEndian(this.Span.Slice(4, 4));
        
        /// <summary>
        /// Total length, including the header and its content.
        /// </summary>
        public long TotalLength => this.HeaderLength + this.ContentLength;
    }
}
