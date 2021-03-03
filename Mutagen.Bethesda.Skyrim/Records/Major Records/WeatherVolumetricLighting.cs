using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class WeatherVolumetricLighting
    {
        public IFormLink<IVolumetricLightingGetter> this[TimeOfDay time]
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

        IFormLinkGetter<IVolumetricLightingGetter> IWeatherVolumetricLightingGetter.this[TimeOfDay time] => this[time];
    }

    public partial interface IWeatherVolumetricLighting
    {
        new IFormLink<IVolumetricLightingGetter> this[TimeOfDay time] { get; }
    }

    public partial interface IWeatherVolumetricLightingGetter
    {
        IFormLinkGetter<IVolumetricLightingGetter> this[TimeOfDay time] { get; }
    }

    namespace Internals
    {
        public partial class WeatherVolumetricLightingBinaryOverlay
        {
            public IFormLinkGetter<IVolumetricLightingGetter> this[TimeOfDay time] => time switch
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
