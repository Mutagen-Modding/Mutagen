namespace Mutagen.Bethesda.Starfield;

public partial class ScenePhaseFragment
{
    [Flags]
    public enum Flag
    {
        OnStart = 0x01,
        OnCompletion = 0x02,
    }
}