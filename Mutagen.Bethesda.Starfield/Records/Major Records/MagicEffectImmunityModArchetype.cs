using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Starfield;

public partial class MagicEffectImmunityModArchetype
{
    public MagicEffectArchetype.TypeEnum Type => MagicEffectArchetype.TypeEnum.Immunity;
    public override FormKey AssociationKey => Association.FormKey;
}