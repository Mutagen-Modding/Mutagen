namespace Mutagen.Bethesda.Fallout4;

[Flags]
public enum EquipTypeFlag
{
    HandToHand = 0x00001,
    OneHandSword = 0x00002,
    OneHandDagger = 0x00004,
    OneHandAxe = 0x00008,
    OneHandMace = 0x00010,
    TwoHandSword = 0x00020,
    TwoHandAxe = 0x00040,
    Bow = 0x00080,
    Staff = 0x00100,
    Gun = 0x00200,
    Grenade = 0x00400,
    Mine = 0x00800,
    Spell = 0x01000,
    Shield = 0x02000,
    Torch = 0x04000,
}