using System;

namespace Mutagen.Bethesda.Skyrim;

public partial class StoryManagerQuestNode
{
    [Flags]
    public enum QuestFlag
    {
        Random = 0x0000_0001,
        WarnIfNoChildQuestStarted = 0x0000_0002,
        DoAllBeforeRepeating = 0x0001_0000,
        SharesEvent = 0x0002_0000,
        NumQuestsToRun = 0x0004_0000,
    }
}