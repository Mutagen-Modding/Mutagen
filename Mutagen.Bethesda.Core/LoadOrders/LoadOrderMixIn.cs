using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using DynamicData;
using Mutagen.Bethesda.LoadOrders;

namespace Mutagen.Bethesda
{
    public static class LoadOrderMixIn
    {
        public static IEnumerable<ModKey> OrderListings(this IEnumerable<ModKey> e)
        {
            return LoadOrder.OrderListings(e, i => i);
        }

        public static IEnumerable<LoadOrderListing> OrderListings(this IEnumerable<LoadOrderListing> e)
        {
            return LoadOrder.OrderListings(e, i => i.ModKey);
        }

        public static IObservable<IChangeSet<ModKey>> OrderListings(this IObservable<IChangeSet<ModKey>> e)
        {
            return e.Sort(ModKey.ByTypeComparer);
        }

        public static IObservable<IChangeSet<LoadOrderListing>> OrderListings(this IObservable<IChangeSet<LoadOrderListing>> e)
        {
            return ObservableListEx.Sort(e, LoadOrderListing.GetComparer(ModKey.ByTypeComparer));
        }
        public static bool TryGetIfEnabled<TMod>(this LoadOrder<IModListing<TMod>> loadOrder, ModKey modKey, [MaybeNullWhen(false)] out IModListing<TMod> item)
            where TMod : class, IModGetter
        {
            if (loadOrder.TryGetValue(modKey, out var listing)
                && listing.Enabled)
            {
                item = listing;
                return true;
            }

            item = default;
            return false;
        }

        public static IModListing<TMod> GetIfEnabled<TMod>(this LoadOrder<IModListing<TMod>> loadOrder, ModKey modKey)
            where TMod : class, IModGetter
        {
            if (TryGetIfEnabled(loadOrder, modKey, out var listing))
            {
                return listing;
            }
            throw new MissingModException(modKey);
        }

        public static bool TryGetIfEnabledAndExists<TMod>(this LoadOrder<IModListing<TMod>> loadOrder, ModKey modKey, [MaybeNullWhen(false)] out TMod item)
            where TMod : class, IModGetter
        {
            if (!TryGetIfEnabled(loadOrder, modKey, out var listing))
            {
                item = default;
                return false;
            }

            item = listing.Mod;
            return item != null;
        }

        public static TMod GetIfEnabledAndExists<TMod>(this LoadOrder<IModListing<TMod>> loadOrder, ModKey modKey)
            where TMod : class, IModGetter
        {
            if (TryGetIfEnabledAndExists(loadOrder, modKey, out var mod))
            {
                return mod;
            }
            throw new MissingModException(modKey);
        }
    }
}
