using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Internals;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Meta;

/// <summary>
/// Reference for Record alignment and length constants
/// </summary>
public record RecordHeaderConstants
{
    /// <summary>
    /// Type of object the constants are associated with
    /// </summary>
    public ObjectType ObjectType { get; }
        
    /// <summary>
    /// The length that the header itself takes
    /// </summary>
    public byte HeaderLength { get; }
        
    /// <summary>
    /// Number of bytes that hold length information
    /// </summary>
    public byte LengthLength { get; }
        
    /// <summary>
    /// Number of bytes in the header following the length information
    /// </summary>
    public byte LengthAfterLength { get; }
        
    /// <summary>
    /// Number of bytes in the header following the record type information
    /// </summary>
    public byte LengthAfterType { get; }
        
    /// <summary>
    /// Size of the record type and length bytes
    /// </summary>
    public byte TypeAndLengthLength { get; }
        
    /// <summary>
    /// Whether the size of the header itself is included in the length bytes, in addition to the content length
    /// </summary>
    public bool HeaderIncludedInLength { get; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="type">Type of object to associate the constants with</param>
    /// <param name="headerLength">Length of the header</param>
    /// <param name="lengthLength">Number of bytes containing content length information</param>
    public RecordHeaderConstants(
        ObjectType type,
        byte headerLength,
        byte lengthLength)
    {
        ObjectType = type;
        HeaderLength = headerLength;
        LengthLength = lengthLength;
        LengthAfterLength = (byte)(HeaderLength - Constants.HeaderLength - LengthLength);
        LengthAfterType = (byte)(HeaderLength - Constants.HeaderLength);
        TypeAndLengthLength = (byte)(Constants.HeaderLength + LengthLength);
        HeaderIncludedInLength = type == ObjectType.Group;
    }

    public VariableHeader VariableMeta(ReadOnlyMemorySlice<byte> span) => new VariableHeader(this, span);
    public VariableHeader GetVariableMeta(IBinaryReadStream stream, int offset = 0) => new VariableHeader(this, stream.GetMemory(HeaderLength, offset));
    public VariableHeader ReadVariableMeta(IBinaryReadStream stream) => new VariableHeader(this, stream.ReadMemory(HeaderLength));
}