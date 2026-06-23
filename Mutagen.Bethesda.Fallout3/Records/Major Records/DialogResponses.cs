namespace Mutagen.Bethesda.Fallout3;

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
    public enum InfoFlag
    {
        // Byte 0 (xEdit "Flags 1")
        Goodbye = 0x0001,
        Random = 0x0002,
        SayOnce = 0x0004,
        RunImmediately = 0x0008,
        InfoRefusal = 0x0010,
        RandomEnd = 0x0020,
        RunForRumors = 0x0040,
        SpeechChallenge = 0x0080,
        // Byte 1 (xEdit "Flags 2"); FO3 defines only the first two, the rest are FNV-only
        SayOnceADay = 0x0100,
        AlwaysDarken = 0x0200,
        LowIntelligence = 0x1000,
        HighIntelligence = 0x2000,
    }

    public enum EmotionType
    {
        Neutral = 0,
        Anger = 1,
        Disgust = 2,
        Fear = 3,
        Sad = 4,
        Happy = 5,
        Surprise = 6,
        Pained = 7,
    }

    public enum SpeechChallengeEnum
    {
        None = 0,
        VeryEasy = 1,
        Easy = 2,
        Average = 3,
        Hard = 4,
        VeryHard = 5,
    }
}
