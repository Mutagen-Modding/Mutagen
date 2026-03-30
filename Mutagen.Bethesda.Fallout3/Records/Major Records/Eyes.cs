namespace Mutagen.Bethesda.Fallout3;

public partial class Eyes
{
    [Flags]
    public enum Flag
    {
        Playable = 0x01,
        NotMale = 0x02,
        NotFemale = 0x04,
    }
}