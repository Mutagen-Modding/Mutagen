using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class Eye
    {
        [Flags]
        public enum Flag
        {
            Playable = 0x01,
            NotMale = 0x02,
            NotFemale = 0x04,
        }
    }
}
