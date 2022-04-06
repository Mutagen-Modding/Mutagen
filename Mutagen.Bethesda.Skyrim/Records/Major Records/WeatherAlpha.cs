using System;

namespace Mutagen.Bethesda.Skyrim;

public partial class WeatherAlpha
{
    public float this[TimeOfDay time]
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

public partial interface IWeatherAlpha
{
    new float this[TimeOfDay time] { get; set; }
}

public partial interface IWeatherAlphaGetter
{
    float this[TimeOfDay time] { get; }
}

partial class WeatherAlphaBinaryOverlay
{
    public float this[TimeOfDay time] => time switch
    {
        TimeOfDay.Sunrise => this.Sunrise,
        TimeOfDay.Day => this.Day,
        TimeOfDay.Night => this.Night,
        TimeOfDay.Sunset => this.Sunset,
        _ => throw new NotImplementedException(),
    };
}