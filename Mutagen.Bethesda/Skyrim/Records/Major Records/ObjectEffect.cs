using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class ObjectEffect
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
        public enum Flag
        {
            NoAutoCalc = 0x01,
            ExtendDurationOnRecast = 0x04
        }

        public enum EnchantType
        {
            Enchantment = 0x06,
            StaffEnchantment = 0x0C
        }
    }

    namespace Internals
    {
        public partial class ObjectEffectBinaryOverlay
        {
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            String INamedRequiredGetter.Name => this.Name ?? string.Empty;
        }
    }
}
