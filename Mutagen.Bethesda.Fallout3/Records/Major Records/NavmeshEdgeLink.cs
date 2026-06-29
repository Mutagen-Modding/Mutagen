namespace Mutagen.Bethesda.Fallout3;

public partial class NavmeshEdgeLink
{
    public enum LinkType : uint
    {
        Portal = 0,
        LedgeUp = 1,
        LedgeDown = 2,
        EnableDisablePortal = 3,
    }
}
