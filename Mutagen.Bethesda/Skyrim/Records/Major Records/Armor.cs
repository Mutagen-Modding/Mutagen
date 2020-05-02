using Mutagen.Bethesda.Binary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class Armor
    {
        #region Interfaces
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        String INamedRequiredGetter.Name => this.Name ?? string.Empty;
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        String INamedRequired.Name
        {
            get => this.Name ?? string.Empty;
            set => this.Name = value;
        }
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        IObjectBoundsGetter IObjectBoundedGetter.ObjectBounds => this.ObjectBounds;
        #endregion

        [Flags]
        public enum MajorFlag
        {
            NonPlayable = 0x0000_0004,
            Shield = 0x0000_0040
        }
    }

    public partial interface IArmor
    {
        new public decimal? ArmorRating
        {
            get => ((IArmorGetter)this).ArmorRating;
            set => this.ArmorRatingRaw = value == null ? default(int?) : checked((int)Math.Round(value.Value * 100));
        }
    }

    public partial interface IArmorGetter
    {
        public decimal? ArmorRating => this.ArmorRatingRaw.HasValue ? this.ArmorRatingRaw.Value / 100m : default(decimal?);
    }

    namespace Internals
    {
        public partial class ArmorBinaryOverlay
        {
            #region Interfaces
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            String INamedRequiredGetter.Name => this.Name ?? string.Empty;
            #endregion
        }
    }
}
