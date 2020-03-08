using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class RegionDataObject
    {
        [Flags]
        public enum Flag
        {
            ConformToSlope = 0x001,
            PaintVertices = 0x002,
            SizeVariance = 0x004,
            X = 0x008,
            Y = 0x010,
            Z = 0x020,
            Tree = 0x040,
            HugeRock = 0x080
        }
    }
}
