using System;

namespace Mutagen.Bethesda.Fallout4;

public partial class Key
{
    [Flags]
    public enum MajorFlag
    {
        CalcValueFromComponents = 0x0000_0800,
        PackInUseOnly = 0x0000_2000
    }
}