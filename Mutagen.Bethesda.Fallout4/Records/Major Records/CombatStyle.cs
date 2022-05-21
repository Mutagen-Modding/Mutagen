namespace Mutagen.Bethesda.Fallout4;

partial class CombatStyle
{
    [Flags]
    public enum MajorFlag
    {
        AllowDualWeilding = 0x0008_0000
    }

    [Flags]
    public enum Flag
    {
        Dueling = 0x01,
        Flanking = 0x02,
        AllowDualWeilding = 0x04,
        Charging = 0x08,
        RetargetAnyNearbyMeleeTarget = 0x10,
    }
}
