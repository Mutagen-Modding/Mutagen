using System;

namespace Mutagen.Bethesda.Skyrim;

public partial class Ammunition
{
    [Flags]
    public enum MajorFlag
    {
        NonPlayable = 0x4
    }

    [Flags]
    public enum Flag
    {
        IgnoresNormalWeaponResistance = 0x01,
        NonPlayable = 0x02,
        NonBolt = 0x04,
    }
}