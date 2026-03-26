using System.Diagnostics;

namespace Mutagen.Bethesda.FalloutNV;

public partial class Ingredient
{
    [Flags]
    public enum IngredientFlag
    {
        NoAutoCalculation = 0x01,
        FoodItem = 0x02,
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IReadOnlyList<IEffectGetter> IHasEffectsGetter.Effects => Effects;
}
