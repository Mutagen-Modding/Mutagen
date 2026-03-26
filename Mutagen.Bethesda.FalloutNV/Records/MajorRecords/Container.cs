namespace Mutagen.Bethesda.FalloutNV;

public partial class Container
{
    [Flags]
    public enum MajorFlag
    {
        QuestItem = 0x0000_0400,
        RandomAnimStart = 0x0001_0000,
        Obstacle = 0x0200_0000,
        NavMeshFilter = 0x0400_0000,
        NavMeshBoundingBox = 0x0800_0000,
        NavMeshGround = 0x4000_0000,
    }

    [Flags]
    public enum ContainerFlag : byte
    {
        Respawns = 0x02,
    }
}
