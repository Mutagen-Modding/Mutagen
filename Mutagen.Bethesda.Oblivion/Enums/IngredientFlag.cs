using System;

namespace Mutagen.Bethesda.Oblivion;

[Flags]
public enum IngredientFlag
{
    ManualValue = 0x01,
    FoodItem = 0x02
}