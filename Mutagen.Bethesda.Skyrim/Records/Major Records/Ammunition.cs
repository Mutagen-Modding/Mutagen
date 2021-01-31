using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class Ammunition
    {
        #region Interfaces
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IIconsGetter? IHasIconsGetter.Icons => this.Icons;
        #endregion

        [Flags]
        public enum MajorFlag
        {
            NonPlayable = 0x4
        }

        [Flags]
        public enum Flag
        {
            IgnoresNormalWeaponResistance = 0x01,
            NonPlayable = 0x02,
            NonBolt = 0x04,
        }
    }
}
