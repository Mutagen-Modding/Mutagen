namespace Mutagen.Bethesda.Fallout3;

public partial class LeveledCreature
{
    [Flags]
    public enum Flag
    {
        CalculateFromAllLevelsLessThanOrEqualPlayer = 0x01,
        CalculateForEachItemInCount = 0x02,
    }
}
