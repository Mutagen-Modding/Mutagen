using Mutagen.Bethesda.Starfield.Internals;

namespace Mutagen.Bethesda.Starfield;

public partial class Container
{
    [Flags]
    public enum MajorFlag
    {
        HasDistantLod = 0x0000_8000,
        RandomAnimStart = 0x0001_0000,
        Obstacle = 0x0200_0000,
        NavMeshFilter = 0x0400_0000,
        NavMeshBoundingBox = 0x0800_0000,
        NavMeshOnlyCut = 0x1000_0000,
        NavMeshIgnoreErosion = 0x1000_0000,
        NavMeshGround = 0x4000_0000
    }

    [Flags]
    public enum Flag
    {
        AllowSoundsWhenAnimation = 0x01,
        Respawns = 0x02,
        ShowOwner = 0x04
    }

    public enum Property
    {
        Keyword = RecordTypeInts.CKEY,
    }
}