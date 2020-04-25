using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class Light
    {
        [Flags]
        public enum MajorFlag
        {
            RandomAnimStart = 0x0001_0000,
            PortalStrict = 0x0002_0000,
            Obstacle = 0x0200_0000
        }
    }
}
