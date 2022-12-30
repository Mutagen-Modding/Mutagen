using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Fallout4;

public partial class MagicEffectSummonCreatureArchetype
{
    public FormLink<INpcGetter> Association => this.AssociationKey.ToLink<INpcGetter>();

    IFormLink<INpcGetter> IMagicEffectSummonCreatureArchetype.Association => this.Association;
    IFormLinkGetter<INpcGetter> IMagicEffectSummonCreatureArchetypeGetter.Association => this.Association;

    public MagicEffectSummonCreatureArchetype()
        : base(TypeEnum.SummonCreature)
    {
    }
}

public partial interface IMagicEffectSummonCreatureArchetype
{
    new IFormLink<INpcGetter> Association { get; }
}

public partial interface IMagicEffectSummonCreatureArchetypeGetter
{
    IFormLinkGetter<INpcGetter> Association { get; }
}

partial class MagicEffectSummonCreatureArchetypeBinaryOverlay
{
    public IFormLinkGetter<INpcGetter> Association => this.AssociationKey.ToLink<INpcGetter>();
}