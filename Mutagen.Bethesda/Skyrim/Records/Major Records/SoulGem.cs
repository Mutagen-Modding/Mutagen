using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class SoulGem
    {
        #region Interfaces
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IObjectBoundsGetter? IObjectBoundedOptionalGetter.ObjectBounds => this.ObjectBounds;
        #endregion

        [Flags]
        public enum MajorFlag
        {
            CanHoldNpcSoul = 0x0002_0000
        }

        public enum Level
        {
            None,
            Petty,
            Lesser,
            Common,
            Greater,
            Grand
        }
    }
}
