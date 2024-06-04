using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Fallout4;

public partial class MagicEffectPeakValueModArchetype
{
    public MagicEffectArchetype.TypeEnum Type => MagicEffectArchetype.TypeEnum.PeakValueModifier;
    public override IFormLinkIdentifier AssociationKey => Association;
}
