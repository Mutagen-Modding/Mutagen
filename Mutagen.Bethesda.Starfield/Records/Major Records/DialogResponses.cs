namespace Mutagen.Bethesda.Starfield;

partial class DialogResponses
{
    [Flags]
    public enum MajorFlag
    {
        InfoGroup = 0x0000_0040,
        ExcludeFromExport = 0x0000_0080,
        ActorChanged = 0x0000_2000
    }

    [Flags]
    public enum Flag
    {
        Random = 0x2,
        ForceAllChildrenPlayerActivateOnly = 0x8,
        RandomEnd = 0x20,
        ChildInfosDontInheritResetData = 0x100,
        ForceAllChildrenRandom = 0x200,
        DoNotDoAllBeforeRepeating = 0x800,
    }
    
    public enum SubtitlePriorityLevel
    {
        Low = 1,
        Normal = 2,
        Force = 4,
    }
}