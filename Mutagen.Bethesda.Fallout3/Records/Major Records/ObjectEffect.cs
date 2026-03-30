using System.Diagnostics;

namespace Mutagen.Bethesda.Fallout3;

public partial class ObjectEffect
{
    public enum EnchantmentType
    {
        Weapon = 2,
        Apparel = 3
    }

    [Flags]
    public enum Flag
    {
        NoAutoCalc = 0x01,
        HideEffect = 0x04
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IReadOnlyList<IEffectGetter> IHasEffectsGetter.Effects => Effects;
}
