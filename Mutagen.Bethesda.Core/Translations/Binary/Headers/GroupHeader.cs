using Mutagen.Bethesda.Internals;
using Noggog;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Binary
{
    /// <summary>
    /// A ref struct that overlays on top of bytes that is able to retrive Group header data on demand.
    /// </summary>
    public ref struct GroupHeader
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
        public GroupHeader(GameConstants meta, ReadOnlySpan<byte> span)
        {
            this.Meta = meta;
            this.Span = span.Slice(0, meta.GroupConstants.HeaderLength);
        }

        /// <summary>
        /// GameMode associated with header
        /// </summary>
        public GameMode GameMode => Meta.GameMode;
        
        /// <summary>
        /// The length that the header itself takes
        /// </summary>
        public sbyte HeaderLength => Meta.GroupConstants.HeaderLength;
        
        /// <summary>
        /// RecordType of the Group header.
        /// Should always be GRUP, unless struct is overlaid on bad data.
        /// </summary>
        public RecordType RecordType => new RecordType(BinaryPrimitives.ReadInt32LittleEndian(this.Span.Slice(0, 4)));
        
        /// <summary>
        /// The length explicitly contained in the length bytes of the header
        /// Note that for Groups, this is equivalent to TotalLength
        /// </summary>
        public uint RecordLength => BinaryPrimitives.ReadUInt32LittleEndian(this.Span.Slice(4, 4));
        
        /// <summary>
        /// The raw bytes of the RecordType of the records contained by the Group
        /// </summary>
        public ReadOnlySpan<byte> ContainedRecordTypeSpan => this.Span.Slice(8, 4);
        
        /// <summary>
        /// The RecordType of the records contained by the Group
        /// </summary>
        public RecordType ContainedRecordType => new RecordType(BinaryPrimitives.ReadInt32LittleEndian(this.ContainedRecordTypeSpan));
        
        /// <summary>
        /// The integer representing a Group's Type enum.
        /// Since each game has its own Group Enum, this field is offered as an int that should
        /// be casted to the appropriate enum for use.
        /// </summary>
        public int GroupType => BinaryPrimitives.ReadInt32LittleEndian(this.Span.Slice(12, 4));
        
        /// <summary>
        /// The raw bytes of the last modified data
        /// </summary>
        public ReadOnlySpan<byte> LastModifiedSpan => this.Span.Slice(16, 4);
        
        /// <summary>
        /// Total length of the Group, including the header and its content.
        /// </summary>
        public long TotalLength => this.RecordLength;
        
        /// <summary>
        /// True if RecordType == "GRUP".
        /// Should always be true, unless struct is overlaid on bad data.
        /// </summary>
        public bool IsGroup => this.RecordType == Constants.Group;
        
        /// <summary>
        /// The length of the content of the Group, excluding the header bytes.
        /// </summary>
        public uint ContentLength => checked((uint)(this.TotalLength - this.HeaderLength));
        
        /// <summary>
        /// The length of the RecordType and the length bytes
        /// </summary>
        public int TypeAndLengthLength => Meta.GroupConstants.TypeAndLengthLength;
        
        /// <summary>
        /// True if GroupType is marked as top level. (GroupType == 0)
        /// </summary>
        public bool IsTopLevel => this.GroupType == 0;
    }

    /// <summary>
    /// A ref struct that overlays on top of bytes that is able to retrive Group data on demand.
    /// </summary>
    public ref struct GroupFrame
    {
        /// <summary>
        /// Header ref struct for accessing header data
        /// </summary>
        public GroupHeader Header { get; }
        
        /// <summary>
        /// Raw bytes of both a Group's header and content data
        /// </summary>
        public ReadOnlySpan<byte> HeaderAndContentData { get; }
        
        /// <summary>
        /// Raw bytes of a Group's content data, excluding the header
        /// </summary>
        public ReadOnlySpan<byte> Content => HeaderAndContentData.Slice(this.Header.HeaderLength, checked((int)this.Header.RecordLength));

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="meta">Game metadata to use as reference for alignment</param>
        /// <param name="span">Span to overlay on, aligned to the start of the Group's header</param>
        public GroupFrame(GameConstants meta, ReadOnlySpan<byte> span)
        {
            this.Header = meta.Group(span);
            this.HeaderAndContentData = span.Slice(0, checked((int)this.Header.TotalLength));
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="header">Existing GroupHeader struct</param>
        /// <param name="span">Span to overlay on, aligned to the start of the Group's header</param>
        public GroupFrame(GroupHeader header, ReadOnlySpan<byte> span)
        {
            this.Header = header;
            this.HeaderAndContentData = span.Slice(0, checked((int)this.Header.TotalLength));
        }
    }

    /// <summary>
    /// A ref struct that overlays on top of bytes that is able to retrive Group data on demand.
    /// Unlike GroupFrame, this struct exposes its data members as MemorySlices instead of Spans
    /// </summary>
    public ref struct GroupMemoryFrame
    {
        /// <summary>
        /// Header ref struct for accessing header data
        /// </summary>
        public GroupHeader Header { get; }
        
        /// <summary>
        /// Raw bytes of both a Group's header and content data
        /// </summary>
        public ReadOnlyMemorySlice<byte> HeaderAndContentData { get; }
        
        /// <summary>
        /// Raw bytes of a Group's content data, excluding the header
        /// </summary>
        public ReadOnlySpan<byte> Content => HeaderAndContentData.Slice(this.Header.HeaderLength, checked((int)this.Header.RecordLength));

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="meta">Game metadata to use as reference for alignment</param>
        /// <param name="span">Span to overlay on, aligned to the start of the Group's header</param>
        public GroupMemoryFrame(GameConstants meta, ReadOnlyMemorySlice<byte> span)
        {
            this.Header = meta.Group(span);
            this.HeaderAndContentData = span.Slice(0, checked((int)this.Header.TotalLength));
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="header">Existing GroupHeader struct</param>
        /// <param name="span">Span to overlay on, aligned to the start of the Group's header</param>
        public GroupMemoryFrame(GroupHeader meta, ReadOnlyMemorySlice<byte> span)
        {
            this.Header = meta;
            this.HeaderAndContentData = span.Slice(0, checked((int)this.Header.TotalLength));
        }
    }
}
