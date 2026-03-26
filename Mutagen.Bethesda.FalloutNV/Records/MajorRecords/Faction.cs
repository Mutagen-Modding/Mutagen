namespace Mutagen.Bethesda.FalloutNV;

public partial class Faction
{
    [Flags]
    public enum FactionFlag
    {
        HiddenFromPlayer = 0x0001,
        Evil = 0x0002,
        SpecialCombat = 0x0004,
        TrackCrime = 0x0010,
        AllowSell = 0x0020,
    }
}