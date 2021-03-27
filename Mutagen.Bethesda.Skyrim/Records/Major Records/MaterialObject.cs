using Mutagen.Bethesda.Binary;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class MaterialObject
    {
        [Flags]
        public enum Flag : ulong
        {
            SinglePass = 0x01,
        }
    }
}
