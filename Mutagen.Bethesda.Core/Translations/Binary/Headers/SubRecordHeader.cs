using Noggog;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Binary
{
    /// <summary>
    /// A ref struct that overlays on top of bytes that is able to retrive Sub Record header data on demand.
    /// </summary>
    public ref struct SubrecordHeader
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
        /// <param name="span">Span to overlay on, aligned to the start of the Sub Record's header</param>
        public SubrecordHeader(GameConstants meta, ReadOnlySpan<byte> span)
        {
            this.Meta = meta;
            this.Span = span.Slice(0, meta.SubConstants.HeaderLength);
        }

        /// <summary>
        /// GameMode associated with header
        /// </summary>
        public GameMode GameMode => Meta.GameMode;
        
        /// <summary>
        /// The length that the header itself takes
        /// </summary>
        public sbyte HeaderLength => Meta.SubConstants.HeaderLength;
        
        /// <summary>
        /// RecordType of the header
        /// </summary>
        public RecordType RecordType => new RecordType(this.RecordTypeInt);
        
        public int RecordTypeInt => BinaryPrimitives.ReadInt32LittleEndian(this.Span.Slice(0, 4));
        
        /// <summary>
        /// The length explicitly contained in the length bytes of the header
        /// Note that for Sub Records, this is equivalent to ContentLength
        /// </summary>
        public ushort RecordLength => BinaryPrimitives.ReadUInt16LittleEndian(this.Span.Slice(4, 2));
        
        /// <summary>
        /// The length of the content of the Sub Record, excluding the header bytes.
        /// </summary>
        public ushort ContentLength => RecordLength;
        
        /// <summary>
        /// Total length of the Major Record, including the header and its content.
        /// </summary>
        public int TotalLength => this.HeaderLength + this.ContentLength;

        /// <inheritdoc/>
        public override string ToString() => $"{RecordType} => {ContentLength}";
    }

    /// <summary>
    /// A ref struct that overlays on top of bytes that is able to retrive Sub Record data on demand.
    /// </summary>
    public ref struct SubrecordFrame
    {
        /// <summary>
        /// Header ref struct for accessing header data
        /// </summary>
        public SubrecordHeader Header { get; }
        
        /// <summary>
        /// Raw bytes of both header and content data
        /// </summary>
        public ReadOnlySpan<byte> HeaderAndContentData { get; }
        
        /// <summary>
        /// Raw bytes of the content data, excluding the header
        /// </summary>
        public ReadOnlySpan<byte> Content => HeaderAndContentData.Slice(this.Header.HeaderLength, checked((int)this.Header.ContentLength));

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="meta">Game metadata to use as reference for alignment</param>
        /// <param name="span">Span to overlay on, aligned to the start of the header</param>
        public SubrecordFrame(GameConstants meta, ReadOnlySpan<byte> span)
        {
            this.Header = meta.Subrecord(span);
            this.HeaderAndContentData = span.Slice(0, checked((int)this.Header.TotalLength));
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="header">Existing SubrecordHeader struct</param>
        /// <param name="span">Span to overlay on, aligned to the start of the header</param>
        public SubrecordFrame(SubrecordHeader meta, ReadOnlySpan<byte> span)
        {
            this.Header = meta;
            this.HeaderAndContentData = span.Slice(0, checked((int)this.Header.TotalLength));
        }

        /// <inheritdoc/>
        public override string ToString() => this.Header.ToString();
    }

    /// <summary>
    /// A ref struct that overlays on top of bytes that is able to retrive Major Record data on demand.
    /// Unlike SubrecordFrame, this struct exposes its data members as MemorySlices instead of Spans
    /// </summary>
    public ref struct SubrecordMemoryFrame
    {
        /// <summary>
        /// Header ref struct for accessing header data
        /// </summary>
        public SubrecordHeader Header { get; }
        
        /// <summary>
        /// Raw bytes of both header and content data
        /// </summary>
        public ReadOnlyMemorySlice<byte> HeaderAndContentData { get; }
        
        /// <summary>
        /// Raw bytes of the content data, excluding the header
        /// </summary>
        public ReadOnlyMemorySlice<byte> Content => HeaderAndContentData.Slice(this.Header.HeaderLength, checked((int)this.Header.ContentLength));

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="meta">Game metadata to use as reference for alignment</param>
        /// <param name="span">Span to overlay on, aligned to the start of the header</param>
        public SubrecordMemoryFrame(GameConstants meta, ReadOnlyMemorySlice<byte> span)
        {
            this.Header = meta.Subrecord(span);
            this.HeaderAndContentData = span.Slice(0, checked((int)this.Header.TotalLength));
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="header">Existing SubrecordHeader struct</param>
        /// <param name="span">Span to overlay on, aligned to the start of the header</param>
        public SubrecordMemoryFrame(SubrecordHeader meta, ReadOnlyMemorySlice<byte> span)
        {
            this.Header = meta;
            this.HeaderAndContentData = span.Slice(0, checked((int)this.Header.TotalLength));
        }
        
        /// <inheritdoc/>
        public override string ToString() => this.Header.ToString();
    }
}
