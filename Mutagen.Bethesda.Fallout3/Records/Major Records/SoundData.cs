namespace Mutagen.Bethesda.Fallout3;

public partial class SoundData
{
    [Flags]
    public enum Flag
    {
        RandomFrequencyShift = 0x0001,
        PlayAtRandom = 0x0002,
        EnvironmentIgnored = 0x0004,
        RandomLocation = 0x0008,
        Loop = 0x0010,
        MenuSound = 0x0020,
        TwoDimensional = 0x0040,
        LFE360 = 0x0080,
        DialogueSound = 0x0100,
        EnvelopeFast = 0x0200,
        EnvelopeSlow = 0x0400,
        TwoDimensionalRadius = 0x0800,
        MuteWhenSubmerged = 0x1000,
        StartAtRandomPosition = 0x2000,
    }
}
