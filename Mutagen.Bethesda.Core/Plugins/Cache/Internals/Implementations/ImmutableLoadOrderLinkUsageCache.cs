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
        object? Typed);
    
    private readonly ILinkCache _linkCache;
    private readonly Dictionary<CacheKey, CacheItem> _cache = new();
    private readonly object _untypedReferencesCacheLock = new();
    private Dictionary<FormKey, HashSet<FormKey>>? _untypedReferencesCache;
    private static HashSet<FormKey> _emptySet = new();
    
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
            UserRecordType: typeof(IMajorRecordGetter));
        
        lock (_cache)
        {
            if (_cache.TryGetValue(key, out var cached)) return cached.Untyped.Value;
        }
        
        return GetUsagesOfGeneric<IMajorRecordGetter>(identifier).Untyped.Value;
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

    public ILinkUsageResults<IMajorRecordGetter> GetUsagesOf(FormKey formKey)
    {
        var key = new CacheKey(formKey, 
            UserRecordType: typeof(IMajorRecordGetter));
        
        lock (_cache)
        {
            if (_cache.TryGetValue(key, out var cached)) return cached.Untyped.Value;
        }
        
        var untypedReferencesCache = GetUntypedReferencesCache();

        var formKeys = untypedReferencesCache.GetValueOrDefault(formKey, _emptySet);
        var result = new Results<IMajorRecordGetter>(
            new HashSet<IFormLinkGetter<IMajorRecordGetter>>(
                formKeys.Select(fk => new FormLink<IMajorRecordGetter>(fk))));
        
        lock (_cache)
        {
            _cache[key] = new CacheItem(
                Untyped: new Lazy<ILinkUsageResults<IMajorRecordGetter>>(result),
                Typed: null);
        }

        return result;
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
                links.Add(record.ToLink());
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
        private readonly Lazy<IReadOnlySet<FormKey>> _formKeys;
        private readonly Lazy<IReadOnlySet<IFormLinkIdentifier>> _identifiers;

        public IReadOnlySet<IFormLinkGetter<TScope>> UsageLinks { get; }

        public Results(IReadOnlySet<IFormLinkGetter<TScope>> links)
        {
            UsageLinks = links;
            _formKeys = new Lazy<IReadOnlySet<FormKey>>(() =>
            {
                return links
                    .Select(l => l.FormKey)
                    .ToHashSet();
            });
            _identifiers = new Lazy<IReadOnlySet<IFormLinkIdentifier>>(() =>
            {
                return links
                    .Select(l => new FormLinkInformation(l.FormKey, l.Type))
                    .ToHashSet(IFormLinkExt.FormLinkInformationEqualityComparerWithDualInheritanceConsideration);
            });
        }
        
        public bool Contains(FormKey formKey)
        {
            return _formKeys.Value.Contains(formKey);
        }
        
        public bool Contains(IFormLinkIdentifier identifier)
        {
            List<int> l;
            IReadOnlyCollection<IFormLinkIdentifier> set = _identifiers.Value;
            return set.Contains(identifier);
        }
        
        public bool Contains(IFormLinkGetter<TScope> link)
        {
            return UsageLinks.Contains(link);
        }
        
        public bool Contains(TScope record)
        {
            return UsageLinks.Contains(record.ToLink<TScope>());
        }
    }
}