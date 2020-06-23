using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class PackageScriptFragments
    {
        [Flags]
        public enum Flag
        {
            OnBegin = 0x01,
            OnEnd = 0x02,
            OnChange = 0x04,
        }
    }
}
