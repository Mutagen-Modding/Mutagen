using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.FalloutNV;

public partial class MagicEffectSummonCreatureArchetype
{
    public MagicEffectArchetype.TypeEnum Type => MagicEffectArchetype.TypeEnum.SummonCreature;
    public override IFormLinkIdentifier AssociationKey => Association;
}