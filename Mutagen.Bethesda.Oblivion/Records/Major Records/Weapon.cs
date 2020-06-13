using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class Weapon
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        String INamedRequiredGetter.Name => this.Name ?? string.Empty;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        String INamedRequired.Name
        {
            get => this.Name ?? string.Empty;
            set => this.Name = value;
        }

        public enum WeaponType
        {
            BladeOneHand,
            BladeTwoHand,
            BluntOneHand,
            BluntTwoHand,
            Bow,
            Staff,
        }

        [Flags]
        public enum WeaponFlag
        {
            IgnoresNormalWeaponResistance = 0x01
        }
    }

    namespace Internals
    {
        public partial class WeaponBinaryOverlay
        {
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            String INamedRequiredGetter.Name => this.Name ?? string.Empty;
        }
    }
}
