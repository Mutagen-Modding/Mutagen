namespace Mutagen.Bethesda.Fallout76;

partial class EncounterZone
{
    [Flags]
    public enum MajorFlag
    {
    }

    [Flags]
    public enum Flag
    {
        NeverResets = 0x01,
        MatchPcBelowMinimumLevel = 0x02,
        DisableCombatBoundary = 0x04,
        Workshop = 0x08,
    }
}
