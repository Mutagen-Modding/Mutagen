using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class MagicEffectKeywordArchetype
    {
        public FormLink<IKeywordGetter> Association => this.AssociationKey.AsLink<IKeywordGetter>();

        IFormLink<IKeywordGetter> IMagicEffectKeywordArchetype.Association => this.Association;
        IFormLinkGetter<IKeywordGetter> IMagicEffectKeywordArchetypeGetter.Association => this.Association;

        public MagicEffectKeywordArchetype()
            : base(TypeEnum.PeakValueModifier)
        {
        }
    }

    public partial interface IMagicEffectKeywordArchetype
    {
        new IFormLink<IKeywordGetter> Association { get; }
    }

    public partial interface IMagicEffectKeywordArchetypeGetter
    {
        IFormLinkGetter<IKeywordGetter> Association { get; }
    }

    namespace Internals
    {
        public partial class MagicEffectKeywordArchetypeBinaryOverlay
        {
            public IFormLinkGetter<IKeywordGetter> Association => this.AssociationKey.AsLink<IKeywordGetter>();
        }
    }
}
