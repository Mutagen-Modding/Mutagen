using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class MagicEffectNpcArchetype
    {
        public FormLink<INpcGetter> Association => this.AssociationKey.AsLink<INpcGetter>();

        IFormLink<INpcGetter> IMagicEffectNpcArchetype.Association => this.Association;
        IFormLinkGetter<INpcGetter> IMagicEffectNpcArchetypeGetter.Association => this.Association;

        public MagicEffectNpcArchetype()
            : base(TypeEnum.SummonCreature)
        {
        }
    }

    public partial interface IMagicEffectNpcArchetype
    {
        new IFormLink<INpcGetter> Association { get; }
    }

    public partial interface IMagicEffectNpcArchetypeGetter
    {
        IFormLinkGetter<INpcGetter> Association { get; }
    }

    namespace Internals
    {
        public partial class MagicEffectNpcArchetypeBinaryOverlay
        {
            public IFormLinkGetter<INpcGetter> Association => this.AssociationKey.AsLink<INpcGetter>();
        }
    }
}
