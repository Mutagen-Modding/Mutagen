using Noggog;
namespace Mutagen.Bethesda.Starfield;

public interface IHasEffects : IStarfieldMajorRecordInternal, IHasEffectsGetter
{
    new ExtendedList<Effect> Effects { get; init; }
}

/// <summary>
/// Common interface for records that have effects.
/// </summary>
public interface IHasEffectsGetter : IStarfieldMajorRecordGetter
{
    IReadOnlyList<IEffectGetter> Effects { get; }
}
