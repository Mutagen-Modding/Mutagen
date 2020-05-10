using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class BodyTemplate
    {
        [Flags]
        public enum Flag
        {
            ModulatesVoice = 0x01,
            NonPlayable = 0x10,
        }
    }
}
