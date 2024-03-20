namespace Mutagen.Bethesda.Starfield;

public partial class LeveledItem
{
    [Flags]
    public enum Flag
    {
        CalculateFromAllLevelsLessThanOrEqualPlayer = 0x01,
        CalculateForEachItemInCount = 0x02,
        UseAll = 0x04
    }
}