using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen
{
    public struct RecordType : IEquatable<RecordType>, IEquatable<char[]>
    {
        public readonly string Type;
        public const byte HEADER_LENGTH = 4;
        public string HeaderName => $"{Type}_HEADER";

        internal RecordType (string type, bool validate)
        {
            this.Type = type;
            if (!validate) return;
            if (this.Type == null
                || this.Type.Length != HEADER_LENGTH)
            {
                throw new ArgumentException($"Type String not expected length: {HEADER_LENGTH}.");
            }
        }

        public RecordType(string type)
            : this(type, validate: true)
        {
        }

        public override bool Equals(object obj)
        {
            if (!(obj is RecordType rhs)) return false;
            return Equals(rhs);
        }

        public bool Equals(RecordType other)
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

        public override string ToString()
        {
            return this.Type;
        }
    }
}
