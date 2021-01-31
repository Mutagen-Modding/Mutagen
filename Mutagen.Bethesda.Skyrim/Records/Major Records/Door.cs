using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class Door
    {
        #region Interfaces
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IModelGetter? IModeledGetter.Model => this.Model;
        #endregion

        [Flags]
        public enum MajorFlag
        {
            HasDistantLOD = 0x0000_8000,
            RandomAnimStart = 0x0001_0000,
            IsMarker = 0x0080_0000
        }

        [Flags]
        public enum Flag
        {
            Automatic = 0x02,
            Hidden = 0x04,
            MinimalUse = 0x08,
            Sliding = 0x10,
            DoNotOpenInCombatSearch = 0x20,
        }
    }
}
