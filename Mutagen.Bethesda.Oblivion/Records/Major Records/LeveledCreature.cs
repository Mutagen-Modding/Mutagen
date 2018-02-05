using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class LeveledCreature
    {
        [Flags]
        public enum Flag
        {
            CalculateFromAllLevelsLessThanPlayers,
            CalculateForEachItemInCount
        }
    }
}
