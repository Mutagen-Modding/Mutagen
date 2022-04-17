using System;

namespace Mutagen.Bethesda.Strings;

public enum StringsFileFormat
{
    /// <summary>
    /// .strings format
    /// </summary>
    Normal,

    /// <summary>
    /// .dlstrings and .ilstrings format
    /// </summary>
    LengthPrepended,
}