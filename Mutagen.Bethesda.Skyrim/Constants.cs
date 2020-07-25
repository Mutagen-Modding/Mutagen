using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Skyrim
{
    public static class Constants
    {
        public static readonly ModKey Skyrim = new ModKey("Skyrim", type: ModType.Master);
        public static readonly FormKey Player = new FormKey(Skyrim, id: 0x14);
    }
}
