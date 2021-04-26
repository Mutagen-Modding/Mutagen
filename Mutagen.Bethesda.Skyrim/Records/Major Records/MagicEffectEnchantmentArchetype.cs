using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class MagicEffectEnchantmentArchetype
    {
        public FormLink<IObjectEffect> Association => new FormLink<IObjectEffect>(this.AssociationKey);

        public MagicEffectEnchantmentArchetype()
            : base(TypeEnum.EnhanceWeapon)
        {
        }
    }
}
