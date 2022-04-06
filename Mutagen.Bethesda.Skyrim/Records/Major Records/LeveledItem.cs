namespace Mutagen.Bethesda.Skyrim;

public partial class LeveledItem
{
    public enum Flag
    {
        CalculateFromAllLevelsLessThanOrEqualPlayer = 0x01,
        CalculateForEachItemInCount = 0x02,
        UseAll = 0x04,
        SpecialLoot = 0x08,
    }
}