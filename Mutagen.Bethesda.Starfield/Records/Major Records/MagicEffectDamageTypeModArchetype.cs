using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Starfield;

public partial class MagicEffectDamageTypeModArchetype
{
    public MagicEffectArchetype.TypeEnum Type => MagicEffectArchetype.TypeEnum.Damage;
    public override FormKey AssociationKey => Association.FormKey;
}