namespace Mutagen.Bethesda.Starfield;

public partial class TerminalMenu
{
    public enum ShowBodyTextOption
    {
        Folder,
        Document,
    }
    
    [Flags]
    public enum MenuButtonStyleOption
    {
        CanBeDesktop = 0x01,
        UseLongKioskEntry = 0x02,
    }
    
    public enum StyleOption
    {
        Windows,
        Kiosk,
    }
}