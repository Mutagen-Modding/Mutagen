using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class LockData
    {
        [Flags]
        public enum Flag
        {
            LeveledLock = 0x1
        }
    }
}
