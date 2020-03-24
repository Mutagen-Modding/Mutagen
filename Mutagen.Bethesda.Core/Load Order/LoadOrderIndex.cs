using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    /// <summary>
    /// A struct representing an index of a mod in a LoadOrder
    /// </summary>
    public struct LoadOrderIndex
    {
        public static readonly LoadOrderIndex Zero = new LoadOrderIndex(0);
        public readonly int ID;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">ID to assign.  Cannot be negative</param>
        /// <exception cref="ArgumentException">Will throw if ID given is negative.</exception>
        public LoadOrderIndex(int id)
        {
            if (id < 0)
            {
                throw new ArgumentException("A ModIndex cannot be negative.");
            }
            this.ID = id;
        }

        public override string ToString()
        {
            return ID.ToString("X");
        }

        public override bool Equals(object obj)
        {
            if (!(obj is LoadOrderIndex)) return false;
            LoadOrderIndex rhs = (LoadOrderIndex)obj;
            return this.ID == rhs.ID;
        }

        public override int GetHashCode()
        {
            return this.ID.GetHashCode();
        }

        public static bool operator ==(LoadOrderIndex a, LoadOrderIndex b)
        {
            return a.ID == b.ID;
        }

        public static bool operator !=(LoadOrderIndex a, LoadOrderIndex b)
        {
            return !(a == b);
        }

        public static implicit operator LoadOrderIndex(int i)
        {
            return new LoadOrderIndex(i);
        }
    }
}
