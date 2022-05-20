using Mutagen.Bethesda.Plugins.Internals;
using Mutagen.Bethesda.Plugins.Meta;
using Noggog;
using System.Buffers.Binary;
using System.Collections;

namespace Mutagen.Bethesda.Plugins.Binary.Headers;

/// <summary>
/// A struct that overlays on top of bytes that is able to retrieve Group header data on demand.
/// </summary>
public readonly struct GroupHeader
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

    /// <summary>
    /// Whether this group type is allowed to contain subgroups
    /// </summary>
    public bool CanHaveSubGroups => Meta.GroupConstants.CanHaveSubGroups(GroupType);

    /// <inheritdoc/>
    public override string ToString() => $"{RecordType} ({ContainedRecordType}) [0x{ContentLength:X}]";
}

/// <summary>
/// A struct that overlays on top of bytes that is able to retrieve Group header data on demand.
/// In addition, it keeps track of its location relative to its parent stream
/// </summary>
public readonly struct GroupPinHeader
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
    /// Location of the group relative to its parent stream
    /// </summary>
    public long Location { get; }
    
    /// <summary>
    /// Location where the group header ends.  This is equivalent to Location + HeaderLength
    /// </summary>
    public long EndLocation => Location + HeaderLength;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="meta">Game metadata to use as reference for alignment</param>
    /// <param name="span">Span to overlay on, aligned to the start of the Group's header</param>
    /// <param name="pinLocation">Location pin</param>
    public GroupPinHeader(GameConstants meta, ReadOnlyMemorySlice<byte> span, long pinLocation)
    {
        Meta = meta;
        HeaderData = span.Slice(0, meta.GroupConstants.HeaderLength);
        Location = pinLocation;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="header">Existing GroupHeader struct</param>
    /// <param name="pinLocation">Location pin</param>
    public GroupPinHeader(GroupHeader header, long pinLocation)
    {
        Meta = header.Meta;
        HeaderData = header.HeaderData;
        Location = pinLocation;
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

    /// <summary>
    /// Whether this group type is allowed to contain subgroups
    /// </summary>
    public bool CanHaveSubGroups => Meta.GroupConstants.CanHaveSubGroups(GroupType);

    /// <inheritdoc/>
    public override string ToString() => $"{RecordType} ({ContainedRecordType}) [0x{ContentLength:X}] @ 0x{Location:X}";

    public static implicit operator GroupHeader(GroupPinHeader frame)
    {
        return new GroupHeader(frame.Meta, span: frame.HeaderData);
    }
}

/// <summary>
/// A struct that overlays on top of bytes that is able to retrieve Group data on demand.
/// </summary>
public readonly struct GroupFrame : IEnumerable<MajorRecordPinFrame>
{
    /// <summary>
    /// Header struct contained in the frame
    /// </summary>
    public GroupHeader Header { get; }
        
    /// <summary>
    /// Raw bytes of both header and content data
    /// </summary>
    public ReadOnlyMemorySlice<byte> HeaderAndContentData { get; }
        
    /// <summary>
    /// Raw bytes of the content data, excluding the header
    /// </summary>
    public ReadOnlyMemorySlice<byte> Content => HeaderAndContentData.Slice(Header.HeaderLength, checked((int)Header.ContentLength));

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
        Header = meta.GroupHeader(span);
        HeaderAndContentData = span.Slice(0, checked((int)Header.TotalLength));
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="header">Existing GroupHeader struct</param>
    /// <param name="span">Span to overlay on, aligned to the start of the Group's header</param>
    public GroupFrame(GroupHeader header, ReadOnlyMemorySlice<byte> span)
    {
        Header = header;
        HeaderAndContentData = span.Slice(0, checked((int)Header.TotalLength));
    }
        
    /// <inheritdoc/>
    public override string ToString() => Header.ToString();

    /// <inheritdoc/>
    public IEnumerator<MajorRecordPinFrame> GetEnumerator() => HeaderExt.EnumerateRecords(this).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    #region Header Forwarding
    public GameConstants Meta => Header.Meta;

    /// <summary>
    /// Raw bytes of header data
    /// </summary>
    public ReadOnlyMemorySlice<byte> HeaderData => Header.HeaderData;

    /// <summary>
    /// Game release associated with header
    /// </summary>
    public GameRelease Release => Header.Release;

    /// <summary>
    /// The length that the header itself takes
    /// </summary>
    public byte HeaderLength => Header.HeaderLength;

    /// <summary>
    /// RecordType of the Group header.
    /// Should always be GRUP, unless struct is overlaid on bad data.
    /// </summary>
    public RecordType RecordType => Header.RecordType;

    /// <summary>
    /// The raw bytes of the RecordType of the records contained by the Group
    /// </summary>
    public ReadOnlyMemorySlice<byte> ContainedRecordTypeData => Header.ContainedRecordTypeData;

    /// <summary>
    /// The RecordType of the records contained by the Group
    /// </summary>
    public RecordType ContainedRecordType => Header.ContainedRecordType;

    /// <summary>
    /// The integer representing a Group's Type enum.
    /// Since each game has its own Group Enum, this field is offered as an int that should
    /// be casted to the appropriate enum for use.
    /// </summary>
    public int GroupType => Header.GroupType;

    /// <summary>
    /// The raw bytes of the last modified data
    /// </summary>
    public ReadOnlyMemorySlice<byte> LastModifiedData => Header.LastModifiedData;

    /// <summary>
    /// True if RecordType == "GRUP".
    /// Should always be true, unless struct is overlaid on bad data.
    /// </summary>
    public bool IsGroup => Header.IsGroup;

    /// <summary>
    /// The length of the content of the Group, excluding the header bytes.
    /// </summary>
    public uint ContentLength => (uint)Content.Length;

    /// <summary>
    /// The length of the RecordType and the length bytes
    /// </summary>
    public int TypeAndLengthLength => Header.TypeAndLengthLength;

    /// <summary>
    /// True if GroupType is marked as top level. (GroupType == 0)
    /// </summary>
    public bool IsTopLevel => Header.IsTopLevel;

    /// <summary>
    /// Whether this group type is allowed to contain subgroups
    /// </summary>
    public bool CanHaveSubGroups => Meta.GroupConstants.CanHaveSubGroups(GroupType);
    #endregion
}

/// <summary>
/// A struct that overlays on top of bytes that is able to retrieve Group data on demand.
/// In addition, it keeps track of its location relative to its parent stream
/// </summary>
public readonly struct GroupPinFrame : IEnumerable<MajorRecordPinFrame>
{
    /// <summary>
    /// Frame struct contained in the pin
    /// </summary>
    public GroupFrame Frame { get; }

    /// <summary>
    /// Header struct contained in the pin
    /// </summary>
    public GroupHeader Header => Frame.Header;

    /// <summary>
    /// Location of the group relative to its parent stream
    /// </summary>
    public long Location { get; }
    
    /// <summary>
    /// Location where the group ends.  This is equivalent to Location + TotalLength
    /// </summary>
    public long EndLocation => Location + TotalLength;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="meta">Game metadata to use as reference for alignment</param>
    /// <param name="span">Span to overlay on, aligned to the start of the header</param>
    /// <param name="pinLocation">Location pin</param>
    public GroupPinFrame(GameConstants meta, ReadOnlyMemorySlice<byte> span, long pinLocation)
    {
        Frame = new GroupFrame(meta, span);
        Location = pinLocation;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="frame">Existing GroupFrame struct</param>
    /// <param name="pinLocation">Location pin</param>
    public GroupPinFrame(GroupFrame frame, long pinLocation)
    {
        Frame = frame;
        Location = pinLocation;
    }
        
    /// <inheritdoc/>
    public override string ToString() => $"{Frame.ToString()} => 0x{ContentLength:X} @ 0x{Location:X}";

    /// <inheritdoc/>
    public IEnumerator<MajorRecordPinFrame> GetEnumerator() => HeaderExt.EnumerateRecords(Frame).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    #region Header Forwarding

    /// <summary>
    /// Raw bytes of both header and content data
    /// </summary>
    public ReadOnlyMemorySlice<byte> HeaderAndContentData => Frame.HeaderAndContentData;

    /// <summary>
    /// Raw bytes of the content data, excluding the header
    /// </summary>
    public ReadOnlyMemorySlice<byte> Content => Frame.Content;

    /// <summary> 
    /// Total length of the Group Record, including the header and its content. 
    /// </summary> 
    public long TotalLength => Frame.TotalLength;
    
    public GameConstants Meta => Frame.Header.Meta;

    /// <summary>
    /// Raw bytes of header data
    /// </summary>
    public ReadOnlyMemorySlice<byte> HeaderData => Frame.Header.HeaderData;

    /// <summary>
    /// Game release associated with header
    /// </summary>
    public GameRelease Release => Frame.Header.Release;

    /// <summary>
    /// The length that the header itself takes
    /// </summary>
    public byte HeaderLength => Frame.Header.HeaderLength;

    /// <summary>
    /// RecordType of the Group header.
    /// Should always be GRUP, unless struct is overlaid on bad data.
    /// </summary>
    public RecordType RecordType => Frame.Header.RecordType;

    /// <summary>
    /// The raw bytes of the RecordType of the records contained by the Group
    /// </summary>
    public ReadOnlyMemorySlice<byte> ContainedRecordTypeData => Frame.Header.ContainedRecordTypeData;

    /// <summary>
    /// The RecordType of the records contained by the Group
    /// </summary>
    public RecordType ContainedRecordType => Frame.Header.ContainedRecordType;

    /// <summary>
    /// The integer representing a Group's Type enum.
    /// Since each game has its own Group Enum, this field is offered as an int that should
    /// be casted to the appropriate enum for use.
    /// </summary>
    public int GroupType => Frame.Header.GroupType;

    /// <summary>
    /// The raw bytes of the last modified data
    /// </summary>
    public ReadOnlyMemorySlice<byte> LastModifiedData => Frame.Header.LastModifiedData;

    /// <summary>
    /// True if RecordType == "GRUP".
    /// Should always be true, unless struct is overlaid on bad data.
    /// </summary>
    public bool IsGroup => Frame.Header.IsGroup;

    /// <summary>
    /// The length of the content of the Group, excluding the header bytes.
    /// </summary>
    public uint ContentLength => (uint)Content.Length;

    /// <summary>
    /// The length of the RecordType and the length bytes
    /// </summary>
    public int TypeAndLengthLength => Frame.Header.TypeAndLengthLength;

    /// <summary>
    /// True if GroupType is marked as top level. (GroupType == 0)
    /// </summary>
    public bool IsTopLevel => Frame.Header.IsTopLevel;

    /// <summary>
    /// Whether this group type is allowed to contain subgroups
    /// </summary>
    public bool CanHaveSubGroups => Meta.GroupConstants.CanHaveSubGroups(GroupType);
    #endregion

    public static implicit operator GroupHeader(GroupPinFrame pin)
    {
        return pin.Header;
    }

    public static implicit operator GroupFrame(GroupPinFrame pin)
    {
        return pin.Frame;
    }

    public static implicit operator GroupPinHeader(GroupPinFrame pin)
    {
        return new GroupPinHeader(pin.Header, pin.Location);
    }
}
