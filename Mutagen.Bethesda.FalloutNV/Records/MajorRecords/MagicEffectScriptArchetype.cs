using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.FalloutNV;

public partial class MagicEffectScriptArchetype
{
    public MagicEffectArchetype.TypeEnum Type => MagicEffectArchetype.TypeEnum.Script;
    public override IFormLinkIdentifier AssociationKey => Association;
}