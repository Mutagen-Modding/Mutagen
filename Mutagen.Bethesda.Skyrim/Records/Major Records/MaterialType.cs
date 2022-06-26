namespace Mutagen.Bethesda.Skyrim;

public partial class MaterialType
{
    [Flags]
    public enum Flag
    {
        StairMaterial = 0x01,
        ArrowsStick = 0x02,
    }
}