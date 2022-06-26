namespace Mutagen.Bethesda.Fallout4;

public partial class RegionData
{
    [Flags]
    public enum RegionDataFlag
    {
        Override = 0x01
    }

    public enum RegionDataType
    {
        Object = 2,
        Weather = 3,
        Map = 4,
        Land = 5,
        Grass = 6,
        Sound = 7,
        Imposter = 8,
    }

    public abstract float? LodDisplayDistanceMultiplier { get; set; }
    public abstract float? OcclusionAccuracyDist { get; set; }
}

partial interface IRegionDataGetter
{
    float? LodDisplayDistanceMultiplier { get; }
    float? OcclusionAccuracyDist { get; }
}

partial interface IRegionData
{
    float? LodDisplayDistanceMultiplier { get; set; }
    float? OcclusionAccuracyDist { get; set; }
}

partial class RegionDataBinaryOverlay
{
    public abstract float? LodDisplayDistanceMultiplier { get; }
    public abstract float? OcclusionAccuracyDist { get; }
}