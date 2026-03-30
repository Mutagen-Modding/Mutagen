namespace Mutagen.Bethesda.Fallout3;

public partial class Landscape
{
    [Flags]
    public enum Flag
    {
        HasVertexNormalsHeightMap = 0x001,
        HasVertexColors = 0x002,
        HasLayers = 0x004,
        Unknown4 = 0x008,
        AutoCalcNormals = 0x010,
        Ignored = 0x400,
    }
}
