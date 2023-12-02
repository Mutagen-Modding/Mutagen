using Noggog;

namespace Mutagen.Bethesda.Plugins.Order.DI;

/// <summary>
/// Orders objects by their associated ModKeys
/// </summary>
public interface IOrderListings
{
    /// <summary>
    /// Orders a given set of objects
    /// </summary>
    /// <param name="e">Objects to order</param>
    /// <param name="selector">How to retrieve a ModKey from them</param>
    /// <returns>Ordered objects</returns>
    IEnumerable<T> Order<T>(IEnumerable<T> e, Func<T, ModKey> selector);

    /// <summary>
    /// Orders given sets of objects associated with different sources
    /// </summary>
    /// <param name="implicitListings">Objects associated with implicit listings</param>
    /// <param name="pluginsListings">Objects associated with plugin listings</param>
    /// <param name="creationClubListings">Objects associated with creation club listings</param>
    /// <param name="selector">How to retrieve a ModKey from them</param>
    /// <returns>Ordered objects</returns>
    IEnumerable<T> Order<T>(
        IEnumerable<T> implicitListings,
        IEnumerable<T> pluginsListings,
        IEnumerable<T> creationClubListings,
        Func<T, ModKey> selector);
}

public sealed class OrderListings : IOrderListings
{
    /// <inheritdoc />
    public IEnumerable<T> Order<T>(IEnumerable<T> e, Func<T, ModKey> selector)
    {
        return e.OrderBy(e => selector(e).Type);
    }

    /// <inheritdoc />
    public IEnumerable<T> Order<T>(
        IEnumerable<T> implicitListings,
        IEnumerable<T> pluginsListings,
        IEnumerable<T> creationClubListings,
        Func<T, ModKey> selector)
    {
        var plugins = pluginsListings
            .Select(selector)
            .ToList();
        return implicitListings
            .Concat(
                Order(creationClubListings
                    // If CC mod is on plugins list, refer to its ordering
                    .OrderBy(selector, Comparer<ModKey>.Create((x, y) =>
                    {
                        var xIndex = plugins.IndexOf(x);
                        var yIndex = plugins.IndexOf(y);
                        if (xIndex == yIndex) return 0;
                        return xIndex - yIndex;
                    })), selector))
            .Concat(pluginsListings)
            .Distinct(selector);
    }
}