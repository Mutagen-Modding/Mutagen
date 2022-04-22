using System;

namespace Mutagen.Bethesda.Oblivion;

public partial class EffectShader
{
    [Flags]
    public enum Flag
    {
        NoMembraneShader = 0x001,
        NoParticleShader = 0x008,
        EdgeEffectInverse = 0x010,
        MembraneShaderAffectSkinOnly = 0x020,
    }

    public enum SourceBlendMode
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
        SourceAlphaSAT = 11,
    }

    public enum BlendOperation
    {
        Add = 1,
        Subtract = 2,
        ReverseSubtract = 3,
        Minimum = 4,
        Maximum = 5,
    }

    public enum ZTestFunction
    {
        EqualTo = 3,
        Normal = 4,
        GreaterThan = 5,
        GreaterThanOrEqualTo = 7
    }
}