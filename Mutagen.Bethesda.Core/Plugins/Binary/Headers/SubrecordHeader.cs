using Mutagen.Bethesda.Plugins.Meta;
using Noggog;
using System.Buffers.Binary;

namespace Mutagen.Bethesda.Plugins.Binary.Headers;

/// <summary>
/// A struct that overlays on top of bytes that is able to retrieve Sub Record header data on demand.
/// </summary>
public readonly struct SubrecordHeader
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
        Meta = meta;
        HeaderData = span.Slice(0, meta.SubConstants.HeaderLength);
    }

    /// <summary>
    /// Game release associated with header
    /// </summary>
    public GameRelease Release => Meta.Release;
        
    /// <summary>
    /// The length that the header itself takes
    /// </summary>
    public byte HeaderLength => Meta.SubConstants.HeaderLength;
        
    /// <summary>
    /// RecordType of the header
    /// </summary>
    public RecordType RecordType => new RecordType(RecordTypeInt);
        
    /// <summary>
    /// RecordType of the header, represented as an int
    /// </summary>
    public int RecordTypeInt => BinaryPrimitives.ReadInt32LittleEndian(HeaderData.Slice(0, 4));
        
    /// <summary>
    /// The length of the content of the Sub Record, excluding the header bytes.
    /// </summary>
    public ushort ContentLength => BinaryPrimitives.ReadUInt16LittleEndian(HeaderData.Slice(4, 2));

    /// <summary>
    /// Total length of the Sub Record, including the header and its content.
    /// </summary>
    public int TotalLength => HeaderLength + ContentLength;

    /// <inheritdoc/>
    public override string ToString() => $"{RecordType.ToString()} => 0x{ContentLength:X}";
}

/// <summary>
/// A struct that overlays on top of bytes that is able to retrieve Sub Record header data on demand.
/// In addition, it keeps track of its location relative to its parent MajorRecordFrame
/// </summary>
public readonly struct SubrecordPinHeader
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
    /// Location of the subrecord relative to the parent MajorRecordFrame's data.<br/>
    /// E.g., relative to the position of the RecordType of the parent MajorRecord.
    /// </summary>
    public int Location { get; }
    
    /// <summary>
    /// Location where the subrecord header ends.  This is equivalent to Location + HeaderLength
    /// </summary>
    public int EndLocation => Location + HeaderLength;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="meta">Game metadata to use as reference for alignment</param>
    /// <param name="span">Span to overlay on, aligned to the start of the Sub Record's header</param>
    /// <param name="pinLocation">Location pin tracker relative to parent MajorRecordFrame</param>
    public SubrecordPinHeader(GameConstants meta, ReadOnlyMemorySlice<byte> span, int pinLocation)
    {
        Meta = meta;
        HeaderData = span.Slice(0, meta.SubConstants.HeaderLength);
        Location = pinLocation;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="header">Existing SubrecordHeader struct</param>
    /// <param name="pinLocation">Location pin tracker relative to parent MajorRecordFrame</param>
    public SubrecordPinHeader(SubrecordHeader header, int pinLocation)
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
    public byte HeaderLength => Meta.SubConstants.HeaderLength;
        
    /// <summary>
    /// RecordType of the header
    /// </summary>
    public RecordType RecordType => new RecordType(RecordTypeInt);
        
    /// <summary>
    /// RecordType of the header, represented as an int
    /// </summary>
    public int RecordTypeInt => BinaryPrimitives.ReadInt32LittleEndian(HeaderData.Slice(0, 4));
        
    /// <summary>
    /// The length of the content of the Sub Record, excluding the header bytes.
    /// </summary>
    public ushort ContentLength => BinaryPrimitives.ReadUInt16LittleEndian(HeaderData.Slice(4, 2));

    /// <summary>
    /// Total length of the Sub Record, including the header and its content.
    /// </summary>
    public int TotalLength => HeaderLength + ContentLength;

    /// <inheritdoc/>
    public override string ToString() => $"{RecordType.ToString()} => 0x{ContentLength.ToString("X")} @ {Location.ToString()}";

    public static implicit operator SubrecordHeader(SubrecordPinHeader frame)
    {
        return new SubrecordHeader(frame.Meta, span: frame.HeaderData);
    }
}

