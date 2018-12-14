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
            var ret = Color.FromArgb(
                stream.ReadUInt8(),
                stream.ReadUInt8(),
                stream.ReadUInt8());
            if (stream.Remaining > 0)
            {
                stream.ReadUInt8();
            }
            return ret;
        }
    }
}
