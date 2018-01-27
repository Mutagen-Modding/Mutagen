using Noggog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Internals
{
    public struct FileSection : IEquatable<FileSection>
    {
        public readonly RangeInt64 Range;
        public FileLocation Min => new FileLocation(Range.Min);
        public FileLocation Max => new FileLocation(Range.Max);
        public ContentLength Width => new ContentLength(Range.Width);
        public ContentLength Difference => new ContentLength(Range.Difference);

        public FileSection(FileLocation from, FileLocation to)
        {
            this.Range = new RangeInt64(from.Offset, to.Offset);
        }

        public FileSection(long from, long to)
        {
            this.Range = new RangeInt64(from, to);
        }

        public FileSection(RangeInt64 r)
        {
            this.Range = r;
        }

        public override string ToString()
        {
            return $"({FileLocation.ToString(this.Range.Min)} - {FileLocation.ToString(this.Range.Max)})";
        }

        public override bool Equals(object obj)
        {
            if (!(obj is FileSection sec)) return false;
            return this.Equals(sec);
        }

        public bool Equals(FileSection other)
        {
            return this.Range.Equals(other.Range);
        }

        public override int GetHashCode()
        {
            return this.Range.GetHashCode();
        }
    }
}
