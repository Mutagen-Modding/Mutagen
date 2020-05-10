using Mutagen.Bethesda.Binary;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class ModHeader
    {
        [Flags]
        public enum HeaderFlag
        {
            Master = 0x01
        }
    }
}
