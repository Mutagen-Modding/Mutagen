using System;

namespace Mutagen.Bethesda.Skyrim;

public partial class ActivateParents
{
    [Flags]
    public enum Flag
    {
        ParentActivateOnly = 0x01
    }
}