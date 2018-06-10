using Noggog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Mutagen.Bethesda
{
    public static class IBinaryStreamExt
    {
        public static Color ReadColor(this IBinaryStream stream)
        {
            var ret = Color.FromRgb(
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
