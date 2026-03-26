using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.FalloutNV;

public partial class Creature
{
    [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
    IFormLinkNullableGetter<IVoiceTypeGetter> IHasVoiceTypeGetter.Voice => Voice;
}
