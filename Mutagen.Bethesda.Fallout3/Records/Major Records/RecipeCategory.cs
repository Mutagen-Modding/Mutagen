using System;

namespace Mutagen.Bethesda.Fallout3;

public partial class RecipeCategory
{
    [Flags]
    public enum RecipeCategoryFlag
    {
        Subcategory = 0x01,
    }
}
