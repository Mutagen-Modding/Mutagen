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
}
