namespace Mutagen.Bethesda.Starfield;

partial class Planet
{
    public enum BodyTypeEnum
    {
        Undefined = 0,
        Star = 1,
        Planet = 2,
        Moon = 3,
        Orbital = 4,
        AsteroidBelt = 5,
        Station = 6,
    }

    [Flags]
    public enum PlayerKnowledgeFlag
    {
        InitialScan = 0x0000_0001,
        Visited = 0x0000_0002,
        EnteredSystem = 0x0000_0004,
    }
}
