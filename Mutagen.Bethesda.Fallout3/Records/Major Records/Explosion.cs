namespace Mutagen.Bethesda.Fallout3;

public partial class Explosion
{
    [Flags]
    public enum Flag
    {
        AlwaysUsesWorldOrientation = 0x002,
        KnockDownAlways = 0x004,
        KnockDownByFormula = 0x008,
        IgnoreLosCheck = 0x010,
        PushExplosionSourceRefOnly = 0x020,
        IgnoreImageSpaceSwap = 0x040,
    }
}
