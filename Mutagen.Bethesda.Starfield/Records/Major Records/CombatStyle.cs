namespace Mutagen.Bethesda.Starfield;

partial class CombatStyle
{
    [Flags]
    public enum MajorFlag
    {
        AllowDualWielding = 0x0008_0000
    }

    [Flags]
    public enum Flag
    {
        Dueling = 0x01,
        Flanking = 0x02,
        AllowDualWielding = 0x04,
        Charging = 0x08,
        RetargetAnyNearbyMeleeTarget = 0x10,
        PreferWalkingWhileStrafing = 0x20,
        UseGroupCohesion = 0x40,
        MaintainAttackFromCoverUntilSuppressed = 0x80,
    }
}