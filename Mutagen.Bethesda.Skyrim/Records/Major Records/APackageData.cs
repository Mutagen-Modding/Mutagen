using Mutagen.Bethesda.Binary;
using Noggog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class APackageData
    {
        [Flags]
        public enum Flag
        {
            Public = 0x01
        }
    }

    namespace Internals
    {
        public partial class APackageDataBinaryOverlay
        {
            public string? Name => throw new NotImplementedException();

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            string INamedRequiredGetter.Name => this.Name ?? string.Empty;

            public APackageData.Flag? Flags => throw new NotImplementedException();
        }
    }
}
