using System;

namespace Mutagen.Bethesda.Skyrim;

public partial class DialogBranch
{
    [Flags]
    public enum Flag
    {
        TopLevel = 0x01,
        Blocking = 0x02,
        Exclusive = 0x04,
    }
}