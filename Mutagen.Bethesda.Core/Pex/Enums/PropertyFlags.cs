using System;

namespace Mutagen.Bethesda.Pex
{
    [Flags]
    public enum PropertyFlags : byte
    {
        Read = 1,
        Write = 2,
        AutoVar = 4
    }
}
