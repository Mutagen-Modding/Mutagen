namespace Mutagen.Bethesda.Starfield;

partial class Message
{
    [Flags]
    public enum Flag
    {
        MessageBox = 0x01,
        DelayInitialDisplay = 0x02,
    }
}