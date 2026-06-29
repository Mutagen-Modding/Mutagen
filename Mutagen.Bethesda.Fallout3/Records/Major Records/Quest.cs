namespace Mutagen.Bethesda.Fallout3;

public partial class Quest
{
    [Flags]
    public enum Flag
    {
        StartGameEnabled = 0x01,
        AllowRepeatedConversationTopics = 0x04,
        AllowRepeatedStages = 0x08,
        DefaultScriptProcessingDelay = 0x10,
    }
}
