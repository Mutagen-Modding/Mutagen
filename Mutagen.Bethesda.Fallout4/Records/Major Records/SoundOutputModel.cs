namespace Mutagen.Bethesda.Fallout4;

partial class SoundOutputModel
{
    [Flags]
    public enum Flag
    {
        AttenuatesWithDistance = 0x01,
        AllowsRumble = 0x02,
        AppliesDoppler = 0x04,
        AppliesDistanceDelay = 0x08,
        PlayerOutputModel = 0x10,
        TryPlayOnController = 0x20,
        CausesDucking = 0x40,
        AvoidsDucking = 0x80,
    }

    public enum TypeEnum
    {
        UsesHrtf,
        DefinedSpeakerOutput,
    }
}