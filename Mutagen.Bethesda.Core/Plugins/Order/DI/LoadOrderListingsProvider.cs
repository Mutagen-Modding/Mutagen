using System.Collections.Generic;
using System.Linq;

namespace Mutagen.Bethesda.Plugins.Order.DI
{
    public interface ILoadOrderListingsProvider : IListingsProvider
    {
    }

    public class LoadOrderListingsProvider : ILoadOrderListingsProvider
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
        public IEnumerable<IModListingGetter> Get()
        {
            var implicitListings = _implicitListingsProvider.Get().ToArray();
            return _orderListings.Order(
                implicitListings: implicitListings,
                pluginsListings: _pluginListingsProvider.Get().Except(implicitListings),
                creationClubListings: _cccListingsProvider.Get(throwIfMissing: false),
                selector: x => x.ModKey);
        }
    }

    public class LoadOrderListingsInjector : ILoadOrderListingsProvider
    {
        private IModListingGetter[] _listings;
        
        public LoadOrderListingsInjector(IEnumerable<IModListingGetter> listings)
        {
            _listings = listings.ToArray();
        }
        
        public LoadOrderListingsInjector(params IModListingGetter[] listings)
        {
            _listings = listings;
        }
        
        public LoadOrderListingsInjector(params ModKey[] keys)
        {
            _listings = keys
                .Select<ModKey, IModListingGetter>(x => new ModListing(x, true))
                .ToArray();
        }
        
        public IEnumerable<IModListingGetter> Get() => _listings;
    }
}