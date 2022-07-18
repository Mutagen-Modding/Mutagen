namespace Mutagen.Bethesda.Skyrim;

public partial class LeveledNpc
{
    [Flags]
    public enum Flag
    {
        CalculateFromAllLevelsLessThanOrEqualPlayer = 0x01,
        CalculateForEachItemInCount = 0x02
    }
}