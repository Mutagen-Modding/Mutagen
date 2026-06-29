using Noggog;

namespace Mutagen.Bethesda.Fallout3;

public interface IHasEffects : IFallout3MajorRecordInternal, IHasEffectsGetter
{
    new ExtendedList<Effect> Effects { get; init; }
}

public interface IHasEffectsGetter : IFallout3MajorRecordGetter
{
    IReadOnlyList<IEffectGetter> Effects { get; }
}
