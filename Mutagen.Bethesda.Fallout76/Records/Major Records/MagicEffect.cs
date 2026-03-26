using Mutagen.Bethesda.Plugins;
using System;

namespace Mutagen.Bethesda.Fallout76;

public partial class MagicEffect
{
    [Flags]
    public enum MajorFlag
    {
    }

    [Flags]
    public enum Flag
    {
        Hostile = 0x0000_0001,
        Recover = 0x0000_0002,
        Detrimental = 0x0000_0004,
        SnapToNavmesh = 0x0000_0008,
        NoHitEvent = 0x0000_0010,
        DispelWithKeywords = 0x0000_0100,
        NoDuration = 0x0000_0200,
        NoMagnitude = 0x0000_0400,
        NoArea = 0x0000_0800,
        FXPersist = 0x0000_1000,
        GoryVisuals = 0x0000_4000,
        HideInUI = 0x0000_8000,
        NoRecast = 0x0002_0000,
        PowerAffectsMagnitude = 0x0020_0000,
        PowerAffectsDuration = 0x0040_0000,
        Painless = 0x0400_0000,
        NoHitEffect = 0x0800_0000,
        NoDeathDispel = 0x1000_0000,
    }

    public enum SoundType
    {
        SheathDraw = 0,
        Charge = 1,
        Ready = 2,
        Release = 3,
        ConcentrationCastLoop = 4,
        OnHit = 5,
    }
}

partial class MagicEffectBinaryOverlay
{
}
