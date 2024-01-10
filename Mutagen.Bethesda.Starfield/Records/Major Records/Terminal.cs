namespace Mutagen.Bethesda.Starfield;

partial class Terminal
{
    [Flags]
    public enum MajorFlag
    {
        HasDistantLod = 0x0000_8000,
        RandomAnimStart = 0x0001_0000
    }

    public enum BackgroundType
    {
        Constellation,
        FreestarCollective,
        Default,
        NASA,
        RyujinIndustries,
        SlaytonAerospace,
        UnitedColonies,
        CrimsonFleet,
    }
}