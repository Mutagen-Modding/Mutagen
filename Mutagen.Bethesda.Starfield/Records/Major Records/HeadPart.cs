namespace Mutagen.Bethesda.Starfield;

public partial class HeadPart
{
    [Flags]
    public enum Flag
    {
        Playable = 0x01,
        Male = 0x02,
        Female = 0x04,
        IsExtraPart = 0x08,
        UseSolidTint = 0x10,
        UsesBodyTexture = 0x40,
        HideWithHideEarMorph = 0x80,
    }

    [Flags]
    public enum MajorFlag
    {
        NonPlayable = 0x0000_0004,
    }

    public enum TypeEnum
    {
        Misc = 0,
        Face = 1,
        RightEye = 2,
        Hair = 3,
        FacialHair = 4,
        Scar = 5,
        Eyebrows = 6,
        Jewelry = 7,
        Meatcaps = 8,
        Teeth = 9,
        HeadRear = 10,
        ExtraHair = 11,
        LeftEye = 12,
        Eyelashes = 13,
        CreatureHead = 14,
        CreatureTorso = 15,
        CreatureArms = 16,
        CreatureLegs = 17,
        CreatureTail = 18,
        CreatureWings = 19,
    }
}