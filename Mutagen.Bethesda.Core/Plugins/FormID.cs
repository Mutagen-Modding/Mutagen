using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Mutagen.Bethesda.Plugins.Masters;

namespace Mutagen.Bethesda.Plugins;

/// <summary>
/// A struct representing a FormID as it exists on disk:
///   - The ID of a record (6 bytes)
///   - The Mod Index the ID associated with, relative to the container Mod's master header list.
///
/// Note:
/// FormID should be used sparingly, as it's prone to Mod indexing errors if mishandled.
/// FormKey is a more preferable struct for normal use.
/// </summary>
public readonly struct FormID : IEquatable<FormID>
{
    public const uint Max = 0xFFFFFF;
    
    /// <summary>
    /// A static readonly singleton that represents a null FormID (all zeros).
    /// </summary>
    public static readonly FormID Null = new FormID();
        
    /// <summary>
    /// The raw uint as it would be stored on disk with both the ID and Mod index.
    /// </summary>
    public readonly uint Raw;
        
    /// <summary>
    /// The ModIndex bytes of the FormID
    /// </summary>
    public ModIndex ModIndex => ModIndex.GetModIndexFromUInt(Raw);
        
    /// <summary>
    /// The ID bytes of a FormID.
    /// Exposed as a uint, but will only ever have values filling the first 6 bytes.
    /// </summary>
    public uint ID => Raw & 0x00FFFFFF;

    /// <summary>
    /// Constructor taking a Mod index and ID as separate parameters
    /// </summary>
    /// <param name="modID">Mod index to use</param>
    /// <param name="id">Record ID to use.  Must be less than 0x00FFFFFF.</param>
    /// <exception cref="ArgumentException">ID needs to contain no data in upper two bytes, or it will throw.</exception>
    public FormID(ModIndex modID, uint id)
    {
        if ((id & 0xFF000000) != 0)
        {
            throw new ArgumentException("Data present in Mod index bytes of id");
        }
        Raw = (uint)(modID.ID << 24);
        Raw += Raw + id & 0x00FFFFFF;
    }

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
    /// Convert an array to a FormID, with protection against ModKey overflow
    /// </summary>
    /// <param name="bytes">Input byte array</param>
    /// <param name="masters">Master list to reference to handle overflow</param>
    /// <returns>Converted FormID</returns>
    /// <exception cref="ArgumentException">Thrown if array size less than 4</exception>
    public static FormID Factory(ReadOnlySpan<byte> bytes, IReadOnlyMasterReferenceCollection masters)
    {
        return Factory(BinaryPrimitives.ReadUInt32LittleEndian(bytes), masters);
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

    /// <summary>
    /// Wrap a uint with a FormID, with protection against ModKey overflow
    /// </summary>
    /// <param name="idWithModIndex">Mod index and Record ID to use</param>
    /// <param name="masters">Master list to reference to handle overflow</param>
    /// <returns>Converted FormID</returns>
    public static FormID Factory(uint idWithModIndex, IReadOnlyMasterReferenceCollection masters)
    {
        var ret = new FormID(idWithModIndex);
        if (ret.ModIndex.ID <= masters.Masters.Count) return ret;
        return new FormID(new ModIndex(checked((byte)masters.Masters.Count)), ret.ID);
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
    /// Converts to a hex string: FFFFFFFF
    /// </summary>
    /// <returns>Hex string</returns>
    public string ToHex()
    {
        return $"{ModIndex}{IDString()}";
    }

    /// <summary>
    /// Converts to a hex string: FFFFFFFF
    /// </summary>
    /// <returns>Hex string</returns>
    public override string ToString()
    {
        return $"({ModIndex}){IDString()}";
    }

    /// <summary>
    /// Converts to a hex string containing only the ID section: FFFFFF
    /// </summary>
    /// <returns>Hex string</returns>
    public string IDString()
    {
        return ID.ToString("X6");
    }

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

internal readonly struct LightMasterFormID
{
    public readonly uint Raw;
    public const uint Max = 0xFFF;
    public uint ID => Raw & 0xFFF;
    public uint ModIndex => (Raw & 0x00FFF000) >> 12;
    
    public LightMasterFormID(uint modID, uint id)
    {
        if ((id & 0xFFFFF000) != 0)
        {
            throw new ArgumentException("Data present in Mod index bytes of id");
        }
        if ((modID & 0xFFFFF000) != 0)
        {
            throw new ArgumentException("ModID was bigger than allowed");
        }
        Raw = (uint)(modID << 12);
        Raw += Raw + id & Max;
        Raw += 0xFE000000;
    }
    
    public LightMasterFormID(uint idWithModIndex)
    {
        if ((idWithModIndex & 0xFF000000) != 0xFE000000)
        {
            throw new ArgumentException("ID with mod index must start with FE for Light FormIDs");
        }
        Raw = idWithModIndex;
    }

    public byte[] ToBytes()
    {
        return BitConverter.GetBytes(Raw);
    }
}

internal readonly struct MediumMasterFormID
{
    public readonly uint Raw;
    public const uint Max = 0xFFFF;
    public uint ID => Raw & 0xFFFF;
    public byte ModIndex => (byte)((Raw & 0x00FF0000) >> 16);
    
    public MediumMasterFormID(byte modID, uint id)
    {
        if ((id & 0xFFFF0000) != 0)
        {
            throw new ArgumentException("Data present in Mod index bytes of id");
        }
        Raw = (uint)(modID << 16);
        Raw += Raw + id & Max;
        Raw += 0xFD000000;
    }
    
    public MediumMasterFormID(uint idWithModIndex)
    {
        if ((idWithModIndex & 0xFF000000) != 0xFD000000)
        {
            throw new ArgumentException("ID with mod index must start with FD for Medium FormIDs");
        }
        Raw = idWithModIndex;
    }

    public byte[] ToBytes()
    {
        return BitConverter.GetBytes(Raw);
    }
}