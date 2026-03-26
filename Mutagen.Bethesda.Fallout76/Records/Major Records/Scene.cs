namespace Mutagen.Bethesda.Fallout76;

partial class Scene
{
    [Flags]
    public enum MajorFlag
    {
    }

    [Flags]
    public enum Flag
    {
        BeginOnQuestStart = 0x0001,
        StopOnQuestEnd = 0x0002,
        ShowAllText = 0x0004,
        RepeatConditionsWhileTrue = 0x0008,
        Interruptable = 0x0010,
        PreventPlayerExitDialogue = 0x0040,
        DisableDialogueCamera = 0x0800,
        NoFollowerIdleChatter = 0x1000,
    }

    public enum PhaseType
    {
        Dialogue = 0,
        Package = 1,
        Timer = 2,
        PlayerDialogue = 3,
        StartScene = 4,
        NpcResponseDialogue = 5,
        Radio = 6,
    }
}

