using Noggog;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    public static class IBinaryStreamExt
    {
        public static Color ReadColor(this IBinaryReadStream stream)
        {
            return ReadColor(stream.ReadSpan(stream.Remaining >= 4 ? 4 : 3));
        }

        public static Color ReadColor(this ReadOnlySpan<byte> span)
        {
            return Color.FromArgb(
                alpha: span.Length >= 4 ? span[3] : 0,
                red: span[0],
                green: span[1],
                blue: span[2]);
        }

        public static Color ReadColor(this ReadOnlyMemorySlice<byte> span)
        {
            return span.Span.ReadColor();
        }
    }
}
