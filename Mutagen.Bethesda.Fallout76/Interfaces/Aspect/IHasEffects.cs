using Noggog;

namespace Mutagen.Bethesda.Fallout76;

public interface IHasEffects : IFallout76MajorRecordInternal, IHasEffectsGetter
{
    new ExtendedList<Effect> Effects { get; init; }
}

/// <summary>
/// Common interface for records that have effects.
/// </summary>
public interface IHasEffectsGetter : IFallout76MajorRecordGetter
{
    IReadOnlyList<IEffectGetter> Effects { get; }
}
