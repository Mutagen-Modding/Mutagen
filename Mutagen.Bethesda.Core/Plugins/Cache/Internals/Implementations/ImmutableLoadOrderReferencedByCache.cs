using Mutagen.Bethesda.Plugins.Records;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Cache.Internals.Implementations;

public sealed class ImmutableLoadOrderReferencedByCache : IReferencedByCache
{
    private record struct CacheKey(
        FormKey FormKey,
        Type Type);
    
    private record CacheItem(
        IReadOnlyCollection<FormKey>? FormKeys,
        object? FormLinks);
    
    private readonly ILinkCache _linkCache;
    private readonly Dictionary<CacheKey, CacheItem> _cache = new();
    
    public ImmutableLoadOrderReferencedByCache(
        ILinkCache linkCache)
    {
        _linkCache = linkCache;
    }
    
    public IReadOnlyCollection<IFormLinkGetter<TReferencedBy>> GetReferencedBy<TReferencedBy>(
        IMajorRecordGetter majorRecord) 
        where TReferencedBy : class, IMajorRecordGetter
    {
        return GetReferencedBy<TReferencedBy>(majorRecord.ToStandardizedIdentifier());
    }
    
    public IReadOnlyCollection<FormKey> GetReferencedBy(FormKey formKey)
    {
        var key = new CacheKey(formKey, typeof(IMajorRecordGetter));
        
        lock (_cache)
        {
            if (_cache.TryGetValue(key, out var cached)) return cached.FormKeys ?? [];
        }
        
        HashSet<FormKey> result = new();
        foreach (var record in _linkCache.PriorityOrder.WinningOverrides<IMajorRecordGetter>()) 
        {
            if (record.EnumerateFormLinks().Any(reference => reference.FormKey == formKey)) 
            {
                result.Add(record.FormKey);
            }
        }
        
        lock (_cache)
        {
            _cache[key] = new CacheItem(
                FormKeys: result,
                FormLinks: null);
        }

        return result;
    }
    
    public IReadOnlyCollection<IFormLinkGetter<TReferencedBy>> GetReferencedBy<TReferencedBy>(
        IFormLinkIdentifier identifier)
        where TReferencedBy : class, IMajorRecordGetter
    {
        var key = new CacheKey(identifier.FormKey, identifier.Type);

        lock (_cache)
        {
            if (_cache.TryGetValue(key, out var cached))
            {
                if (cached.FormLinks is IReadOnlyCollection<IFormLinkGetter<TReferencedBy>> links)
                {
                    return links;
                }
                return [];
            }
        }
        
        HashSet<IFormLinkGetter<TReferencedBy>> result = new();
        foreach (var record in _linkCache.PriorityOrder.WinningOverrides<TReferencedBy>()) 
        {
            // ToDo
            // Upgrade EnumerateFormLinks to query on type so we're not looping all links unnecessarily
            if (record.EnumerateFormLinks().Any(reference => reference.FormKey == identifier.FormKey)) 
            {
                result.Add(record.ToLink());
            }
        }
        
        lock (_cache)
        {
            _cache[key] = new CacheItem(
                FormKeys: null,
                FormLinks: result);
        }

        return result;
    }
}