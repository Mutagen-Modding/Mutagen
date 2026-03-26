using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Fallout76;

public partial class MagicEffectEnhanceWeaponArchetype
{
    public MagicEffectArchetype.TypeEnum Type => MagicEffectArchetype.TypeEnum.EnhanceWeapon;
    public override IFormLinkIdentifier AssociationKey => Association;
}
