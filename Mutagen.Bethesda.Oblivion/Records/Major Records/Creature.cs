using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class Creature
    {
        [Flags]
        public enum CreatureFlag
        {
            Biped = 0x000001,
            Essential = 0x000002,
            WeaponAndShield = 0x000004,
            Respawn = 0x000008,
            Swims = 0x000010,
            Flies = 0x000020,
            Walks = 0x000040,
            PCLevelOffset = 0x000080,
            NoLowLevelProcessing = 0x000200,
            NoBloodSpray = 0x000800,
            NoBloodDecal = 0x001000,
            NoHead = 0x008000,
            NoRightArm = 0x010000,
            NoLeftArm = 0x020000,
            NoCombatInWater = 0x040000,
            NoShadow = 0x080000,
            NoCorpseCheck = 0x100000
        }

        public enum CreatureTypeEnum
        {
            Creature = 0,
            Daedra = 1,
            Undead = 2,
            Humanoid = 3,
            Horse = 4,
            Giant = 5,
        }
    }
}
