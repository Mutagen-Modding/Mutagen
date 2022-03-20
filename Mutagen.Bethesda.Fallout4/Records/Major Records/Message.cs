using Mutagen.Bethesda.Binary;
using Noggog;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mutagen.Bethesda.Fallout4
{
    public partial class Message
    {
        [Flags]
        public enum Flag
        {
            MessageBox = 0x01,
            DelayInitialDisplay = 0x02,
        }
    }
}
