using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class Hazard
    {
        #region Interfaces
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IObjectBoundsGetter IObjectBoundedGetter.ObjectBounds => this.ObjectBounds;
        #endregion

        [Flags]
        public enum Flag
        {
            AffectsPlayerOnly = 0x01,
            InheritDurationFromSpawnSpell = 0x02,
            AlignToImpactNormal = 0x04,
            InheritRadiusFromSpawnSpell = 0x08,
            DropToGround = 0x10
        }
    }
}
