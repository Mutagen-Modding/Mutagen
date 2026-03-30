namespace Mutagen.Bethesda.Fallout3;

public partial class Armor
{
    [Flags]
    public enum MajorFlag
    {
        QuestItem = 0x0000_0400,
        HasPlatformSpecificTextures = 0x0008_0000,
    }
}

public partial class ArmorAnimationSound
{
    public enum SoundType : uint
    {
        Walk = 17,
        Sneak = 18,
        Run = 19,
        SneakArmor = 20,
        RunArmor = 21,
        WalkArmor = 22,
    }
}
