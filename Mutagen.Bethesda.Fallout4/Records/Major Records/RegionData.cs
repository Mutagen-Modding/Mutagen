using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mutagen.Bethesda.Fallout4;

public partial class RegionData
{
    [Flags]
    public enum RegionDataFlag
    {
        Override = 0x01
    }

    public enum RegionDataType
    {
        Object = 2,
        Weather = 3,
        Map = 4,
        Land = 5,
        Grass = 6,
        Sound = 7,
        Imposter = 8,
    }
}