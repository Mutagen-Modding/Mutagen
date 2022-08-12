namespace Mutagen.Bethesda.Fallout4;

public partial class LeveledNpc
{
    [Flags]
    public enum Flag
    {
        CalculateFromAllLevelsLessThanOrEqualPlayer = 0x01,
        CalculateForEachItemInCount = 0x02,
        CalculateAll = 0x04
    }
}