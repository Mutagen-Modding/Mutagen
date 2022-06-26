namespace Mutagen.Bethesda.Fallout4;

partial class SoundCategory
{
    [Flags]
    public enum Flag
    {
        MuteWhenSubmerged = 0x01,
        ShouldAppearOnMenu = 0x02,
        ImmuneToTimeSpeedup = 0x04,
        PauseDuringMenusImmediately = 0x08,
        PauseDuringMenusFade = 0x10,
        ExcludeFromPlayerOpmOverride = 0x20,
        PauseDuringStartMenu = 0x40,
    }
}