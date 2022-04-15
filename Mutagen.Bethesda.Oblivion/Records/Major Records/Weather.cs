using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Oblivion
{
    public partial class Weather
    {
        public enum WeatherClassification
        {
            None = 0,
            Pleasant = 1,
            Cloudy = 2,
            Unknown3 = 3,
            Rainy = 4,
            Snow = 8,
            Pleasant2 = 0xC1,
        }
    }
}
