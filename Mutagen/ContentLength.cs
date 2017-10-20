using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen
{
    public struct ContentLength : IEquatable<ContentLength>
    {
        public readonly int Length;

        public ContentLength(int length)
        {
            this.Length = length;
        }

        public override string ToString()
        {
            return $"0x{this.Length.ToString("X")}";
        }
        
        public static implicit operator ContentLength(int loc)
        {
            return new ContentLength(loc);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ContentLength rhs)) return false;
            return Equals(rhs);
        }

        public bool Equals(ContentLength other)
        {
            return this.Length == other.Length;
        }

        public override int GetHashCode()
        {
            return this.Length.GetHashCode();
        }

        public static bool operator ==(ContentLength c1, ContentLength c2)
        {
            return c1.Length == c2.Length;
        }

        public static bool operator !=(ContentLength c1, ContentLength c2)
        {
            return c1.Length != c2.Length;
        }

        public static bool operator <(ContentLength c1, ContentLength c2)
        {
            return c1.Length < c2.Length;
        }

        public static bool operator >(ContentLength c1, ContentLength c2)
        {
            return c1.Length > c2.Length;
        }

        public static bool operator >=(ContentLength c1, ContentLength c2)
        {
            return c1.Length >= c2.Length;
        }

        public static bool operator <=(ContentLength c1, ContentLength c2)
        {
            return c1.Length <= c2.Length;
        }

        public static ContentLength operator +(ContentLength c1, ContentLength c2)
        {
            return c1.Length + c2.Length;
        }

        public static ContentLength operator -(ContentLength c1, ContentLength c2)
        {
            return c1.Length - c2.Length;
        }
    }
}
