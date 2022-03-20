using System;
using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Fallout4
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
