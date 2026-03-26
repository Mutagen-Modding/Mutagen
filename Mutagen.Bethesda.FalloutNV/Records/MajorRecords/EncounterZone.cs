namespace Mutagen.Bethesda.FalloutNV;

public partial class EncounterZone
{
    [Flags]
    public enum Flag
    {
        NeverResets = 0x01,
        MatchPcBelowMinimumLevel = 0x02,
    }
}
