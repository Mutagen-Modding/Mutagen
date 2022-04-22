using System;

namespace Mutagen.Bethesda.Oblivion;

public partial class RegionSound
{
    [Flags]
    public enum Flag
    {
        Pleasant = 0x01,
        Cloudy = 0x02,
        Rainy = 0x04,
        Snowy = 0x08,
    }
}