using System;

namespace Mutagen.Bethesda.Skyrim;

public partial class RegionSound
{
    [Flags]
    public enum Flag
    {
        Pleasent = 0x01,
        Cloudy = 0x02,
        Rainy = 0x04,
        Snowy = 0x08,
    }
}