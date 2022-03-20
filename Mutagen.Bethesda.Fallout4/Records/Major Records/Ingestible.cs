using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mutagen.Bethesda.Fallout4
{
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
    }
}
