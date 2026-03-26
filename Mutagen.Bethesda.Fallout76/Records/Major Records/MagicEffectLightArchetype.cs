using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Fallout76;

public partial class MagicEffectLightArchetype
{
    public MagicEffectArchetype.TypeEnum Type => MagicEffectArchetype.TypeEnum.Light;
    public override IFormLinkIdentifier AssociationKey => Association;
}
