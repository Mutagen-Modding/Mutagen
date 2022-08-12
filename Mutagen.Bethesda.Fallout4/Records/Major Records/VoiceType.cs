namespace Mutagen.Bethesda.Fallout4;

public partial class VoiceType
{
    [Flags]
    public enum Flag
    {
        AllowDefaultDialog = 0x01,
        Female = 0x02,
    }
}