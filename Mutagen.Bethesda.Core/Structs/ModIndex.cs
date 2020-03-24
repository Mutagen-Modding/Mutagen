using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    /// <summary>
    /// A struct representing a index of a master within a FormID.
    /// Mods can only reference a byte's worth of masters, so indices must be limited to a byte.
    /// </summary>
    public struct ModIndex
    {
        /// <summary>
        /// A static readonly singleton ModID with value 0
        /// </summary>
        public static readonly ModIndex Zero = new ModIndex(0);
        
        /// <summary>
        /// Index value
        /// </summary>
        public readonly byte ID;

        /// <summary>
        /// Constructor
        /// </summary>
        public ModIndex(byte id)
        {
            this.ID = id;
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
        public override bool Equals(object obj)
        {
            if (!(obj is ModIndex)) return false;
            ModIndex rhs = (ModIndex)obj;
            return this.ID == rhs.ID;
        }

        /// <summary>
        /// Hashcode retrieved from index
        /// </summary>
        /// <returns>Hashcode retrieved from index</returns>
        public override int GetHashCode()
        {
            return this.ID.GetHashCode();
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
        public static byte GetModIDByteFromUInt(uint i)
        {
            return (byte)(i >> 24);
        }
    }
}
