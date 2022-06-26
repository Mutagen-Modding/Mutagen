using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Skyrim;

public partial class MagicEffectWerewolfArchetype
{
    public FormLink<IRaceGetter> Association => this.AssociationKey.AsLink<IRaceGetter>();

    IFormLink<IRaceGetter> IMagicEffectWerewolfArchetype.Association => this.Association;
    IFormLinkGetter<IRaceGetter> IMagicEffectWerewolfArchetypeGetter.Association => this.Association;

    public MagicEffectWerewolfArchetype()
        : base(TypeEnum.Werewolf)
    {
    }
}

public partial interface IMagicEffectWerewolfArchetype
{
    new IFormLink<IRaceGetter> Association { get; }
}

public partial interface IMagicEffectWerewolfArchetypeGetter
{
    IFormLinkGetter<IRaceGetter> Association { get; }
}

partial class MagicEffectWerewolfArchetypeBinaryOverlay
{
    public IFormLinkGetter<IRaceGetter> Association => this.AssociationKey.AsLink<IRaceGetter>();
}