using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class DebrisModel
    {
        [Flags]
        public enum Flag
        {
            HasCollisionData = 0x01
        }
    }
}
