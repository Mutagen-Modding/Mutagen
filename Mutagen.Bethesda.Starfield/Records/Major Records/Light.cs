namespace Mutagen.Bethesda.Starfield;

partial class Light
{
    [Flags]
    public enum MajorFlag
    {
        RandomAnimStart = 0x0001_0000,
        Obstacle = 0x0200_0000,
        PortalStrict = 0x1000_0000
    }
}