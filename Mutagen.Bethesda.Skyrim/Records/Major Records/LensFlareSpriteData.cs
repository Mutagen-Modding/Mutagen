namespace Mutagen.Bethesda.Skyrim;

public partial class LensFlareSpriteData
{
    [Flags]
    public enum Flag
    {
        Rotates = 0x01,
        ShrinksWhenOccluded = 0x02,
    }
}