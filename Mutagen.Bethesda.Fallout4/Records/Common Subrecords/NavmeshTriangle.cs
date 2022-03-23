using System;

namespace Mutagen.Bethesda.Fallout4
{
    public partial class NavmeshTriangle
    {
        [Flags]
        public enum Flag
        {
            EdgeLink_0_1 = 0x0001,
            EdgeLink_1_2 = 0x0002,
            EdgeLink_2_0 = 0x0004,
            NoLargeCreatures = 0x0010,
            Overlapping = 0x0020,
            Preferred = 0x0040,
            Water = 0x0200,
            Door = 0x0400,
            Found = 0x0800,
        }
    }
}
