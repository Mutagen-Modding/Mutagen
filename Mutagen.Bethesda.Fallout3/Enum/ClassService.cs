namespace Mutagen.Bethesda.Fallout3;

[Flags]
public enum ClassService
{
    Weapons = 0x0000_0001,
    Armor = 0x0000_0002,
    Alcohol = 0x0000_0004,
    Books = 0x0000_0008,
    Food = 0x0000_0010,
    Chems = 0x0000_0020,
    Stimpaks = 0x0000_0040,
    Miscellaneous = 0x0000_0400,
    Training = 0x0000_4000,
    Recharge = 0x0001_0000,
    Repair = 0x0002_0000,
}