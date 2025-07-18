using System.Diagnostics;
namespace Mutagen.Bethesda.Skyrim;

public partial class Ingredient
{
    [Flags]
    public enum Flag
    {
        NoAutoCalculation = 0x001,
        FoodItem = 0x002,
        ReferencesPersist = 0x100
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IReadOnlyList<IEffectGetter> IHasEffectsGetter.Effects => Effects;
}