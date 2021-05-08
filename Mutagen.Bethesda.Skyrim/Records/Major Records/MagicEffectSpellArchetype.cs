using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class MagicEffectSpellArchetype
    {
        public FormLink<ISpellGetter> Association => this.AssociationKey.AsLink<ISpellGetter>();

        IFormLink<ISpellGetter> IMagicEffectSpellArchetype.Association => this.Association;
        IFormLinkGetter<ISpellGetter> IMagicEffectSpellArchetypeGetter.Association => this.Association;

        public MagicEffectSpellArchetype()
            : base(TypeEnum.Cloak)
        {
        }
    }

    public partial interface IMagicEffectSpellArchetype
    {
        new IFormLink<ISpellGetter> Association { get; }
    }

    public partial interface IMagicEffectSpellArchetypeGetter
    {
        IFormLinkGetter<ISpellGetter> Association { get; }
    }

    namespace Internals
    {
        public partial class MagicEffectSpellArchetypeBinaryOverlay
        {
            public IFormLinkGetter<ISpellGetter> Association => this.AssociationKey.AsLink<ISpellGetter>();
        }
    }
}
