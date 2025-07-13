using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Plugins.Cache.Internals.Implementations;

public sealed class ImmutableLoadOrderLinkUsageCache : ILinkUsageCache
{
    private record struct CacheKey(
        FormKey FormKey,
        Type UserRecordType);
    
    private record CacheItem(
        Lazy<ILinkUsageResults<IMajorRecordGetter>> Untyped,
        object? Typed);
    
    private readonly ILinkCache _linkCache;
    private readonly Dictionary<CacheKey, CacheItem> _cache = new();
    
    public ImmutableLoadOrderLinkUsageCache(
        ILinkCache linkCache)
    {
        _linkCache = linkCache;
    }
    
    public ILinkUsageResults<TUserRecordScope> GetUsagesOf<TUserRecordScope>(
        IMajorRecordGetter majorRecord) 
        where TUserRecordScope : class, IMajorRecordGetter
    {
        return GetUsagesOf<TUserRecordScope>(majorRecord.ToStandardizedIdentifier());
    }
    
    public ILinkUsageResults<IMajorRecordGetter> GetUsagesOf(IMajorRecordGetter majorRecord)
    {
        return GetUsagesOf(majorRecord.ToStandardizedIdentifier());
    }

    public ILinkUsageResults<IMajorRecordGetter> GetUsagesOf(IFormLinkIdentifier identifier)
    {
        var key = new CacheKey(identifier.FormKey, 
            UserRecordType: identifier.Type);
        
        lock (_cache)
        {
            if (_cache.TryGetValue(key, out var cached)) return cached.Untyped.Value;
        }
        
        return GetUsagesOfGeneric<IMajorRecordGetter>(identifier).Untyped.Value;
    }
    
    public ILinkUsageResults<IMajorRecordGetter> GetUsagesOf(FormKey formKey)
    {
        return GetUsagesOf(new FormLinkInformation(formKey, typeof(IMajorRecordGetter)));
    }
    
    public ILinkUsageResults<TUserRecordScope> GetUsagesOf<TUserRecordScope>(
        IFormLinkIdentifier identifier)
        where TUserRecordScope : class, IMajorRecordGetter
    {
        var cacheItem = GetUsagesOfGeneric<TUserRecordScope>(identifier);
        if (cacheItem.Typed is ILinkUsageResults<TUserRecordScope> links) return links;
        throw new ArgumentException($"Could not get cached formlinks for {identifier} referenced by {typeof(TUserRecordScope)}");
    }
    
    private CacheItem GetUsagesOfGeneric<TUserRecordScope>(
        IFormLinkIdentifier identifier)
        where TUserRecordScope : class, IMajorRecordGetter
    {
        var key = new CacheKey(
            identifier.FormKey,
            UserRecordType: typeof(TUserRecordScope));

        lock (_cache)
        {
            if (_cache.TryGetValue(key, out var cached))
            {
                return cached;
            }
        }
        
        HashSet<IFormLinkGetter<TUserRecordScope>> links = new();
        foreach (var record in _linkCache.PriorityOrder.WinningOverrides<TUserRecordScope>()) 
        {
            // ToDo
            // Upgrade EnumerateFormLinks to query on type so we're not looping all links unnecessarily
            if (record.EnumerateFormLinks().Any(reference => reference.FormKey == identifier.FormKey)) 
            {
                links.Add(record.ToLinkGetter());
            }
        }

        var untyped = new Lazy<ILinkUsageResults<IMajorRecordGetter>>(() =>
        {
            return new Results<IMajorRecordGetter>(
                links.Select<IFormLinkGetter<TUserRecordScope>, IFormLinkGetter<IMajorRecordGetter>>(x => x)
                    .ToHashSet());
        });
        
        var ret = new CacheItem(
            Untyped: untyped,
            Typed: new Results<TUserRecordScope>(links));
        
        lock (_cache)
        {
            _cache[key] = ret;
        }

        return ret;
    }
    
    private class Results<TScope> : ILinkUsageResults<TScope>
        where TScope : class, IMajorRecordGetter
    {
        private readonly Lazy<IReadOnlyCollection<IFormLinkIdentifier>> _identifiers;

        public IReadOnlySet<IFormLinkGetter<TScope>> UsageLinks { get; }

        public Results(IReadOnlySet<IFormLinkGetter<TScope>> links)
        {
            UsageLinks = links;
            _identifiers = new Lazy<IReadOnlyCollection<IFormLinkIdentifier>>(() =>
            {
                return links
                    .Select(l => new FormLinkInformation(l.FormKey, l.Type))
                    .ToHashSet(IFormLinkExt.FormLinkInformationEqualityComparerWithDualInheritanceConsideration);
            });
        }
        
        public bool Contains(FormKey formKey)
        {
            return _identifiers.Value.Contains(new FormLinkInformation(formKey, typeof(IMajorRecordGetter)));
        }
        
        public bool Contains(IFormLinkIdentifier identifier)
        {
            return _identifiers.Value.Contains(identifier);
        }
        
        public bool Contains(IFormLinkGetter<TScope> link)
        {
            return UsageLinks.Contains(link);
        }
        
        public bool Contains(TScope record)
        {
            return UsageLinks.Contains(record.ToLinkGetter<TScope>());
        }
    }
}