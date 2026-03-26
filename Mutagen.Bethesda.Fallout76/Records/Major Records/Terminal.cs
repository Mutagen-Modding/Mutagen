using System;

namespace Mutagen.Bethesda.Fallout76;

partial class Terminal
{
    [Flags]
    public enum MajorFlag
    {
        HasDistantLod = 0x0000_8000,
        RandomAnimStart = 0x0001_0000
    }
}
