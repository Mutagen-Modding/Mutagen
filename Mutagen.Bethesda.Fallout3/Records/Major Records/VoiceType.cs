namespace Mutagen.Bethesda.Fallout3;

public partial class VoiceType
{
    [Flags]
    public enum Flag
    {
        AllowDefaultDialog = 0x01,
        Female = 0x02,
    }
}
