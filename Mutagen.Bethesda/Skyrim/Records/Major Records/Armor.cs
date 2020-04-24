using Mutagen.Bethesda.Binary;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class Armor
    {
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
}
