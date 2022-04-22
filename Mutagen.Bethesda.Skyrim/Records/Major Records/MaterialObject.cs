using System;

namespace Mutagen.Bethesda.Skyrim;

public partial class MaterialObject
{
    [Flags]
    public enum Flag : ulong
    {
        SinglePass = 0x01,
    }
}