using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Fallout4;

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