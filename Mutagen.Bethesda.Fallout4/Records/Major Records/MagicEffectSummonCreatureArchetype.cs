using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Fallout4;

public partial class MagicEffectSummonCreatureArchetype
{
    public MagicEffectArchetype.TypeEnum Type => MagicEffectArchetype.TypeEnum.SummonCreature;
    public override FormKey AssociationKey => Association.FormKey;
}
