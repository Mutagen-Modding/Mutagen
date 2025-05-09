namespace Mutagen.Bethesda.Starfield;

public partial class Activator
{
    [Flags]
    public enum MajorFlag
    {
        HeadingMarker = 0x4,
        NonOccluder = 0x10,
        NeverFades = 0x40,
        MustUpdateAnims = 0x100,
        HiddenFromLocalMap = 0x200,
        HeadtrackMarker = 0x400,
        UsedasPlatform = 0x800,
        PackInUseOnly = 0x2000,
        HasDistantLOD = 0x8000,
        RandomAnimStart = 0x10000,
        Dangerous = 0x20000,
        IgnoreObjectInteraction = 0x100000,
        IsMarker = 0x800000,
        Obstacle = 0x2000000,
        NavmeshFilter = 0x4000000,
        NavmeshBoundingBox = 0x8000000,
        NavmeshOnlyCut = 0x10000000,
        NavmeshIgnoreErosion = 0x20000000,
        NavmeshGround = 0x40000000,
    }
    
    [Flags]
    public enum Flag
    {
        NoDisplacement = 0x01,
        IgnoredBySandbox = 0x02, 
        IsWater = 0x04,
        NonPlanar = 0x08,
        IsARadio = 0x10,
        AllowDisplacements = 0x20,
        UseGlobalProbe = 0x40,
    }
}