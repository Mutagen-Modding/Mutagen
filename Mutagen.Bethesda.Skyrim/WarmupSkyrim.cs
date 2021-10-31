using System;
using System.Collections.Generic;
using System.Text;
using Mutagen.Bethesda.Plugins.Cache.Internals;

namespace Mutagen.Bethesda.Skyrim
{
    public static partial class WarmupSkyrim
    {
        static partial void InitCustom()
        {
            OverrideMaskRegistrations.Warmup();
        }
    }
}
