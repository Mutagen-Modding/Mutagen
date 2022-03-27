using System;

namespace Mutagen.Bethesda.Fallout4
{
    public partial class Armor
    {
        [Flags]
        public enum MajorFlag
        {
            NonPlayable = 0x0000_0004,
            Shield = 0x0000_0040
        }

        public enum Property
        {
            Enchantments,
            BashImpactDataSet,
            BlockMaterial,
            Keywords,
            Weight,
            Value,
            Rating,
            AddonIndex,
            BodyPart,
            DamageTypeValue,
            ActorValues,
            Health,
            ColorRemappingIndex,
            MaterialSwaps
        }
    }
}
