namespace Mutagen.Bethesda.Starfield;

public partial class HeadPart
{
    // ToDo
    // Copy paste risk
    [Flags]
    public enum Flag
    {
        Playable = 0x01,
        Male = 0x02,
        Female = 0x04,
        IsExtraPart = 0x08,
        UseSolidTint = 0x10,
        UsesBodyTexture = 0x40
    }

    // ToDo
    // Copy paste risk
    [Flags]
    public enum MajorFlag
    {
        NonPlayable = 0x0000_0004,
    }

    // ToDo
    // Copy paste risk
    public enum TypeEnum
    {
        Misc,
        Face,
        Eyes,
        Hair,
        FacialHair,
        Scars,
        Eyebrows,
        Meatcaps,
        Teeth,
        HeadRear
    }
}