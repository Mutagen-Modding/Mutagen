using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Oblivion;

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
public interface IModeledGetter : IMajorRecordQueryableGetter
{
    IModelGetter? Model { get; }
}