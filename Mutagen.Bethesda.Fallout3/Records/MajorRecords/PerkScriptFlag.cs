namespace Mutagen.Bethesda.Fallout3;

public partial class PerkScriptFlag
{
    [Flags]
    public enum Flag
    {
        RunImmediately = 0x01,
        ReplaceDefault = 0x02,
    }
}
