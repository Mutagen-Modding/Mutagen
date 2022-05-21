namespace Mutagen.Bethesda.Fallout4;

partial class Scene
{
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
}

