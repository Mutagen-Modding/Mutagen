using Mutagen.Bethesda.Binary;
using Noggog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Mutagen.Bethesda
{
    public static class MutagenReaderExt
    {
        public static Color ReadColor(this MutagenReader stream)
        {
            var ret = Color.FromRgb(
                stream.ReadByte(),
                stream.ReadByte(),
                stream.ReadByte());
            stream.ReadByte();
            return ret;
        }
    }
}
