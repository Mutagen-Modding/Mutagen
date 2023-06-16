using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Records;
using System.Collections.Immutable;
using Mutagen.Bethesda.Plugins.Analysis;

namespace Mutagen.Bethesda.Plugins.Utility;

/// <summary>
/// A class that can query and cache record locations by record type
/// </summary>
public sealed class RecordTypeInfoCacheReader
{
    private record CacheItem(IReadOnlyList<FormKey> List, HashSet<FormKey> Set);
    private readonly Func<IMutagenReadStream> _streamCreator;
    private ImmutableDictionary<Type, CacheItem> _cachedLocs = ImmutableDictionary<Type, CacheItem>.Empty;

    public RecordTypeInfoCacheReader(Func<IMutagenReadStream> streamCreator)
    {
        _streamCreator = streamCreator;
    }

    public bool IsOfRecordType<T>(FormKey formKey)
        where T : IMajorRecordGetter
    {
        if (formKey.IsNull) return false;
        return GetCacheItem<T>().Set.Contains(formKey);
    }

    public FormKey GetNthFormKey<T>(int n)
        where T : IMajorRecordGetter
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