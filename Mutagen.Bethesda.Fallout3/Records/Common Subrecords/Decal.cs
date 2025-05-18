namespace Mutagen.Bethesda.Fallout3;

public partial class Decal
{
    [Flags]
    public enum Flag
    {
        Parallax = 0x01,
        AlphaBlending = 0x02,
        AlphaTesting = 0x04,
    }
}