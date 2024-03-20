namespace Mutagen.Bethesda.Starfield;

public partial class QuestObjective
{
    [Flags]
    public enum Flag
    {
        OrWithPrevious = 0x1,
        NoStatsTracking = 0x2,
    }
}