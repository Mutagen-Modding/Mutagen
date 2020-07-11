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
        public ReadOnlySpan<byte> HeaderData { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="meta">Game metadata to use as reference for alignment</param>
        /// <param name="span">Span to overlay on, aligned to the start of the Group's header</param>
        public GroupHeader(GameConstants meta, ReadOnlySpan<byte> span)
        {
            this.Meta = meta;
            this.HeaderData = span.Slice(0, meta.GroupConstants.HeaderLength);
        }

        /// <summary>
        /// Game release associated with header
        /// </summary>
        public GameRelease Release => Meta.Release;
        
        /// <summary>
        /// The length that the header itself takes
        /// </summary>
        public sbyte HeaderLength => Meta.GroupConstants.HeaderLength;
        
        /// <summary>
        /// RecordType of the Group header.
        /// Should always be GRUP, unless struct is overlaid on bad data.
        /// </summary>
        public RecordType RecordType => new RecordType(BinaryPrimitives.ReadInt32LittleEndian(this.HeaderData.Slice(0, 4)));
        
        /// <summary>
        /// The length explicitly contained in the length bytes of the header
        /// Note that for Groups, this is equivalent to TotalLength
        /// </summary>
        public uint RecordLength => BinaryPrimitives.ReadUInt32LittleEndian(this.HeaderData.Slice(4, 4));
        
        /// <summary>
        /// The raw bytes of the RecordType of the records contained by the Group
        /// </summary>
        public ReadOnlySpan<byte> ContainedRecordTypeSpan => this.HeaderData.Slice(8, 4);
        
        /// <summary>
        /// The RecordType of the records contained by the Group
        /// </summary>
        public RecordType ContainedRecordType => new RecordType(BinaryPrimitives.ReadInt32LittleEndian(this.ContainedRecordTypeSpan));
        
        /// <summary>
        /// The integer representing a Group's Type enum.
        /// Since each game has its own Group Enum, this field is offered as an int that should
        /// be casted to the appropriate enum for use.
        /// </summary>
        public int GroupType => BinaryPrimitives.ReadInt32LittleEndian(this.HeaderData.Slice(12, 4));
        
        /// <summary>
        /// The raw bytes of the last modified data
        /// </summary>
        public ReadOnlySpan<byte> LastModifiedSpan => this.HeaderData.Slice(16, 4);
        
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

        /// <inheritdoc/>
        public override string ToString() => $"{RecordType} ({ContainedRecordType}) => 0x{ContentLength:X}";
    }

    /// <summary>
    /// A ref struct that overlays on top of bytes that is able to retrive Group data on demand.
    /// </summary>
    public ref struct GroupFrame
    {
        private readonly GroupHeader _header { get; }
        
        /// <summary>
        /// Raw bytes of both header and content data
        /// </summary>
        public ReadOnlySpan<byte> HeaderAndContentData { get; }
        
        /// <summary>
        /// Raw bytes of the content data, excluding the header
        /// </summary>
        public ReadOnlySpan<byte> Content => HeaderAndContentData.Slice(this._header.HeaderLength, checked((int)this._header.ContentLength));

        /// <summary> 
        /// Total length of the Group Record, including the header and its content. 
        /// </summary> 
        public long TotalLength => this.HeaderLength + this.Content.Length;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="meta">Game metadata to use as reference for alignment</param>
        /// <param name="span">Span to overlay on, aligned to the start of the header</param>
        public GroupFrame(GameConstants meta, ReadOnlySpan<byte> span)
        {
            this._header = meta.Group(span);
            this.HeaderAndContentData = span.Slice(0, checked((int)this._header.TotalLength));
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="header">Existing GroupHeader struct</param>
        /// <param name="span">Span to overlay on, aligned to the start of the Group's header</param>
        public GroupFrame(GroupHeader header, ReadOnlySpan<byte> span)
        {
            this._header = header;
            this.HeaderAndContentData = span.Slice(0, checked((int)this._header.TotalLength));
        }
        
        /// <inheritdoc/>
        public override string ToString() => this._header.ToString();

        #region Header Forwarding
        public GameConstants Meta => _header.Meta;

        /// <summary>
        /// Raw bytes of header data
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
        /// RecordType of the Group header.
        /// Should always be GRUP, unless struct is overlaid on bad data.
        /// </summary>
        public RecordType RecordType => _header.RecordType;

        /// <summary>
        /// The length explicitly contained in the length bytes of the header
        /// Note that for Groups, this is equivalent to TotalLength
        /// </summary>
        public uint RecordLength => _header.RecordLength;

        /// <summary>
        /// The raw bytes of the RecordType of the records contained by the Group
        /// </summary>
        public ReadOnlySpan<byte> ContainedRecordTypeSpan => _header.ContainedRecordTypeSpan;

        /// <summary>
        /// The RecordType of the records contained by the Group
        /// </summary>
        public RecordType ContainedRecordType => _header.ContainedRecordType;

        /// <summary>
        /// The integer representing a Group's Type enum.
        /// Since each game has its own Group Enum, this field is offered as an int that should
        /// be casted to the appropriate enum for use.
        /// </summary>
        public int GroupType => _header.GroupType;

        /// <summary>
        /// The raw bytes of the last modified data
        /// </summary>
        public ReadOnlySpan<byte> LastModifiedSpan => _header.LastModifiedSpan;

        /// <summary>
        /// True if RecordType == "GRUP".
        /// Should always be true, unless struct is overlaid on bad data.
        /// </summary>
        public bool IsGroup => _header.IsGroup;

        /// <summary>
        /// The length of the content of the Group, excluding the header bytes.
        /// </summary>
        public uint ContentLength => (uint)Content.Length;

        /// <summary>
        /// The length of the RecordType and the length bytes
        /// </summary>
        public int TypeAndLengthLength => _header.TypeAndLengthLength;

        /// <summary>
        /// True if GroupType is marked as top level. (GroupType == 0)
        /// </summary>
        public bool IsTopLevel => _header.IsTopLevel;
        #endregion
    }

    /// <summary>
    /// A ref struct that overlays on top of bytes that is able to retrive Group data on demand.
    /// Unlike GroupFrame, this struct exposes its data members as MemorySlices instead of Spans
    /// </summary>
    public ref struct GroupMemoryFrame
    {
        private readonly GroupHeader _header;
        
        /// <summary>
        /// Raw bytes of both header and content data
        /// </summary>
        public ReadOnlyMemorySlice<byte> HeaderAndContentData { get; }

        /// <summary> 
        /// Total length of the Group Record, including the header and its content. 
        /// </summary> 
        public long TotalLength => this.HeaderLength + this.Content.Length;

        /// <summary>
        /// Raw bytes of the content data, excluding the header
        /// </summary>
        public ReadOnlyMemorySlice<byte> Content => HeaderAndContentData.Slice(this._header.HeaderLength, checked((int)this._header.ContentLength));

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="meta">Game metadata to use as reference for alignment</param>
        /// <param name="span">Span to overlay on, aligned to the start of the header</param>
        public GroupMemoryFrame(GameConstants meta, ReadOnlyMemorySlice<byte> span)
        {
            this._header = meta.Group(span);
            this.HeaderAndContentData = span.Slice(0, checked((int)this._header.TotalLength));
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="header">Existing GroupHeader struct</param>
        /// <param name="span">Span to overlay on, aligned to the start of the Group's header</param>
        public GroupMemoryFrame(GroupHeader header, ReadOnlyMemorySlice<byte> span)
        {
            this._header = header;
            this.HeaderAndContentData = span.Slice(0, checked((int)this._header.TotalLength));
        }
        
        /// <inheritdoc/>
        public override string ToString() => this._header.ToString();

        #region Header Forwarding
        public GameConstants Meta => _header.Meta;

        /// <summary>
        /// Raw bytes of header data
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
        /// RecordType of the Group header.
        /// Should always be GRUP, unless struct is overlaid on bad data.
        /// </summary>
        public RecordType RecordType => _header.RecordType;

        /// <summary>
        /// The length explicitly contained in the length bytes of the header
        /// Note that for Groups, this is equivalent to TotalLength
        /// </summary>
        public uint RecordLength => _header.RecordLength;

        /// <summary>
        /// The raw bytes of the RecordType of the records contained by the Group
        /// </summary>
        public ReadOnlySpan<byte> ContainedRecordTypeSpan => _header.ContainedRecordTypeSpan;

        /// <summary>
        /// The RecordType of the records contained by the Group
        /// </summary>
        public RecordType ContainedRecordType => _header.ContainedRecordType;

        /// <summary>
        /// The integer representing a Group's Type enum.
        /// Since each game has its own Group Enum, this field is offered as an int that should
        /// be casted to the appropriate enum for use.
        /// </summary>
        public int GroupType => _header.GroupType;

        /// <summary>
        /// The raw bytes of the last modified data
        /// </summary>
        public ReadOnlySpan<byte> LastModifiedSpan => _header.LastModifiedSpan;

        /// <summary>
        /// True if RecordType == "GRUP".
        /// Should always be true, unless struct is overlaid on bad data.
        /// </summary>
        public bool IsGroup => _header.IsGroup;

        /// <summary>
        /// The length of the content of the Group, excluding the header bytes.
        /// </summary>
        public uint ContentLength => (uint)Content.Length;

        /// <summary>
        /// The length of the RecordType and the length bytes
        /// </summary>
        public int TypeAndLengthLength => _header.TypeAndLengthLength;

        /// <summary>
        /// True if GroupType is marked as top level. (GroupType == 0)
        /// </summary>
        public bool IsTopLevel => _header.IsTopLevel;
        #endregion
    }
}
