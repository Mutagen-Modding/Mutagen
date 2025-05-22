namespace Mutagen.Bethesda.Fallout3;

[Flags]
public enum ClassService
{
    Weapons = 0x1,
    Armor = 0x2,
    Alcohol = 0x4,
    Books = 0x8,
    Food = 0x10,
    Chems = 0x02,
    Stimpacks = 0x04,
    Miscellaneous = 0x400,
    Training = 0x4000,
    Recharge = 0x10000,
    Repair = 0x20000
}