namespace Mutagen.Bethesda.Oblivion;

public partial class Race
{
    [Flags]
    public enum Flag
    {
        Playable = 1,
    }

    public enum FaceIndex
    {
        Head = 0,
        EarMale = 1,
        EarFemale = 2,
        Mouth = 3,
        TeethLower = 4,
        TeethUpper = 5,
        Tongue = 6,
        EyeLeft = 7,
        EyeRight = 8,
    }

    public enum BodyIndex
    {
        UpperBody = 0,
        LowerBody = 1,
        Hand = 2,
        Foot = 3,
        Tail = 4,
    }
}