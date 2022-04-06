using System;

namespace Mutagen.Bethesda.Skyrim;

public partial class LandscapeTexture
{
    [Flags]
    public enum Flag
    {
        /// <summary>
        /// SSE Only
        /// </summary>
        IsSnow = 0x01,
    }
}