namespace Mutagen.Bethesda.Fallout3;

public partial class Quest
{
    [Flags]
    public enum Flag
    {
        StartGameEnabled = 0x01,
        AllowRepeatedStages = 0x04,
        AllowRepeatedConversationTopics = 0x08,
    }

    [Flags]
    public enum TargetFlag
    {
        CompassMarkerIgnoresLocks = 0x1,
    }
}
