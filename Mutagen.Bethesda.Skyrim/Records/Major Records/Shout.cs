using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mutagen.Bethesda.Skyrim;

public partial class Shout
{
    [Flags]
    public enum MajorFlag
    {
        TreatSpellsAsPowers = 0x80
    }
}