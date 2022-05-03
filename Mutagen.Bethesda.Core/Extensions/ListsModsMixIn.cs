using System.Diagnostics.CodeAnalysis;
using DynamicData;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;

namespace Mutagen.Bethesda;

public static class ListsModsMixIn
{
    /// <summary>
    /// Checks whether a given mod is in the collection of listings.
    /// </summary>
    /// <param name="listings">Listings to look through</param>
    /// <param name="modKey">ModKey to look for</param>
    /// <param name="enabled">Whether the ModKey should be enabled/disabled.  Default is no preference</param>
    /// <returns>True if ModKey is in the listings, with the desired enabled state</returns>
    public static bool ListsMod(this IEnumerable<ILoadOrderListingGetter> listings, ModKey modKey, bool? enabled = null)
    {
        foreach (var listing in listings)
        {
            if (listing.ModKey == modKey
                && (enabled == null || listing.Enabled == enabled))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Asserts a given mod is in the collection of listings.
    /// </summary>
    /// <param name="listings">Listings to look through</param>
    /// <param name="modKey">ModKey to look for</param>
    /// <param name="enabled">Whether the ModKey should be enabled/disabled.  Default is no preference</param>
    /// <param name="message">Message to attach to exception if mod doesn't exist</param>
    /// <exception cref="MissingModException">
    /// Thrown if given mod is not on the collection of listings
    /// </exception>
    public static void AssertListsMod(this IEnumerable<ILoadOrderListingGetter> listings, ModKey modKey, bool? enabled = null, string? message = null)
    {
        if (!ListsMod(listings, modKey, enabled))
        {
            throw new MissingModException(modKey, message: message);
        }
    }

    /// <summary>
    /// Checks whether all of a given set of ModKeys are in the collection of listings.
    /// </summary>
    /// <param name="listings">Listings to look through</param>
    /// <param name="modKeys">ModKeys to look for</param>
    /// <returns>True if all ModKeys are present in the listings</returns>
    public static bool ListsMods(this IEnumerable<ILoadOrderListingGetter> listings, params ModKey[] modKeys)
    {
        return ListsMods(listings.Select(x => x.ModKey), modKeys);
    }

    /// <summary>
    /// Asserts all of a given set of ModKeys are in the collection of listings.
    /// </summary>
    /// <param name="listings">Listings to look through</param>
    /// <param name="modKeys">ModKeys to look for</param>
    /// <exception cref="MissingModException">
    /// Thrown if given mod is not on the collection of listings
    /// </exception>
    public static void AssertListsMods(this IEnumerable<ILoadOrderListingGetter> listings, params ModKey[] modKeys)
    {
        AssertListsMods(listings, message: null, modKeys);
    }

    /// <summary>
    /// Asserts all of a given set of ModKeys are in the collection of listings.
    /// </summary>
    /// <param name="listings">Listings to look through</param>
    /// <param name="message">Message to attach to exception if mod doesn't exist</param>
    /// <param name="modKeys">ModKeys to look for</param>
    /// <exception cref="MissingModException">
    /// Thrown if given mod is not on the collection of listings
    /// </exception>
    public static void AssertListsMods(this IEnumerable<ILoadOrderListingGetter> listings, string? message, params ModKey[] modKeys)
    {
        AssertListsMods(listings.Select(x => x.ModKey), message, modKeys);
    }

    /// <summary>
    /// Checks whether all of a given set of ModKeys are in the collection of listings.
    /// </summary>
    /// <param name="listings">Listings to look through</param>
    /// <param name="modKeys">ModKeys to look for</param>
    /// <returns>True if all ModKeys are present in the listings</returns>
    public static bool ListsMods(this IEnumerable<ILoadOrderListingGetter> listings, IEnumerable<ModKey> modKeys)
    {
        return ListsMods(listings, modKeys.ToArray());
    }

    /// <summary>
    /// Asserts all of a given set of ModKeys are in the collection of listings.
    /// </summary>
    /// <param name="listings">Listings to look through</param>
    /// <param name="modKeys">ModKeys to look for</param>
    /// <param name="message">Message to attach to exception if mod doesn't exist</param>
    /// <exception cref="MissingModException">
    /// Thrown if given mod is not on the collection of listings
    /// </exception>
    public static void AssertListsMods(this IEnumerable<ILoadOrderListingGetter> listings, IEnumerable<ModKey> modKeys, string? message = null)
    {
        AssertListsMods(listings, message, modKeys.ToArray());
    }

    /// <summary>
    /// Checks whether all of a given set of ModKeys are in the collection of listings.
    /// </summary>
    /// <param name="listings">Listings to look through</param>
    /// <param name="enabled">Whether the ModKey should be enabled/disabled</param>
    /// <param name="modKeys">ModKey to look for</param>
    /// <returns>True if all ModKeys are present in the listings, with the desired enabled state</returns>
    public static bool ListsMods(this IEnumerable<ILoadOrderListingGetter> listings, bool enabled, params ModKey[] modKeys)
    {
        if (modKeys.Length == 0) return true;
        if (modKeys.Length == 1) return ListsMod(listings, modKeys[0], enabled);
        var set = modKeys.ToHashSet();
        foreach (var listing in listings)
        {
            if (listing.Enabled == enabled
                && set.Remove(listing.ModKey)
                && set.Count == 0)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Asserts all of a given set of ModKeys are in the collection of listings.
    /// </summary>
    /// <param name="listings">Listings to look through</param>
    /// <param name="enabled">Whether the ModKey should be enabled/disabled</param>
    /// <param name="modKeys">ModKey to look for</param>
    /// <exception cref="MissingModException">
    /// Thrown if given mod is not on the collection of listings
    /// </exception>
    public static void AssertListsMods(this IEnumerable<ILoadOrderListingGetter> listings, bool enabled, params ModKey[] modKeys)
    {
        AssertListsMods(listings, enabled, message: null, modKeys: modKeys);
    }

    /// <summary>
    /// Assertsall of a given set of ModKeys are in the collection of listings.
    /// </summary>
    /// <param name="listings">Listings to look through</param>
    /// <param name="enabled">Whether the ModKey should be enabled/disabled</param>
    /// <param name="message">Message to attach to exception if mod doesn't exist</param>
    /// <param name="modKeys">ModKey to look for</param>
    /// <exception cref="MissingModException">
    /// Thrown if given mod is not on the collection of listings
    /// </exception>
    public static void AssertListsMods(this IEnumerable<ILoadOrderListingGetter> listings, bool enabled, string? message, params ModKey[] modKeys)
    {
        if (modKeys.Length == 0) return;
        if (modKeys.Length == 1)
        {
            AssertListsMod(listings, modKeys[0], enabled);
            return;
        }
        var set = modKeys.ToHashSet();
        foreach (var listing in listings)
        {
            if (listing.Enabled == enabled
                && set.Remove(listing.ModKey)
                && set.Count == 0)
            {
                return;
            }
        }
        if (set.Count > 0)
        {
            throw new MissingModException(set, message: message);
        }
    }

    /// <summary>
    /// Checks whether all of a given set of ModKeys are in the collection of listings.
    /// </summary>
    /// <param name="listings">Listings to look through</param>
    /// <param name="enabled">Whether the ModKey should be enabled/disabled</param>
    /// <param name="modKeys">ModKey to look for</param>
    /// <returns>True if all ModKeys are present in the listings, with the desired enabled state</returns>
    public static bool ListsMods(this IEnumerable<ILoadOrderListingGetter> listings, bool enabled, IEnumerable<ModKey> modKeys)
    {
        return ListsMods(listings, enabled, modKeys.ToArray());
    }

    /// <summary>
    /// Asserts all of a given set of ModKeys are in the collection of listings.
    /// </summary>
    /// <param name="listings">Listings to look through</param>
    /// <param name="enabled">Whether the ModKey should be enabled/disabled</param>
    /// <param name="modKeys">ModKey to look for</param>
    /// <param name="message">Message to attach to exception if mod doesn't exist</param>
    /// <exception cref="MissingModException">
    /// Thrown if given mod is not on the collection of listings
    /// </exception>
    public static void AssertListsMods(this IEnumerable<ILoadOrderListingGetter> listings, bool enabled, IEnumerable<ModKey> modKeys, string? message = null)
    {
        AssertListsMods(listings, enabled, message: message, modKeys: modKeys.ToArray());
    }

    /// <summary>
    /// Checks whether a given mod is in the collection of keys.
    /// </summary>
    /// <param name="keys">Listings to look through</param>
    /// <param name="modKey">ModKey to look for</param>
    /// <returns>True if ModKey is in the listings, with the desired enabled state</returns>
    public static bool ListsMod(this IEnumerable<ModKey> keys, ModKey modKey)
    {
        return keys.Contains(modKey);
    }

    /// <summary>
    /// Asserts a given mod is in the collection of keys.
    /// </summary>
    /// <param name="keys">Listings to look through</param>
    /// <param name="modKey">ModKey to look for</param>
    /// <param name="message">Message to attach to exception if mod doesn't exist</param>
    /// <exception cref="MissingModException">
    /// Thrown if given mod is not on the collection of listings
    /// </exception>
    public static void AssertListsMod(this IEnumerable<ModKey> keys, ModKey modKey, string? message = null)
    {
        var modKeys = keys as IReadOnlyCollection<ModKey> ?? keys.ToArray();
        if (!ListsMod(modKeys, modKey))
        {
            throw new MissingModException(modKeys, message: message);
        }
    }

    /// <summary>
    /// Checks whether all of a given set of ModKeys are in the collection of listings.
    /// </summary>
    /// <param name="keys">Listings to look through</param>
    /// <param name="modKeys">ModKeys to look for</param>
    /// <returns>True if all ModKeys are present in the listings</returns>
    public static bool ListsMods(this IEnumerable<ModKey> keys, params ModKey[] modKeys)
    {
        if (modKeys.Length == 0) return true;
        if (modKeys.Length == 1) return ListsMod(keys, modKeys[0]);
        var set = modKeys.ToHashSet();
        foreach (var listing in keys)
        {
            if (set.Remove(listing)
                && set.Count == 0)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Asserts all of a given set of ModKeys are in the collection of listings.
    /// </summary>
    /// <param name="keys">Listings to look through</param>
    /// <param name="modKeys">ModKeys to look for</param>
    /// <exception cref="MissingModException">
    /// Thrown if given mod is not on the collection of listings
    /// </exception>
    public static void AssertListsMods(this IEnumerable<ModKey> keys, params ModKey[] modKeys)
    {
        AssertListsMods(keys, message: null, modKeys);
    }

    /// <summary>
    /// Asserts all of a given set of ModKeys are in the collection of listings.
    /// </summary>
    /// <param name="keys">Listings to look through</param>
    /// <param name="message">Message to attach to exception if mod doesn't exist</param>
    /// <param name="modKeys">ModKeys to look for</param>
    /// <exception cref="MissingModException">
    /// Thrown if given mod is not on the collection of listings
    /// </exception>
    public static void AssertListsMods(this IEnumerable<ModKey> keys, string? message, params ModKey[] modKeys)
    {
        if (modKeys.Length == 0) return;
        if (modKeys.Length == 1)
        {
            AssertListsMod(keys, modKeys[0]);
            return;
        }
        var set = modKeys.ToHashSet();
        foreach (var listing in keys)
        {
            if (set.Remove(listing)
                && set.Count == 0)
            {
                return;
            }
        }
        if (set.Count > 0)
        {
            throw new MissingModException(set, message: message);
        }
    }

    /// <summary>
    /// Checks whether all of a given set of ModKeys are in the collection of listings.
    /// </summary>
    /// <param name="keys">Listings to look through</param>
    /// <param name="modKeys">ModKeys to look for</param>
    /// <returns>True if all ModKeys are present in the listings</returns>
    public static bool ListsMods(this IEnumerable<ModKey> keys, IEnumerable<ModKey> modKeys)
    {
        return ListsMods(keys, modKeys.ToArray());
    }

    /// <summary>
    /// Asserts all of a given set of ModKeys are in the collection of listings.
    /// </summary>
    /// <param name="keys">Listings to look through</param>
    /// <param name="modKeys">ModKeys to look for</param>
    /// <param name="message">Message to attach to exception if mod doesn't exist</param>
    /// <exception cref="MissingModException">
    /// Thrown if given mod is not on the collection of listings
    /// </exception>
    public static void AssertListsMods(this IEnumerable<ModKey> keys, IEnumerable<ModKey> modKeys, string? message = null)
    {
        AssertListsMods(keys, message: message, modKeys: modKeys.ToArray());
    }
    
    /// <summary>
    /// Checks whether a given mod is in the collection of keys.
    /// </summary>
    /// <param name="loadOrder">LoadOrder to query</param>
    /// <param name="modKey">ModKey to look for</param>
    /// <returns>True if ModKey is in the listings, with the desired enabled state</returns>
    public static bool ListsMod(this ILoadOrderGetter loadOrder, ModKey modKey)
    {
        return loadOrder.ContainsKey(modKey);
    }

    /// <summary>
    /// Asserts a given mod is in the collection of keys.
    /// </summary>
    /// <param name="loadOrder">LoadOrder to query</param>
    /// <param name="modKey">ModKey to look for</param>
    /// <param name="message">Message to attach to exception if mod doesn't exist</param>
    /// <exception cref="MissingModException">
    /// Thrown if given mod is not on the collection of listings
    /// </exception>
    public static void AssertListsMod(this ILoadOrderGetter loadOrder, ModKey modKey, string? message = null)
    {
        if (!ListsMod(loadOrder, modKey))
        {
            throw new MissingModException(loadOrder.ListedOrder, message: message);
        }
    }

    /// <summary>
    /// Checks whether all of a given set of ModKeys are in the collection of listings.
    /// </summary>
    /// <param name="loadOrder">LoadOrder to query</param>
    /// <param name="modKeys">ModKeys to look for</param>
    /// <returns>True if all ModKeys are present in the listings</returns>
    public static bool ListsMods(this ILoadOrderGetter loadOrder, params ModKey[] modKeys)
    {
        foreach (var key in modKeys)
        {
            if (!loadOrder.ContainsKey(key)) return false;
        }
        return true;
    }

    /// <summary>
    /// Asserts all of a given set of ModKeys are in the collection of listings.
    /// </summary>
    /// <param name="loadOrder">LoadOrder to query</param>
    /// <param name="modKeys">ModKeys to look for</param>
    /// <exception cref="MissingModException">
    /// Thrown if given mod is not on the collection of listings
    /// </exception>
    public static void AssertListsMods(this ILoadOrderGetter loadOrder, params ModKey[] modKeys)
    {
        AssertListsMods(loadOrder, message: null, modKeys);
    }

    /// <summary>
    /// Asserts all of a given set of ModKeys are in the collection of listings.
    /// </summary>
    /// <param name="loadOrder">LoadOrder to query</param>
    /// <param name="message">Message to attach to exception if mod doesn't exist</param>
    /// <param name="modKeys">ModKeys to look for</param>
    /// <exception cref="MissingModException">
    /// Thrown if given mod is not on the collection of listings
    /// </exception>
    public static void AssertListsMods(this ILoadOrderGetter loadOrder, string? message, params ModKey[] modKeys)
    {
        AssertListsMods(loadOrder.ListedOrder, message, modKeys);
    }

    /// <summary>
    /// Checks whether all of a given set of ModKeys are in the collection of listings.
    /// </summary>
    /// <param name="loadOrder">LoadOrder to query</param>
    /// <param name="modKeys">ModKeys to look for</param>
    /// <returns>True if all ModKeys are present in the listings</returns>
    public static bool ListsMods(this ILoadOrderGetter loadOrder, IEnumerable<ModKey> modKeys)
    {
        foreach (var key in modKeys)
        {
            if (!loadOrder.ContainsKey(key)) return false;
        }
        return true;
    }

    /// <summary>
    /// Asserts all of a given set of ModKeys are in the collection of listings.
    /// </summary>
    /// <param name="loadOrder">LoadOrder to query</param>
    /// <param name="modKeys">ModKeys to look for</param>
    /// <param name="message">Message to attach to exception if mod doesn't exist</param>
    /// <exception cref="MissingModException">
    /// Thrown if given mod is not on the collection of listings
    /// </exception>
    public static void AssertListsMods(this ILoadOrderGetter loadOrder, IEnumerable<ModKey> modKeys, string? message = null)
    {
        AssertListsMods(loadOrder, message: message, modKeys: modKeys.ToArray());
    }
        
    /// <summary>
    /// Checks whether a given mod is in the collection of listings.
    /// </summary>
    /// <param name="loadOrder">Listings to look through</param>
    /// <param name="modKey">ModKey to look for</param>
    /// <param name="enabled">Whether the ModKey should be enabled/disabled.  Default is no preference</param>
    /// <returns>True if ModKey is in the listings, with the desired enabled state</returns>
    public static bool ListsMod(this ILoadOrderGetter<IModListingGetter> loadOrder, ModKey modKey, bool? enabled = null)
    {
        if (loadOrder.TryGetValue(modKey, out var listing))
        {
            return enabled == null || listing.Enabled == enabled;
        }

        return false;
    }

    /// <summary>
    /// Asserts a given mod is in the collection of listings.
    /// </summary>
    /// <param name="loadOrder">Listings to look through</param>
    /// <param name="modKey">ModKey to look for</param>
    /// <param name="enabled">Whether the ModKey should be enabled/disabled.  Default is no preference</param>
    /// <param name="message">Message to attach to exception if mod doesn't exist</param>
    /// <exception cref="MissingModException">
    /// Thrown if given mod is not on the collection of listings
    /// </exception>
    public static void AssertListsMod(this ILoadOrderGetter<IModListingGetter> loadOrder, ModKey modKey, bool? enabled = null, string? message = null)
    {
        if (!ListsMod(loadOrder, modKey, enabled))
        {
            throw new MissingModException(modKey, message: message);
        }
    }

    /// <summary>
    /// Checks whether all of a given set of ModKeys are in the collection of listings.
    /// </summary>
    /// <param name="loadOrder">Listings to look through</param>
    /// <param name="modKeys">ModKeys to look for</param>
    /// <returns>True if all ModKeys are present in the listings</returns>
    public static bool ListsMods(this ILoadOrderGetter<IModListingGetter> loadOrder, params ModKey[] modKeys)
    {
        foreach (var key in modKeys)
        {
            if (!loadOrder.ContainsKey(key)) return false;
        }

        return true;
    }

    /// <summary>
    /// Asserts all of a given set of ModKeys are in the collection of listings.
    /// </summary>
    /// <param name="loadOrder">Listings to look through</param>
    /// <param name="modKeys">ModKeys to look for</param>
    /// <exception cref="MissingModException">
    /// Thrown if given mod is not on the collection of listings
    /// </exception>
    public static void AssertListsMods(this ILoadOrderGetter<IModListingGetter> loadOrder, params ModKey[] modKeys)
    {
        AssertListsMods(loadOrder, message: null, modKeys);
    }

    /// <summary>
    /// Asserts all of a given set of ModKeys are in the collection of listings.
    /// </summary>
    /// <param name="loadOrder">Listings to look through</param>
    /// <param name="message">Message to attach to exception if mod doesn't exist</param>
    /// <param name="modKeys">ModKeys to look for</param>
    /// <exception cref="MissingModException">
    /// Thrown if given mod is not on the collection of listings
    /// </exception>
    public static void AssertListsMods(this ILoadOrderGetter<IModListingGetter> loadOrder, string? message, params ModKey[] modKeys)
    {
        AssertListsMods(loadOrder.ListedOrder.Select(x => x.ModKey), message, modKeys);
    }

    /// <summary>
    /// Checks whether all of a given set of ModKeys are in the collection of listings.
    /// </summary>
    /// <param name="loadOrder">Listings to look through</param>
    /// <param name="modKeys">ModKeys to look for</param>
    /// <returns>True if all ModKeys are present in the listings</returns>
    public static bool ListsMods(this ILoadOrderGetter<IModListingGetter> loadOrder, IEnumerable<ModKey> modKeys)
    {
        return ListsMods(loadOrder, modKeys.ToArray());
    }

    /// <summary>
    /// Asserts all of a given set of ModKeys are in the collection of listings.
    /// </summary>
    /// <param name="loadOrder">Listings to look through</param>
    /// <param name="modKeys">ModKeys to look for</param>
    /// <param name="message">Message to attach to exception if mod doesn't exist</param>
    /// <exception cref="MissingModException">
    /// Thrown if given mod is not on the collection of listings
    /// </exception>
    public static void AssertListsMods(this ILoadOrderGetter<IModListingGetter> loadOrder, IEnumerable<ModKey> modKeys, string? message = null)
    {
        AssertListsMods(loadOrder, message, modKeys.ToArray());
    }

    /// <summary>
    /// Checks whether all of a given set of ModKeys are in the collection of listings.
    /// </summary>
    /// <param name="loadOrder">Listings to look through</param>
    /// <param name="enabled">Whether the ModKey should be enabled/disabled</param>
    /// <param name="modKeys">ModKey to look for</param>
    /// <returns>True if all ModKeys are present in the listings, with the desired enabled state</returns>
    public static bool ListsMods(this ILoadOrderGetter<IModListingGetter> loadOrder, bool enabled, params ModKey[] modKeys)
    {
        if (modKeys.Length == 0) return true;
        if (modKeys.Length == 1) return ListsMod(loadOrder, modKeys[0], enabled);
        var set = modKeys.ToHashSet();
        foreach (var listing in loadOrder.ListedOrder)
        {
            if (listing.Enabled == enabled
                && set.Remove(listing.ModKey)
                && set.Count == 0)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Asserts all of a given set of ModKeys are in the collection of listings.
    /// </summary>
    /// <param name="loadOrder">Listings to look through</param>
    /// <param name="enabled">Whether the ModKey should be enabled/disabled</param>
    /// <param name="modKeys">ModKey to look for</param>
    /// <exception cref="MissingModException">
    /// Thrown if given mod is not on the collection of listings
    /// </exception>
    public static void AssertListsMods(this ILoadOrderGetter<IModListingGetter> loadOrder, bool enabled, params ModKey[] modKeys)
    {
        AssertListsMods(loadOrder, enabled, message: null, modKeys: modKeys);
    }

    /// <summary>
    /// Asserts all of a given set of ModKeys are in the collection of listings.
    /// </summary>
    /// <param name="loadOrder">Listings to look through</param>
    /// <param name="enabled">Whether the ModKey should be enabled/disabled</param>
    /// <param name="message">Message to attach to exception if mod doesn't exist</param>
    /// <param name="modKeys">ModKey to look for</param>
    /// <exception cref="MissingModException">
    /// Thrown if given mod is not on the collection of listings
    /// </exception>
    public static void AssertListsMods(this ILoadOrderGetter<IModListingGetter> loadOrder, bool enabled, string? message, params ModKey[] modKeys)
    {
        if (modKeys.Length == 0) return;
        if (modKeys.Length == 1) AssertListsMod(loadOrder, modKeys[0], enabled);
        var set = modKeys.ToHashSet();
        foreach (var listing in loadOrder.ListedOrder)
        {
            if (listing.Enabled == enabled
                && set.Remove(listing.ModKey)
                && set.Count == 0)
            {
                return;
            }
        }
        if (set.Count > 0)
        {
            throw new MissingModException(set, message: message);
        }
    }

    /// <summary>
    /// Checks whether all of a given set of ModKeys are in the collection of listings.
    /// </summary>
    /// <param name="loadOrder">Listings to look through</param>
    /// <param name="enabled">Whether the ModKey should be enabled/disabled</param>
    /// <param name="modKeys">ModKey to look for</param>
    /// <returns>True if all ModKeys are present in the listings, with the desired enabled state</returns>
    public static bool ListsMods(this ILoadOrderGetter<IModListingGetter> loadOrder, bool enabled, IEnumerable<ModKey> modKeys)
    {
        return ListsMods(loadOrder, enabled, modKeys.ToArray());
    }

    /// <summary>
    /// Asserts all of a given set of ModKeys are in the collection of listings.
    /// </summary>
    /// <param name="loadOrder">Listings to look through</param>
    /// <param name="enabled">Whether the ModKey should be enabled/disabled</param>
    /// <param name="modKeys">ModKey to look for</param>
    /// <param name="message">Message to attach to exception if mod doesn't exist</param>
    /// <exception cref="MissingModException">
    /// Thrown if given mod is not on the collection of listings
    /// </exception>
    public static void AssertListsMods(this ILoadOrderGetter<IModListingGetter> loadOrder, bool enabled, IEnumerable<ModKey> modKeys, string? message = null)
    {
        AssertListsMods(loadOrder, enabled, message: message, modKeys: modKeys.ToArray());
    }
    
    public static IEnumerable<ModKey> OrderListings(this IEnumerable<ModKey> e)
    {
        return LoadOrder.OrderListings(e, i => i);
    }

    public static IEnumerable<TListing> OrderListings<TListing>(this IEnumerable<TListing> e)
        where TListing : IModListingGetter
    {
        return LoadOrder.OrderListings(e, i => i.ModKey);
    }

    public static IObservable<IChangeSet<ModKey>> OrderListings(this IObservable<IChangeSet<ModKey>> e)
    {
        return e.Sort(ModKey.ByTypeComparer);
    }

    public static IObservable<IChangeSet<TListing>> OrderListings<TListing>(this IObservable<IChangeSet<TListing>> e)
        where TListing : IModListingGetter
    {
        return ObservableListEx.Sort(e, ModListing.GetComparer<TListing>(ModKey.ByTypeComparer));
    }
        
    public static bool TryGetIfEnabled<TListing>(this ILoadOrderGetter<TListing> loadOrder, ModKey modKey, [MaybeNullWhen(false)] out TListing item)
        where TListing : IModListingGetter
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

    public static TListing GetIfEnabled<TListing>(this ILoadOrderGetter<TListing> loadOrder, ModKey modKey)
        where TListing : IModListingGetter
    {
        if (TryGetIfEnabled(loadOrder, modKey, out var listing))
        {
            return listing;
        }
        throw new MissingModException(modKey);
    }

    public static bool TryGetIfEnabledAndExists<TMod>(this ILoadOrderGetter<IModListingGetter<TMod>> loadOrder, ModKey modKey, [MaybeNullWhen(false)] out TMod item)
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

    public static TMod GetIfEnabledAndExists<TMod>(this ILoadOrderGetter<IModListingGetter<TMod>> loadOrder, ModKey modKey)
        where TMod : class, IModGetter
    {
        if (TryGetIfEnabledAndExists<TMod>(loadOrder, modKey, out var mod))
        {
            return mod;
        }
        throw new MissingModException(modKey);
    }

    public static ILoadOrder<TListing> ToLoadOrder<TListing>(this IEnumerable<TListing> listings)
        where TListing : ILoadOrderListingGetter
    {
        var ret = new LoadOrder<TListing>();
        ret.Add(listings);
        return ret;
    }

    public static IModListingGetter ToModListing(this ILoadOrderListingGetter listing, bool existsOnDisk)
    {
        return new ModListing(listing.ModKey, listing.Enabled, ghostSuffix: listing.GhostSuffix, existsOnDisk: existsOnDisk);
    }
}