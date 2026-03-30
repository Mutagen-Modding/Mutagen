namespace Mutagen.Bethesda.Fallout3;

public partial class WeatherSound
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
