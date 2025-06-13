using Mutagen.Bethesda.Plugins.Records;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Cache.Internals.Implementations;

public sealed class ImmutableLoadOrderLinkUsageCache : ILinkUsageCache
{
    private record struct CacheKey(
        FormKey FormKey,
        Type ReferencedType,
        Type ReferencedByType);
    
    private record CacheItem(
        IReadOnlyCollection<FormKey> FormKeys,
        object? FormLinks);
    
    private readonly ILinkCache _linkCache;
    private readonly Dictionary<CacheKey, CacheItem> _cache = new();
    private readonly object _untypedReferencesCacheLock = new();
    private Dictionary<FormKey, HashSet<FormKey>>? _untypedReferencesCache;
    
    public ImmutableLoadOrderLinkUsageCache(
        ILinkCache linkCache)
    {
        _linkCache = linkCache;
    }
    
    public IReadOnlyCollection<IFormLinkGetter<TUserRecordScope>> GetUsagesOf<TUserRecordScope>(
        IMajorRecordGetter majorRecord) 
        where TUserRecordScope : class, IMajorRecordGetter
    {
        return GetUsagesOf<TUserRecordScope>(majorRecord.ToStandardizedIdentifier());
    }
    
    public IReadOnlyCollection<FormKey> GetUsagesOf(IMajorRecordGetter majorRecord)
    {
        return GetUsagesOf(majorRecord.ToStandardizedIdentifier());
    }

    public IReadOnlyCollection<FormKey> GetUsagesOf(IFormLinkIdentifier identifier)
    {
        var key = new CacheKey(identifier.FormKey, 
            ReferencedType: identifier.Type,
            ReferencedByType: typeof(IMajorRecordGetter));
        
        lock (_cache)
        {
            if (_cache.TryGetValue(key, out var cached)) return cached.FormKeys;
        }
        
        return GetUsagesOfGeneric<IMajorRecordGetter>(identifier).FormKeys;
    }
    
    private Dictionary<FormKey, HashSet<FormKey>> GetUntypedReferencesCache()
    {
        lock (_untypedReferencesCacheLock)
        {
            if (_untypedReferencesCache is not null) return _untypedReferencesCache;
            _untypedReferencesCache = new();
            foreach (var record in _linkCache.PriorityOrder.WinningOverrides<IMajorRecordGetter>())
            {
                foreach (var fk in record.EnumerateFormLinks()
                             .Select(x => x.FormKey)
                             .Where(x => !x.IsNull))
                {
                    _untypedReferencesCache
                        .GetOrAdd(fk)
                        .Add(record.FormKey);
                }
            }
            return _untypedReferencesCache;
        }
    }

    public IReadOnlyCollection<FormKey> GetUsagesOf(FormKey formKey)
    {
        var key = new CacheKey(formKey, 
            ReferencedType: typeof(IMajorRecordGetter),
            ReferencedByType: typeof(IMajorRecordGetter));
        
        lock (_cache)
        {
            if (_cache.TryGetValue(key, out var cached)) return cached.FormKeys;
        }
        
        var untypedReferencesCache = GetUntypedReferencesCache();

        IReadOnlyCollection<FormKey> result;
        if (untypedReferencesCache.TryGetValue(formKey, out var set))
        {
            result = set;
        }
        else
        {
            result = [];
        }
        
        lock (_cache)
        {
            _cache[key] = new CacheItem(
                FormKeys: result,
                FormLinks: null);
        }

        return result;
    }
    
    public IReadOnlyCollection<IFormLinkGetter<TUserRecordScope>> GetUsagesOf<TUserRecordScope>(
        IFormLinkIdentifier identifier)
        where TUserRecordScope : class, IMajorRecordGetter
    {
        var cacheItem = GetUsagesOfGeneric<TUserRecordScope>(identifier);
        if (cacheItem.FormLinks is IReadOnlyCollection<IFormLinkGetter<TUserRecordScope>> links) return links;
        throw new ArgumentException($"Could not get cached formlinks for {identifier} referenced by {typeof(TUserRecordScope)}");
    }
    
    private CacheItem GetUsagesOfGeneric<TUserRecordScope>(
        IFormLinkIdentifier identifier)
        where TUserRecordScope : class, IMajorRecordGetter
    {
        var key = new CacheKey(
            identifier.FormKey,
            ReferencedType: identifier.Type,
            ReferencedByType: typeof(TUserRecordScope));

        lock (_cache)
        {
            if (_cache.TryGetValue(key, out var cached))
            {
                return cached;
            }
        }
        
        HashSet<IFormLinkGetter<TUserRecordScope>> formLinks = new();
        HashSet<FormKey> formKeys = new();
        foreach (var record in _linkCache.PriorityOrder.WinningOverrides<TUserRecordScope>()) 
        {
            // ToDo
            // Upgrade EnumerateFormLinks to query on type so we're not looping all links unnecessarily
            if (record.EnumerateFormLinks().Any(reference => reference.FormKey == identifier.FormKey)) 
            {
                formLinks.Add(record.ToLink());
                formKeys.Add(record.FormKey);
            }
        }

        var ret = new CacheItem(
            FormKeys: formKeys,
            FormLinks: formLinks);
        
        lock (_cache)
        {
            _cache[key] = ret;
        }

        return ret;
    }
}