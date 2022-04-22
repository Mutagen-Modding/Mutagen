using System;

namespace Mutagen.Bethesda.Oblivion;

public partial class Enchantment
{
    public enum EnchantmentType
    {
        Scroll = 0,
        Staff = 1,
        Weapon = 2,
        Apparel = 3
    }

    [Flags]
    public enum Flag
    {
        ManualEnchantCost = 1
    }
}