using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class ContainerData
    {
        [Flags]
        public enum Flag
        {
            AllowSoundsWhenAnimation = 0x01,
            Respawns = 0x02,
            ShowOwner = 0x04
        }
    }
}
