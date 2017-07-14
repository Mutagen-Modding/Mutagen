using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen
{
    public struct ModID
    {
        private readonly ushort ID;

        public ModID(ushort id)
        {
            this.ID = id;
        }

        public override string ToString()
        {
            return ID.ToString("X4");
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
    }
}
