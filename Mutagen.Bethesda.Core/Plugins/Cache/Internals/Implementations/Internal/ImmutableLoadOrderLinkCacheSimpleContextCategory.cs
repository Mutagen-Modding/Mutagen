using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Loqui;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Records.Internals;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Cache.Internals.Implementations.Internal
{
    public interface IImmutableLoadOrderLinkCacheSimpleContextCategory<TKey>
        where TKey : notnull
    {
        bool TryResolveSimpleContext(
            TKey key,
            ModKey? modKey,
            Type type,
            [MaybeNullWhen(false)] out IModContext<IMajorRecordGetter> majorRec);

        IEnumerable<IModContext<IMajorRecordGetter>> ResolveAllSimpleContexts(
            TKey key,
            ModKey? modKey,
            Type type);
    }

    internal class ImmutableLoadOrderLinkCacheSimpleContextCategory<TKey> : IImmutableLoadOrderLinkCacheSimpleContextCategory<TKey>
        where TKey : notnull
    {
        private readonly bool _simple;
        private readonly bool _hasAny;
        private readonly ILinkCache _linkCache;
        private readonly IReadOnlyList<IModGetter> _listedOrder;
        private readonly IReadOnlyDictionary<Type, ILoquiRegistration[]> _linkInterfaces;
        private readonly Func<IMajorRecordGetter, TryGet<TKey>> _keyGetter;
        private readonly Func<TKey, bool> _shortCircuit;
        private readonly Dictionary<Type, DepthCache<TKey, IModContext<IMajorRecordGetter>>> _winningContexts = new();
        private readonly Dictionary<Type, DepthCache<TKey, ImmutableList<IModContext<IMajorRecordGetter>>>> _allContexts = new();

        public ImmutableLoadOrderLinkCacheSimpleContextCategory(
            GameCategory category,
            ILinkInterfaceMapGetter linkInterfaceMapGetter,
            bool simple,
            bool hasAny,
            ILinkCache linkCache,
            IReadOnlyList<IModGetter> listedOrder,
            Func<IMajorRecordGetter, TryGet<TKey>> keyGetter,
            Func<TKey, bool> shortCircuit)
        {
            _simple = simple;
            _hasAny = hasAny;
            _linkCache = linkCache;
            _listedOrder = listedOrder;
            _keyGetter = keyGetter;
            _linkInterfaces = linkInterfaceMapGetter.InterfaceToObjectTypes(category);
            _shortCircuit = shortCircuit;
        }

        public bool TryResolveSimpleContext(TKey key, ModKey? modKey, Type type, [MaybeNullWhen(false)] out IModContext<IMajorRecordGetter> majorRec)
        {
            if (_simple)
            {
                throw new ArgumentException("Queried for record on a simple cache");
            }
            
            if (!_hasAny || _shortCircuit(key))
            {
                majorRec = default;
                return false;
            }

            DepthCache<TKey, IModContext<IMajorRecordGetter>>? cache;
            lock (_winningContexts)
            {
                // Get cache object by type
                if (!_winningContexts.TryGetValue(type, out cache))
                {
                    cache = new DepthCache<TKey, IModContext<IMajorRecordGetter>>();
                    if (type == typeof(IMajorRecord)
                        || type == typeof(IMajorRecordGetter))
                    {
                        _winningContexts[typeof(IMajorRecord)] = cache;
                        _winningContexts[typeof(IMajorRecordGetter)] = cache;
                    }
                    else if (LoquiRegistration.TryGetRegister(type, out var registration))
                    {
                        _winningContexts[registration.ClassType] = cache;
                        _winningContexts[registration.GetterType] = cache;
                        _winningContexts[registration.SetterType] = cache;
                        if (registration.InternalGetterType != null)
                        {
                            _winningContexts[registration.InternalGetterType] = cache;
                        }
                        if (registration.InternalSetterType != null)
                        {
                            _winningContexts[registration.InternalSetterType] = cache;
                        }
                    }
                    else
                    {
                        if (!_linkInterfaces.TryGetValue(type, out var objs))
                        {
                            throw new ArgumentException($"A lookup was queried for an unregistered type: {type.Name}");
                        }
                        _winningContexts[type] = cache;
                    }
                }
            }

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
                    majorRec = default!;
                    return false;
                }

                while (!InternalImmutableLoadOrderLinkCache.ShouldStopQuery(modKey, _listedOrder.Count, cache))
                {
                    // Get next unprocessed mod
                    var targetIndex = _listedOrder.Count - cache.Depth - 1;
                    var targetMod = _listedOrder[targetIndex];
                    cache.Depth++;
                    cache.PassedMods.Add(targetMod.ModKey);

                    void AddRecords(IModGetter mod, Type type)
                    {
                        foreach (var record in mod.EnumerateMajorRecordSimpleContexts(_linkCache, type))
                        {
                            var key = _keyGetter(record.Record);
                            if (key.Failed) continue;
                            cache.AddIfMissing(key.Value, record);
                        }
                    }

                    // Add records from that mod that aren't already cached
                    if (_linkInterfaces.TryGetValue(type, out var objs))
                    {
                        foreach (var regis in objs)
                        {
                            AddRecords(targetMod, regis.GetterType);
                        }
                    }
                    else
                    {
                        AddRecords(targetMod, type);
                    }
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

        public IEnumerable<IModContext<IMajorRecordGetter>> ResolveAllSimpleContexts(TKey key, ModKey? modKey, Type type)
        {
            if (!_hasAny || _shortCircuit(key))
            {
                yield break;
            }

            // Grab the type cache
            DepthCache<TKey, ImmutableList<IModContext<IMajorRecordGetter>>> cache;
            lock (_allContexts)
            {
                cache = _allContexts.GetOrAdd(type);
            }

            // Grab the formkey's list
            ImmutableList<IModContext<IMajorRecordGetter>>? list;
            int consideredDepth;
            lock (cache)
            {
                if (!cache.TryGetValue(key, out list))
                {
                    list = ImmutableList<IModContext<IMajorRecordGetter>>.Empty;
                    cache.Add(key, list);
                }
                consideredDepth = cache.Depth;
            }

            // Return everyhing we have already
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
                            foreach (var item in mod.EnumerateMajorRecordSimpleContexts(_linkCache, type))
                            {
                                var iterKey = _keyGetter(item.Record);
                                if (iterKey.Failed) continue;
                                if (!cache.TryGetValue(iterKey.Value, out var targetList))
                                {
                                    targetList = ImmutableList<IModContext<IMajorRecordGetter>>.Empty;
                                }
                                cache.Set(iterKey.Value, targetList.Add(item));
                            }
                            if (cache.TryGetValue(key, out var requeriedList))
                            {
                                list = requeriedList;
                            }
                        }

                        // Add records from that mod that aren't already cached
                        if (_linkInterfaces.TryGetValue(type, out var objs))
                        {
                            foreach (var regis in objs)
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
    }
}