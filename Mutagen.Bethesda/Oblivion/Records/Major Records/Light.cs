using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class Light
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
        public enum LightFlag
        {
            Dynamic = 0x001,
            CanBeCarried = 0x002,
            Negative = 0x004,
            Flicker = 0x008,
            OffByDefault = 0x020,
            FlickerSlow = 0x040,
            Pulse = 0x080,
            PulseSlow = 0x100,
            SpotLight = 0x200,
            SpotShadow = 0x400,
        }
    }

    namespace Internals
    {
        public partial class LightBinaryOverlay
        {
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            String INamedRequiredGetter.Name => this.Name ?? string.Empty;
        }
    }
}
