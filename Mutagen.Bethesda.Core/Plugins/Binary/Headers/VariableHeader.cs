using Mutagen.Bethesda.Plugins.Meta;
using Noggog;
using System.Buffers.Binary;

namespace Mutagen.Bethesda.Plugins.Binary.Headers;

/// <summary>
/// A ref struct that overlays on top of bytes that is able to retrieve basic header data on demand
/// utilizing a given constants object to define the lengths
/// </summary>
public readonly struct VariableHeader
{
    /// <summary>
    /// Bytes overlaid onto
    /// </summary>
    public ReadOnlyMemorySlice<byte> HeaderAndContentData { get; }
    
    /// <summary>
    /// Bytes overlaid onto
    /// </summary>
    public ReadOnlyMemorySlice<byte> Content => HeaderAndContentData.Slice(HeaderConstants.HeaderLength, checked((int)ContentLength));
        
    /// <summary>
    /// GameConstants
    /// </summary>
    public GameConstants Constants { get; }
    
    /// <summary>
    /// Record metadata to use as reference for alignment
    /// </summary>
    public RecordHeaderConstants HeaderConstants { get; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="constants">Record constants to use as reference for alignment</param>
    /// <param name="span">Span to overlay on, aligned to the start of the header</param>
    public VariableHeader(GameConstants constants, ObjectType objectType, ReadOnlyMemorySlice<byte> span)
    {
        Constants = constants;
        HeaderAndContentData = span;
        HeaderConstants = constants.Constants(objectType);
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="constants">Record constants to use as reference for alignment</param>
    /// <param name="span">Span to overlay on, aligned to the start of the header</param>
    public VariableHeader(GameConstants constants, RecordHeaderConstants headerConstants, ReadOnlyMemorySlice<byte> span)
    {
        Constants = constants;
        HeaderAndContentData = span;
        HeaderConstants = headerConstants;
    }

    /// <summary>
    /// The length that the header itself takes
    /// </summary>
    public byte HeaderLength => HeaderConstants.HeaderLength;
        
    public int RecordTypeInt => BinaryPrimitives.ReadInt32LittleEndian(HeaderAndContentData.Slice(0, 4));
        
    /// <summary>
    /// RecordType of the header
    /// </summary>
    public RecordType RecordType => new(RecordTypeInt);
        
    public uint RecordLength
    {
        get
        {
            switch (HeaderConstants.LengthLength)
            {
                case 1:
                    return HeaderAndContentData[4];
                case 2:
                    return BinaryPrimitives.ReadUInt16LittleEndian(HeaderAndContentData.Slice(4, 2));
                case 4:
                    return BinaryPrimitives.ReadUInt32LittleEndian(HeaderAndContentData.Slice(4, 4));
                default:
                    throw new NotImplementedException();
            }
        }
    }
        
    /// <summary>
    /// The length of the RecordType and the length bytes
    /// </summary>
    public int TypeAndLengthLength => HeaderConstants.TypeAndLengthLength;
        
    /// <summary>
    /// Total length of the record, including the header and its content.
    /// </summary>
    public long TotalLength => HeaderConstants.HeaderIncludedInLength ? RecordLength : (HeaderLength + RecordLength);
        
    /// <summary>
    /// True if RecordType == "GRUP"
    /// </summary>
    public bool IsGroup => HeaderConstants.ObjectType == ObjectType.Group;
        
    /// <summary>
    /// The length of the content, excluding the header bytes.
    /// </summary>
    public long ContentLength => HeaderConstants.HeaderIncludedInLength ? RecordLength - HeaderLength : RecordLength;

    /// <inheritdoc/>
    public override string ToString() => $"{RecordType} [0x{ContentLength:X}]";
}

/// <summary>
/// A ref struct that overlays on top of bytes that is able to retrieve basic header data on demand
/// utilizing a given constants object to define the lengths
/// </summary>
public readonly struct VariablePinHeader
{
    /// <summary>
    /// Header struct contained in the pin
    /// </summary>
    public VariableHeader Header { get; }

    /// <summary>
    /// Location of the record relative to the parent GroupFrame's data.<br/>
    /// E.g., relative to the position of the start of the parent Group
    /// </summary>
    public int Location { get; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="constants">Record constants to use as reference for alignment</param>
    /// <param name="span">Span to overlay on, aligned to the start of the header</param>
    /// <param name="pinLocation">Location pin tracker relative to parent GroupFrame</param>
    public VariablePinHeader(GameConstants constants, ObjectType objectType, ReadOnlyMemorySlice<byte> span, int pinLocation)
    {
        Header = new VariableHeader(constants, objectType, span);
        Location = pinLocation;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="constants">Record constants to use as reference for alignment</param>
    /// <param name="span">Span to overlay on, aligned to the start of the header</param>
    /// <param name="pinLocation">Location pin tracker relative to parent GroupFrame</param>
    public VariablePinHeader(GameConstants constants, RecordHeaderConstants recordHeaderConstants, ReadOnlyMemorySlice<byte> span, int pinLocation)
    {
        Header = new VariableHeader(constants, recordHeaderConstants, span);
        Location = pinLocation;
    }

    public VariablePinHeader(VariableHeader header, int pinLocation)
    {
        Header = header;
        Location = pinLocation;
    }

    /// <summary>
    /// The length that the header itself takes
    /// </summary>
    public byte HeaderLength => HeaderConstants.HeaderLength;

    public int RecordTypeInt => Header.RecordTypeInt;
        
    /// <summary>
    /// RecordType of the header
    /// </summary>
    public RecordType RecordType => Header.RecordType;
        
    public uint RecordLength => Header.RecordLength;

    /// <summary>
    /// Bytes overlaid onto
    /// </summary>
    public ReadOnlyMemorySlice<byte> HeaderAndContentData => Header.HeaderAndContentData;
    
    /// <summary>
    /// Bytes overlaid onto
    /// </summary>
    public ReadOnlyMemorySlice<byte> Content => Header.Content;
    
    /// <summary>
    /// GameConstants
    /// </summary>
    public GameConstants Constants => Header.Constants;

    /// <summary>
    /// Record metadata to use as reference for alignment
    /// </summary>
    public RecordHeaderConstants HeaderConstants => Header.HeaderConstants;
        
    /// <summary>
    /// The length of the RecordType and the length bytes
    /// </summary>
    public int TypeAndLengthLength => HeaderConstants.TypeAndLengthLength;
        
    /// <summary>
    /// Total length of the record, including the header and its content.
    /// </summary>
    public long TotalLength => HeaderConstants.HeaderIncludedInLength ? RecordLength : (HeaderLength + RecordLength);
        
    /// <summary>
    /// True if RecordType == "GRUP"
    /// </summary>
    public bool IsGroup => HeaderConstants.ObjectType == ObjectType.Group;
        
    /// <summary>
    /// The length of the content, excluding the header bytes.
    /// </summary>
    public long ContentLength => HeaderConstants.HeaderIncludedInLength ? RecordLength - HeaderLength : RecordLength;

    /// <inheritdoc/>
    public override string ToString() => $"{RecordType} [0x{ContentLength:X}] @ 0x{Location:X}";
}