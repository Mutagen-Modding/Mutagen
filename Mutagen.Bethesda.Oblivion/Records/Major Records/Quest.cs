namespace Mutagen.Bethesda.Oblivion;

public partial class Quest
{
    [Flags]
    public enum Flag
    {
        StartGameEnabled = 0x01,
        AllowRepeatedConversationTopics = 0x04,
        AllowRepeatedStages = 0x08
    }
}