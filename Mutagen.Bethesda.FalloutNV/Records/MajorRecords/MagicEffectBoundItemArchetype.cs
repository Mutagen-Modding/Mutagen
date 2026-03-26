using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.FalloutNV;

public partial class MagicEffectBoundItemArchetype
{
    public MagicEffectArchetype.TypeEnum Type => MagicEffectArchetype.TypeEnum.BoundItem;
    public override IFormLinkIdentifier AssociationKey => Association;
}