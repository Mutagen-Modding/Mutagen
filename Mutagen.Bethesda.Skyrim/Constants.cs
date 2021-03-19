using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Skyrim
{
    public class Constants
    {
        public static readonly ModKey Skyrim = new ModKey("Skyrim", type: ModType.Master);
        public static readonly ModKey Update = new ModKey("Update", type: ModType.Master);
        public static readonly ModKey Dawnguard = new ModKey("Dawnguard", type: ModType.Master);
        public static readonly ModKey HearthFires = new ModKey("HearthFires", type: ModType.Master);
        public static readonly ModKey Dragonborn = new ModKey("Dragonborn", type: ModType.Master);
        public static readonly FormKey Player = new FormKey(Skyrim, id: 0x14);
    }
}
