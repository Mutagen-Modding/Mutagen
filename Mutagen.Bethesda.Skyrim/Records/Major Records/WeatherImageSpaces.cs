using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class WeatherImageSpaces
    {
        public FormLink<IImageSpaceAdapterGetter> this[TimeOfDay time]
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

    public partial interface IWeatherImageSpaces
    {
        new FormLink<IImageSpaceAdapterGetter> this[TimeOfDay time] { get; set; }
    }

    public partial interface IWeatherImageSpacesGetter
    {
        FormLink<IImageSpaceAdapterGetter> this[TimeOfDay time] { get; }
    }

    namespace Internals
    {
        public partial class WeatherImageSpacesBinaryOverlay
        {
            public FormLink<IImageSpaceAdapterGetter> this[TimeOfDay time] => time switch
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
