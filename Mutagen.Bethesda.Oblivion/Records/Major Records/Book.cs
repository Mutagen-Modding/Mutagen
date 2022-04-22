using System;

namespace Mutagen.Bethesda.Oblivion;

public partial class Book
{
    [Flags]
    public enum BookFlag
    {
        Scroll = 0x01,
        CantBeTaken = 0x02,
    }
}