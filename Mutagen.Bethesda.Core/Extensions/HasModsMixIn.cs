using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;

namespace Mutagen.Bethesda;

public static class HasModsMixIn
{
    /// <summary>
    /// Checks whether a given mod is in the collection of listings.
    /// </summary>
    /// <param name="listings">Listings to look through</param>
    /// <param name="modKey">ModKey to look for</param>
    /// <param name="enabled">Whether the ModKey should be enabled/disabled.  Default is no preference</param>
    /// <returns>True if ModKey is in the listings, with the desired enabled state</returns>
    public static bool ModExists(this IEnumerable<IModListingGetter> listings, ModKey modKey, bool? enabled = null)
    {
        foreach (var listing in listings)
        {
            if (listing.ModKey == modKey
                && listing.ExistsOnDisk
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
    public static void AssertModExists(this IEnumerable<IModListingGetter> listings, ModKey modKey, bool? enabled = null, string? message = null)
    {
        if (!ModExists(listings, modKey, enabled))
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
    public static bool ModsExist(this IEnumerable<IModListingGetter> listings, IEnumerable<ModKey> modKeys)
    {
        return ModsExist(listings, modKeys: modKeys.ToArray());
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
    public static void AssertModsExist(this IEnumerable<IModListingGetter> listings, IEnumerable<ModKey> modKeys, string? message = null)
    {
        AssertModsExist(listings, enabled: null, message: message, modKeys.ToArray());
    }

    /// <summary>
    /// Checks whether all of a given set of ModKeys are in the collection of listings.
    /// </summary>
    /// <param name="listings">Listings to look through</param>
    /// <param name="enabled">Whether the ModKey should be enabled/disabled.  Default is no preference</param>
    /// <param name="modKeys">ModKey to look for</param>
    /// <returns>True if all ModKeys are present in the listings, with the desired enabled state</returns>
    public static bool ModsExist(this IEnumerable<IModListingGetter> listings, bool? enabled, params ModKey[] modKeys)
    {
        if (modKeys.Length == 0) return true;
        if (modKeys.Length == 1) return ModExists(listings, modKeys[0], enabled);
        var set = modKeys.ToHashSet();
        foreach (var listing in listings)
        {
            if ((enabled == null || listing.Enabled == enabled)
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
    /// <param name="enabled">Whether the ModKey should be enabled/disabled.  Default is no preference</param>
    /// <param name="modKeys">ModKey to look for</param>
    /// <exception cref="MissingModException">
    /// Thrown if given mod is not on the collection of listings
    /// </exception>
    public static void AssertModsExist(this IEnumerable<IModListingGetter> listings, bool? enabled, params ModKey[] modKeys)
    {
        AssertModsExist(listings, message: null, enabled: enabled, modKeys: modKeys);
    }

    /// <summary>
    /// Asserts all of a given set of ModKeys are in the collection of listings.
    /// </summary>
    /// <param name="listings">Listings to look through</param>
    /// <param name="message">Message to attach to exception if mod doesn't exist</param>
    /// <param name="enabled">Whether the ModKey should be enabled/disabled.  Default is no preference</param>
    /// <param name="modKeys">ModKeys to look for</param>
    /// <exception cref="MissingModException">
    /// Thrown if given mod is not on the collection of listings
    /// </exception>
    public static void AssertModsExist(this IEnumerable<IModListingGetter> listings, bool? enabled, string? message, params ModKey[] modKeys)
    {
        if (modKeys.Length == 0) return;
        if (modKeys.Length == 1)
        {
            AssertModExists(listings, modKeys[0]);
            return;
        }
        var set = modKeys.ToHashSet();
        foreach (var listing in listings)
        {
            if (listing.ExistsOnDisk
                && (enabled == null || listing.Enabled == enabled)
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
    /// <param name="enabled">Whether the ModKey should be enabled/disabled.  Default is no preference</param>
    /// <param name="modKeys">ModKey to look for</param>
    /// <returns>True if all ModKeys are present in the listings, with the desired enabled state</returns>
    public static bool ModsExist(this IEnumerable<IModListingGetter> listings, bool? enabled, IEnumerable<ModKey> modKeys)
    {
        return ModsExist(listings, enabled, modKeys.ToArray());
    }

    /// <summary>
    /// Asserts all of a given set of ModKeys are in the collection of listings.
    /// </summary>
    /// <param name="listings">Listings to look through</param>
    /// <param name="enabled">Whether the ModKey should be enabled/disabled.  Default is no preference</param>
    /// <param name="modKeys">ModKey to look for</param>
    /// <param name="message">Message to attach to exception if mod doesn't exist</param>
    /// <exception cref="MissingModException">
    /// Thrown if given mod is not on the collection of listings
    /// </exception>
    public static void AssertModsExist(this IEnumerable<IModListingGetter> listings, bool? enabled, IEnumerable<ModKey> modKeys, string? message = null)
    {
        AssertModsExist(listings, enabled: enabled, message: message, modKeys: modKeys.ToArray());
    }

    /// <summary>
    /// Checks whether a given mod is in the collection of keys.
    /// </summary>
    /// <param name="keys">Listings to look through</param>
    /// <param name="modKey">ModKey to look for</param>
    /// <returns>True if ModKey is in the listings, with the desired enabled state</returns>
    public static bool ModExists(this IEnumerable<ModKey> keys, ModKey modKey)
    {
        return keys.Contains(modKey);
    }

    /// <summary>
    /// Checks whether all of a given set of ModKeys are in the collection of listings.
    /// </summary>
    /// <param name="keys">Listings to look through</param>
    /// <param name="modKeys">ModKeys to look for</param>
    /// <returns>True if all ModKeys are present in the listings</returns>
    public static bool ModsExist(this IEnumerable<IModListingGetter> keys, params ModKey[] modKeys)
    {
        if (modKeys.Length == 0) return true;
        if (modKeys.Length == 1) return ModExists(keys, modKeys[0]);
        var set = modKeys.ToHashSet();
        foreach (var listing in keys)
        {
            if (!listing.ExistsOnDisk) return false;
            if (set.Remove(listing.ModKey)
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
    public static void AssertModsExist(this IEnumerable<IModListingGetter> keys, params ModKey[] modKeys)
    {
        AssertModsExist(keys, enabled: null, message: null, modKeys);
    }
    
    /// <summary>
    /// Checks whether all of a given set of ModKeys are in the collection of listings.
    /// </summary>
    /// <param name="listings">Listings to look through</param>
    /// <param name="enabled">Whether the ModKey should be enabled/disabled.  Default is no preference</param>
    /// <param name="present">Whether the ModKey should be present on disk</param>
    /// <param name="modKeys">ModKeys to look for</param>
    /// <returns>True if all ModKeys are present in the listings</returns>
    public static bool ModsExist<TMod>(this IEnumerable<IModListingGetter<TMod>> listings, bool? enabled, bool? present, params ModKey[] modKeys)
        where TMod : class, IModKeyed
    {
        if (modKeys.Length == 0) return true;
        if (modKeys.Length == 1) return ModExists(listings, modKeys[0], enabled);
        var set = modKeys.ToHashSet();
        foreach (var listing in listings)
        {
            if (enabled != null && listing.Enabled != enabled) continue;
            if (present != null && (listing.Mod != null) != present) continue;
            if (set.Remove(listing.ModKey)
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
    /// <param name="enabled">Whether the ModKey should be enabled/disabled.  Default is no preference</param>
    /// <param name="present">Whether the ModKey should be present on disk</param>
    /// <param name="modKeys">ModKeys to look for</param>
    /// <exception cref="MissingModException">
    /// Thrown if given mod is not on the collection of listings
    /// </exception>
    public static void AssertModsExist<TMod>(this IEnumerable<IModListingGetter<TMod>> listings, bool? enabled, bool? present, params ModKey[] modKeys)
        where TMod : class, IModKeyed
    {
        AssertModsExist(listings, enabled: enabled, present: present, message: null, modKeys: modKeys);
    }

    /// <summary>
    /// Asserts all of a given set of ModKeys are in the collection of listings.
    /// </summary>
    /// <param name="listings">Listings to look through</param>
    /// <param name="enabled">Whether the ModKey should be enabled/disabled.  Default is no preference</param>
    /// <param name="present">Whether the ModKey should be present on disk</param>
    /// <param name="message">Message to attach to exception if mod doesn't exist</param>
    /// <param name="modKeys">ModKeys to look for</param>
    /// <exception cref="MissingModException">
    /// Thrown if given mod is not on the collection of listings
    /// </exception>
    public static void AssertModsExist<TMod>(this IEnumerable<IModListingGetter<TMod>> listings, bool? enabled, bool? present, string? message, params ModKey[] modKeys)
        where TMod : class, IModKeyed
    {
        if (modKeys.Length == 0) return;
        if (modKeys.Length == 1)
        {
            AssertModExists(listings, modKeys[0], enabled);
            return;
        }
        var set = modKeys.ToHashSet();
        foreach (var listing in listings)
        {
            if (enabled != null && listing.Enabled != enabled) continue;
            if (present != null && (listing.Mod != null) != present) continue;
            if (set.Remove(listing.ModKey)
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
    /// Checks whether a given mod is in the collection of listings.
    /// </summary>
    /// <param name="loadOrder">Listings to look through</param>
    /// <param name="modKey">ModKey to look for</param>
    /// <param name="enabled">Whether the ModKey should be enabled/disabled.  Default is no preference</param>
    /// <returns>True if ModKey is in the listings, with the desired enabled state</returns>
    public static bool ModExists(this ILoadOrderGetter<IModListingGetter> loadOrder, ModKey modKey, bool? enabled = null)
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
    public static void AssertModExists(this ILoadOrderGetter<IModListingGetter> loadOrder, ModKey modKey, bool? enabled = null, string? message = null)
    {
        if (!ModExists(loadOrder, modKey, enabled))
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
    public static bool ModsExist(this ILoadOrderGetter<IModListingGetter> loadOrder, params ModKey[] modKeys)
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
    public static void AssertModsExist(this ILoadOrderGetter<IModListingGetter> loadOrder, params ModKey[] modKeys)
    {
        AssertModsExist(loadOrder, message: null, modKeys);
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
    public static void AssertModsExist(this ILoadOrderGetter<IModListingGetter> loadOrder, string? message, params ModKey[] modKeys)
    {
        AssertModsExist(loadOrder.ListedOrder, enabled: null, message: message, modKeys);
    }

    /// <summary>
    /// Checks whether all of a given set of ModKeys are in the collection of listings.
    /// </summary>
    /// <param name="loadOrder">Listings to look through</param>
    /// <param name="modKeys">ModKeys to look for</param>
    /// <returns>True if all ModKeys are present in the listings</returns>
    public static bool ModsExist(this ILoadOrderGetter<IModListingGetter> loadOrder, IEnumerable<ModKey> modKeys)
    {
        return ModsExist(loadOrder, modKeys.ToArray());
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
    public static void AssertModsExist(this ILoadOrderGetter<IModListingGetter> loadOrder, IEnumerable<ModKey> modKeys, string? message = null)
    {
        AssertModsExist(loadOrder, message, modKeys.ToArray());
    }

    /// <summary>
    /// Checks whether all of a given set of ModKeys are in the collection of listings.
    /// </summary>
    /// <param name="loadOrder">Listings to look through</param>
    /// <param name="enabled">Whether the ModKey should be enabled/disabled.  Default is no preference</param>
    /// <param name="modKeys">ModKey to look for</param>
    /// <returns>True if all ModKeys are present in the listings, with the desired enabled state</returns>
    public static bool ModsExist(this ILoadOrderGetter<IModListingGetter> loadOrder, bool? enabled, params ModKey[] modKeys)
    {
        if (modKeys.Length == 0) return true;
        if (modKeys.Length == 1) return ModExists(loadOrder, modKeys[0], enabled);
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
    /// <param name="enabled">Whether the ModKey should be enabled/disabled.  Default is no preference</param>
    /// <param name="modKeys">ModKey to look for</param>
    /// <exception cref="MissingModException">
    /// Thrown if given mod is not on the collection of listings
    /// </exception>
    public static void AssertModsExist(this ILoadOrderGetter<IModListingGetter> loadOrder, bool? enabled, params ModKey[] modKeys)
    {
        AssertModsExist(loadOrder, enabled, message: null, modKeys: modKeys);
    }

    /// <summary>
    /// Asserts all of a given set of ModKeys are in the collection of listings.
    /// </summary>
    /// <param name="loadOrder">Listings to look through</param>
    /// <param name="enabled">Whether the ModKey should be enabled/disabled.  Default is no preference</param>
    /// <param name="message">Message to attach to exception if mod doesn't exist</param>
    /// <param name="modKeys">ModKey to look for</param>
    /// <exception cref="MissingModException">
    /// Thrown if given mod is not on the collection of listings
    /// </exception>
    public static void AssertModsExist(this ILoadOrderGetter<IModListingGetter> loadOrder, bool? enabled, string? message, params ModKey[] modKeys)
    {
        if (modKeys.Length == 0) return;
        if (modKeys.Length == 1) AssertModExists(loadOrder, modKeys[0], enabled);
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
    /// <param name="enabled">Whether the ModKey should be enabled/disabled.  Default is no preference</param>
    /// <param name="modKeys">ModKey to look for</param>
    /// <returns>True if all ModKeys are present in the listings, with the desired enabled state</returns>
    public static bool ModsExist(this ILoadOrderGetter<IModListingGetter> loadOrder, bool? enabled, IEnumerable<ModKey> modKeys)
    {
        return ModsExist(loadOrder, enabled, modKeys.ToArray());
    }

    /// <summary>
    /// Asserts all of a given set of ModKeys are in the collection of listings.
    /// </summary>
    /// <param name="loadOrder">Listings to look through</param>
    /// <param name="enabled">Whether the ModKey should be enabled/disabled.  Default is no preference</param>
    /// <param name="modKeys">ModKey to look for</param>
    /// <param name="message">Message to attach to exception if mod doesn't exist</param>
    /// <exception cref="MissingModException">
    /// Thrown if given mod is not on the collection of listings
    /// </exception>
    public static void AssertModsExist(this ILoadOrderGetter<IModListingGetter> loadOrder, bool? enabled, IEnumerable<ModKey> modKeys, string? message = null)
    {
        AssertModsExist(loadOrder, enabled, message: message, modKeys: modKeys.ToArray());
    }
}