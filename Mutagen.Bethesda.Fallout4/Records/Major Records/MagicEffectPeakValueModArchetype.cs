using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Fallout4
{
    public partial class MagicEffectPeakValueModArchetype
    {
        public FormLink<IKeywordGetter> Association => this.AssociationKey.AsLink<IKeywordGetter>();

        IFormLink<IKeywordGetter> IMagicEffectPeakValueModArchetype.Association => this.Association;
        IFormLinkGetter<IKeywordGetter> IMagicEffectPeakValueModArchetypeGetter.Association => this.Association;

        public MagicEffectPeakValueModArchetype()
            : base(TypeEnum.PeakValueModifier)
        {
        }
    }

    public partial interface IMagicEffectPeakValueModArchetype
    {
        new IFormLink<IKeywordGetter> Association { get; }
    }

    public partial interface IMagicEffectPeakValueModArchetypeGetter
    {
        IFormLinkGetter<IKeywordGetter> Association { get; }
    }

    namespace Internals
    {
        public partial class MagicEffectPeakValueModArchetypeBinaryOverlay
        {
            public IFormLinkGetter<IKeywordGetter> Association => this.AssociationKey.AsLink<IKeywordGetter>();
        }
    }
}