using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Starfield;

public partial class AMagicEffectArchetype
{
    MagicEffectArchetype.TypeEnum IAMagicEffectArchetypeGetter.Type => throw new NotImplementedException();
    public abstract FormKey AssociationKey { get; }
}

public partial interface IAMagicEffectArchetypeGetter
{
    MagicEffectArchetype.TypeEnum Type { get; }
    FormKey AssociationKey { get; }
}

partial class AMagicEffectArchetypeBinaryOverlay
{
    public abstract FormKey AssociationKey { get; }
    MagicEffectArchetype.TypeEnum IAMagicEffectArchetypeGetter.Type => throw new NotImplementedException();
}