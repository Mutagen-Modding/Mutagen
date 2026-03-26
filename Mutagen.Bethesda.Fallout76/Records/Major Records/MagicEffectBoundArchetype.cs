using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Fallout76;

public partial class MagicEffectBoundArchetype
{
    public MagicEffectArchetype.TypeEnum Type => MagicEffectArchetype.TypeEnum.Bound;
    public override IFormLinkIdentifier AssociationKey => Association;
}
