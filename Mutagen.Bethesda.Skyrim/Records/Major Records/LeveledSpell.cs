namespace Mutagen.Bethesda.Skyrim;

public partial class LeveledSpell
{
    public enum Flag
    {
        CalculateFromAllLevelsLessThanOrEqualPlayer = 0x01,
        CalculateForEachItemInCount = 0x02,
        UseAllSpells = 0x04,
    }
}