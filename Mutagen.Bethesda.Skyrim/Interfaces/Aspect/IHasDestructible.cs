namespace Mutagen.Bethesda.Skyrim;

/// <summary>
/// Common interface for records that have destructible data
/// </summary>
public interface IHasDestructible : IHasDestructibleGetter
{
    new Destructible? Destructible { get; set; }
}

/// <summary>
/// Common interface for records that have destructible data
/// </summary>
public interface IHasDestructibleGetter
{
    IDestructibleGetter? Destructible { get; }
}