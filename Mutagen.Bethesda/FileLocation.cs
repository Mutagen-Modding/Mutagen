using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Internals
{
    public struct FileLocation : IEquatable<FileLocation>, IComparable, IComparable<FileLocation>, IComparable<long>
    {
        public readonly long Offset;

        public FileLocation(long offset)
        {
            this.Offset = offset;
        }

        public override string ToString()
        {
            if (this.Offset >= 0)
            {
                return $"0x{this.Offset.ToString("X")}";
            }
            else
            {
                return $"-0x{(this.Offset * -1).ToString("X")}";
            }
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

        public int CompareTo(object obj)
        {
            if (obj is FileLocation loc)
            {
                return this.CompareTo(loc);
            }
            else if (obj is long l)
            {
                return this.CompareTo(l);
            }
            else
            {
                throw new ArgumentException();
            }
        }

        public int CompareTo(FileLocation other)
        {
            return this.Offset.CompareTo(other.Offset);
        }

        public int CompareTo(long other)
        {
            return this.Offset.CompareTo(other);
        }

        public static implicit operator long(FileLocation loc)
        {
            return loc.Offset;
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
            return new FileLocation(c1.Offset + c2);
        }

        public static FileLocation operator -(FileLocation c1, int c2)
        {
            return new FileLocation(c1.Offset - c2);
        }

        public static FileLocation operator +(FileLocation c1, byte c2)
        {
            return new FileLocation(c1.Offset + c2);
        }

        public static FileLocation operator -(FileLocation c1, byte c2)
        {
            return new FileLocation(c1.Offset - c2);
        }

        public static FileLocation operator +(FileLocation c1, ContentLength c2)
        {
            return new FileLocation(c1.Offset + c2.Value);
        }

        public static FileLocation operator -(FileLocation c1, ContentLength c2)
        {
            return new FileLocation(c1.Offset - c2.Value);
        }
    }
}
