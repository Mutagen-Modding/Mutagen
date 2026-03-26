namespace Mutagen.Bethesda.FalloutNV;

public partial class Door
{
    [Flags]
    public enum MajorFlag
    {
        QuestItem = 0x0000_0400,
        VisibleWhenDistant = 0x0000_8000,
        RandomAnimStart = 0x0001_0000,
    }

    [Flags]
    public enum Flag
    {
        AutomaticDoor = 0x02,
        Hidden = 0x04,
        MinimalUse = 0x08,
        SlidingDoor = 0x10,
    }
}
