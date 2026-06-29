using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Fallout3;

public partial class MagicEffectSummonCreatureArchetype
{
    public MagicEffectArchetype.TypeEnum Type => MagicEffectArchetype.TypeEnum.SummonCreature;
    public override IFormLinkIdentifier AssociationKey => Association;
}