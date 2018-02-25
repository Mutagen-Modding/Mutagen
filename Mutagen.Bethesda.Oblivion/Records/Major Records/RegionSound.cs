using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class RegionSound
    {
        [Flags]
        public enum Flag
        {
            Pleasant = 0x01,
            Cloudy = 0x02,
            Rainy = 0x04,
            Snowy = 0x08,
        }
    }
}
