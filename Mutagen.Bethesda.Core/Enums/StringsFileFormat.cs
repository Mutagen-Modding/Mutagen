using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda
{
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
}
