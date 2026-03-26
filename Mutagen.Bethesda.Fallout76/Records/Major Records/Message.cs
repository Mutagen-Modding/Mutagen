namespace Mutagen.Bethesda.Fallout76;

public partial class Message
{
    [Flags]
    public enum MajorFlag
    {
    }

    [Flags]
    public enum Flag
    {
        MessageBox = 0x01,
        DelayInitialDisplay = 0x02,
    }
}