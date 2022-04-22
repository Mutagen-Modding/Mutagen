using System;

namespace Mutagen.Bethesda.Skyrim;

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
        IkDataHeadTracking = 0x20,
        ToHitChanceAbsolute = 0x40,
    }

    public enum PartType
    {
        Torso,
        Head,
        Eye,
        LookAt,
        FlyGrab,
        Saddle,
    }
}