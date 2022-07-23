namespace Mutagen.Bethesda.Fallout4;

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
        ForceGreetHello = 0x80,
        PlayerAddress = 0x100,
        ForceSubtitle = 0x200,
        CanMoveWhileGreeting = 0x400,
        NoLIPFile = 0x800,
        RequiresPostProcessing = 0x1000,
        AudioOutputOverride = 0x2000,
        HasCapture = 0x4000,
    }

    public enum ChallengeType
    {
        None = 0,
        Easy = 1,
        Medium = 2,
        Hard = 3,
        AlwaysSucceeds = 4,
        EasyRepeatable = 5,
        MediumRepeatable = 6,
        HardRepeatable = 7,
    }

    public enum SubtitlePriorityLevel
    {
        Low = 1,
        Normal = 2,
        Force = 4,
    }
}
