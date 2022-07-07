namespace Mutagen.Bethesda.Plugins;

/// <summary>
/// A struct representing a index of a master within a FormID.
/// Mods can only reference a byte's worth of masters, so indices must be limited to a byte.
/// </summary>
public readonly struct ModIndex
{
    /// <summary>
    /// A static readonly singleton ModID with value 0
    /// </summary>
    public static readonly ModIndex Zero = new ModIndex(0);

    public const byte MaxIndex = 254;

    /// <summary>
    /// Index value
    /// </summary>
    public readonly byte ID;

    /// <summary>
    /// Constructor
    /// </summary>
    public ModIndex(byte id)
    {
        ID = id;
    }

    /// <summary>
    /// Prints index in hex format: FF
    /// </summary>
    public override string ToString()
    {
        return ID.ToString("X2");
    }

    /// <summary>
    /// Default equality operator
    /// </summary>
    /// <param name="obj">object to compare to</param>
    /// <returns>True if ModIndex with equal index</returns>
    public override bool Equals(object? obj)
    {
        if (obj is not ModIndex rhs) return false;
        return ID == rhs.ID;
    }

    /// <summary>
    /// Hashcode retrieved from index
    /// </summary>
    /// <returns>Hashcode retrieved from index</returns>
    public override int GetHashCode()
    {
        return ID.GetHashCode();
    }

    public static bool operator ==(ModIndex a, ModIndex b)
    {
        return a.ID == b.ID;
    }

    public static bool operator !=(ModIndex a, ModIndex b)
    {
        return !(a == b);
    }

    /// <summary>
    /// Extracts the index byte from a uint input
    /// </summary>
    /// <param name="i">uint to retrieve mod index from</param>
    /// <returns>Byte containing the mod index</returns>
    public static byte GetModIndexByteFromUInt(uint i)
    {
        return (byte)(i >> 24);
    }
}