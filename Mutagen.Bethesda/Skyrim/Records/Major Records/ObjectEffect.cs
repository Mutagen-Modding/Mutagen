using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class ObjectEffect
    {
        [Flags]
        public enum Flag
        {
            NoAutoCalc = 0x01,
            ExtendDurationOnRecast = 0x04
        }

        public enum EnchantType
        {
            Enchantment = 0x06,
            StaffEnchantment = 0x0C
        }
    }
}
