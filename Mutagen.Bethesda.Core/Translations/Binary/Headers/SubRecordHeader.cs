using Noggog;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Binary
{
    /// <summary>
    /// A struct that overlays on top of bytes that is able to retrive Sub Record header data on demand.
    /// </summary>
    public struct SubrecordHeader
    {
        /// <summary>
        /// Game metadata to use as reference for alignment
        /// </summary>
        public GameConstants Meta { get; }
        
        /// <summary>
        /// Bytes overlaid onto
        /// </summary>
        public ReadOnlyMemorySlice<byte> HeaderData { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="meta">Game metadata to use as reference for alignment</param>
        /// <param name="span">Span to overlay on, aligned to the start of the Sub Record's header</param>
        public SubrecordHeader(GameConstants meta, ReadOnlyMemorySlice<byte> span)
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
        
        /// <summary>
        /// RecordType of the header, represented as an int
        /// </summary>
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
        public override string ToString() => $"{RecordType} => 0x{ContentLength:X}";
    }

    /// <summary>
    /// A struct that overlays on top of bytes that is able to retrive Sub Record data on demand.
    /// </summary>
    public struct SubrecordFrame
    {
        /// <summary>
        /// Header struct contained in the frame
        /// </summary>
        public SubrecordHeader Header { get; }

        /// <summary>
        /// Raw bytes of both header and content data
        /// </summary>
        public ReadOnlyMemorySlice<byte> HeaderAndContentData { get; }

        /// <summary>
        /// Total length of the Sub Record, including the header and its content.
        /// </summary>
        public int TotalLength => HeaderAndContentData.Length;

        /// <summary>
        /// Raw bytes of the content data, excluding the header
        /// </summary>
        public ReadOnlyMemorySlice<byte> Content => HeaderAndContentData.Slice(this.Header.HeaderLength, checked((int)this.Header.ContentLength));

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="meta">Game metadata to use as reference for alignment</param>
        /// <param name="span">Span to overlay on, aligned to the start of the header</param>
        public SubrecordFrame(GameConstants meta, ReadOnlyMemorySlice<byte> span)
        {
            this.Header = meta.Subrecord(span);
            this.HeaderAndContentData = span.Slice(0, checked((int)this.Header.TotalLength));
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="header">Existing SubrecordHeader struct</param>
        /// <param name="span">Span to overlay on, aligned to the start of the header</param>
        public SubrecordFrame(SubrecordHeader header, ReadOnlyMemorySlice<byte> span)
        {
            this.Header = header;
            this.HeaderAndContentData = span.Slice(0, checked((int)this.Header.TotalLength));
        }

        /// <inheritdoc/>
        public override string ToString() => this.Header.ToString();

        #region Header Forwarding
        /// <summary>
        /// Game metadata to use as reference for alignment
        /// </summary>
        public GameConstants Meta => Header.Meta;

        /// <summary>
        /// Raw bytes of header
        /// </summary>
        public ReadOnlyMemorySlice<byte> HeaderData => Header.HeaderData;

        /// <summary>
        /// Game release associated with header
        /// </summary>
        public GameRelease Release => Header.Release;

        /// <summary>
        /// The length that the header itself takes
        /// </summary>
        public sbyte HeaderLength => Header.HeaderLength;

        /// <summary>
        /// RecordType of the header
        /// </summary>
        public RecordType RecordType => Header.RecordType;

        /// <summary>
        /// RecordType of the header, represented as an int
        /// </summary>
        public int RecordTypeInt => Header.RecordTypeInt;

        /// <summary>
        /// The length explicitly contained in the length bytes of the header
        /// Note that for Sub Records, this is equivalent to ContentLength
        /// </summary>
        public ushort RecordLength => Header.RecordLength;

        /// <summary>
        /// The length of the content of the Sub Record, excluding the header bytes.
        /// </summary>
        public ushort ContentLength => (ushort)Content.Length;
        #endregion

        public static implicit operator SubrecordHeader(SubrecordFrame frame)
        {
            return frame.Header;
        }
    }

    /// <summary>
    /// A struct that overlays on top of bytes that is able to retrive Sub Record data on demand.
    /// In addition, it keeps track of its location relative to its parent MajorRecordFrame
    /// </summary>
    public struct SubrecordPinFrame
    {
        /// <summary>
        /// Frame struct contained in the pin
        /// </summary>
        public SubrecordFrame Frame { get; }

        /// <summary>
        /// Location of the subrecord relative to the parent MajorRecordFrame's data.<br/>
        /// E.g., relative to the position of the RecordType of the parent MajorRecord.
        /// </summary>
        public int Location { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="meta">Game metadata to use as reference for alignment</param>
        /// <param name="span">Span to overlay on, aligned to the start of the header</param>
        /// <param name="pinLocation">Location pin tracker relative to parent MajorRecordFrame</param>
        public SubrecordPinFrame(GameConstants meta, ReadOnlyMemorySlice<byte> span, int pinLocation)
        {
            this.Frame = new SubrecordFrame(meta, span);
            this.Location = pinLocation;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="header">Existing SubrecordHeader struct</param>
        /// <param name="span">Span to overlay on, aligned to the start of the header</param>
        /// <param name="pinLocation">Location pin tracker relative to parent MajorRecordFrame</param>
        public SubrecordPinFrame(SubrecordHeader header, ReadOnlyMemorySlice<byte> span, int pinLocation)
        {
            this.Frame = new SubrecordFrame(header, span);
            this.Location = pinLocation;
        }

        /// <inheritdoc/>
        public override string ToString() => $"{this.Frame.ToString()} @ {Location}";

        #region Forwarding
        /// <summary>
        /// Header struct contained in the pin
        /// </summary>
        public SubrecordHeader Header => Frame.Header;

        /// <summary>
        /// Raw bytes of both header and content data
        /// </summary>
        public ReadOnlyMemorySlice<byte> HeaderAndContentData => Frame.HeaderAndContentData;

        /// <summary>
        /// Total length of the Sub Record, including the header and its content.
        /// </summary>
        public int TotalLength => Frame.TotalLength;

        /// <summary>
        /// Raw bytes of the content data, excluding the header
        /// </summary>
        public ReadOnlyMemorySlice<byte> Content => Frame.Content;

        /// <summary>
        /// Game metadata to use as reference for alignment
        /// </summary>
        public GameConstants Meta => Frame.Meta;

        /// <summary>
        /// Raw bytes of header
        /// </summary>
        public ReadOnlyMemorySlice<byte> HeaderData => Frame.HeaderData;

        /// <summary>
        /// Game release associated with header
        /// </summary>
        public GameRelease Release => Frame.Release;

        /// <summary>
        /// The length that the header itself takes
        /// </summary>
        public sbyte HeaderLength => Frame.HeaderLength;

        /// <summary>
        /// RecordType of the header
        /// </summary>
        public RecordType RecordType => Frame.RecordType;

        /// <summary>
        /// RecordType of the header, represented as an int
        /// </summary>
        public int RecordTypeInt => Frame.RecordTypeInt;

        /// <summary>
        /// The length explicitly contained in the length bytes of the header
        /// Note that for Sub Records, this is equivalent to ContentLength
        /// </summary>
        public ushort RecordLength => Frame.RecordLength;

        /// <summary>
        /// The length of the content of the Sub Record, excluding the header bytes.
        /// </summary>
        public ushort ContentLength => Frame.ContentLength;
        #endregion

        public static implicit operator SubrecordHeader(SubrecordPinFrame pin)
        {
            return pin.Header;
        }

        public static implicit operator SubrecordFrame(SubrecordPinFrame pin)
        {
            return pin.Frame;
        }
    }
}
