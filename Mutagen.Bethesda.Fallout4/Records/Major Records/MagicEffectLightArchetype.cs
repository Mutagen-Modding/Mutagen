using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Fallout4;

public partial class MagicEffectLightArchetype
{
    public FormLink<ILightGetter> Association => this.AssociationKey.ToLink<ILightGetter>();

    IFormLink<ILightGetter> IMagicEffectLightArchetype.Association => this.Association;
    IFormLinkGetter<ILightGetter> IMagicEffectLightArchetypeGetter.Association => this.Association;

    public MagicEffectLightArchetype()
        : base(TypeEnum.Light)
    {
    }
}

public partial interface IMagicEffectLightArchetype
{
    new IFormLink<ILightGetter> Association { get; }
}

public partial interface IMagicEffectLightArchetypeGetter
{
    IFormLinkGetter<ILightGetter> Association { get; }
}

partial class MagicEffectLightArchetypeBinaryOverlay
{
    public IFormLinkGetter<ILightGetter> Association => this.AssociationKey.ToLink<ILightGetter>();
}