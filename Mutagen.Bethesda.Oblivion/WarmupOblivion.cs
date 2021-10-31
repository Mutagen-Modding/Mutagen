using System;
using System.Collections.Generic;
using System.Text;
using Mutagen.Bethesda.Plugins.Cache.Internals;

namespace Mutagen.Bethesda.Oblivion
{
    public static partial class WarmupOblivion
    {
        static partial void InitCustom()
        {
            OverrideMaskRegistrations.Warmup();
        }
    }
}
