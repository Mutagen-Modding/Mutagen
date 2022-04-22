using System;

namespace Mutagen.Bethesda.Fallout4;

public partial class Light
{
    [Flags]
    public enum MajorFlag
    {
        RandomAnimStart = 0x0001_0000,
        Obstacle = 0x0200_0000,
        PortalStrict = 0x1000_0000
    }

    [Flags]
    public enum Flag
    {
        CanBeCarried = 0x0000_0002,
        Flicker = 0x0000_0008,
        OffByDefault = 0x0000_0020,
        Pulse = 0x0000_0080,
        ShadowSpotlight = 0x0000_0400,
        ShadowHemisphere = 0x0000_0800,
        ShadowOmnidirectional = 0x0000_1000,
        NonShadowSpotlight = 0x0000_4000,
        NonSpecular = 0x0000_8000,
        AttenuationOnly = 0x0001_0000,
        NonShadowBox = 0x0002_0000,
        IgnoreRoughness = 0x0004_0000,
        NoRimLighting = 0x0008_0000,
        AmbientOnly = 0x0010_0000
    }
}