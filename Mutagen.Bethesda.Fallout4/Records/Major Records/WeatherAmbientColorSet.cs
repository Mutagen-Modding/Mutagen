namespace Mutagen.Bethesda.Fallout4;

partial class WeatherAmbientColorSet
{
    IAmbientColorsGetter IWeatherAmbientColorSetGetter.this[TimeOfDay time] => time switch
    {
        TimeOfDay.Sunrise => this.Sunrise,
        TimeOfDay.Day => this.Day,
        TimeOfDay.Night => this.Night,
        TimeOfDay.Sunset => this.Sunset,
        TimeOfDay.EarlySunrise => this.EarlySunrise,
        TimeOfDay.LateSunrise => this.LateSunrise,
        TimeOfDay.EarlySunset => this.EarlySunset,
        TimeOfDay.LateSunset => this.LateSunset,
        _ => throw new NotImplementedException(),
    };

    public AmbientColors this[TimeOfDay time]
    {
        get => time switch
        {
            TimeOfDay.Sunrise => this.Sunrise,
            TimeOfDay.Day => this.Day,
            TimeOfDay.Night => this.Night,
            TimeOfDay.Sunset => this.Sunset,
            TimeOfDay.EarlySunrise => this.EarlySunrise,
            TimeOfDay.LateSunrise => this.LateSunrise,
            TimeOfDay.EarlySunset => this.EarlySunset,
            TimeOfDay.LateSunset => this.LateSunset,
            _ => throw new NotImplementedException(),
        };

        set
        {
            switch (time)
            {
                case TimeOfDay.Sunrise:
                    this.Sunrise = value;
                    break;
                case TimeOfDay.Day:
                    this.Day = value;
                    break;
                case TimeOfDay.Sunset:
                    this.Sunset = value;
                    break;
                case TimeOfDay.Night:
                    this.Night = value;
                    break;
                case TimeOfDay.EarlySunrise:
                    this.EarlySunrise = value;
                    break;
                case TimeOfDay.LateSunrise:
                    this.LateSunrise = value;
                    break;
                case TimeOfDay.EarlySunset:
                    this.EarlySunset = value;
                    break;
                case TimeOfDay.LateSunset:
                    this.LateSunset = value;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}

public partial interface IWeatherAmbientColorSet
{
    new AmbientColors this[TimeOfDay time] { get; set; }
}

public partial interface IWeatherAmbientColorSetGetter
{
    IAmbientColorsGetter this[TimeOfDay time] { get; }
}

partial class WeatherAmbientColorSetBinaryOverlay
{
    public IAmbientColorsGetter Sunrise => throw new NotImplementedException();

    public IAmbientColorsGetter Day => throw new NotImplementedException();

    public IAmbientColorsGetter Sunset => throw new NotImplementedException();

    public IAmbientColorsGetter Night => throw new NotImplementedException();
    public IAmbientColorsGetter EarlySunrise => throw new NotImplementedException();

    public IAmbientColorsGetter LateSunrise => throw new NotImplementedException();

    public IAmbientColorsGetter EarlySunset => throw new NotImplementedException();

    public IAmbientColorsGetter LateSunset => throw new NotImplementedException();

    public IAmbientColorsGetter this[TimeOfDay time] => time switch
    {
        TimeOfDay.Sunrise => this.Sunrise,
        TimeOfDay.Day => this.Day,
        TimeOfDay.Night => this.Night,
        TimeOfDay.Sunset => this.Sunset,
        TimeOfDay.EarlySunrise => this.EarlySunrise,
        TimeOfDay.LateSunrise => this.LateSunrise,
        TimeOfDay.EarlySunset => this.EarlySunset,
        TimeOfDay.LateSunset => this.LateSunset,
        _ => throw new NotImplementedException(),
    };
}
