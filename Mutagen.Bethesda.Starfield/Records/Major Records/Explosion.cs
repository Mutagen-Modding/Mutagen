namespace Mutagen.Bethesda.Starfield;

public partial class Explosion
{
    [Flags]
    public enum Flag
    {
        TaperEffectiveness = 0x0000_0001,
        AlwaysUsesWorldOrientation = 0x0000_0002,
        KnockDownAlways = 0x0000_0004,
        KnockDownByFormula = 0x0000_0008,
        IgnoreLosCheck = 0x0000_0010,
        PushExplosionSourceRefOnly = 0x0000_0020,
        IgnoreImageSpaceSwap = 0x0000_0040,
        Chain = 0x0000_0080,
        NoControllerVibration = 0x0000_0100,
        PlacedObjectPersists = 0x0000_0200,
        SkipUnderwaterTests = 0x0000_0400,
    }
}