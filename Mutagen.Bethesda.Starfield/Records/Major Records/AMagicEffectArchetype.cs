using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Starfield;

public partial class AMagicEffectArchetype
{
    MagicEffectArchetype.TypeEnum IAMagicEffectArchetypeGetter.Type => throw new NotImplementedException();
    public abstract IFormLinkIdentifier AssociationKey { get; }
}

public partial interface IAMagicEffectArchetypeGetter
{
    MagicEffectArchetype.TypeEnum Type { get; }
    IFormLinkIdentifier AssociationKey { get; }
}

partial class AMagicEffectArchetypeBinaryOverlay
{
    public abstract IFormLinkIdentifier AssociationKey { get; }
    MagicEffectArchetype.TypeEnum IAMagicEffectArchetypeGetter.Type => throw new NotImplementedException();
}