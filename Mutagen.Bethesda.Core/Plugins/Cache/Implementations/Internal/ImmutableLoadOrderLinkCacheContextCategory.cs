using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Loqui;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Cache.Implementations.Internal
{
    internal class ImmutableLoadOrderLinkCacheContextCategory<TMod, TModGetter, TKey>
        where TKey : notnull
        where TMod : class, IContextMod<TMod, TModGetter>, TModGetter
        where TModGetter : class, IContextGetterMod<TMod, TModGetter>
    {
        private readonly ImmutableLoadOrderLinkCache<TMod, TModGetter> _parent;
        private readonly Func<IMajorRecordCommonGetter, TryGet<TKey>> _keyGetter;
        private readonly Func<TKey, bool> _shortCircuit;
        private readonly Dictionary<Type, DepthCache<TKey, IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter>>> _winningContexts = new();
        private readonly Dictionary<Type, DepthCache<TKey, ImmutableList<IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter>>>> _allContexts = new();

        public ImmutableLoadOrderLinkCacheContextCategory(
            ImmutableLoadOrderLinkCache<TMod, TModGetter> parent,
            Func<IMajorRecordCommonGetter, TryGet<TKey>> keyGetter,
            Func<TKey, bool> shortCircuit)
        {
            _parent = parent;
            _keyGetter = keyGetter;
            _shortCircuit = shortCircuit;
        }

        public bool TryResolveContext(
            TKey key,
            ModKey? modKey,
            Type type,
            [MaybeNullWhen(false)] out IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter> majorRec)
        {
            if (_parent._simple)
            {
                throw new ArgumentException("Queried for record on a simple cache");
            }
            
            if (!_parent._hasAny || _shortCircuit(key))
            {
                majorRec = default;
                return false;
            }

            DepthCache<TKey, IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter>>? cache;
            lock (_winningContexts)
            {
                // Get cache object by type
                if (!_winningContexts.TryGetValue(type, out cache))
                {
                    cache = new DepthCache<TKey, IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter>>();
                    if (type.Equals(typeof(IMajorRecordCommon))
                        || type.Equals(typeof(IMajorRecordCommonGetter)))
                    {
                        _winningContexts[typeof(IMajorRecordCommon)] = cache;
                        _winningContexts[typeof(IMajorRecordCommonGetter)] = cache;
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
                        if (!_parent._linkInterfaces.TryGetValue(type, out var objs))
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
                if (ImmutableLoadOrderLinkCache.ShouldStopQuery(modKey, _parent._listedOrder.Count, cache))
                {
                    majorRec = default!;
                    return false;
                }

                while (!ImmutableLoadOrderLinkCache.ShouldStopQuery(modKey, _parent._listedOrder.Count, cache))
                {
                    // Get next unprocessed mod
                    var targetIndex = _parent._listedOrder.Count - cache.Depth - 1;
                    var targetMod = _parent._listedOrder[targetIndex];
                    cache.Depth++;
                    cache.PassedMods.Add(targetMod.ModKey);

                    void AddRecords(TModGetter mod, Type type)
                    {
                        foreach (var record in mod.EnumerateMajorRecordContexts(_parent, type))
                        {
                            var key = _keyGetter(record.Record);
                            if (key.Failed) continue;
                            cache.AddIfMissing(key.Value, record);
                        }
                    }

                    // Add records from that mod that aren't already cached
                    if (_parent._linkInterfaces.TryGetValue(type, out var objs))
                    {
                        foreach (var objType in objs)
                        {
                            AddRecords(targetMod, LoquiRegistration.GetRegister(objType).GetterType);
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

        public IEnumerable<IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter>> ResolveAllContexts(
            TKey key,
            ModKey? modKey,
            Type type)
        {
            if (!_parent._hasAny || _shortCircuit(key))
            {
                yield break;
            }

            // Grab the type cache
            DepthCache<TKey, ImmutableList<IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter>>> cache;
            lock (_allContexts)
            {
                cache = _allContexts.GetOrAdd(type);
            }

            // Grab the formkey's list
            ImmutableList<IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter>>? list;
            int consideredDepth;
            lock (cache)
            {
                if (!cache.TryGetValue(key, out list))
                {
                    list = ImmutableList<IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter>>.Empty;
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
            bool more = !ImmutableLoadOrderLinkCache.ShouldStopQuery(modKey, _parent._listedOrder.Count, cache);

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
                        var targetIndex = _parent._listedOrder.Count - cache.Depth - 1;
                        var targetMod = _parent._listedOrder[targetIndex];
                        cache.Depth++;
                        cache.PassedMods.Add(targetMod.ModKey);

                        void AddRecords(TModGetter mod, Type type)
                        {
                            foreach (var item in mod.EnumerateMajorRecordContexts(_parent, type))
                            {
                                var iterKey = _keyGetter(item.Record);
                                if (iterKey.Failed) continue;
                                if (!cache.TryGetValue(iterKey.Value, out var targetList))
                                {
                                    targetList = ImmutableList<IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter>>.Empty;
                                }
                                cache.Set(iterKey.Value, targetList.Add(item));
                            }
                            if (cache.TryGetValue(key, out var requeriedList))
                            {
                                list = requeriedList;
                            }
                        }

                        // Add records from that mod that aren't already cached
                        if (_parent._linkInterfaces.TryGetValue(type, out var objs))
                        {
                            foreach (var objType in objs)
                            {
                                AddRecords(targetMod, LoquiRegistration.GetRegister(objType).GetterType);
                            }
                        }
                        else
                        {
                            AddRecords(targetMod, type);
                        }
                    }
                    consideredDepth = cache.Depth;
                    more = !ImmutableLoadOrderLinkCache.ShouldStopQuery(modKey, _parent._listedOrder.Count, cache);
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