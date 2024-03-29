namespace Mutagen.Bethesda.Fallout4;

public partial class Grass
{
    public enum UnitsFromWaterTypeEnum
    {
        AboveAtLeast,
        AboveAtMost,
        BelowAtLeast,
        BelowAtMost,
        EitherAtLeast,
        EitherAtMost,
        EitherAtMostAbove,
        EitherAtMostBelow,
    }

    [Flags]
    public enum Flag
    {
        VertexLighting = 0x01,
        UniformScaling = 0x02,
        FitToSlope = 0x04,
    }
}