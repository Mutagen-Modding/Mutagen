using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mutagen.Bethesda.Fallout4;

public partial class MiscItem
{
    [Flags]
    public enum MajorFlag
    {
        CalcFromComponents = 11,
        PackInUseOnly = 13
    }
}