/// <summary>
/// A struct that overlays on top of bytes that is able to retrieve Sub Record header and content data on demand.
/// </summary>
public readonly struct SubrecordFrame
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
    public ReadOnlyMemorySlice<byte> Content => HeaderAndContentData.Slice(Header.HeaderLength);

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="meta">Game metadata to use as reference for alignment</param>
    /// <param name="span">Span to overlay on, aligned to the start of the header</param>
    public SubrecordFrame(GameConstants meta, ReadOnlyMemorySlice<byte> span)
    {
        Header = meta.SubrecordHeader(span);
        HeaderAndContentData = span.Slice(0, Header.TotalLength);
    }

    private SubrecordFrame(SubrecordHeader header, ReadOnlyMemorySlice<byte> span)
    {
        Header = header;
        HeaderAndContentData = span;
    }

    /// <summary>
    /// Factory
    /// </summary>
    /// <param name="header">Existing SubrecordHeader struct</param>
    /// <param name="span">Span to overlay on, aligned to the start of the header</param>
    public static SubrecordFrame Factory(SubrecordHeader header, ReadOnlyMemorySlice<byte> span)
    {
        return new SubrecordFrame(header, span.Slice(0, header.TotalLength));
    }

    /// <summary>
    /// Factory
    /// </summary>
    /// <param name="header">Existing SubrecordHeader struct</param>
    /// <param name="span">Span to overlay on, aligned to the start of the header</param>
    public static SubrecordFrame FactoryNoTrim(SubrecordHeader header, ReadOnlyMemorySlice<byte> span)
    {
        return new SubrecordFrame(header, span);
    }

    /// <inheritdoc/>
    public override string ToString() => Header.ToString();

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
    public byte HeaderLength => Header.HeaderLength;

    /// <summary>
    /// RecordType of the header
    /// </summary>
    public RecordType RecordType => Header.RecordType;

    /// <summary>
    /// RecordType of the header, represented as an int
    /// </summary>
    public int RecordTypeInt => Header.RecordTypeInt;

    /// <summary>
    /// The length of the content of the Sub Record, excluding the header bytes.
    /// </summary>
    public ushort ContentLength => (ushort)Content.Length;
    #endregion

    public static implicit operator SubrecordHeader(SubrecordFrame frame)
    {
        return frame.Header;
    }

    public SubrecordPinFrame Pin(int loc)
    {
        return new SubrecordPinFrame(this, loc);
    }
}

/// <summary>
/// A struct that overlays on top of bytes that is able to retrieve Sub Record data on demand.
/// In addition, it keeps track of its location relative to its parent MajorRecordFrame
/// </summary>
public readonly struct SubrecordPinFrame
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
    /// Location where the subrecord ends.  This is equivalent to Location + TotalLength
    /// </summary>
    public int EndLocation => Location + TotalLength;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="meta">Game metadata to use as reference for alignment</param>
    /// <param name="span">Span to overlay on, aligned to the start of the header</param>
    /// <param name="pinLocation">Location pin tracker relative to parent MajorRecordFrame</param>
    public SubrecordPinFrame(GameConstants meta, ReadOnlyMemorySlice<byte> span, int pinLocation)
    {
        Frame = new SubrecordFrame(meta, span);
        Location = pinLocation;
    }

    public SubrecordPinFrame(SubrecordFrame frame, int pinLocation)
    {
        Frame = frame;
        Location = pinLocation;
    }

    /// <summary>
    /// Factory
    /// </summary>
    /// <param name="header">Existing SubrecordHeader struct</param>
    /// <param name="span">Span to overlay on, aligned to the start of the header</param>
    /// <param name="pinLocation">Location pin tracker relative to parent MajorRecordFrame</param>
    public static SubrecordPinFrame Factory(SubrecordHeader header, ReadOnlyMemorySlice<byte> span, int pinLocation)
    {
        return new SubrecordPinFrame(
            SubrecordFrame.Factory(header, span),
            pinLocation);
    }

    /// <summary>
    /// Factory
    /// </summary>
    /// <param name="header">Existing SubrecordHeader struct</param>
    /// <param name="span">Span to overlay on, aligned to the start of the header</param>
    /// <param name="pinLocation">Location pin tracker relative to parent MajorRecordFrame</param>
    public static SubrecordPinFrame FactoryNoTrim(SubrecordHeader header, ReadOnlyMemorySlice<byte> span, int pinLocation)
    {
        return new SubrecordPinFrame(
            SubrecordFrame.FactoryNoTrim(header, span),
            pinLocation);
    }

    /// <inheritdoc/>
    public override string ToString() => $"{Frame.ToString()} @ {Location.ToString()}";

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
    public byte HeaderLength => Frame.HeaderLength;

    /// <summary>
    /// RecordType of the header
    /// </summary>
    public RecordType RecordType => Frame.RecordType;

    /// <summary>
    /// RecordType of the header, represented as an int
    /// </summary>
    public int RecordTypeInt => Frame.RecordTypeInt;

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

    public static implicit operator SubrecordPinHeader(SubrecordPinFrame pin)
    {
        return new SubrecordPinHeader(pin.Header, pin.Location);
    }
}