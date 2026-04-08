namespace Mutagen.Bethesda.Fallout3;

public partial class Message
{
    [Flags]
    public enum Flag
    {
        MessageBox = 0x01,
        AutoDisplay = 0x02,
    }
}
