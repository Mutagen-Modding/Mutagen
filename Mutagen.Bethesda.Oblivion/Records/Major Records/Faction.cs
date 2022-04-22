using System;

namespace Mutagen.Bethesda.Oblivion;

public partial class Faction
{
    [Flags]
    public enum FactionFlag
    {
        HiddenFromPlayer = 0x01,
        Evil = 0x02,
        SpecialCombat = 0x04
    }
}