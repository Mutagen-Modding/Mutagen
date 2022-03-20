using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mutagen.Bethesda.Fallout4
{
    public partial class LeveledNpc
    {
        public enum Flag
        {
            CalculateFromAllLevelsLessThanOrEqualPlayer = 0x01,
            CalculateForEachItemInCount = 0x02,
            CalculateAll = 0x04
        }
    }
}
