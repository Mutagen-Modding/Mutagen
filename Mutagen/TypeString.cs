using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen
{
    public struct TypeString : IEquatable<TypeString>, IEquatable<char[]>
    {
        public readonly string Type;
        public const byte HEADER_LENGTH = 4;

        public TypeString(string type)
        {
            this.Type = type;
            if (this.Type == null
                || this.Type.Length != HEADER_LENGTH)
            {
                throw new ArgumentException($"Type String not expected length: {HEADER_LENGTH}.");
            }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is TypeString rhs)) return false;
            return Equals(rhs);
        }

        public bool Equals(TypeString other)
        {
            return object.Equals(this.Type, other.Type);
        }

        public bool Equals(char[] other)
        {
            if (other == null) return false;
            if (other.Length != HEADER_LENGTH) return false;
            for (int i = 0; i < HEADER_LENGTH; i++)
            {
                if (this.Type[i] != other[i]) return false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            return HashHelper.GetHashCode(this.Type);
        }
    }
}
