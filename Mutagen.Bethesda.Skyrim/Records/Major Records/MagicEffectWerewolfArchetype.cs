using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Skyrim;

public partial class MagicEffectWerewolfArchetype
{
    public MagicEffectArchetype.TypeEnum Type => MagicEffectArchetype.TypeEnum.Werewolf;
    public override FormKey AssociationKey => Association.FormKey;
}
