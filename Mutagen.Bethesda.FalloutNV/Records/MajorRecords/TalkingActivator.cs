namespace Mutagen.Bethesda.FalloutNV;

public partial class TalkingActivator
{
    [Flags]
    public enum MajorFlag
    {
        OnLocalMap = 0x0000_0200,
        QuestItem = 0x0000_0400,
        NoVoiceFilter = 0x0000_2000,
        RandomAnimStart = 0x0001_0000,
        RadioStation = 0x0002_0000,
        NonPipboy = 0x1000_0000,
        ContinuousBroadcast = 0x4000_0000,
    }
}
