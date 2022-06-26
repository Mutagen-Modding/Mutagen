namespace Mutagen.Bethesda.Fallout4;

public partial class BodyTemplate
{
    [Flags]
    public enum Flag : uint
    {
        HairTop = 0x1,
        HairLong = 0x2,
        FaceGenHead = 0x4,
        BODY = 0x8,
        LeftHand = 0x10,
        RightHand = 0x20,
        UTorso = 0x40,
        ULeftArm = 0x80,
        URightArm = 0x100,
        ULeftLeg = 0x200,
        URightLeg = 0x400,
        ATorso = 0x800,
        ALeftArm = 0x1000,
        ARightArm = 0x2000,
        ALeftLeg = 0x4000,
        ARightLeg = 0x8000,
        Headband = 0x10000,
        Eyes = 0x20000,
        Beard = 0x40000,
        Mouth = 0x80000,
        Neck = 0x100000,
        Ring = 0x200000,
        Scalp = 0x400000,
        Decapitation = 0x800000,
        Shield = 0x20000000,
        Pipboy = 0x40000000,
        FX = 0x80000000,
    }
}
