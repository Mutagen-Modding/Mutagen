using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    public struct RecordType : IEquatable<RecordType>, IEquatable<string>
    {
        public readonly string Type;
        public const byte HEADER_LENGTH = 4;

        [DebuggerStepThrough]
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

        [DebuggerStepThrough]
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

        public bool Equals(string other)
        {
            return string.Equals(other, Type);
        }

        public static bool operator ==(RecordType r1, RecordType r2)
        {
            return r1.Equals(r2);
        }

        public static bool operator !=(RecordType r1, RecordType r2)
        {
            return !r1.Equals(r2);
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
