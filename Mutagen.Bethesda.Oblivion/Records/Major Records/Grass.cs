namespace Mutagen.Bethesda.Oblivion;

public partial class Grass
{
    public enum UnitFromWaterType
    {
        AboveAtLeast = 0,
        AboveAtMost = 1,
        BelowAtLeast = 2,
        BelowAtMost = 3,
        EitherAtLeast = 4,
        EitherAtMost = 5,
        EitherAtMostAbove = 6,
        EitherAtMostBelow = 7,
    }
        
    [Flags]
    public enum GrassFlag
    {
        VertexLighting = 0x01,
        UniformScaling = 0x02,
        FitToSlope = 0x04
    }
}