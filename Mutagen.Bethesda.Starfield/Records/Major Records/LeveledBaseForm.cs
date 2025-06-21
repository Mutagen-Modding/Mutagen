namespace Mutagen.Bethesda.Starfield;

public partial class LeveledBaseForm
{
    [Flags]
    public enum MajorFlag
    {
        CalculateAll = 0x8000
    }
    
    [Flags]
    public enum Flag
    {
        CalculateFromAllLevelsLessThanOrEqualPlayer = 0x01,
        CalculateForEachItemInCount = 0x02,
        CalculateAll = 0x04,
        DisableIfDoesNotResolve1 = 0x08,
        DisableIfDoesNotResolve2 = 0x10,
        AllowShiftUp = 0x40,
        DoAllBeforeRepeating = 0x100,
        ContainsOnlySpaceshipBaseForms = 0x200,
    }
}