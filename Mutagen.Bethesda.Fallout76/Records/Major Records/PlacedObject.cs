using System;

namespace Mutagen.Bethesda.Fallout76;

partial class PlacedObject
{
    [Flags]
    public enum MajorFlag : uint
    {
        GroundPiece = 0x0000_0010,
        LodRespectsEnableState = 0x0000_0100,
        Persistent = 0x0000_0400,
        InitiallyDisabled = 0x0000_0800,
        IsFullLod = 0x0001_0000,
        FilterCollisionGeometry = 0x0400_0000,
        BoundingBoxCollisionGeometry = 0x0800_0000,
        ReflectedByAutoWater = 0x1000_0000,
        Ground = 0x4000_0000,
        MultiBound = 0x8000_0000,
    }

    public enum ActionFlag
    {
        UseDefault,
        Activate,
        Open,
        OpenByDefault,
    }
}
