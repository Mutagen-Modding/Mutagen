using System;

namespace Mutagen.Bethesda.Fallout3;

[Flags]
public enum LeveledFlag : byte
{
    CalculateFromAllLevelsLessThanOrEqualPlayer = 0x01,
    CalculateForEachItemInCount = 0x02,
}
