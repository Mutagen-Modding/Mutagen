using System;

namespace Mutagen.Bethesda.Fallout4;

public partial class Decal
{
    [Flags]
    public enum Flag
    {
        POMShadows = 0x01,
        AlphaBlending = 0x02,
        AlphaTesting = 0x04,
        NoSubtextures = 0x08,
        MultiplicativeBlending = 0x10
    }
}