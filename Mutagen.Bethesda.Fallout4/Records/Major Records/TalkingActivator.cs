using System.Diagnostics;
using Mutagen.Bethesda.Plugins;
namespace Mutagen.Bethesda.Fallout4;

public partial class TalkingActivator
{
    [Flags]
    public enum MajorFlag
    {
        HiddenFromLocalMap = 0x0000_0200,
        RandomAnimStart = 0x0001_0000,
        RadioStation = 0x0002_0000,
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IFormLinkNullableGetter<IVoiceTypeGetter> IHasVoiceTypeGetter.Voice => Voice;
}