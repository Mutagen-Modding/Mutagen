namespace Mutagen.Bethesda.Fallout4;

partial class Relationship
{
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

    [Flags]
    public enum Flag
    {
        Secret = 0x80
    }
}
