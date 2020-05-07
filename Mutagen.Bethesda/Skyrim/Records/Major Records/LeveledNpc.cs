using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class LeveledNpc
    {
        public enum Flag
        {
            CalculateFromAllLevelsLessThanOrEqualPlayer = 0x01,
            CalculateForEachItemInCount = 0x02
        }
    }
}
