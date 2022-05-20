using System;

namespace Mutagen.Bethesda.Fallout4;

public partial class QuestObjective
{
    [Flags]
    public enum Flag
    {
        OrWithPrevious = 0x1,
        NoStatsTracking = 0x2,
    }
}