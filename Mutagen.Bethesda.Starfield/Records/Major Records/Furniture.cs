namespace Mutagen.Bethesda.Starfield;

partial class Furniture
{
    [Flags]
    public enum MajorFlag
    {
        HasContainer = 0x0000_0004,
        IsPerch = 0x0000_0080,
        HasDistantLod = 0x0000_8000,
        RandomAnimStart = 0x0001_0000,
        IsMarker = 0x0080_0000,
        PowerArmor = 0x0200_0000,
        MustExitToTalk = 0x1000_0000,
        ChildCanUse = 0x2000_0000
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
