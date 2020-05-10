using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class CriticalData
    {
        [Flags]
        public enum Flag
        {
            OnDeath = 0x01
        }
    }
}
