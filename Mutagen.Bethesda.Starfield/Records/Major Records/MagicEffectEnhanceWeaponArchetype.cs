using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Starfield;

public partial class MagicEffectEnhanceWeaponArchetype
{
    public MagicEffectArchetype.TypeEnum Type => MagicEffectArchetype.TypeEnum.EnhanceWeapon;
    public override IFormLinkIdentifier AssociationKey => Association;
}