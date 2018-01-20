using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class CreatureSound
    {
        public enum CreatureSoundType
        {
            LeftFoot = 0,
            RightFoot = 1,
            LeftBackFoot = 2,
            RightBackFoot = 3,
            Idle = 4,
            Aware = 5,
            Attack = 6,
            Hit = 7,
            Death = 8,
            Weapon = 9
        }
    }
}
