using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Plugins.Aspects;

public interface IWeightValue : IWeightValueGetter, IMajorRecordQueryable
{
    new uint Value { get; set; }
    new float Weight { get; set; }
}

public interface IWeightValueGetter : IMajorRecordQueryableGetter
{
    uint Value { get; }
    float Weight { get; }
}