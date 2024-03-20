namespace Mutagen.Bethesda.Starfield;

public partial class PerkEntryPointAddActivateChoice
{
    [Flags]
    public enum Flag
    {
        RunImmediately = 0x01,
        ReplaceDefault = 0x02,
    }
}