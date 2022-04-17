using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Skyrim;

public partial class Relationship
{
    [Flags]
    public enum MajorFlag
    {
        Secret = 0x40,
    }

    [Flags]
    public enum Flag
    {
        Secret = 0x80
    }

    public enum RankType
    {
        Lover,
        Ally,
        Confidant,
        Friend,
        Acquaintance,
        Rival,
        Foe,
        Enemy,
        Archnemesis,
    }
}