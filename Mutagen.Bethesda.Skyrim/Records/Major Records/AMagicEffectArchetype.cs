using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Skyrim;

public partial class AMagicEffectArchetype
{
    public static readonly ActorValue ActorValueDefault = ActorValue.None;

    MagicEffectArchetype.TypeEnum IAMagicEffectArchetypeGetter.Type => throw new NotImplementedException();
    public abstract FormKey AssociationKey { get; }
    public virtual ActorValue ActorValue { get; set; } = ActorValueDefault;
}

public partial interface IAMagicEffectArchetypeGetter
{
    MagicEffectArchetype.TypeEnum Type { get; }
    FormKey AssociationKey { get; }
}

partial class AMagicEffectArchetypeBinaryOverlay
{
    public abstract FormKey AssociationKey { get; }
    public ActorValue ActorValue => throw new NotImplementedException();
    public abstract MagicEffectArchetype.TypeEnum Type { get; }
}