using System.Diagnostics;
namespace Mutagen.Bethesda.Fallout4;

public partial class Spell
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IReadOnlyList<IEffectGetter> IHasEffectsGetter.Effects => Effects;
}