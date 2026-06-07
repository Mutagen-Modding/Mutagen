namespace Mutagen.Bethesda.Fallout3;

public partial class BodyPart
{
    [Flags]
    public enum Flag
    {
        Severable = 0x01,
        IkData = 0x02,
        IkDataBipedData = 0x04,
        Explodable = 0x08,
        IkDataIsHead = 0x10,
        IkDataHeadtracking = 0x20,
        ToHitChanceAbsolute = 0x40,
    }

    public enum PartType
    {
        None = -1,
        Torso = 0,
        Head1 = 1,
        Head2 = 2,
        LeftArm1 = 3,
        LeftArm2 = 4,
        RightArm1 = 5,
        RightArm2 = 6,
        LeftLeg1 = 7,
        LeftLeg2 = 8,
        LeftLeg3 = 9,
        RightLeg1 = 10,
        RightLeg2 = 11,
        RightLeg3 = 12,
        Brain = 13,
        Weapon = 14,
    }
}
