namespace Mutagen.Bethesda.Skyrim;

public partial class CombatStyle
{
    [Flags]
    public enum MajorFlag
    {
        AllowDualWielding = 0x0008_0000
    }

    [Flags]
    public enum Flag
    {
        Dueling = 0x01,
        Flanking = 0x02,
        AllowDualWielding = 0x04,
    }
}