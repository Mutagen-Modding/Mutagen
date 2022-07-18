namespace Mutagen.Bethesda.Oblivion;

public partial class DialogItem
{
    [Flags]
    public enum Flag
    {
        Goodbye = 0x001,
        Random = 0x002,
        SayOnce = 0x004,
        RunImmediately = 0x008,
        InfoRefusal = 0x010,
        RandomEnd = 0x020,
        RunForRumors = 0x040
    }
}