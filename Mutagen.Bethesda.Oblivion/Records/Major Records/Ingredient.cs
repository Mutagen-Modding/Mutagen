using System.Diagnostics;

namespace Mutagen.Bethesda.Oblivion;

public partial class Ingredient
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IReadOnlyList<IEffectGetter> IHasEffectsGetter.Effects => Effects;
}