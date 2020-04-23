using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class Eyes
    {
        [Flags]
        public enum MajorFlag
        {
            NonPlayable = 0x0000_0004,
        }

        [Flags]
        public enum Flag
        {
            Playable = 0x01,
            NotMale = 0x02,
            NotFemale = 0x04,
        }
    }
}
