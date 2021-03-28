using System;

namespace Mutagen.Bethesda.Core.Pex.Enums
{
    [Flags]
    public enum FunctionFlags : byte
    {
        GlobalFunction = 0,
        NativeFunction = 1
    }
}
