namespace Mutagen.Bethesda.Fallout76;

public partial class IdleAnimation
{
    [Flags]
    public enum MajorFlag
    {
    }

    [Flags]
    public enum Flag
    {
        Parent = 0x01,
        Sequence = 0x02,
        NoAttacking = 0x04,
        Blocking = 0x08,
    }
}
