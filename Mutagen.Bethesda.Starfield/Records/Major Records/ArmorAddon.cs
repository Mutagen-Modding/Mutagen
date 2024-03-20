namespace Mutagen.Bethesda.Starfield;

partial class ArmorAddon
{
    [Flags]
    public enum MajorFlag
    {
        NoUnderarmorScaling = 0x0000_0040,
        IsSkin = 0x0000_0080,
        HiResFirstPersonOnly = 0x4000_0000
    }
}