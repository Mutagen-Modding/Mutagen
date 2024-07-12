using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Mutagen.Bethesda.Plugins.Masters;

namespace Mutagen.Bethesda.Plugins;

/// <summary>
/// A struct representing a FormID.  It wraps a raw uint, and exposes sections of it depending on if
/// it's a normal, light, or medium FormID.
///
/// Note:
/// FormID should be used sparingly, as it's prone to Mod indexing errors if mishandled.
/// FormKey is a more preferable struct for normal use.
/// </summary>
public readonly struct FormID : IEquatable<FormID>
{
    /// <summary>
    /// A static readonly singleton that represents a null FormID (all zeros).
    /// </summary>
    public static readonly FormID Null = new(0);
    
    public const uint SmallMasterMarker = 0xFE;
    public const uint MediumMasterMarker = 0xFD;
    internal const uint SmallMasterMarkerShifted = 0xFE000000;
    internal const uint MediumMasterMarkerShifted = 0xFD000000;
    
    public const uint FullIdMask =   0x00FFFFFF;
    public const uint MediumIdMask = 0x0000FFFF;
    public const uint SmallIdMask =  0x00000FFF;
        
    /// <summary>
    /// The raw uint as it would be stored on disk with both the ID and Mod index.
    /// </summary>
    public readonly uint Raw;
        
    /// <summary>
    /// The ID bytes of a FormID.
    /// Exposed as a uint, but will only ever have values filling the last 6 bytes.
    /// </summary>
    public uint FullId => Raw & FullIdMask;
    
    /// <summary>
    /// The ID bytes of a FormID.
    /// Exposed as a uint, but will only ever have values filling the last 4 bytes.
    /// </summary>
    public uint MediumId => Raw & MediumIdMask;
    
    /// <summary>
    /// The ID bytes of a FormID.
    /// Exposed as a uint, but will only ever have values filling the last 3 bytes.
    /// </summary>
    public uint LightId => Raw & SmallIdMask;
    
    public const uint FullMasterIndexMask =   0xFF000000;
    public const uint MediumMasterIndexMask = 0x00FF0000;
    public const uint LightMasterIndexMask =  0x00FFF000;
    public const byte FullMasterIndexShift = 24;
    public const byte MediumMasterIndexShift = 16;
    public const byte LightMasterIndexShift = 12;
        
    /// <summary>
    /// The Master Index bytes of a Full FormID: 0xFF000000
    /// </summary>
    public uint FullMasterIndex => (Raw & FullMasterIndexMask) >> FullMasterIndexShift;
    
    /// <summary>
    /// The Master Index bytes of a Medium FormID: 0x00FF0000
    /// </summary>
    public uint MediumMasterIndex => (Raw & MediumMasterIndexMask) >> MediumMasterIndexShift;
    
    /// <summary>
    /// The Master Index bytes of a Light FormID: 0x00FFF000
    /// </summary>
    public uint LightMasterIndex => (Raw & LightMasterIndexMask) >> LightMasterIndexShift;

    /// <summary>
    /// Constructor taking a Mod index and ID as a single uint, as it would be stored on-disk.
    /// Mod index is stored in the upper two bytes of the value.
    /// </summary>
    /// <param name="idWithModIndex">Mod index and Record ID to use</param>
    public FormID(uint idWithModIndex)
    {
        Raw = idWithModIndex;
    }

    /// <summary>
    /// Converts a char span in hexadecimal format to a FormID
    /// </summary>
    /// <param name="hexStr">string in hexadecimal format: (0x)FFFFFFFF</param>
    /// <returns>Converted FormID</returns>
    /// <exception cref="ArgumentException">Thrown on unconvertable string input</exception>
    public static FormID Factory(ReadOnlySpan<char> hexStr)
    {
        if (!TryFactory(hexStr, out var result))
        {
            throw new ArgumentException($"Invalid FormID hex: {hexStr.ToString()}");
        }
        return result;
    }

    /// <summary> 
    /// Attempts to convert a string in hexadecimal format to a FormID 
    /// </summary> 
    /// <param name="hexStr">string in hexadecimal format: (0x)FFFFFFFF</param> 
    /// <param name="id">Converted FormID if successful</param> 
    /// <param name="strictLength">If the input string has to be 8 chars long</param> 
    /// <returns>True if successful</returns> 
    public static bool TryFactory(ReadOnlySpan<char> hexStr, [MaybeNullWhen(false)] out FormID id, bool strictLength = true)
    {
        if (hexStr.StartsWith("0x"))
        {
            hexStr = hexStr.Slice(2);
        }

        if (strictLength && hexStr.Length != 8)
        {
            id = default;
            return false;
        }

        if (!uint.TryParse(hexStr, NumberStyles.HexNumber, null, out var intID))
        {
            id = default;
            return false;
        }
        id = new FormID(intID);
        return true;
    }

    /// <summary> 
    /// Attempts to convert a string in hexadecimal format to a FormID 
    /// </summary> 
    /// <param name="hexStr">string in hexadecimal format: (0x)FFFFFFFF</param> 
    /// <param name="strictLength">If the input string has to be 8 chars long</param> 
    /// <returns>Converted FormID if successful, otherwise null</returns> 
    public static FormID? TryFactory(ReadOnlySpan<char> hexStr, bool strictLength = true)
    {
        if (TryFactory(hexStr, out var id, strictLength: strictLength))
        {
            return id;
        }
        return default;
    }

    /// <summary>
    /// Convert an array to a FormID
    /// </summary>
    /// <param name="bytes">Input byte array</param>
    /// <returns>Converted FormID</returns>
    /// <exception cref="ArgumentException">Thrown if array size less than 4</exception>
    public static FormID Factory(ReadOnlySpan<byte> bytes)
    {
        return Factory(BinaryPrimitives.ReadUInt32LittleEndian(bytes));
    }
    
    /// <summary>
    /// Wrap a uint with a FormID
    /// </summary>
    /// <param name="idWithModIndex">Mod index and Record ID to use</param>
    /// <returns>Converted FormID</returns>
    public static FormID Factory(uint idWithModIndex)
    {
        return new FormID(idWithModIndex);
    }

    public static FormID Factory(MasterStyle style, uint masterIndex, uint id)
    {
        byte shift;
        uint mask;
        uint upperValue;
        
        switch (style)
        {
            case MasterStyle.Full:
                shift = FullMasterIndexShift;
                mask = FullIdMask;
                upperValue = 0;
                break;
            case MasterStyle.Medium:
                shift = MediumMasterIndexShift;
                mask = MediumIdMask;
                upperValue = MediumMasterMarkerShifted;
                break;
            case MasterStyle.Small:
                shift = LightMasterIndexShift;
                mask = SmallIdMask;
                upperValue = SmallMasterMarkerShifted;
                break;
            default:
                throw new NotImplementedException();
        }

        var raw = masterIndex << shift;
        id &= mask;
        raw += id;
        raw += upperValue;
        return new FormID(raw);
    }

    /// <summary>
    /// Converts to a byte array of size 4.
    /// </summary>
    /// <returns>byte array of size 4 with raw contents</returns>
    public byte[] ToBytes()
    {
        return BitConverter.GetBytes(Raw);
    }

    /// <summary>
    /// Converts to a hex string
    /// </summary>
    /// <returns>Hex string</returns>
    public override string ToString()
    {
        return Raw.ToString("X");
    }

    /// <summary>
    /// Converts to a hex string containing only the ID section
    /// </summary>
    /// <returns>Hex string</returns>
    public string IdString(MasterStyle style)
    {
        switch (style)
        {
            case MasterStyle.Full:
                return FullId.ToString("X6");
            case MasterStyle.Small:
                return LightId.ToString("X3");
            case MasterStyle.Medium:
                return MediumId.ToString("X4");
            default:
                throw new ArgumentOutOfRangeException(nameof(style), style, null);
        }
    }

    /// <summary>
    /// Interprets the Id with the given master style
    /// </summary>
    /// <param name="style">Master style to interpret with</param>
    /// <returns>Id number without master index bytes</returns>
    public uint Id(MasterStyle style)
    {
        switch (style)
        {
            case MasterStyle.Full:
                return FullId;
            case MasterStyle.Small:
                return LightId;
            case MasterStyle.Medium:
                return MediumId;
            default:
                throw new ArgumentOutOfRangeException(nameof(style), style, null);
        }
    }

    /// <summary>
    /// Gets the Maximum mask for the given style
    /// </summary>
    /// <param name="style">Master style to get</param>
    /// <returns>Mask to apply to retrieve the id portion of the value</returns>
    public static uint IdMask(MasterStyle style) => style switch
    {
        MasterStyle.Small => SmallIdMask,
        MasterStyle.Medium => MediumIdMask,
        MasterStyle.Full => FullIdMask,
        _ => throw new NotImplementedException()
    };

    /// <summary>
    /// Gets the Master Index shift for the given style
    /// </summary>
    /// <param name="style">Master style to get</param>
    /// <returns>shift to apply to retrieve the master index</returns>
    public static uint MasterIndexShift(MasterStyle style) => style switch
    {
        MasterStyle.Small => LightMasterIndexShift,
        MasterStyle.Medium => MediumMasterIndexShift,
        MasterStyle.Full => FullMasterIndexShift,
        _ => throw new NotImplementedException()
    };

    /// <summary>
    /// Gets the Master Index for the given style
    /// </summary>
    /// <param name="style">Master style to get</param>
    /// <returns>Master index of the FormID given the style</returns>
    public uint MasterIndex(MasterStyle style) => style switch
    {
        MasterStyle.Small => LightMasterIndex,
        MasterStyle.Medium => MediumMasterIndex,
        MasterStyle.Full => FullMasterIndex,
        _ => throw new NotImplementedException()
    };

    /// <summary>
    /// Default equality operator
    /// </summary>
    /// <param name="obj">object to compare to</param>
    /// <returns>True if FormID with equal raw value</returns>
    public override bool Equals(object? obj)
    {
        if (obj is not FormID formID) return false;
        return Equals(formID);
    }

    /// <summary>
    /// FormID equality operator
    /// </summary>
    /// <param name="other">FormID to compare to</param>
    /// <returns>True equal raw value</returns>
    public bool Equals(FormID other)
    {
        return Raw == other.Raw;
    }

    /// <summary>
    /// Hashcode retrieved from Raw value.
    /// </summary>
    /// <returns>Hashcode retrieved from Raw value.</returns>
    public override int GetHashCode()
    {
        return Raw.GetHashCode();
    }

    public static bool operator ==(FormID a, FormID b)
    {
        return a.Raw == b.Raw;
    }

    public static bool operator !=(FormID a, FormID b)
    {
        return !(a == b);
    }
}
