using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Noggog;

namespace Mutagen.Bethesda.Starfield;

partial class DialogResponses
{
    [Flags]
    public enum MajorFlag
    {
        InfoGroup = 0x0000_0040,
        ExcludeFromExport = 0x0000_0080,
        ActorChanged = 0x0000_2000
    }

    [Flags]
    public enum Flag
    {
        StartSceneOnEnd = 0x1,
        Random = 0x2,
        SayOnce = 0x4,
        RequiresPlayerActivation = 0x8,
        RandomEnd = 0x20,
        EndRunningScene = 0x40,
        NeedsReview = 0x80,
        PlayerAddress = 0x100,
        CanMoveWhileGreeting = 0x400,
        NoLipFile = 0x800,
        DoNotGrayOut = 0x1000,
        AudioOutputOverride = 0x2000,
        HasCapture = 0x4000,
    }
    
    [Flags]
    public enum InfoFlag
    {
        Random = 0x2,
        ForceAllChildrenPlayerActivateOnly = 0x8,
        RandomEnd = 0x20,
        ChildInfosDontInheritResetData = 0x100,
        ForceAllChildrenRandom = 0x200,
        DoNotDoAllBeforeRepeating = 0x800,
    }
    
    public enum SubtitlePriorityLevel
    {
        Low = 1,
        Normal = 2,
        High = 3,
        Force = 4,
    }
}
