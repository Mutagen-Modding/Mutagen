namespace Mutagen.Bethesda.Fallout3;

public partial class Static
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

    public enum PassthroughSoundType : byte
    {
        None = 0xFF,
        BushA = 0,
        BushB = 1,
        BushC = 2,
        BushD = 3,
        BushE = 4,
        BushF = 5,
        BushG = 6,
        BushH = 7,
        BushI = 8,
        BushJ = 9,
    }
}
