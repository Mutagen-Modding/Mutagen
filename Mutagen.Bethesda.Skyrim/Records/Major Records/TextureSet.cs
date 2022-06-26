namespace Mutagen.Bethesda.Skyrim;

public partial class TextureSet
{
    [Flags]
    public enum Flag
    {
        NoSpecularMap = 0x01,
        FaceGenTextures = 0x02,
        HasModelSpaceNormalMap = 0x04,
    }
}