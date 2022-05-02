using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Skyrim;

public partial class WeatherImageSpaces
{
    public IFormLink<IImageSpaceGetter> this[TimeOfDay time]
    {
        get => time switch
        {
            TimeOfDay.Sunrise => Sunrise,
            TimeOfDay.Day => Day,
            TimeOfDay.Night => Night,
            TimeOfDay.Sunset => Sunset,
            _ => throw new NotImplementedException(),
        };
    }

    IFormLinkGetter<IImageSpaceGetter> IWeatherImageSpacesGetter.this[TimeOfDay time] => this[time];
}

public partial interface IWeatherImageSpaces
{
    new IFormLink<IImageSpaceGetter> this[TimeOfDay time] { get; }
}

public partial interface IWeatherImageSpacesGetter
{
    IFormLinkGetter<IImageSpaceGetter> this[TimeOfDay time] { get; }
}

partial class WeatherImageSpacesBinaryOverlay
{
    public IFormLinkGetter<IImageSpaceGetter> this[TimeOfDay time] => time switch
    {
        TimeOfDay.Sunrise => Sunrise,
        TimeOfDay.Day => Day,
        TimeOfDay.Night => Night,
        TimeOfDay.Sunset => Sunset,
        _ => throw new NotImplementedException(),
    };
}