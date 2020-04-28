using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class MiscItem
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        String INamedRequiredGetter.Name => this.Name ?? string.Empty;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        String INamedRequired.Name
        {
            get => this.Name ?? string.Empty;
            set => this.Name = value;
        }

        [Flags]
        public enum MajorFlag
        {
            NonPlayable = 0x4
        }
    }

    namespace Internals
    {
        public partial class MiscItemBinaryOverlay
        {
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            String INamedRequiredGetter.Name => this.Name ?? string.Empty;
        }
    }
}
