namespace Mutagen.Bethesda.FalloutNV;

public partial class DialogResponses
{
    public enum InfoType
    {
        Topic = 0,
        Conversation = 1,
        Combat = 2,
        Persuasion = 3,
        Detection = 4,
        Service = 5,
        Miscellaneous = 6,
        Radio = 7,
    }

    public enum NextSpeakerType
    {
        Target = 0,
        Self = 1,
        Either = 2,
    }

    [Flags]
    public enum InfoFlag1
    {
        Goodbye = 0x01,
        Random = 0x02,
        SayOnce = 0x04,
        RunImmediately = 0x08,
        InfoRefusal = 0x10,
        RandomEnd = 0x20,
        RunForRumors = 0x40,
        SpeechChallenge = 0x80,
    }

    [Flags]
    public enum InfoFlag2
    {
        SayOnceADay = 0x01,
        AlwaysDarken = 0x02,
        Unknown2 = 0x04,
        Unknown3 = 0x08,
        LowIntelligence = 0x10,
        HighIntelligence = 0x20,
    }
}
