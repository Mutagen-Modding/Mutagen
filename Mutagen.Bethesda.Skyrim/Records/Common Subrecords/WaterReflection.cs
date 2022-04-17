using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim;

public partial class WaterReflection
{
    [Flags]
    public enum Flag
    {
        Reflection = 0x1,
        Refraction = 0x2,
    }
}