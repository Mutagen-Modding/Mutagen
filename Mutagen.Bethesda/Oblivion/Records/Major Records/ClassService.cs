using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Oblivion
{
    [Flags]
    public enum ClassService
    {
        Weapons = 0x1,
        Armor = 0x2,
        Clothing = 0x4,
        Books = 0x8,
        Ingredients = 0x10,
        Lights = 0x80,
        Apparatus = 0x100,
        Miscellaneous = 0x400,
        Spells = 0x800,
        MagicItems = 0x1000,
        Potions = 0x2000,
        Training = 0x4000,
        Recharge = 0x10000,
        Repair = 0x20000
    }
}
