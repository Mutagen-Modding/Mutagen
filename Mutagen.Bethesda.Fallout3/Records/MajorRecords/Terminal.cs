namespace Mutagen.Bethesda.Fallout3;

public partial class Terminal
{
    [Flags]
    public enum MajorFlag
    {
        QuestItem = 0x0000_0400,
        RandomAnimStart = 0x0001_0000,
    }

    [Flags]
    public enum TerminalFlag
    {
        Leveled = 0x01,
        Unlocked = 0x02,
        AlternateColors = 0x04,
        HideWelcomeTextWhenDisplayingImage = 0x08,
    }

    public enum TerminalServerType
    {
        Server1 = 0,
        Server2 = 1,
        Server3 = 2,
        Server4 = 3,
        Server5 = 4,
        Server6 = 5,
        Server7 = 6,
        Server8 = 7,
        Server9 = 8,
        Server10 = 9,
    }
}
