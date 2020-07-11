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
        public ReadOnlySpan<byte> HeaderData { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="meta">Game metadata to use as reference for alignment</param>
        /// <param name="span">Span to overlay on, aligned to the start of the Sub Record's header</param>
        public SubrecordHeader(GameConstants meta, ReadOnlySpan<byte> span)
        {
            this.Meta = meta;
            this.HeaderData = span.Slice(0, meta.SubConstants.HeaderLength);
        }

        /// <summary>
        /// Game release associated with header
        /// </summary>
        public GameRelease Release => Meta.Release;
        
        /// <summary>
        /// The length that the header itself takes
        /// </summary>
        public sbyte HeaderLength => Meta.SubConstants.HeaderLength;
        
        /// <summary>
        /// RecordType of the header
        /// </summary>
        public RecordType RecordType => new RecordType(this.RecordTypeInt);
        
        public int RecordTypeInt => BinaryPrimitives.ReadInt32LittleEndian(this.HeaderData.Slice(0, 4));
        
        /// <summary>
        /// The length explicitly contained in the length bytes of the header
        /// Note that for Sub Records, this is equivalent to ContentLength
        /// </summary>
        public ushort RecordLength => BinaryPrimitives.ReadUInt16LittleEndian(this.HeaderData.Slice(4, 2));
        
        /// <summary>
        /// The length of the content of the Sub Record, excluding the header bytes.
        /// </summary>
        public ushort ContentLength => RecordLength;
        
        /// <summary>
        /// Total length of the Sub Record, including the header and its content.
        /// </summary>
        public int TotalLength => this.HeaderLength + this.ContentLength;

        /// <inheritdoc/>
        public override string ToString() => $"{RecordType} =>0x{ContentLength:X}";
    }

    /// <summary>
    /// A ref struct that overlays on top of bytes that is able to retrive Sub Record data on demand.
    /// </summary>
    public ref struct SubrecordFrame
    {
        private readonly SubrecordHeader _header { get; }
        
        /// <summary>
        /// Raw bytes of both header and content data
        /// </summary>
        public ReadOnlySpan<byte> HeaderAndContentData { get; }

        /// <summary>
        /// Total length of the Sub Record, including the header and its content.
        /// </summary>
        public int TotalLength => this.Content.Length + this._header.HeaderLength;
        
        /// <summary>
        /// Raw bytes of the content data, excluding the header
        /// </summary>
        public ReadOnlySpan<byte> Content => HeaderAndContentData.Slice(this._header.HeaderLength, checked((int)this._header.ContentLength));

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="meta">Game metadata to use as reference for alignment</param>
        /// <param name="span">Span to overlay on, aligned to the start of the header</param>
        public SubrecordFrame(GameConstants meta, ReadOnlySpan<byte> span)
        {
            this._header = meta.Subrecord(span);
            this.HeaderAndContentData = span.Slice(0, checked((int)this._header.TotalLength));
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="header">Existing SubrecordHeader struct</param>
        /// <param name="span">Span to overlay on, aligned to the start of the header</param>
        public SubrecordFrame(SubrecordHeader header, ReadOnlySpan<byte> span)
        {
            this._header = header;
            this.HeaderAndContentData = span.Slice(0, checked((int)this._header.TotalLength));
        }

        /// <inheritdoc/>
        public override string ToString() => this._header.ToString();

        #region Header Forwarding
        /// <summary>
        /// Game metadata to use as reference for alignment
        /// </summary>
        public GameConstants Meta => _header.Meta;

        /// <summary>
        /// Raw bytes of header
        /// </summary>
        public ReadOnlySpan<byte> HeaderData => _header.HeaderData;

        /// <summary>
        /// Game release associated with header
        /// </summary>
        public GameRelease Release => _header.Release;

        /// <summary>
        /// The length that the header itself takes
        /// </summary>
        public sbyte HeaderLength => _header.HeaderLength;

        /// <summary>
        /// RecordType of the header
        /// </summary>
        public RecordType RecordType => _header.RecordType;

        public int RecordTypeInt => _header.RecordTypeInt;

        /// <summary>
        /// The length explicitly contained in the length bytes of the header
        /// Note that for Sub Records, this is equivalent to ContentLength
        /// </summary>
        public ushort RecordLength => _header.RecordLength;

        /// <summary>
        /// The length of the content of the Sub Record, excluding the header bytes.
        /// </summary>
        public ushort ContentLength => (ushort)Content.Length;
        #endregion
    }

    /// <summary>
    /// A ref struct that overlays on top of bytes that is able to retrive Major Record data on demand.
    /// Unlike SubrecordFrame, this struct exposes its data members as MemorySlices instead of Spans
    /// </summary>
    public ref struct SubrecordMemoryFrame
    {
        private readonly SubrecordHeader _header;
        
        /// <summary>
        /// Raw bytes of both header and content data
        /// </summary>
        public ReadOnlyMemorySlice<byte> HeaderAndContentData { get; }

        /// <summary> 
        /// Total length of the Sub Record, including the header and its content. 
        /// </summary> 
        public int TotalLength => this.Content.Length + this._header.HeaderLength;

        /// <summary>
        /// Raw bytes of the content data, excluding the header
        /// </summary>
        public ReadOnlyMemorySlice<byte> Content => HeaderAndContentData.Slice(this._header.HeaderLength, checked((int)this._header.ContentLength));

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="meta">Game metadata to use as reference for alignment</param>
        /// <param name="span">Span to overlay on, aligned to the start of the header</param>
        public SubrecordMemoryFrame(GameConstants meta, ReadOnlyMemorySlice<byte> span)
        {
            this._header = meta.Subrecord(span);
            this.HeaderAndContentData = span.Slice(0, checked((int)this._header.TotalLength));
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="header">Existing SubrecordHeader struct</param>
        /// <param name="span">Span to overlay on, aligned to the start of the header</param>
        public SubrecordMemoryFrame(SubrecordHeader header, ReadOnlyMemorySlice<byte> span)
        {
            this._header = header;
            this.HeaderAndContentData = span.Slice(0, checked((int)this._header.TotalLength));
        }
        
        /// <inheritdoc/>
        public override string ToString() => this._header.ToString();

        #region Header Forwarding
        /// <summary>
        /// Game metadata to use as reference for alignment
        /// </summary>
        public GameConstants Meta => _header.Meta;

        /// <summary>
        /// Raw bytes of header
        /// </summary>
        public ReadOnlySpan<byte> HeaderData => _header.HeaderData;

        /// <summary>
        /// Game release associated with header
        /// </summary>
        public GameRelease Release => _header.Release;

        /// <summary>
        /// The length that the header itself takes
        /// </summary>
        public sbyte HeaderLength => _header.HeaderLength;

        /// <summary>
        /// RecordType of the header
        /// </summary>
        public RecordType RecordType => _header.RecordType;

        public int RecordTypeInt => _header.RecordTypeInt;

        /// <summary>
        /// The length explicitly contained in the length bytes of the header
        /// Note that for Sub Records, this is equivalent to ContentLength
        /// </summary>
        public ushort RecordLength => _header.RecordLength;

        /// <summary>
        /// The length of the content of the Sub Record, excluding the header bytes.
        /// </summary>
        public ushort ContentLength => (ushort)Content.Length;
        #endregion
    }
}
