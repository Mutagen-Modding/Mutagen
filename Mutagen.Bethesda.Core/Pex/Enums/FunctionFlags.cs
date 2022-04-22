using System;

namespace Mutagen.Bethesda.Pex;

[Flags]
public enum FunctionFlags : byte
{
    GlobalFunction = 0,
    NativeFunction = 1
}