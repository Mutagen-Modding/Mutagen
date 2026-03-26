namespace Mutagen.Bethesda.FalloutNV;

public partial class VoiceType
{
    [Flags]
    public enum VoiceTypeFlag
    {
        AllowDefaultDialog = 0x01,
        Female = 0x02,
    }
}
