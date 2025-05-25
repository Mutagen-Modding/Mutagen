namespace Mutagen.Bethesda.Starfield;

public partial class LocationOwnerData
{
    [Flags]
    public enum EncounterFlag
    {
        NeverResets = 0x0001,
        MatchPcLevel = 0x0002,
        DisableCombatBoundry = 0x0004,
        Workshop = 0x0008,
        UseParentsLockedLevel = 0x0010,
    }
}