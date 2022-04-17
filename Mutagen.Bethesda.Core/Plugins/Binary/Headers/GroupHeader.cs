using Mutagen.Bethesda.Plugins.Internals;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
using System.Buffers.Binary;
using System.Collections;
using System.Collections.Generic;

namespace Mutagen.Bethesda.Plugins.Binary.Headers;

/// <summary>
/// A struct that overlays on top of bytes that is able to retrive Group header data on demand.
/// </summary>
public struct GroupHeader
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
    /// <param name="span">Span to overlay on, aligned to the start of the Group's header</param>
    public GroupHeader(GameConstants meta, ReadOnlyMemorySlice<byte> span)
    {
        Meta = meta;
        HeaderData = span.Slice(0, meta.GroupConstants.HeaderLength);
    }

    /// <summary>
    /// Game release associated with header
    /// </summary>
    public GameRelease Release => Meta.Release;
        
    /// <summary>
    /// The length that the header itself takes
    /// </summary>
    public byte HeaderLength => Meta.GroupConstants.HeaderLength;
        
    /// <summary>
    /// RecordType of the Group header.
    /// Should always be GRUP, unless struct is overlaid on bad data.
    /// </summary>
    public RecordType RecordType => new RecordType(BinaryPrimitives.ReadInt32LittleEndian(HeaderData.Slice(0, 4)));
        
    private uint RecordLength => BinaryPrimitives.ReadUInt32LittleEndian(HeaderData.Slice(4, 4));
        
    /// <summary>
    /// The raw bytes of the RecordType of the records contained by the Group
    /// </summary>
    public ReadOnlyMemorySlice<byte> ContainedRecordTypeData => HeaderData.Slice(8, 4);
        
    /// <summary>
    /// The RecordType of the records contained by the Group
    /// </summary>
    public RecordType ContainedRecordType => new RecordType(BinaryPrimitives.ReadInt32LittleEndian(ContainedRecordTypeData));
        
    /// <summary>
    /// The integer representing a Group's Type enum.
    /// Since each game has its own Group Enum, this field is offered as an int that should
    /// be casted to the appropriate enum for use.
    /// </summary>
    public int GroupType => BinaryPrimitives.ReadInt32LittleEndian(HeaderData.Slice(12, 4));
        
    /// <summary>
    /// The raw bytes of the last modified data
    /// </summary>
    public ReadOnlyMemorySlice<byte> LastModifiedData => HeaderData.Slice(16, 4);
        
    /// <summary>
    /// Total length of the Group, including the header and its content.
    /// </summary>
    public long TotalLength => RecordLength;
        
    /// <summary>
    /// True if RecordType == "GRUP".
    /// Should always be true, unless struct is overlaid on bad data.
    /// </summary>
    public bool IsGroup => RecordType == Constants.Group;
        
    /// <summary>
    /// The length of the content of the Group, excluding the header bytes.
    /// </summary>
    public uint ContentLength => checked((uint)(TotalLength - HeaderLength));
        
    /// <summary>
    /// The length of the RecordType and the length bytes
    /// </summary>
    public int TypeAndLengthLength => Meta.GroupConstants.TypeAndLengthLength;
        
    /// <summary>
    /// True if GroupType is marked as top level. (GroupType == 0)
    /// </summary>
    public bool IsTopLevel => GroupType == 0;

    /// <inheritdoc/>
    public override string ToString() => $"{RecordType} ({ContainedRecordType}) => 0x{ContentLength:X}";
}

/// <summary>
/// A struct that overlays on top of bytes that is able to retrive Group data on demand.
/// </summary>
public struct GroupFrame : IEnumerable<MajorRecordPinFrame>
{
    private readonly GroupHeader _header { get; }
        
    /// <summary>
    /// Raw bytes of both header and content data
    /// </summary>
    public ReadOnlyMemorySlice<byte> HeaderAndContentData { get; }
        
    /// <summary>
    /// Raw bytes of the content data, excluding the header
    /// </summary>
    public ReadOnlyMemorySlice<byte> Content => HeaderAndContentData.Slice(_header.HeaderLength, checked((int)_header.ContentLength));

    /// <summary> 
    /// Total length of the Group Record, including the header and its content. 
    /// </summary> 
    public long TotalLength => HeaderLength + Content.Length;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="meta">Game metadata to use as reference for alignment</param>
    /// <param name="span">Span to overlay on, aligned to the start of the header</param>
    public GroupFrame(GameConstants meta, ReadOnlyMemorySlice<byte> span)
    {
        _header = meta.Group(span);
        HeaderAndContentData = span.Slice(0, checked((int)_header.TotalLength));
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="header">Existing GroupHeader struct</param>
    /// <param name="span">Span to overlay on, aligned to the start of the Group's header</param>
    public GroupFrame(GroupHeader header, ReadOnlyMemorySlice<byte> span)
    {
        _header = header;
        HeaderAndContentData = span.Slice(0, checked((int)_header.TotalLength));
    }
        
    /// <inheritdoc/>
    public override string ToString() => _header.ToString();

    /// <inheritdoc/>
    public IEnumerator<MajorRecordPinFrame> GetEnumerator() => HeaderExt.EnumerateRecords(this).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    #region Header Forwarding
    public GameConstants Meta => _header.Meta;

    /// <summary>
    /// Raw bytes of header data
    /// </summary>
    public ReadOnlyMemorySlice<byte> HeaderData => _header.HeaderData;

    /// <summary>
    /// Game release associated with header
    /// </summary>
    public GameRelease Release => _header.Release;

    /// <summary>
    /// The length that the header itself takes
    /// </summary>
    public byte HeaderLength => _header.HeaderLength;

    /// <summary>
    /// RecordType of the Group header.
    /// Should always be GRUP, unless struct is overlaid on bad data.
    /// </summary>
    public RecordType RecordType => _header.RecordType;

    /// <summary>
    /// The raw bytes of the RecordType of the records contained by the Group
    /// </summary>
    public ReadOnlyMemorySlice<byte> ContainedRecordTypeData => _header.ContainedRecordTypeData;

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
    public ReadOnlyMemorySlice<byte> LastModifiedData => _header.LastModifiedData;

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