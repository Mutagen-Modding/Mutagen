using System.Diagnostics;

namespace Mutagen.Bethesda.Oblivion;

public partial class Potion
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IReadOnlyList<IEffectGetter> IHasEffectsGetter.Effects => Effects;
}