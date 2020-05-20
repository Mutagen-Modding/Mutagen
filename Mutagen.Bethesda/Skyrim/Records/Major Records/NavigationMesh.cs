using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class NavigationMesh
    {
        [Flags]
        public enum MajorFlag : uint
        {
            AutoGen = 0x0400_0000,
            NavmeshGenCell = 0x8000_0000,
        }
    }
}
