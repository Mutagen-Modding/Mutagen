namespace Mutagen.Bethesda.Fallout3;

public partial class Furniture
{
    [Flags]
    public enum MajorFlag
    {
        QuestItem = 0x0000_0400,
        RandomAnimStart = 0x0001_0000,
        ChildCanUse = 0x2000_0000,
    }
}
