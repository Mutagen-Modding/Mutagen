using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Starfield;

public partial class MagicEffectSummonCreatureArchetype
{
    public MagicEffectArchetype.TypeEnum Type => MagicEffectArchetype.TypeEnum.SummonCreature;
    public override FormKey AssociationKey => Association.FormKey;
}