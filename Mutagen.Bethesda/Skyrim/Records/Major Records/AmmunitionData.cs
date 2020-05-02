using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class AmmunitionData
    {
        [Flags]
        public enum Flag
        {
            IgnoresNormalWeaponResistance = 0x01,
            NonPlayable = 0x02,
            NonBolt = 0x04,
        }

        public float Weight
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }
    }

    namespace Internals
    {
        public partial class AmmunitionDataBinaryOverlay
        {
            public float Weight => throw new NotImplementedException();
        }
    }
}
