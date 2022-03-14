using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using Loqui;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Records.Internals;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Cache.Internals.Implementations.Internal
{
    internal class ImmutableLoadOrderLinkCacheCategory<TKey>
        where TKey : notnull
    {
        private readonly GameCategory _gameCategory;
        private readonly bool _hasAny;
        private readonly bool _simple;
        private readonly IReadOnlyList<IModGetter> _listedOrder;
        private readonly IMetaInterfaceMapGetter _metaInterfaceMapGetter;
        private readonly Func<IMajorRecordGetter, TryGet<TKey>> _keyGetter;
        private readonly Func<TKey, bool> _shortCircuit;
        private readonly Dictionary<Type, DepthCache<TKey, LinkCacheItem>> _winningRecords = new();
        private readonly Dictionary<Type, DepthCache<TKey, ImmutableList<LinkCacheItem>>> _allRecords = new();

        public ImmutableLoadOrderLinkCacheCategory(
            GameCategory gameCategory,
            bool hasAny,
            bool simple,
            IReadOnlyList<IModGetter> listedOrder,
            IMetaInterfaceMapGetter metaInterfaceMapGetter,
            Func<IMajorRecordGetter, TryGet<TKey>> keyGetter,
            Func<TKey, bool> shortCircuit)
        {
            _gameCategory = gameCategory;
            _hasAny = hasAny;
            _simple = simple;
            _listedOrder = listedOrder;
            _metaInterfaceMapGetter = metaInterfaceMapGetter;
            _keyGetter = keyGetter;
            _shortCircuit = shortCircuit;
        }

        private DepthCache<TKey, LinkCacheItem> GetTypeCache(Type type)
        {
            lock (_winningRecords)
            {
                // Get cache object by type
                if (!_winningRecords.TryGetValue(type, out var cache))
                {
                    cache = new DepthCache<TKey, LinkCacheItem>();
                    if (type == typeof(IMajorRecord)
                        || type == typeof(IMajorRecordGetter))
                    {
                        _winningRecords[typeof(IMajorRecord)] = cache;
                        _winningRecords[typeof(IMajorRecordGetter)] = cache;
                    }
                    else if (LoquiRegistration.TryGetRegister(type, out var registration))
                    {
                        _winningRecords[registration.ClassType] = cache;
                        _winningRecords[registration.GetterType] = cache;
                        _winningRecords[registration.SetterType] = cache;
                        if (registration.InternalGetterType != null)
                        {
                            _winningRecords[registration.InternalGetterType] = cache;
                        }
                        if (registration.InternalSetterType != null)
                        {
                            _winningRecords[registration.InternalSetterType] = cache;
                        }
                    }
                    else
                    {
                        if (!_metaInterfaceMapGetter.TryGetRegistrationsForInterface(_gameCategory, type, out _))
                        {
                            throw new ArgumentException($"A lookup was queried for an unregistered type: {type.Name}");
                        }
                        _winningRecords[type] = cache;
                    }
                }
                return cache;
            }
        }

        private void FillNextCacheDepth(DepthCache<TKey, LinkCacheItem> cache, Type type)
        {
            // Get next unprocessed mod 
            var targetIndex = _listedOrder.Count - cache.Depth - 1;
            var targetMod = _listedOrder[targetIndex];
            cache.Depth++;
            cache.PassedMods.Add(targetMod.ModKey);

            void AddRecords(IModGetter mod, Type type)
            {
                foreach (var record in mod.EnumerateMajorRecords(type)
                    // ToDo
                    // Capture and expose errors optionally via TryResolve /w out param
                    .Catch((Exception ex) => { }))
                {
                    var key = _keyGetter(record);
                    if (key.Failed) continue;
                    cache.AddIfMissing(key.Value, LinkCacheItem.Factory(record, _simple));
                }
            }

            // Add records from that mod that aren't already cached 
            if (_metaInterfaceMapGetter.TryGetRegistrationsForInterface(_gameCategory, type, out var objs))
            {
                foreach (var regis in objs.Registrations)
                {
                    AddRecords(targetMod, regis.GetterType);
                }
            }
            else
            {
                AddRecords(targetMod, type);
            }
        }

        public bool TryResolve(
            TKey key,
            ModKey? modKey,
            Type type,
            [MaybeNullWhen(false)] out LinkCacheItem majorRec)
        {
            if (!_hasAny || _shortCircuit(key))
            {
                majorRec = default;
                return false;
            }

            DepthCache<TKey, LinkCacheItem> cache = GetTypeCache(type);

            // If we're done, we can just query without locking
            if (cache.Done)
            {
                return cache.TryGetValue(key, out majorRec);
            }

            // Potentially more to query, need to lock
            lock (cache)
            {
                // Check for record 
                if (cache.TryGetValue(key, out majorRec))
                {
                    return true;
                }
                if (InternalImmutableLoadOrderLinkCache.ShouldStopQuery(modKey, _listedOrder.Count, cache))
                {
                    majorRec = default;
                    return false;
                }

                while (!InternalImmutableLoadOrderLinkCache.ShouldStopQuery(modKey, _listedOrder.Count, cache))
                {
                    FillNextCacheDepth(cache, type);

                    // Check again 
                    if (cache.TryGetValue(key, out majorRec))
                    {
                        return true;
                    }
                }
                // Record doesn't exist 
                majorRec = default;
                return false;
            }
        }

        public IEnumerable<LinkCacheItem> ResolveAll(
            TKey key,
            ModKey? modKey,
            Type type)
        {
            if (!_hasAny || _shortCircuit(key))
            {
                yield break;
            }

            // Grab the type cache
            DepthCache<TKey, ImmutableList<LinkCacheItem>> cache;
            lock (_allRecords)
            {
                cache = _allRecords.GetOrAdd(type);
            }

            // If we're done, we can just query without locking
            if (cache.Done)
            {
                if (cache.TryGetValue(key, out var doneList))
                {
                    foreach (var val in doneList)
                    {
                        yield return val;
                    }
                }
                yield break;
            }

            // Grab the formkey's list
            ImmutableList<LinkCacheItem>? list;
            int consideredDepth;
            lock (cache)
            {
                if (!cache.TryGetValue(key, out list))
                {
                    list = ImmutableList<LinkCacheItem>.Empty;
                    cache.Add(key, list);
                }
                consideredDepth = cache.Depth;
            }

            // Return everything we have already
            foreach (var item in list)
            {
                yield return item;
            }

            int iteratedCount = list.Count;
            bool more = !InternalImmutableLoadOrderLinkCache.ShouldStopQuery(modKey, _listedOrder.Count, cache);

            // While there's more depth to consider
            while (more)
            {
                // Process one more mod
                lock (cache)
                {
                    // Only process if no one else has done some work
                    if (consideredDepth == cache.Depth)
                    {
                        // Get next unprocessed mod
                        var targetIndex = _listedOrder.Count - cache.Depth - 1;
                        var targetMod = _listedOrder[targetIndex];
                        cache.Depth++;
                        cache.PassedMods.Add(targetMod.ModKey);

                        void AddRecords(IModGetter mod, Type type)
                        {
                            foreach (var item in mod.EnumerateMajorRecords(type)
                                // ToDo
                                // Capture and expose errors optionally via TryResolve /w out param
                                .Catch((Exception ex) => { }))
                            {
                                var iterKey = _keyGetter(item);
                                if (iterKey.Failed) continue;
                                if (!cache.TryGetValue(iterKey.Value, out var targetList))
                                {
                                    targetList = ImmutableList<LinkCacheItem>.Empty;
                                }
                                cache.Set(iterKey.Value, targetList.Add(LinkCacheItem.Factory(item, _simple)));
                            }
                            if (cache.TryGetValue(key, out var requeriedList))
                            {
                                list = requeriedList;
                            }
                        }

                        // Add records from that mod that aren't already cached
                        if (_metaInterfaceMapGetter.TryGetRegistrationsForInterface(_gameCategory, type, out var objs))
                        {
                            foreach (var regis in objs.Registrations)
                            {
                                AddRecords(targetMod, regis.GetterType);
                            }
                        }
                        else
                        {
                            AddRecords(targetMod, type);
                        }
                    }
                    consideredDepth = cache.Depth;
                    more = !InternalImmutableLoadOrderLinkCache.ShouldStopQuery(modKey, _listedOrder.Count, cache);
                }

                // Return any new data
                for (int i = iteratedCount; i < list.Count; i++)
                {
                    yield return list[i];
                }
                iteratedCount = list.Count;
            }
        }

        public IEnumerable<LinkCacheItem> AllIdentifiers(Type type, CancellationToken? cancel)
        {
            if (!_hasAny || (cancel?.IsCancellationRequested ?? true))
            {
                return Enumerable.Empty<LinkCacheItem>();
            }

            DepthCache<TKey, LinkCacheItem> cache = GetTypeCache(type);
            if (cancel?.IsCancellationRequested ?? true)
            {
                return Enumerable.Empty<LinkCacheItem>();
            }

            // If we're done, we can just query without locking
            if (cache.Done)
            {
                return cache.Values;
            }

            // Potentially more to query, need to lock
            lock (cache)
            {
                // Fill all
                while (!InternalImmutableLoadOrderLinkCache.ShouldStopQuery(modKey: null, _listedOrder.Count, cache))
                {
                    if (cancel?.IsCancellationRequested ?? false) return Enumerable.Empty<LinkCacheItem>();
                    FillNextCacheDepth(cache, type);
                }
            }

            // Safe to return not-locked, because this particular cache will never be modified anymore, as it's fully queried
            if (cancel?.IsCancellationRequested ?? false) return Enumerable.Empty<LinkCacheItem>();
            return cache.Values;
        }

        public void Warmup(Type type)
        {
            DepthCache<TKey, LinkCacheItem> cache = GetTypeCache(type);

            lock (cache)
            {
                // Fill all
                while (!InternalImmutableLoadOrderLinkCache.ShouldStopQuery(modKey: null, _listedOrder.Count, cache))
                {
                    FillNextCacheDepth(cache, type);
                }
            }
        }
    }
}