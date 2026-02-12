namespace Mutagen.Bethesda.Fallout3;

public partial class Activator
{
    [Flags]
    public enum MajorFlag
    {
        HasTreeLOD = 0x0000_0040,
        OnLocalMap = 0x0000_0200,
        QuestItem = 0x0000_0400,
        VisibleWhenDistant = 0x0000_8000,
        RandomAnimStart = 0x0001_0000,
        Dangerous = 0x0002_0000,
        HasPlatformSpecificTextures = 0x0008_0000,
        Obstacle = 0x0200_0000,
        NavMeshFilter = 0x0400_0000,
        NavMeshBoundingBox = 0x0800_0000,
        ChildCanUse = 0x2000_0000,
        NavMeshGround = 0x4000_0000,
    }
}
