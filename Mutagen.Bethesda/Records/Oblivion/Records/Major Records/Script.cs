using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class Script
    {
        [Flags]
        public enum LocalVariableFlag
        {
            IsLongOrShort = 0x01
        }
    }
}
