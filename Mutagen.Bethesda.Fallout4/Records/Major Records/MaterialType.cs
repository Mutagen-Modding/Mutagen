using System;

namespace Mutagen.Bethesda.Fallout4;

public partial class MaterialType
{
    [Flags]
    public enum Flag
    {
        StairMaterial = 0x01,
        ArrowsStick = 0x02,
        CanTunnel = 0x04,
    }
}