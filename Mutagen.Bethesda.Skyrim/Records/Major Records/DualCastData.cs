using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

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