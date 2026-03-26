using System;

namespace Mutagen.Bethesda.Fallout76;

partial class ArmorAddon
{
    [Flags]
    public enum MajorFlag
    {
        NoUnderarmorScaling = 0x0000_0040,
        HiResFirstPersonOnly = 0x4000_0000
    }
}
