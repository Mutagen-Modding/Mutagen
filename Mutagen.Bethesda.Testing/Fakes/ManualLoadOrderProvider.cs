using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Order.DI;
using Noggog;

namespace Mutagen.Bethesda.Testing.Fakes;

public class ManualLoadOrderProvider : ILoadOrderListingsProvider
{
    private readonly List<ILoadOrderListingGetter> _listings = new();

    public void SetTo(IEnumerable<ILoadOrderListingGetter> listings)
    {
        _listings.SetTo(listings);
    }
    
    public void SetTo(params ModKey[] listings)
    {
        _listings.SetTo(listings.Select(x => new LoadOrderListing(x, enabled: true)));
    }

    public IEnumerable<ILoadOrderListingGetter> Get()
    {
        return _listings;
    }
}