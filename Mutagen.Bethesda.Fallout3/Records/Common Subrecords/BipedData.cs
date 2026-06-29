namespace Mutagen.Bethesda.Fallout3;

public partial class BipedData
{
    [Flags]
    public enum BipedFlag : uint
    {
        Head = 0x0000_0001,
        Hair = 0x0000_0002,
        UpperBody = 0x0000_0004,
        LeftHand = 0x0000_0008,
        RightHand = 0x0000_0010,
        Weapon = 0x0000_0020,
        PipBoy = 0x0000_0040,
        Backpack = 0x0000_0080,
        Necklace = 0x0000_0100,
        Headband = 0x0000_0200,
        Hat = 0x0000_0400,
        EyeGlasses = 0x0000_0800,
        NoseRing = 0x0000_1000,
        Earrings = 0x0000_2000,
        Mask = 0x0000_4000,
        Choker = 0x0000_8000,
        MouthObject = 0x0001_0000,
        BodyAddOn1 = 0x0002_0000,
        BodyAddOn2 = 0x0004_0000,
        BodyAddOn3 = 0x0008_0000,
    }

    [Flags]
    public enum GeneralFlag : uint
    {
        HasBackpack = 0x04, // FNV
        Medium = 0x08,      // FNV
        PowerArmor = 0x20,
        NonPlayable = 0x40,
        Heavy = 0x80,
    }
}
