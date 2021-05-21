using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class MagicEffectEnchantmentArchetype
    {
        public FormLink<IObjectEffectGetter> Association => this.AssociationKey.AsLink<IObjectEffectGetter>();

        IFormLink<IObjectEffectGetter> IMagicEffectEnchantmentArchetype.Association => this.Association;
        IFormLinkGetter<IObjectEffectGetter> IMagicEffectEnchantmentArchetypeGetter.Association => this.Association;

        public MagicEffectEnchantmentArchetype()
            : base(TypeEnum.EnhanceWeapon)
        {
        }
    }

    public partial interface IMagicEffectEnchantmentArchetype
    {
        new IFormLink<IObjectEffectGetter> Association { get; }
    }

    public partial interface IMagicEffectEnchantmentArchetypeGetter
    {
        IFormLinkGetter<IObjectEffectGetter> Association { get; }
    }

    namespace Internals
    {
        public partial class MagicEffectEnchantmentArchetypeBinaryOverlay
        {
            public IFormLinkGetter<IObjectEffectGetter> Association => this.AssociationKey.AsLink<IObjectEffectGetter>();
        }
    }
}
