using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Internals
{
    public struct FileLocation : IEquatable<FileLocation>
    {
        public readonly long Offset;

        public FileLocation(long offset)
        {
            this.Offset = offset;
        }

        public override string ToString()
        {
            return $"0x{this.Offset.ToString("X")}";
        }

        public override bool Equals(object obj)
        {
            if (!(obj is FileLocation rhs)) return false;
            return Equals(rhs);
        }

        public bool Equals(FileLocation other)
        {
            return this.Offset == other.Offset;
        }

        public override int GetHashCode()
        {
            return this.Offset.GetHashCode();
        }

        public static implicit operator long(FileLocation loc)
        {
            return loc.Offset;
        }

        public static implicit operator FileLocation(long loc)
        {
            return new FileLocation(loc);
        }

        public static bool operator ==(FileLocation c1, FileLocation c2)
        {
            return c1.Offset == c2.Offset;
        }

        public static bool operator !=(FileLocation c1, FileLocation c2)
        {
            return c1.Offset != c2.Offset;
        }

        public static bool operator <(FileLocation c1, FileLocation c2)
        {
            return c1.Offset < c2.Offset;
        }

        public static bool operator >(FileLocation c1, FileLocation c2)
        {
            return c1.Offset > c2.Offset;
        }

        public static bool operator >=(FileLocation c1, FileLocation c2)
        {
            return c1.Offset >= c2.Offset;
        }

        public static bool operator <=(FileLocation c1, FileLocation c2)
        {
            return c1.Offset <= c2.Offset;
        }

        public static ContentLength operator +(FileLocation c1, FileLocation c2)
        {
            return new ContentLength((int)(c1.Offset + c2.Offset));
        }

        public static ContentLength operator -(FileLocation c1, FileLocation c2)
        {
            return new ContentLength((int)(c1.Offset - c2.Offset));
        }

        public static FileLocation operator +(FileLocation c1, int c2)
        {
            return c1.Offset + c2;
        }

        public static FileLocation operator -(FileLocation c1, int c2)
        {
            return c1.Offset - c2;
        }

        public static FileLocation operator +(FileLocation c1, byte c2)
        {
            return c1.Offset + c2;
        }

        public static FileLocation operator -(FileLocation c1, byte c2)
        {
            return c1.Offset - c2;
        }

        public static FileLocation operator +(FileLocation c1, ContentLength c2)
        {
            return c1.Offset + c2.Length;
        }

        public static FileLocation operator -(FileLocation c1, ContentLength c2)
        {
            return c1.Offset - c2.Length;
        }
    }
}
