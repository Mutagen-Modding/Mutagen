namespace Mutagen.Bethesda.FalloutNV;

public partial class StaticCollection
{
    [Flags]
    public enum MajorFlag
    {
        HasTreeLOD = 0x0000_0040,
        OnLocalMap = 0x0000_0200,
        QuestItem = 0x0000_0400,
        VisibleWhenDistant = 0x0000_8000,
        Obstacle = 0x0200_0000,
        NavMeshFilter = 0x0400_0000,
        NavMeshBoundingBox = 0x0800_0000,
        NavMeshGround = 0x4000_0000,
    }
}
