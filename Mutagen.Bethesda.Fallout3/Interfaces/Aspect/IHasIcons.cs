using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Fallout3;

/// <summary>
/// Common interface for records that have icons
/// </summary>
public interface IHasIcons : IHasIconsGetter, IMajorRecordQueryable
{
    new Icons? Icons { get; set; }
}

/// <summary>
/// Common interface for records that have icons
/// </summary>
public interface IHasIconsGetter : IMajorRecordQueryableGetter
{
    IIconsGetter? Icons { get; }
}