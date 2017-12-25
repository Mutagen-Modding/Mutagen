using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    public struct NamedFormID : IEquatable<NamedFormID>
    {
        public readonly string ModName;
        public readonly uint ID;

        public NamedFormID(string modName, uint id)
        {
            this.ModName = modName;
            this.ID = id;
        }
        
        public string ToHex()
        {
            return $"{ModName}{ID.ToString("X8")}";
        }

        public override string ToString()
        {
            return $"({ModName}){ID.ToString("X8")}";
        }

        public override bool Equals(object obj)
        {
            if (!(obj is RawFormID formID)) return false;
            return Equals(formID);
        }

        public override int GetHashCode()
        {
            return HashHelper.GetHashCode(this.ModName)
                .CombineHashCode(this.ID.GetHashCode());
        }

        public bool Equals(NamedFormID other)
        {
            return this.ModName.Equals(other.ModName)
                && this.ID == other.ID;
        }

        public static bool operator ==(NamedFormID a, NamedFormID b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(NamedFormID a, NamedFormID b)
        {
            return !a.Equals(b);
        }
    }
}
