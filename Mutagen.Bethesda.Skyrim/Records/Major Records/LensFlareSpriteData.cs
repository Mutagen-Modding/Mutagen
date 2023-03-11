namespace Mutagen.Bethesda.Skyrim.Records.Major_Records;

public class LensFlareSpriteData
{
    [Flags]
    public enum Flag
    {
        Rotates = 0x01,
        ShrinksWhenOccluded = 0x02,
    }
}