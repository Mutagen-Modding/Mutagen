using System;

namespace Mutagen.Bethesda.Skyrim;

public partial class ArtObject
{
    [Flags]
    public enum TypeEnum
    {
        MagicCasting = 0x01,
        MagicHitEffect = 0x02,
        EnchantmentEffect = 0x04,
    }
}