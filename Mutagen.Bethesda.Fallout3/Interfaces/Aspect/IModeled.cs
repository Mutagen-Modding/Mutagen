using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Fallout3;

/// <summary>
/// Common interface for records that have a model
/// </summary>
public interface IModeled : IModeledGetter, IMajorRecordQueryable
{
    new Model? Model { get; set; }
}

/// <summary>
/// Common interface for records that have a model
/// </summary>
public interface IModeledGetter : IMajorRecordQueryable
{
    IModelGetter? Model { get; }
}