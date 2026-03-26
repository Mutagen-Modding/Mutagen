namespace Mutagen.Bethesda.FalloutNV;

public partial class Light
{
    [Flags]
    public enum MajorFlag
    {
        QuestItem = 0x0000_0400,
        RandomAnimStart = 0x0001_0000,
        Obstacle = 0x0200_0000,
    }

    [Flags]
    public enum LightFlag
    {
        Dynamic = 0x0001,
        CanBeCarried = 0x0002,
        Negative = 0x0004,
        Flicker = 0x0008,
        OffByDefault = 0x0020,
        FlickerSlow = 0x0040,
        Pulse = 0x0080,
        PulseSlow = 0x0100,
        SpotLight = 0x0200,
        SpotShadow = 0x0400,
    }
}
