namespace Mutagen.Bethesda.Starfield;

partial class Furniture
{
    [Flags]
    public enum MajorFlag
    {
        HeadingMarker = 0x4,
        NonOccluder = 0x10,
        NeverFades = 0x40,
        IsTemplate = 0x80,
        MustUpdateAnims = 0x100,
        HiddenFromLocalMap = 0x200,
        HeadtrackMarker = 0x400,
        UsedasPlatform = 0x800,
        PackInUseOnly = 0x2000,
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
        IgnoredBySandbox = 0000_0002,
    }
    
    public enum BenchTypes
    {
        None = 0,
        CreateObject = 1,
        Weapons = 2,
        Enchanting = 3,
        EnchantingExperiment = 4,
        Alchemy = 5,
        AlchemyExperiment = 6,
        Armor = 7,
        PowerArmor = 8,
        RobotMod = 9,
        Research = 11,
    }

    [Flags]
    public enum EntryPointType
    {
        Front = 0x01,
        Behind = 0x02,
        Right = 0x04,
        Left = 0x08,
        Up = 0x10
    }

    [Flags]
    public enum EntryParameterType
    {
        Front = 0x01,
        Behind = 0x02,
        Right = 0x04,
        Left = 0x08,
        Other = 0x10
    }

    [Flags]
    public enum AnimationType
    {
        Sit = 1,
        Lay = 2,
        Lean = 4,
    }
}
