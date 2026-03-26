using System;

namespace Mutagen.Bethesda.Fallout76;

partial class Region
{
    [Flags]
    public enum MajorFlag
    {
        BorderRegion = 0x40
    }

    public enum RegionType
    {
        Objects = 0,
        Weather = 1,
        Map = 2,
        Land = 3,
        Grass = 4,
        Sound = 5,
    }
}
