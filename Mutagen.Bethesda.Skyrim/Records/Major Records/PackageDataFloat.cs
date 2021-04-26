using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Mutagen.Bethesda.Plugins.Aspects;

namespace Mutagen.Bethesda.Skyrim
{
    namespace Internals
    {
        public partial class PackageDataFloatBinaryOverlay
        {
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            string INamedRequiredGetter.Name => this.Name ?? string.Empty;

            public float Data => throw new NotImplementedException();
        }
    }
}
