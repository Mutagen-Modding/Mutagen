using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Oblivion
{
    public static class Constants
    {
        public static readonly ModKey Oblivion = new ModKey("Oblivion", master: true);
        public static readonly FormKey Player = new FormKey(Oblivion, id: 0x14);
    }
}
