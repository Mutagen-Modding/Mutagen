using System;
using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class WeatherImageSpaces
    {
        public IFormLink<IImageSpaceAdapterGetter> this[TimeOfDay time]
        {
            get => time switch
            {
                TimeOfDay.Sunrise => this.Sunrise,
                TimeOfDay.Day => this.Day,
                TimeOfDay.Night => this.Night,
                TimeOfDay.Sunset => this.Sunset,
                _ => throw new NotImplementedException(),
            };
        }

        IFormLinkGetter<IImageSpaceAdapterGetter> IWeatherImageSpacesGetter.this[TimeOfDay time] => this[time];
    }

    public partial interface IWeatherImageSpaces
    {
        new IFormLink<IImageSpaceAdapterGetter> this[TimeOfDay time] { get; }
    }

    public partial interface IWeatherImageSpacesGetter
    {
        IFormLinkGetter<IImageSpaceAdapterGetter> this[TimeOfDay time] { get; }
    }

    namespace Internals
    {
        public partial class WeatherImageSpacesBinaryOverlay
        {
            public IFormLinkGetter<IImageSpaceAdapterGetter> this[TimeOfDay time] => time switch
            {
                TimeOfDay.Sunrise => this.Sunrise,
                TimeOfDay.Day => this.Day,
                TimeOfDay.Night => this.Night,
                TimeOfDay.Sunset => this.Sunset,
                _ => throw new NotImplementedException(),
            };
        }
    }
}
