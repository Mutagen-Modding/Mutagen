namespace Mutagen.Bethesda.Starfield;

public partial class BodyPart
{
    [Flags]
    public enum Flag
    {
        Severable = 0x01,
        HitReaction = 0x02,
        HitReactionDefault = 0x04,
        Explodable = 0x08,
        CutMeatCapSever = 0x10,
        OnCripple = 0x20,
        ExplodableAbsoluteChance = 0x40,
        ShowCrippleGeometry = 0x80,
    }

    public enum PartType
    {
        Torso = 0,
        Head1 = 1,
        Eye = 2,
        LookAt = 3,
        FlyGrab = 4,
        Head2 = 5,
        LeftArm1 = 6,
        LeftArm2 = 7,
        RightArm1 = 8,
        RightArm2 = 9,
        LeftLeg1 = 10,
        LeftLeg2 = 11,
        LeftLeg3 = 12,
        RightLeg1 = 13,
        RightLeg2 = 14,
        RightLeg3 = 15,
        Brain = 16,
        Weapon = 17,
        Root = 18,
        COM = 19,
        Pelvis = 20,
        Camera = 21,
        OffsetRoot = 22,
        LeftFoot = 23,
        RightFoot = 24,
        FaceTargetSource = 25,
    }
}
