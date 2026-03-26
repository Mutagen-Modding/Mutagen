namespace Mutagen.Bethesda.Fallout76;

public partial class Grass
{
    [Flags]
    public enum MajorFlag
    {
    }

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