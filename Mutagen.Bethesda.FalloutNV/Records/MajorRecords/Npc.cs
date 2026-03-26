using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.FalloutNV;

public partial class Npc
{
    [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
    IFormLinkNullableGetter<IVoiceTypeGetter> IHasVoiceTypeGetter.Voice => Voice;
}
