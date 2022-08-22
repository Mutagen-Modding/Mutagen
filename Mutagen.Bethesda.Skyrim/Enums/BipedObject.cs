namespace Mutagen.Bethesda.Skyrim;

[Flags]
public enum BipedObjectFlag : uint
{
    Head = 0x0000_0001,
    Hair = 0x0000_0002,
    Body = 0x0000_0004,
    Hands = 0x0000_0008,
    Forearms = 0x0000_0010,
    Amulet = 0x0000_0020,
    Ring = 0x0000_0040,
    Feet = 0x0000_0080,
    Calves = 0x0000_0100,
    Shield = 0x0000_0200,
    Tail = 0x0000_0400,
    LongHair = 0x0000_0800,
    Circlet = 0x0000_1000,
    Ears = 0x0000_2000,
    BodyAddOn3 = 0x0000_4000,
    BodyAddOn4 = 0x0000_8000,
    BodyAddOn5 = 0x000_10000,
    BodyAddOn6 = 0x000_20000,
    BodyAddOn7 = 0x000_40000,
    BodyAddOn8 = 0x000_80000,
    DecapitateHead = 0x0010_0000,
    Decapitate = 0x0020_0000,
    BodyAddOn9 = 0x0040_0000,
    BodyAddOn10 = 0x0080_0000,
    BodyAddOn11 = 0x0100_0000,
    BodyAddOn12 = 0x0200_0000,
    BodyAddOn13 = 0x0400_0000,
    BodyAddOn14 = 0x0800_0000,
    BodyAddOn15 = 0x1000_0000,
    BodyAddOn16 = 0x2000_0000,
    BodyAddOn17 = 0x4000_0000,
    FX01 = 0x8000_0000,
}

public enum BipedObject
{
    None = -1,
    Head = 0,
    Hair = 1,
    Body = 2,
    Hands = 3,
    Forearms = 4,
    Amulet = 5,
    Ring = 6,
    Feet = 7,
    Calves = 8,
    Shield = 9,
    Tail = 10,
    LongHair = 11,
    Circlet = 12,
    Ears = 13,
    DecapitateHead = 20,
    Decapitate = 21,
    FX01 = 31,
}

// ToDo
// Write conversions between enums