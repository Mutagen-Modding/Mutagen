using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    public partial class WeatherColor
    {
        public Color this[TimeOfDay time]
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

    public partial interface IWeatherColor
    {
        new Color this[TimeOfDay time] { get; set; }
    }

    public partial interface IWeatherColorGetter
    {
        Color this[TimeOfDay time] { get; }
    }

    namespace Internals
    {
        public partial class WeatherColorBinaryOverlay
        {
            public Color this[TimeOfDay time] => time switch
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
