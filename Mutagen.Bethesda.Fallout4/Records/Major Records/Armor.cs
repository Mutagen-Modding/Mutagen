using Mutagen.Bethesda.Plugins.Binary.Overlay;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using System;

namespace Mutagen.Bethesda.Fallout4
{
    public partial class Armor
    {
        [Flags]
        public enum MajorFlag
        {
            NonPlayable = 0x0000_0004,
            Shield = 0x0000_0040
        }
    }
}
