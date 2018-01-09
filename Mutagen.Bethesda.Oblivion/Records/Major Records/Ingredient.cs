using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class Ingredient
    {
        [Flags]
        public enum IngredientFlag
        {
            ManualValue = 0x01,
            FoodItem = 0x02
        }
    }
}
