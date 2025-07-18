using System.Diagnostics;
namespace Mutagen.Bethesda.Starfield;

public partial class ObjectEffect
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IReadOnlyList<IEffectGetter> IHasEffectsGetter.Effects => Effects;
}