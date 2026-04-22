namespace Mutagen.Bethesda.Fallout3;

public partial class RegionObject
{
    [Flags]
    public enum Flag
    {
        ConformToSlope = 0x01,
        PaintVertices = 0x02,
        SizeVariancePlusMinus = 0x04,
        XPlusMinus = 0x08,
        YPlusMinus = 0x10,
        ZPlusMinus = 0x20,
        Tree = 0x40,
        HugeRock = 0x80,
    }
}
