namespace Mutagen.Bethesda.Starfield;

partial class SceneActor
{
    [Flags]
    public enum Flag
    {
        NoPlayerActivation = 0x01,
        Optional = 0x02,
        RunOnlyScenePackages = 0x04,
        NoCommandState = 0x08,
    }

    [Flags]
    public enum BehaviorFlag
    {
        DeathPause = 0x001,
        DeathEnd = 0x002,
        CombatPause = 0x004,
        CombatEnd = 0x008,
        DialoguePause = 0x010,
        DialogueEnd = 0x020,
        ObsComPause = 0x040,
        ObsComEnd = 0x080,
    }
}
