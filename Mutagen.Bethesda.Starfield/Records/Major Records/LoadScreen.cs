namespace Mutagen.Bethesda.Starfield;

public partial class LoadScreen
{
    [Flags]
    public enum MajorFlag
    {
        DisplaysInMainMenu = 0x0000_0400,
        NoRotation = 0x0000_8000,
    }
}