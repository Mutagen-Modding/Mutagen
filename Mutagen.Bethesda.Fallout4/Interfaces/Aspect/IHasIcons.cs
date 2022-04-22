namespace Mutagen.Bethesda.Fallout4;

/// <summary>
/// Common interface for records that have icons
/// </summary>
public interface IHasIcons : IHasIconsGetter
{
    new Icons? Icons { get; set; }
}

/// <summary>
/// Common interface for records that have icons
/// </summary>
public interface IHasIconsGetter
{
    IIconsGetter? Icons { get; }
}