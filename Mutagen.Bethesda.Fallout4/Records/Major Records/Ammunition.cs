using System;

namespace Mutagen.Bethesda.Fallout4
{
    public partial class Ammunition
    {
        [Flags]
        public enum MajorFlag
        {
            NonPlayable = 0x0000_0004,
        }

        [Flags]
        public enum Flag
        {
            IgnoresNormalWeaponResistence = 0x01,
            NonPlayable = 0x02,
            HasCountBased3d = 0x04
        }
    }
}
