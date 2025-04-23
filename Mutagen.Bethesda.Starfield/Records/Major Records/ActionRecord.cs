namespace Mutagen.Bethesda.Starfield;

public partial class ActionRecord
{
    [Flags]
    public enum MajorFlag
    {
        Restricted = 0x0008_0000
    }
}