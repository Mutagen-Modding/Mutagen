namespace Mutagen.Bethesda.Fallout4;

public partial class RegionDataObject
{
    [Flags]
    public enum Flag
    {
        ConformToSlope = 0x0001,
        PaintVerticees = 0x0002,
        SizeVariancePlusMinus = 0x0004,
        XPlusMinus = 0x0008,
        YPlusMinus = 0x0010,
        ZPlusMinus = 0x0020,
        Tree = 0x0040,
        HugeRock = 0x0080,
    }
}