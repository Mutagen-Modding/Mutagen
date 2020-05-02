using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class WorkbenchData
    {
        public enum Type
        {
            None = 0,
            CreateObject = 1,
            SmithingWeapon = 2,
            Enchanting = 3,
            EnchantingExperiment = 4,
            Alchemy = 5,
            AlchemyExperiment = 6,
            SmithingArmor = 7,
        }
    }
}
