namespace Mutagen.Bethesda.Plugins.Order.DI;

public interface IListingsProvider
{
    /// <summary>
    /// Parses the typical plugins file to retrieve all ModKeys in expected plugin file format,
    /// </summary>
    /// <returns>Enumerable of ModKeys representing a load order</returns>
    /// <exception cref="InvalidDataException">Line in plugin file is unexpected</exception>
    /// <exception cref="FileNotFoundException">If some required file is missing</exception>
    public IEnumerable<ILoadOrderListingGetter> Get();
}