namespace Mutagen.Bethesda.Oblivion;

public partial class LogEntry
{
    [Flags]
    public enum Flag
    {
        CompleteQuest = 0x01
    }
}