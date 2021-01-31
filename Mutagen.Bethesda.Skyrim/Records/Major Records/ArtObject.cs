using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class ArtObject
    {
        #region Interfaces
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IModelGetter? IModeledGetter.Model => this.Model;
        #endregion

        [Flags]
        public enum TypeEnum
        {
            MagicCasting = 0x01,
            MagicHitEffect = 0x02,
            EnchantmentEffect = 0x04,
        }
    }
}
