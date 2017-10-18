using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Internals
{
    public struct FileLocation
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

        public static implicit operator long(FileLocation loc)
        {
            return loc.Offset;
        }

        public static implicit operator FileLocation(long loc)
        {
            return new FileLocation(loc);
        }
    }
}
