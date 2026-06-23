namespace Mutagen.Bethesda.Fallout3;

public partial class LockData
{
    [Flags]
    public enum Flag : byte
    {
        LeveledLock = 0x4
    }
}
