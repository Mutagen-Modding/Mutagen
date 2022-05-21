namespace Mutagen.Bethesda.Skyrim;

public partial class Scene
{
    [Flags]
    public enum Flag
    {
        BeginOnQuestStart = 0x001,
        StopOnQuestEnd = 0x002,
        RepeatConditionsWhileTrue = 0x008,
        Interruptable = 0x010,
    }
}
