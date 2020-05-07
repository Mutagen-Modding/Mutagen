using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class IngredientData
    {
        [Flags]
        public enum Flag
        {
            NoAutoCalculation = 0x001,
            FoodItem = 0x002,
            ReferencesPersist = 0x100
        }
    }
}
