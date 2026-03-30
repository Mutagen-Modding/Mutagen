namespace Mutagen.Bethesda.Fallout3;

public partial class Race
{
    [Flags]
    public enum Flag
    {
        Playable = 0x01,
        Child = 0x04,
    }

    public enum HairColor
    {
        Bleached = 0,
        Brown = 1,
        Chocolate = 2,
        Platinum = 3,
        Cornsilk = 4,
        Suede = 5,
        Pecan = 6,
        Auburn = 7,
        Ginger = 8,
        Honey = 9,
        Gold = 10,
        Rosewood = 11,
        Black = 12,
        Chestnut = 13,
        Steel = 14,
        Champagne = 15
    }

    public enum HeadPart
    {
        Head = 0,
        Ears = 1,
        Mouth = 2,
        TeethLower = 3,
        TeethUpper = 4,
        Tongue = 5,
        LeftEye = 6,
        RightEye = 7
    }

    public enum BodyPart
    {
        UpperBody = 0,
        LeftHand = 1,
        RightHand = 2,
        UpperBodyTexture = 3,
    }
}