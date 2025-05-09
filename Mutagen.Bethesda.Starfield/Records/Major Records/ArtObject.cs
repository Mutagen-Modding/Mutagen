namespace Mutagen.Bethesda.Starfield;

public partial class ArtObject
{
    public enum TypeEnum
    {
        MagicCasting,
        MagicHitEffect,
        EnchantmentEffect,
        MiscArtObject,
    }

    [Flags]
    public enum Flag
    {
        RotateToFaceTarget = 0x01,
        AttachToCamera = 0x02,
        InheritRotation = 0x04,
        ReferenceEffect = 0x08,
    }
}