namespace Mutagen.Bethesda.Fallout3;

public partial class EnableParent
{
    [Flags]
    public enum Flag
    {
        SetEnableStateToOppositeOfParent = 0x01,
        PopIn = 0x02,
    }
}
