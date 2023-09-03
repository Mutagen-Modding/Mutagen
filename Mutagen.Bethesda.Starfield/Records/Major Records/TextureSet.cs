namespace Mutagen.Bethesda.Starfield;

public partial class TextureSet
{
    // ToDo
    // Copy Paste Risk
    [Flags]
    public enum Flag
    {
        NoSpecularMap = 0x01,
        FaceGenTextures = 0x02,
        HasModelSpaceNormalMap = 0x04,
    }
}