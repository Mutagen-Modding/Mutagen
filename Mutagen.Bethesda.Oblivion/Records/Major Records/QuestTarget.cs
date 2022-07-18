namespace Mutagen.Bethesda.Oblivion;

public partial class QuestTarget
{
    [Flags]
    public enum Flag
    {
        CompassMarkerIgnoresLocks = 0x01
    }
}