using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Fallout3;

public partial class Npc
{
    [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
    IFormLinkNullableGetter<IVoiceTypeGetter> IHasVoiceTypeGetter.Voice => Voice;
}
