using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    namespace Internals
    {
        public partial class PackageDataBoolBinaryOverlay
        {
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            string INamedRequiredGetter.Name => this.Name ?? string.Empty;

            public bool Data => throw new NotImplementedException();
        }
    }
}
