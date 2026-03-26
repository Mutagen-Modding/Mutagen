namespace Mutagen.Bethesda.Fallout76;

public partial class VoiceType
{
    [Flags]
    public enum MajorFlag
    {
    }

    [Flags]
    public enum Flag
    {
        AllowDefaultDialog = 0x01,
        Female = 0x02,
    }
}