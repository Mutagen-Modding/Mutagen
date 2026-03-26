namespace Mutagen.Bethesda.Fallout76;

public partial class ArtObject
{
    [Flags]
    public enum MajorFlag
    {
    }

    [Flags]
    public enum TypeEnum
    {
        MagicCasting = 0x01,
        MagicHitEffect = 0x02,
        EnchantmentEffect = 0x04,
    }
}

partial class ArtObjectBinaryOverlay
{
}
