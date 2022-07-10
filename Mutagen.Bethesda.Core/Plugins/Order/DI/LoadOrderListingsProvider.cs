namespace Mutagen.Bethesda.Plugins.Order.DI;

public interface ILoadOrderListingsProvider : IListingsProvider
{
}

public sealed class LoadOrderListingsProvider : ILoadOrderListingsProvider
{
    private readonly IOrderListings _orderListings;
    private readonly IImplicitListingsProvider _implicitListingsProvider;
    private readonly IPluginListingsProvider _pluginListingsProvider;
    private readonly ICreationClubListingsProvider _cccListingsProvider;

    public LoadOrderListingsProvider(
        IOrderListings orderListings,
        IImplicitListingsProvider implicitListingsProvider,
        IPluginListingsProvider pluginListingsProvider,
        ICreationClubListingsProvider cccListingsProvider)
    {
        _orderListings = orderListings;
        _implicitListingsProvider = implicitListingsProvider;
        _pluginListingsProvider = pluginListingsProvider;
        _cccListingsProvider = cccListingsProvider;
    }
        
    /// <inheritdoc />
    public IEnumerable<ILoadOrderListingGetter> Get()
    {
        var implicitListings = _implicitListingsProvider.Get().ToArray();
        return _orderListings.Order(
            implicitListings: implicitListings,
            pluginsListings: _pluginListingsProvider.Get().Except(implicitListings),
            creationClubListings: _cccListingsProvider.Get(throwIfMissing: false),
            selector: x => x.ModKey);
    }
}

public sealed class LoadOrderListingsInjection : ILoadOrderListingsProvider
{
    private ILoadOrderListingGetter[] _listings;
        
    public LoadOrderListingsInjection(IEnumerable<ILoadOrderListingGetter> listings)
    {
        _listings = listings.ToArray();
    }
        
    public LoadOrderListingsInjection(params ILoadOrderListingGetter[] listings)
    {
        _listings = listings;
    }
        
    public LoadOrderListingsInjection(params ModKey[] keys)
    {
        _listings = keys
            .Select<ModKey, ILoadOrderListingGetter>(x => new LoadOrderListing(x, true))
            .ToArray();
    }
        
    public IEnumerable<ILoadOrderListingGetter> Get() => _listings;
}