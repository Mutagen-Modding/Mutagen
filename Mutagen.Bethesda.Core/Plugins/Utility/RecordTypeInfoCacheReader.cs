using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Records;
using System.Collections.Immutable;
using Mutagen.Bethesda.Plugins.Analysis;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Exceptions;

namespace Mutagen.Bethesda.Plugins.Utility;

/// <summary>
/// A class that can query and cache record locations by record type
/// </summary>
public sealed class RecordTypeInfoCacheReader
{
    private record CacheItem(IReadOnlyList<FormKey> List, HashSet<FormKey> Set);
    private readonly Func<IMutagenReadStream> _streamCreator;
    private readonly ILinkCache? _linkCache;
    private readonly ModKey _modKey;
    private ImmutableDictionary<Type, CacheItem> _cachedLocs = ImmutableDictionary<Type, CacheItem>.Empty;

    public RecordTypeInfoCacheReader(Func<IMutagenReadStream> streamCreator, ModKey modKey, ILinkCache? linkCache = null)
    {
        _streamCreator = streamCreator;
        _linkCache = linkCache;
        _modKey = modKey;
    }

    public bool IsOfRecordType<T>(FormKey formKey)
        where T : class, IMajorRecordGetter
    {
        if (formKey.IsNull) return false;

        // First check if the FormKey is in the current mod's cache
        if (GetCacheItem<T>().Set.Contains(formKey))
        {
            return true;
        }

        // If the FormKey is from a different mod, we need a LinkCache to resolve it
        if (formKey.ModKey != _modKey)
        {
            if (_linkCache == null)
            {
                throw new LinkCacheMissingException(
                    formKey,
                    typeof(T),
                    $"Cannot determine record type for FormKey {formKey} from mod {formKey.ModKey} because it is not in the current mod {_modKey} and no LinkCache was provided for cross-mod resolution.");
            }

            // Try to resolve the record in the LinkCache to check its type
            if (_linkCache.TryResolve<T>(formKey, out var _))
            {
                return true;
            }

            // If we can't resolve it as type T, check if the record exists at all
#pragma warning disable CS0618 // Type or member is obsolete
            if (!_linkCache.TryResolve(formKey, out var _))
#pragma warning restore CS0618
            {
                throw new MissingRecordException(formKey, typeof(T));
            }

            // Record exists but is not of type T
            return false;
        }

        // FormKey is from current mod but not in cache - record doesn't exist or isn't of type T
        return false;
    }

    public FormKey GetNthFormKey<T>(int n)
        where T : class, IMajorRecordGetter
    {
        var list = GetCacheItem<T>().List;
        if (list.Count <= n)
        {
            throw new ArgumentOutOfRangeException(
                $"Nth FormKey for type {typeof(T)} was requested that was too large: {n} >= {list.Count}");
        }
        return list[n];
    }

    private CacheItem GetCacheItem<T>()
    {
        if (_cachedLocs.TryGetValue(typeof(T), out var cache)) return cache;
            
        using var stream = _streamCreator();
        var locs = RecordLocator.GetLocations(
            stream,
            new RecordInterest(
                interestingTypes: PluginUtilityTranslation.GetRecordType<T>()));
        cache = new(locs.FormKeys.ToList(), locs.FormKeys.ToHashSet());

        _cachedLocs = _cachedLocs.SetItem(typeof(T), cache);
        return cache;
    }
}