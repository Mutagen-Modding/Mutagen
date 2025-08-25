using System.Diagnostics;

namespace Mutagen.Bethesda.Oblivion;

public partial class SigilStone
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IReadOnlyList<IEffectGetter> IHasEffectsGetter.Effects => Effects;
}