using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class MagicEffectVampireArchetype
    {
        public FormLink<IRaceGetter> Association => this.AssociationKey.AsLink<IRaceGetter>();

        IFormLink<IRaceGetter> IMagicEffectVampireArchetype.Association => this.Association;
        IFormLinkGetter<IRaceGetter> IMagicEffectVampireArchetypeGetter.Association => this.Association;

        public MagicEffectVampireArchetype()
            : base(TypeEnum.VampireLord)
        {
        }
    }

    public partial interface IMagicEffectVampireArchetype
    {
        new IFormLink<IRaceGetter> Association { get; }
    }

    public partial interface IMagicEffectVampireArchetypeGetter
    {
        IFormLinkGetter<IRaceGetter> Association { get; }
    }

    namespace Internals
    {
        public partial class MagicEffectVampireArchetypeBinaryOverlay
        {
            public IFormLinkGetter<IRaceGetter> Association => this.AssociationKey.AsLink<IRaceGetter>();
        }
    }
}
