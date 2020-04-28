using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class TalkingActivator
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
            HiddenFromLocalMap = 0x0000_0200,
            RandomAnimStart = 0x0001_0000,
            RadioStation = 0x0002_0000,
        }
    }

    namespace Internals
    {
        public partial class TalkingActivatorBinaryOverlay
        {
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            String INamedRequiredGetter.Name => this.Name ?? string.Empty;
        }
    }
}
