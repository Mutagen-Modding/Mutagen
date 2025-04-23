namespace Mutagen.Bethesda.Starfield;

public partial class EffectShader
{
    [Flags]
    public enum Flag
    {
        MembraneShaderOff = 0x0000_0001,
        EdgeEffectInverse = 0x0000_0010,
        AffectSkinOnly = 0x0000_0020,
        IgnoreBaseGeometryTextureAlpha = 0x0000_0200,
        UseAlphaSorting = 0x0000_0800,
        DitherShields = 0x0000_2000,
        UseBloodGeometry = 0x0100_0000,
    }
}