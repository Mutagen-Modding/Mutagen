using System;

namespace Mutagen.Bethesda.Skyrim;

public partial class AssociationType
{
    [Flags]
    public enum Flag
    {
        Family = 0x01
    }
}