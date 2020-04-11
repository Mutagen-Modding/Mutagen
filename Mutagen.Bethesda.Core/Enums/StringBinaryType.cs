using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    /// <summary>
    /// Binary storage patterns for strings
    /// </summary>
    public enum StringBinaryType
    {
        /// <summary>
        /// No custom logic
        /// </summary>
        Plain,
        /// <summary>
        /// Terminated by an extra null character at the end
        /// </summary>
        NullTerminate,
        /// <summary>
        /// Length prepended as a uint
        /// </summary>
        PrependLength,
        /// <summary>
        /// Length prepended as a ushort
        /// </summary>
        PrependLengthUShort,
    }
}
