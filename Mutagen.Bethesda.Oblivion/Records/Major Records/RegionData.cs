using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Oblivion;

public partial class RegionData
{
    [Flags]
    public enum RegionDataFlag
    {
        Override = 0x01
    }

    public enum RegionDataType
    {
        Object = 2,
        Weather = 3,
        Map = 4,
        Icon = 5,
        Grass = 6,
        Sound = 7,
    }
}