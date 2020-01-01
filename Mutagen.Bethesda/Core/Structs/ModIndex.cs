using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    /// <summary>
    /// A struct representing an index of a master.  Outside of a mod represented in binary format,
    /// a mod can have as many masters as needed and so doesn't have to be limited in number.
    /// </summary>
    public struct ModIndex
    {
        public static readonly ModIndex Zero = new ModIndex(0);
        public readonly int ID;

        /// <summary>
        /// ModIndex constructor.
        /// </summary>
        /// <param name="id">ID to assign.  Cannot be negative</param>
        /// <exception cref="ArgumentException">Will throw if ID given is negative.</exception>
        public ModIndex(int id)
        {
            if (id < 0)
            {
                throw new ArgumentException("A ModIndex cannot be negative.");
            }
            this.ID = id;
        }

        public override string ToString()
        {
            return ID.ToString("X2");
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ModIndex)) return false;
            ModIndex rhs = (ModIndex)obj;
            return this.ID == rhs.ID;
        }

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
    }
}
