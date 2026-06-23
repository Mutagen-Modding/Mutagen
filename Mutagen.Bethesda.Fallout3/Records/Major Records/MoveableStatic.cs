namespace Mutagen.Bethesda.Fallout3;

public partial class MoveableStatic
{
    [Flags]
    public enum MajorFlag
    {
        OnLocalMap = 0x0000_0200,
        QuestItem = 0x0000_0400,
        RandomAnimStart = 0x0001_0000,
        Obstacle = 0x0200_0000,
    }
}
