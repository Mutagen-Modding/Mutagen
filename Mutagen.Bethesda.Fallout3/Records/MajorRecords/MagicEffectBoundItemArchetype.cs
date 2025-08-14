using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Fallout3;

public partial class MagicEffectBoundItemArchetype
{
    public MagicEffectArchetype.TypeEnum Type => MagicEffectArchetype.TypeEnum.BoundItem;
    public override IFormLinkIdentifier AssociationKey => Association;
}