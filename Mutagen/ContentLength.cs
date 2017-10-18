using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen
{
    public struct ContentLength
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

        public static implicit operator int(ContentLength loc)
        {
            return loc.Length;
        }

        public static implicit operator ContentLength(int loc)
        {
            return new ContentLength(loc);
        }
    }
}
