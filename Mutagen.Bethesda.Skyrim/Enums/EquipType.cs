namespace Mutagen.Bethesda.Skyrim;

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
    Spell = 0x00200,
    Shield = 0x00400,
    Torch = 0x00800,
    Crossbow = 0x01000,
}