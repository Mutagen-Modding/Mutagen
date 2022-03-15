using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    /// <summary>
    /// Common interface for records that have icons
    /// </summary>
    public interface IHasIcons : IHasIconsGetter
    {
        new Icons? Icons { get; set; }
    }

    /// <summary>
    /// Common interface for records that have icons
    /// </summary>
    public interface IHasIconsGetter
    {
        IIconsGetter? Icons { get; }
    }
}
