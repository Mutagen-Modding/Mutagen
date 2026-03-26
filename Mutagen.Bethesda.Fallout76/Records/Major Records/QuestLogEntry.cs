namespace Mutagen.Bethesda.Fallout76;

public partial class QuestLogEntry
{
    [Flags]
    public enum Flag
    {
        CompleteQuest = 0x01,
        FailQuest = 0x02,
    }
}
