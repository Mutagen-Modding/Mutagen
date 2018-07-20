using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    public struct ModID
    {
        public readonly byte ID;

        public ModID(byte id)
        {
            this.ID = id;
        }

        public override string ToString()
        {
            return ID.ToString("X2");
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ModID)) return false;
            ModID rhs = (ModID)obj;
            return this.ID == rhs.ID;
        }

        public override int GetHashCode()
        {
            return this.ID.GetHashCode();
        }

        public static bool operator ==(ModID a, ModID b)
        {
            return a.ID == b.ID;
        }

        public static bool operator !=(ModID a, ModID b)
        {
            return !(a == b);
        }
    }
}
