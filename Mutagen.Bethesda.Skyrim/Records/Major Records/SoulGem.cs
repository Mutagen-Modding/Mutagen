namespace Mutagen.Bethesda.Skyrim;

public partial class SoulGem
{
    [Flags]
    public enum MajorFlag
    {
        CanHoldNpcSoul = 0x0002_0000
    }

    public enum Level
    {
        None,
        Petty,
        Lesser,
        Common,
        Greater,
        Grand
    }
}