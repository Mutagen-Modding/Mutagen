using System;
using JetBrains.Annotations;

namespace Mutagen.Bethesda.Core.Pex.Enums
{
    [PublicAPI]
    [Flags]
    public enum FunctionFlags : byte
    {
        GlobalFunction = 0,
        NativeFunction = 1
    }
}
