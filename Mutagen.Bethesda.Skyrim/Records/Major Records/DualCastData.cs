using System;

namespace Mutagen.Bethesda.Skyrim;

public partial class DualCastData
{
    [Flags]
    public enum InheritScaleType
    {
        HitEffectArt,
        Projectile,
        Explosion,
    }
}