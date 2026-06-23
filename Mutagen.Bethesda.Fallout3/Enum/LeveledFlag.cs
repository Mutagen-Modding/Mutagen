using System;

namespace Mutagen.Bethesda.Fallout3;

[Flags]
public enum LeveledFlag
{
    CalculateFromAllLevelsLessThanOrEqualPlayer = 0x01,
    CalculateForEachItemInCount = 0x02,
    UseAll = 0x04,

}
