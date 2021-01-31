using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class SoundCategory
    {
        [Flags]
        public enum Flag
        {
            MuteWhenSubmerged = 0x01,
            ShouldAppearOnMenu = 0x02,
        }
    }
}
