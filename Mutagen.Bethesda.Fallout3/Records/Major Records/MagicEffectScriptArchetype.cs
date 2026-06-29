using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Fallout3;

public partial class MagicEffectScriptArchetype
{
    public MagicEffectArchetype.TypeEnum Type => MagicEffectArchetype.TypeEnum.Script;
    public override IFormLinkIdentifier AssociationKey => Association;
}