using System.Diagnostics;
namespace Mutagen.Bethesda.Skyrim;

public partial class Ingestible
{
    [Flags]
    public enum MajorFlag
    {
        Medicine = 0x2000_0000
    }

    [Flags]
    public enum Flag
    {
        NoAutoCalc = 0x0000_0001,
        FoodItem = 0x0000_0002,
        Medicine = 0x0001_0000,
        Poison = 0x0002_0000,
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IReadOnlyList<IEffectGetter> IHasEffectsGetter.Effects => Effects;
}