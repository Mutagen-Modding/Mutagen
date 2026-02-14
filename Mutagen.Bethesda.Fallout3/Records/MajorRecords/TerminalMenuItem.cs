namespace Mutagen.Bethesda.Fallout3;

public partial class TerminalMenuItem
{
    [Flags]
    public enum MenuItemFlag
    {
        AddNote = 0x01,
        ForceRedraw = 0x02,
    }
}
