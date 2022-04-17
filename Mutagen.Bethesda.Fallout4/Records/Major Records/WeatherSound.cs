using System;

namespace Mutagen.Bethesda.Fallout4;

partial class WeatherSound
{
    [Flags]
    public enum TypeEnum
    {
        Default = 1,
        Precipitation = 2,
        Wind = 4,
        Thunder = 8
    }
}
