using Noggog;

namespace Mutagen.Bethesda.Oblivion;

public interface IHasEffects : IOblivionMajorRecordInternal, IHasEffectsGetter
{
    new ExtendedList<Effect> Effects { get; init; }
}

/// <summary>
/// Common interface for records that have effects.
/// </summary>
public interface IHasEffectsGetter : IOblivionMajorRecordGetter
{
    IReadOnlyList<IEffectGetter> Effects { get; }
}
