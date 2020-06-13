using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class DialogResponse
    {
        [Flags]
        public enum Flag
        {
            UseEmotionAnimation = 0x01
        }
    }
}
