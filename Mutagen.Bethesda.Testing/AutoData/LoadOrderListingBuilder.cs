using AutoFixture;
using AutoFixture.Kernel;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Order;

namespace Mutagen.Bethesda.Testing.AutoData;

public class LoadOrderListingBuilder : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        if (request is MultipleRequest mult)
        {
            var req = mult.Request;
            if (req is SeededRequest seed)
            {
                req = seed.Request;
            }
            if (req is Type t)
            {
                if (t == typeof(LoadOrderListing))
                {
                    var keys = context.CreateMany<ModKey>();
                    return keys.Select(x => new LoadOrderListing(x, true));
                }
                if (t == typeof(ILoadOrderListingGetter))
                {
                    var keys = context.CreateMany<ModKey>();
                    return keys.Select(x => (ILoadOrderListingGetter)new LoadOrderListing(x, true));
                }
            }
        }
        else if (request is Type t)
        {
            if (t == typeof(LoadOrderListing)
                || t == typeof(ILoadOrderListingGetter))
            {
                return new LoadOrderListing(TestConstants.PluginModKey, true);
            }
        }
            
        return new NoSpecimen();
    }
}