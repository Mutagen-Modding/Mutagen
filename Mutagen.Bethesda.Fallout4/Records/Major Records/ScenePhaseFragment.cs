namespace Mutagen.Bethesda.Fallout4;

public partial class ScenePhaseFragment
{
    [Flags]
    public enum Flag
    {
        OnStart = 0x01,
        OnCompletion = 0x02,
    }
}