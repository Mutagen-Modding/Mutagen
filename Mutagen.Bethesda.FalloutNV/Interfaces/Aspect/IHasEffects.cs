using Noggog;

namespace Mutagen.Bethesda.FalloutNV;

public interface IHasEffects : IFalloutNVMajorRecordInternal, IHasEffectsGetter
{
    new ExtendedList<Effect> Effects { get; init; }
}

public interface IHasEffectsGetter : IFalloutNVMajorRecordGetter
{
    IReadOnlyList<IEffectGetter> Effects { get; }
}
