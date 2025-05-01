namespace Mutagen.Bethesda.Starfield;

public partial class Grass
{
    [Flags]
    public enum Flag
    {
        VertexLighting = 0x01,
        UniformScaling = 0x02,
        FitsSlope = 0x04,
        ApplyBetween = 0x08,
    }
}