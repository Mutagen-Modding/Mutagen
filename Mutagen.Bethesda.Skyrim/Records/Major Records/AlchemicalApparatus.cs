using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class AlchemicalApparatus
    {
        #region Interfaces
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IIconsGetter? IHasIconsGetter.Icons => this.Icons;
        #endregion

        public enum QualityLevel
        {
            Novice = 0,
            Apprentice = 1,
            Journeyman = 2,
            Expert = 3,
            Master = 4
        }
    }
}
