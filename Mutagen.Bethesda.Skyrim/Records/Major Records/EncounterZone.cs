using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim;

public partial class EncounterZone
{
    [Flags]
    public enum Flag
    {
        NeverResets = 0x01,
        MatchPcBelowMinimumLevel = 0x02,
        DisableCombatBoundary = 0x04,
    }
}