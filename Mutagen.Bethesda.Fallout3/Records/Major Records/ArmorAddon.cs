namespace Mutagen.Bethesda.Fallout3;

public partial class ArmorAddon
{
    [Flags]
    public enum Flag : ushort
    {
        ModulatesVoice = 0x0001,
    }
}
