using System.Diagnostics;
namespace Mutagen.Bethesda.Skyrim;

public partial class Spell
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IReadOnlyList<IEffectGetter> IHasEffectsGetter.Effects => Effects;
}