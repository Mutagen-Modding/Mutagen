namespace Mutagen.Bethesda.Skyrim;

public partial class WeatherAmbientColorSet
{
    IAmbientColorsGetter IWeatherAmbientColorSetGetter.this[TimeOfDay time] => time switch
    {
        TimeOfDay.Sunrise => this.Sunrise,
        TimeOfDay.Day => this.Day,
        TimeOfDay.Night => this.Night,
        TimeOfDay.Sunset => this.Sunset,
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

    public IAmbientColorsGetter this[TimeOfDay time] => time switch
    {
        TimeOfDay.Sunrise => this.Sunrise,
        TimeOfDay.Day => this.Day,
        TimeOfDay.Night => this.Night,
        TimeOfDay.Sunset => this.Sunset,
        _ => throw new NotImplementedException(),
    };
}