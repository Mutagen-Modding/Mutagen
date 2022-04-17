using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mutagen.Bethesda.Skyrim;

public partial class MiscItem
{
    [Flags]
    public enum MajorFlag
    {
        NonPlayable = 0x4
    }
}