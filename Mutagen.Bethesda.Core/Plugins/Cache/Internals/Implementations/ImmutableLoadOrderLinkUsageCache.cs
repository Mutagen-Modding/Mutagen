using System.Collections.Concurrent;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Cache.Internals.Implementations;

public sealed class ImmutableLoadOrderLinkUsageCache : ILinkUsageCache
{
    private record struct CacheKey(
        FormKey FormKey,
        Type UserRecordType);

    private record CacheItem(
        Lazy<ILinkUsageResults<IMajorRecordGetter>> Untyped,
        object? Typed)
    {
        public static CacheItem Empty = new CacheItem(
            new Lazy<ILinkUsageResults<IMajorRecordGetter>>(() => Results<IMajorRecordGetter>.Empty),
            null);
    }
    
    private readonly ILinkCache _linkCache;
    private readonly int? _threadLimit;
    private readonly Dictionary<Type, Lazy<Dictionary<CacheKey, CacheItem>>> _cache = new();
    
    public ImmutableLoadOrderLinkUsageCache(
        ILinkCache linkCache,
        int? threadLimit = null)
    {
        _linkCache = linkCache;
        _threadLimit = threadLimit;
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
        return Results<TUserRecordScope>.Empty;
    }

    private Dictionary<CacheKey, CacheItem> ConstructCacheFor<TUserRecordScope>()
        where TUserRecordScope : class, IMajorRecordGetter
    {
        var scopeType = typeof(TUserRecordScope);
        ConcurrentDictionary<FormKey, ConcurrentBag<IFormLinkGetter<TUserRecordScope>>> accumulation = new();
        Parallel.ForEach(
            _linkCache.PriorityOrder.WinningOverrides<TUserRecordScope>(),
            new ParallelOptions()
            {
                MaxDegreeOfParallelism = _threadLimit ?? -1
            },
            record =>
            {
                var recordLinks = record.EnumerateFormLinks()
                    .Where(link => !link.IsNull)
                    .ToArray();
                if (recordLinks.Length == 0) return;
                var recordLink = record.ToLinkGetter();
                foreach (var link in recordLinks)
                {
                    accumulation.GetOrAdd(link.FormKey).Add(recordLink);
                }
            });

        var cacheItems = new List<KeyValuePair<CacheKey, CacheItem>>();
        foreach (var item in accumulation)
        {
            var untyped = new Lazy<ILinkUsageResults<IMajorRecordGetter>>(() =>
            {
                return new Results<IMajorRecordGetter>(
                    item.Value.Select<IFormLinkGetter<TUserRecordScope>, IFormLinkGetter<IMajorRecordGetter>>(x => x)
                        .ToHashSet());
            });
        
            var cacheItem = new CacheItem(
                Untyped: untyped,
                Typed: new Results<TUserRecordScope>(item.Value.ToHashSet()));
            cacheItems.Add(
                new KeyValuePair<CacheKey, CacheItem>(
                    new CacheKey(item.Key, scopeType),
                    cacheItem));
        }

        var ret = new Dictionary<CacheKey, CacheItem>();
        foreach (var item in cacheItems)
        {
            ret[item.Key] = item.Value;
        }

        return ret;
    }
    
    private CacheItem GetUsagesOfGeneric<TUserRecordScope>(
        IFormLinkIdentifier identifier)
        where TUserRecordScope : class, IMajorRecordGetter
    {
        var scopeType = typeof(TUserRecordScope);
        var key = new CacheKey(
            identifier.FormKey,
            UserRecordType: scopeType);

        Lazy<Dictionary<CacheKey, CacheItem>>? typedCache;
        lock (_cache)
        {
            if (!_cache.TryGetValue(scopeType, out typedCache))
            {
                typedCache = new Lazy<Dictionary<CacheKey, CacheItem>>(ConstructCacheFor<TUserRecordScope>);
                _cache[scopeType] = typedCache;
            }
        }

        if (typedCache.Value.TryGetValue(key, out var cache))
        {
            return cache;
        }
        
        return CacheItem.Empty;
    }

    private class Results<TScope> : ILinkUsageResults<TScope>
        where TScope : class, IMajorRecordGetter
    {
        public static Results<TScope> Empty { get; } = new(new HashSet<IFormLinkGetter<TScope>>());
        
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