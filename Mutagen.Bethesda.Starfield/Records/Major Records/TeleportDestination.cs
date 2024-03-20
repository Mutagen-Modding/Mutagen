namespace Mutagen.Bethesda.Starfield;

partial class TeleportDestination
{
    [Flags]
    public enum Flag
    {
        NoAlarm = 0x1,
        NoLoadScreen = 0x2,
        RelativePosition = 0x4,
    }
}
