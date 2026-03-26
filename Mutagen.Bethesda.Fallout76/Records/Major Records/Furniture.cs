using System;

namespace Mutagen.Bethesda.Fallout76;

public partial class Furniture
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
    public enum Flag : uint
    {
        IgnoredBySandbox = 0x0000_0002,
        AllowAwakeSound = 0x0040_0000,
        EnterWithWeaponDrawn = 0x0080_0000,
        PlayAnimWhenFull = 0x0100_0000,
        DisablesActivation = 0x0200_0000,
        IsPerch = 0x0400_0000,
        MustExitToTalk = 0x0800_0000,
        UseStaticAvoidNode = 0x1000_0000,
        HasModel = 0x4000_0000,
        IsSleepFurniture = 0x8000_0000
    }

    public enum BenchType
    {
        None = 0,
        CreateObject = 1,
        Weapons = 2,
        EnchantingUnused = 3,
        EnchantingExperimentUnused = 4,
        Alchemy = 5,
        AlchemyExperimentUnused = 6,
        Armor = 7,
        PowerArmor = 8,
        RobotMod = 9,
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

    public enum Property
    {
    }
}