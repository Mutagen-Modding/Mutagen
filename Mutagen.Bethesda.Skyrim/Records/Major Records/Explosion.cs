using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class Explosion
    {
        #region Interfaces
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IObjectBoundsGetter IObjectBoundedGetter.ObjectBounds => this.ObjectBounds;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        ObjectBounds? IObjectBoundedOptional.ObjectBounds
        {
            get => this.ObjectBounds;
            set => this.ObjectBounds = value ?? new ObjectBounds();
        }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IObjectBoundsGetter? IObjectBoundedOptionalGetter.ObjectBounds => this.ObjectBounds;
        #endregion

        [Flags]
        public enum Flag
        {
            AlwaysUsesWorldOrientation = 0x002,
            KnockDownAlways = 0x004,
            KnockDownByFormula = 0x008,
            IgnoreLosCheck = 0x010,
            PushExplosionSourceRefOnly = 0x020,
            IgnoreImageSpaceSwap = 0x040,
            Chain = 0x080,
            NoControllerVibration = 0x100,
        }
    }
}
