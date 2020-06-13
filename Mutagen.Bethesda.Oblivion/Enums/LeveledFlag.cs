using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Oblivion
{
    [Flags]
    public enum LeveledFlag
    {
        CalculateFromAllLevelsLessThanPlayers = 0x01,
        CalculateForEachItemInCount = 0x02,
        UseAll = 0x04
    }
}
