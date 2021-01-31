using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class Weapon
    {
        [Flags]
        public enum MajorFlag
        {
            NonPlayable = 0x0000_0004
        }
    }
}
