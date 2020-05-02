using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim
{
    /// <summary>
    /// Common interface for records that have icons
    /// </summary>
    public interface IIcons : IIconsGetter
    {
        new string? LargeIconFilename { get; set; }
    }

    /// <summary>
    /// Common interface for records that have icons
    /// </summary>
    public interface IIconsGetter
    {
        string? LargeIconFilename { get; }
    }
}
