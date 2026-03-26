namespace Mutagen.Bethesda.FalloutNV;

public partial class PlaceableWater
{
    [Flags]
    public enum WaterFlag : uint
    {
        Reflects = 0x0000_0001,
        ReflectsActors = 0x0000_0002,
        ReflectsLand = 0x0000_0004,
        ReflectsLODLand = 0x0000_0008,
        ReflectsLODBuildings = 0x0000_0010,
        ReflectsTrees = 0x0000_0020,
        ReflectsSky = 0x0000_0040,
        ReflectsDynamicObjects = 0x0000_0080,
        ReflectsDeadBodies = 0x0000_0100,
        Refracts = 0x0000_0200,
        RefractsActors = 0x0000_0400,
        RefractsLand = 0x0000_0800,
        RefractsDynamicObjects = 0x0001_0000,
        RefractsDeadBodies = 0x0002_0000,
        SilhouetteReflections = 0x0004_0000,
        Depth = 0x1000_0000,
        ObjectTextureCoordinates = 0x2000_0000,
        NoUnderwaterFog = 0x8000_0000,
    }
}
