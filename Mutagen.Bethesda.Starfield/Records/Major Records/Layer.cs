namespace Mutagen.Bethesda.Starfield;

public partial class Layer
{
    [Flags]
    public enum MajorFlag
    {
        StartsFrozen = 0x0800_0000,
    }

    public enum LodBehaviorType
    {
        Default,
        Aggregate,
        NoLods,
    }
}