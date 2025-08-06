using Noggog;
namespace Mutagen.Bethesda.Skyrim;

public interface IHasEffects : ISkyrimMajorRecordInternal, IHasEffectsGetter
{
    new ExtendedList<Effect> Effects { get; init; }
}

/// <summary>
/// Common interface for records that have effects.
/// </summary>
public interface IHasEffectsGetter : ISkyrimMajorRecordGetter
{
    IReadOnlyList<IEffectGetter> Effects { get; }
}
