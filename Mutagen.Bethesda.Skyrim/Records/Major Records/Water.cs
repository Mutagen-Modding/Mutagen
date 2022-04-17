using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mutagen.Bethesda.Skyrim;

public partial class Water
{
    [Flags]
    public enum Flag
    {
        CausesDamage = 0x01,

        /// <summary>
        /// SSE only
        /// </summary>
        EnableFlowmap = 0x02,

        /// <summary>
        /// SSE only
        /// </summary>
        BlendNormals = 0x04,
    }
}