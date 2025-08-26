using System.Diagnostics;
namespace Mutagen.Bethesda.Skyrim;

public partial class Scroll
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IReadOnlyList<IEffectGetter> IHasEffectsGetter.Effects => Effects;
}