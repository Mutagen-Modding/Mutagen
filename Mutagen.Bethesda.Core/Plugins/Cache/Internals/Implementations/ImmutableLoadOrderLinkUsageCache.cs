using System.Reflection;
using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Plugins.Cache.Internals.Implementations;

public sealed class ImmutableLoadOrderLinkUsageCache : ILinkUsageCache
{
    private static readonly MethodInfo MethodInfo;
    
    static ImmutableLoadOrderLinkUsageCache()
    {
        MethodInfo = typeof(ImmutableLoadOrderLinkUsageCache).GetMethod(nameof(GetReferencedByGeneric), BindingFlags.NonPublic | BindingFlags.Instance)!;
    }
    
    private record struct CacheKey(
        FormKey FormKey,
        Type ReferencedType,
        Type ReferencedByType);
    
    private record CacheItem(
        IReadOnlyCollection<FormKey> FormKeys,
        object? FormLinks);
    
    private readonly ILinkCache _linkCache;
    private readonly Dictionary<CacheKey, CacheItem> _cache = new();
    
    public ImmutableLoadOrderLinkUsageCache(
        ILinkCache linkCache)
    {
        _linkCache = linkCache;
    }
    
    public IReadOnlyCollection<IFormLinkGetter<TReferencedBy>> GetUsagesOf<TReferencedBy>(
        IMajorRecordGetter majorRecord) 
        where TReferencedBy : class, IMajorRecordGetter
    {
        return GetUsagesOf<TReferencedBy>(majorRecord.ToStandardizedIdentifier());
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
        
        var genMethod = MethodInfo.MakeGenericMethod(typeof(IMajorRecordGetter));
        var cacheItem = (CacheItem)genMethod.Invoke(this, [identifier])!;
        return cacheItem.FormKeys;
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
    
    public IReadOnlyCollection<IFormLinkGetter<TReferencedBy>> GetUsagesOf<TReferencedBy>(
        IFormLinkIdentifier identifier)
        where TReferencedBy : class, IMajorRecordGetter
    {
        var cacheItem = GetReferencedByGeneric<TReferencedBy>(identifier);
        if (cacheItem.FormLinks is IReadOnlyCollection<IFormLinkGetter<TReferencedBy>> links) return links;
        throw new ArgumentException($"Could not get cached formlinks for {identifier} referenced by {typeof(TReferencedBy)}");
    }
    
    private CacheItem GetReferencedByGeneric<TReferencedBy>(
        IFormLinkIdentifier identifier)
        where TReferencedBy : class, IMajorRecordGetter
    {
        var key = new CacheKey(
            identifier.FormKey,
            ReferencedType: identifier.Type,
            ReferencedByType: typeof(TReferencedBy));

        lock (_cache)
        {
            if (_cache.TryGetValue(key, out var cached))
            {
                return cached;
            }
        }
        
        HashSet<IFormLinkGetter<TReferencedBy>> formLinks = new();
        HashSet<FormKey> formKeys = new();
        foreach (var record in _linkCache.PriorityOrder.WinningOverrides<TReferencedBy>()) 
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