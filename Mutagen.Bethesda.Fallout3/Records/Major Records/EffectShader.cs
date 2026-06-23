namespace Mutagen.Bethesda.Fallout3;

public partial class EffectShaderData
{
    [Flags]
    public enum Flag
    {
        NoMembraneShader = 0x0000_0001,
        NoParticleShader = 0x0000_0008,
        EdgeEffectInverse = 0x0000_0010,
        AffectSkinOnly = 0x0000_0020,
    }

    public enum BlendMode
    {
        Zero = 1,
        One = 2,
        SourceColor = 3,
        SourceInverseColor = 4,
        SourceAlpha = 5,
        SourceInvertedAlpha = 6,
        DestAlpha = 7,
        DestInvertedAlpha = 8,
        DestColor = 9,
        DestInverseColor = 10,
        SourceAlphaSat = 11,
    }

    public enum BlendOperation
    {
        Add = 1,
        Subtract = 2,
        ReverseSubtract = 3,
        Minimum = 4,
        Maximum = 5,
    }

    public enum ZTest
    {
        EqualTo = 3,
        Normal = 4,
        GreaterThan = 5,
        GreaterThanOrEqualTo = 7,
        AlwaysShow = 8,
    }
}
