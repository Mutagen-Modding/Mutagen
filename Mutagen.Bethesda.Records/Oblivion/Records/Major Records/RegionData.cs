using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class RegionData
    {
        [Flags]
        public enum RegionDataFlag
        {
            Override = 0x01
        }

        public enum RegionDataType
        {
            Objects = 2,
            Weather = 3,
            MapName = 4,
            Icon = 5,
            Grasses = 6,
            Sounds = 7,
        }
    }
}
