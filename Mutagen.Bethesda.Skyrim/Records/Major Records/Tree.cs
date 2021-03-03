using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class Tree
    {
        IFormLinkNullableGetter<IHarvestTargetGetter> IHarvestableGetter.Ingredient => this.Ingredient;
        IFormLinkNullableGetter<ISoundDescriptorGetter> IHarvestableGetter.HarvestSound => this.HarvestSound;

        [Flags]
        public enum MajorFlag
        {
            HasDistantLOD = 0x0000_8000
        }
    }
}
