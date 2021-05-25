using Loqui;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Records.Internals;
using Noggog;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;

namespace Mutagen.Bethesda.Cache.Implementations
{
    /// <summary>
    /// A Link Cache using a LoadOrder as its link target. <br/>
    /// Will resolve links to the highest overriding mod containing the record being sought. <br/>
    /// <br/>
    /// Internal caching will only occur as deep into the load order as necessary, for only the types required
    /// to serve the requested link.
    /// <br/>
    /// All functionality is multithread safe. <br/>
    /// <br/>
    /// Modification of the target LoadOrder, or Mods on the LoadOrder is not safe.  Internal caches can become
    /// incorrect if modifications occur on content already cached.
    /// </summary>
    public class ImmutableLoadOrderLinkCache : ILinkCache
    {
        internal readonly bool _hasAny;
        protected readonly GameCategory _gameCategory;
        internal bool _simple;

        private readonly IReadOnlyList<IModGetter> _listedOrder;
        private readonly IReadOnlyList<IModGetter> _priorityOrder;
        private readonly ImmutableLoadOrderLinkCacheCategory<FormKey> _formKeyCache;
        private readonly ImmutableLoadOrderLinkCacheCategory<string> _editorIdCache;
        internal readonly IReadOnlyDictionary<Type, Type[]> _linkInterfaces;

        /// <inheritdoc />
        public IReadOnlyList<IModGetter> ListedOrder => _listedOrder;

        /// <inheritdoc />
        public IReadOnlyList<IModGetter> PriorityOrder => _priorityOrder;

        public ImmutableLoadOrderLinkCache(IEnumerable<IModGetter> loadOrder, GameCategory gameCategory, LinkCachePreferences? prefs = null)
        {
            prefs ??= LinkCachePreferences.Default;
            this._listedOrder = loadOrder.ToList();
            this._priorityOrder = _listedOrder.Reverse().ToList();
            var firstMod = _listedOrder.FirstOrDefault();
            this._hasAny = firstMod != null;
            this._simple = prefs is LinkCachePreferenceOnlyIdentifiers;
            this._gameCategory = gameCategory;
            this._linkInterfaces = LinkInterfaceMapping.InterfaceToObjectTypes(_gameCategory);
            this._formKeyCache = new ImmutableLoadOrderLinkCacheCategory<FormKey>(
                this,
                m => TryGet<FormKey>.Succeed(m.FormKey),
                f => f.IsNull);
            this._editorIdCache = new ImmutableLoadOrderLinkCacheCategory<string>(
                this,
                m =>
                {
                    var edid = m.EditorID;
                    return TryGet<string>.Create(successful: !string.IsNullOrWhiteSpace(edid), edid!);
                },
                e => e.IsNullOrWhitespace());
        }

        internal static bool ShouldStopQuery<K, T>(ModKey? modKey, int modCount, DepthCache<K, T> cache)
            where K : notnull
        {
            if (cache.Depth >= modCount)
            {
                cache.Done = true;
                return true;
            }

            // If we're going deeper than the originating mod of the target FormKey, we can stop
            if (modKey != null && cache.PassedMods.Contains(modKey.Value))
            {
                return true;
            }

            return false;
        }

        /// <inheritdoc />
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public bool TryResolve(FormKey formKey, [MaybeNullWhen(false)] out IMajorRecordCommonGetter majorRec)
        {
            return TryResolve<IMajorRecordCommonGetter>(formKey, out majorRec);
        }

        /// <inheritdoc />
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public bool TryResolve(string editorId, [MaybeNullWhen(false)] out IMajorRecordCommonGetter majorRec)
        {
            return TryResolve<IMajorRecordCommonGetter>(editorId, out majorRec);
        }

        /// <inheritdoc />
        public bool TryResolve<TMajor>(FormKey formKey, [MaybeNullWhen(false)] out TMajor majorRec)
            where TMajor : class, IMajorRecordCommonGetter
        {
            if (!TryResolve(formKey, typeof(TMajor), out var commonRec))
            {
                majorRec = default;
                return false;
            }

            majorRec = commonRec as TMajor;
            return majorRec != null;
        }

        /// <inheritdoc />
        public bool TryResolve<TMajor>(string editorId, [MaybeNullWhen(false)] out TMajor majorRec)
            where TMajor : class, IMajorRecordCommonGetter
        {
            if (!TryResolve(editorId, typeof(TMajor), out var commonRec))
            {
                majorRec = default;
                return false;
            }

            majorRec = commonRec as TMajor;
            return majorRec != null;
        }

        /// <inheritdoc />
        public bool TryResolve(FormKey formKey, Type type, [MaybeNullWhen(false)] out IMajorRecordCommonGetter majorRec)
        {
            if (_formKeyCache.TryResolve(formKey, formKey.ModKey, type, out var item))
            {
                majorRec = item.Record;
                return true;
            }
            majorRec = default;
            return false;
        }

        /// <inheritdoc />
        public bool TryResolve(string editorId, Type type, [MaybeNullWhen(false)] out IMajorRecordCommonGetter majorRec)
        {
            if (_editorIdCache.TryResolve(editorId, default(ModKey?), type, out var item))
            {
                majorRec = item.Record;
                return true;
            }
            majorRec = default;
            return false;
        }

        /// <inheritdoc />
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public IMajorRecordCommonGetter Resolve(FormKey formKey)
        {
            if (TryResolve<IMajorRecordCommonGetter>(formKey, out var commonRec)) return commonRec;
            throw new MissingRecordException(formKey, typeof(IMajorRecordCommonGetter));
        }

        /// <inheritdoc />
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public IMajorRecordCommonGetter Resolve(string editorId)
        {
            if (TryResolve<IMajorRecordCommonGetter>(editorId, out var commonRec)) return commonRec;
            throw new MissingRecordException(editorId, typeof(IMajorRecordCommonGetter));
        }

        /// <inheritdoc />
        public IMajorRecordCommonGetter Resolve(FormKey formKey, Type type)
        {
            if (TryResolve(formKey, type, out var commonRec)) return commonRec;
            throw new MissingRecordException(formKey, type);
        }

        /// <inheritdoc />
        public IMajorRecordCommonGetter Resolve(string editorId, Type type)
        {
            if (TryResolve(editorId, type, out var commonRec)) return commonRec;
            throw new MissingRecordException(editorId, type);
        }

        /// <inheritdoc />
        public TMajor Resolve<TMajor>(FormKey formKey)
            where TMajor : class, IMajorRecordCommonGetter
        {
            if (TryResolve<TMajor>(formKey, out var commonRec)) return commonRec;
            throw new MissingRecordException(formKey, typeof(TMajor));
        }

        /// <inheritdoc />
        public TMajor Resolve<TMajor>(string editorId)
            where TMajor : class, IMajorRecordCommonGetter
        {
            if (TryResolve<TMajor>(editorId, out var commonRec)) return commonRec;
            throw new MissingRecordException(editorId, typeof(TMajor));
        }

        /// <inheritdoc />
        public IEnumerable<TMajor> ResolveAll<TMajor>(FormKey formKey)
            where TMajor : class, IMajorRecordCommonGetter
        {
            return ResolveAll(formKey, typeof(TMajor)).Cast<TMajor>();
        }

        /// <inheritdoc />
        public IEnumerable<IMajorRecordCommonGetter> ResolveAll(FormKey formKey, Type type)
        {
            return _formKeyCache.ResolveAll(formKey, formKey.ModKey, type).Select(i => i.Record);
        }

        /// <inheritdoc />
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public IEnumerable<IMajorRecordCommonGetter> ResolveAll(FormKey formKey)
        {
            return ResolveAll(formKey, typeof(IMajorRecordCommonGetter));
        }

        /// <inheritdoc />
        public bool TryResolve(FormKey formKey, [MaybeNullWhen(false)] out IMajorRecordCommonGetter majorRec, params Type[] types)
        {
            return TryResolve(formKey, (IEnumerable<Type>)types, out majorRec);
        }

        /// <inheritdoc />
        public bool TryResolve(string editorId, [MaybeNullWhen(false)] out IMajorRecordCommonGetter majorRec, params Type[] types)
        {
            return TryResolve(editorId, (IEnumerable<Type>)types, out majorRec);
        }

        /// <inheritdoc />
        public bool TryResolve(FormKey formKey, IEnumerable<Type> types, [MaybeNullWhen(false)] out IMajorRecordCommonGetter majorRec)
        {
            foreach (var type in types)
            {
                if (TryResolve(formKey, type, out majorRec))
                {
                    return true;
                }
            }
            majorRec = default;
            return false;
        }

        /// <inheritdoc />
        public bool TryResolve(string editorId, IEnumerable<Type> types, [MaybeNullWhen(false)] out IMajorRecordCommonGetter majorRec)
        {
            foreach (var type in types)
            {
                if (TryResolve(editorId, type, out majorRec))
                {
                    return true;
                }
            }
            majorRec = default;
            return false;
        }

        /// <inheritdoc />
        public IMajorRecordCommonGetter Resolve(FormKey formKey, params Type[] types)
        {
            return Resolve(formKey, (IEnumerable<Type>)types);
        }

        /// <inheritdoc />
        public IMajorRecordCommonGetter Resolve(string editorId, params Type[] types)
        {
            return Resolve(editorId, (IEnumerable<Type>)types);
        }

        /// <inheritdoc />
        public IMajorRecordCommonGetter Resolve(FormKey formKey, IEnumerable<Type> types)
        {
            if (TryResolve(formKey, types, out var commonRec)) return commonRec;
            throw new MissingRecordException(formKey, types.ToArray());
        }

        /// <inheritdoc />
        public IMajorRecordCommonGetter Resolve(string editorId, IEnumerable<Type> types)
        {
            if (TryResolve(editorId, types, out var commonRec)) return commonRec;
            throw new MissingRecordException(editorId, types.ToArray());
        }

        /// <inheritdoc />
        public bool TryResolveIdentifier(FormKey formKey, [MaybeNullWhen(false)] out string? editorId)
        {
            if (_formKeyCache.TryResolve(formKey, formKey.ModKey, typeof(IMajorRecordCommonGetter), out var rec))
            {
                editorId = rec.EditorID;
                return true;
            }

            editorId = default;
            return false;
        }

        /// <inheritdoc />
        public bool TryResolveIdentifier(string editorId, [MaybeNullWhen(false)] out FormKey formKey)
        {
            if (_editorIdCache.TryResolve(editorId, default, typeof(IMajorRecordCommonGetter), out var rec))
            {
                formKey = rec.FormKey;
                return true;
            }

            formKey = default;
            return false;
        }

        /// <inheritdoc />
        public bool TryResolveIdentifier(FormKey formKey, Type type, [MaybeNullWhen(false)] out string? editorId)
        {
            if (_formKeyCache.TryResolve(formKey, formKey.ModKey, type, out var rec))
            {
                editorId = rec.EditorID;
                return true;
            }

            editorId = default;
            return false;
        }

        /// <inheritdoc />
        public bool TryResolveIdentifier(string editorId, Type type, [MaybeNullWhen(false)] out FormKey formKey)
        {
            if (_editorIdCache.TryResolve(editorId, default, type, out var rec))
            {
                formKey = rec.FormKey;
                return true;
            }

            formKey = default;
            return false;
        }

        /// <inheritdoc />
        public bool TryResolveIdentifier<TMajor>(FormKey formKey, [MaybeNullWhen(false)] out string? editorId)
            where TMajor : class, IMajorRecordCommonGetter
        {
            if (_formKeyCache.TryResolve(formKey, formKey.ModKey, typeof(TMajor), out var rec))
            {
                editorId = rec.EditorID;
                return true;
            }

            editorId = default;
            return false;
        }

        /// <inheritdoc />
        public bool TryResolveIdentifier<TMajor>(string editorId, [MaybeNullWhen(false)] out FormKey formKey)
            where TMajor : class, IMajorRecordCommonGetter
        {
            if (_editorIdCache.TryResolve(editorId, default, typeof(TMajor), out var rec))
            {
                formKey = rec.FormKey;
                return true;
            }

            formKey = default;
            return false;
        }

        /// <inheritdoc />
        public bool TryResolveIdentifier(FormKey formKey, [MaybeNullWhen(false)] out string? editorId, params Type[] types)
        {
            return TryResolveIdentifier(formKey, (IEnumerable<Type>)types, out editorId);
        }

        /// <inheritdoc />
        public bool TryResolveIdentifier(string editorId, [MaybeNullWhen(false)] out FormKey formKey, params Type[] types)
        {
            return TryResolveIdentifier(editorId, (IEnumerable<Type>)types, out formKey);
        }

        /// <inheritdoc />
        public bool TryResolveIdentifier(FormKey formKey, IEnumerable<Type> types, [MaybeNullWhen(false)] out string? editorId)
        {
            foreach (var type in types)
            {
                if (_formKeyCache.TryResolve(formKey, formKey.ModKey, type, out var rec))
                {
                    editorId = rec.EditorID;
                    return true;
                }
            }

            editorId = default;
            return false;
        }

        /// <inheritdoc />
        public bool TryResolveIdentifier(string editorId, IEnumerable<Type> types, [MaybeNullWhen(false)] out FormKey formKey)
        {
            foreach (var type in types)
            {
                if (_editorIdCache.TryResolve(editorId, default, type, out var rec))
                {
                    formKey = rec.FormKey;
                    return true;
                }
            }

            formKey = default;
            return false;
        }

        /// <inheritdoc />
        public IEnumerable<IMajorRecordIdentifier> AllIdentifiers(Type type, CancellationToken? cancel = null)
        {
            return _formKeyCache.AllIdentifiers(type, cancel);
        }

        /// <inheritdoc />
        public IEnumerable<IMajorRecordIdentifier> AllIdentifiers<TMajor>(CancellationToken? cancel = null)
            where TMajor : class, IMajorRecordCommonGetter
        {
            return _formKeyCache.AllIdentifiers(typeof(TMajor), cancel);
        }

        /// <inheritdoc />
        public IEnumerable<IMajorRecordIdentifier> AllIdentifiers(params Type[] types)
        {
            return AllIdentifiers((IEnumerable<Type>)types, CancellationToken.None);
        }

        /// <inheritdoc />
        public IEnumerable<IMajorRecordIdentifier> AllIdentifiers(IEnumerable<Type> types, CancellationToken? cancel = null)
        {
            return types.SelectMany(type => AllIdentifiers(type, cancel))
                .Distinct(x => x.FormKey);
        }

        /// <inheritdoc />
        public void Dispose()
        {
        }

        /// <inheritdoc />
        public void Warmup(Type type)
        {
            _formKeyCache.Warmup(type);
        }

        /// <inheritdoc />
        public void Warmup<TMajor>()
        {
            _formKeyCache.Warmup(typeof(TMajor));
        }

        /// <inheritdoc />
        public void Warmup(params Type[] types)
        {
            Warmup((IEnumerable<Type>)types);
        }

        /// <inheritdoc />
        public void Warmup(IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                _formKeyCache.Warmup(type);
            }
        }
    }

    internal class ImmutableLoadOrderLinkCacheCategory<K>
        where K : notnull
    {
        private readonly ImmutableLoadOrderLinkCache _parent;
        private readonly Func<IMajorRecordCommonGetter, TryGet<K>> _keyGetter;
        private readonly Func<K, bool> _shortCircuit;
        private readonly Dictionary<Type, DepthCache<K, LinkCacheItem>> _winningRecords = new();
        private readonly Dictionary<Type, DepthCache<K, ImmutableList<LinkCacheItem>>> _allRecords = new();

        public ImmutableLoadOrderLinkCacheCategory(
            ImmutableLoadOrderLinkCache parent,
            Func<IMajorRecordCommonGetter, TryGet<K>> keyGetter,
            Func<K, bool> shortCircuit)
        {
            _parent = parent;
            _keyGetter = keyGetter;
            _shortCircuit = shortCircuit;
        }

        private DepthCache<K, LinkCacheItem> GetTypeCache(Type type)
        {
            lock (_winningRecords)
            {
                // Get cache object by type
                if (!_winningRecords.TryGetValue(type, out var cache))
                {
                    cache = new DepthCache<K, LinkCacheItem>();
                    if (type.Equals(typeof(IMajorRecordCommon))
                        || type.Equals(typeof(IMajorRecordCommonGetter)))
                    {
                        _winningRecords[typeof(IMajorRecordCommon)] = cache;
                        _winningRecords[typeof(IMajorRecordCommonGetter)] = cache;
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
                        if (!_parent._linkInterfaces.TryGetValue(type, out var objs))
                        {
                            throw new ArgumentException($"A lookup was queried for an unregistered type: {type.Name}");
                        }
                        _winningRecords[type] = cache;
                    }
                }
                return cache;
            }
        }

        private void FillNextCacheDepth(DepthCache<K, LinkCacheItem> cache, Type type)
        {
            // Get next unprocessed mod 
            var targetIndex = _parent.ListedOrder.Count - cache.Depth - 1;
            var targetMod = _parent.ListedOrder[targetIndex];
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
                    cache.AddIfMissing(key.Value, LinkCacheItem.Factory(record, _parent._simple));
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

        public bool TryResolve(
            K key,
            ModKey? modKey,
            Type type,
            [MaybeNullWhen(false)] out LinkCacheItem majorRec)
        {
            if (!_parent._hasAny || _shortCircuit(key))
            {
                majorRec = default;
                return false;
            }

            DepthCache<K, LinkCacheItem> cache = GetTypeCache(type);

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
                if (ImmutableLoadOrderLinkCache.ShouldStopQuery(modKey, _parent.ListedOrder.Count, cache))
                {
                    majorRec = default;
                    return false;
                }

                while (!ImmutableLoadOrderLinkCache.ShouldStopQuery(modKey, _parent.ListedOrder.Count, cache))
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
            K key,
            ModKey? modKey,
            Type type)
        {
            if (!_parent._hasAny || _shortCircuit(key))
            {
                yield break;
            }

            // Grab the type cache
            DepthCache<K, ImmutableList<LinkCacheItem>> cache;
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

            // Return everyhing we have already
            foreach (var item in list)
            {
                yield return item;
            }

            int iteratedCount = list.Count;
            bool more = !ImmutableLoadOrderLinkCache.ShouldStopQuery(modKey, _parent.ListedOrder.Count, cache);

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
                        var targetIndex = _parent.ListedOrder.Count - cache.Depth - 1;
                        var targetMod = _parent.ListedOrder[targetIndex];
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
                                cache.Set(iterKey.Value, targetList.Add(LinkCacheItem.Factory(item, _parent._simple)));
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
                    more = !ImmutableLoadOrderLinkCache.ShouldStopQuery(modKey, _parent.ListedOrder.Count, cache);
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
            if (!_parent._hasAny || (cancel?.IsCancellationRequested ?? true))
            {
                return Enumerable.Empty<LinkCacheItem>();
            }

            DepthCache<K, LinkCacheItem> cache = GetTypeCache(type);
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
                while (!ImmutableLoadOrderLinkCache.ShouldStopQuery(modKey: null, _parent.ListedOrder.Count, cache))
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
            DepthCache<K, LinkCacheItem> cache = GetTypeCache(type);

            lock (cache)
            {
                // Fill all
                while (!ImmutableLoadOrderLinkCache.ShouldStopQuery(modKey: null, _parent.ListedOrder.Count, cache))
                {
                    FillNextCacheDepth(cache, type);
                }
            }
        }
    }

    /// <summary>
    /// A Link Cache using a LoadOrder as its link target. <br/>
    /// Will resolve links to the highest overriding mod containing the record being sought. <br/>
    /// <br/>
    /// Internal caching will only occur as deep into the load order as necessary, for only the types required
    /// to serve the requested link.
    /// <br/>
    /// All functionality is multithread safe. <br/>
    /// <br/>
    /// Modification of the target LoadOrder, or Mods on the LoadOrder is not safe.  Internal caches can become
    /// incorrect if modifications occur on content already cached.
    /// </summary>
    /// <typeparam name="TMod">Mod setter type</typeparam>
    /// <typeparam name="TModGetter">Mod getter type</typeparam>
    public class ImmutableLoadOrderLinkCache<TMod, TModGetter> : ImmutableLoadOrderLinkCache, ILinkCache<TMod, TModGetter>
        where TMod : class, IContextMod<TMod, TModGetter>, TModGetter
        where TModGetter : class, IContextGetterMod<TMod, TModGetter>
    {
        public static readonly ImmutableLoadOrderLinkCache<TMod, TModGetter> Empty = new(Enumerable.Empty<TModGetter>(), LinkCachePreferences.Default);

        private readonly ImmutableLoadOrderLinkCacheContextCategory<TMod, TModGetter, FormKey> _formKeyContextCache;
        private readonly ImmutableLoadOrderLinkCacheContextCategory<TMod, TModGetter, string> _editorIdContextCache;

        internal readonly IReadOnlyList<TModGetter> _listedOrder;

        /// <summary>
        /// Constructs a LoadOrderLinkCache around a target load order
        /// </summary>
        /// <param name="loadOrder">LoadOrder to resolve against when linking</param>
        public ImmutableLoadOrderLinkCache(IEnumerable<TModGetter> loadOrder, LinkCachePreferences prefs)
            : base(loadOrder, GameCategoryHelper.FromModType<TModGetter>(), prefs)
        {
            this._listedOrder = loadOrder.ToList();
            this._formKeyContextCache = new ImmutableLoadOrderLinkCacheContextCategory<TMod, TModGetter, FormKey>(
                this,
                m => TryGet<FormKey>.Succeed(m.FormKey),
                f => f.IsNull);
            this._editorIdContextCache = new ImmutableLoadOrderLinkCacheContextCategory<TMod, TModGetter, string>(
                this,
                m =>
                {
                    var edid = m.EditorID;
                    return TryGet<string>.Create(successful: !string.IsNullOrWhiteSpace(edid), edid!);
                },
                e => e.IsNullOrWhitespace());
        }

        /// <inheritdoc />
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public bool TryResolveContext(FormKey formKey, [MaybeNullWhen(false)] out IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter> majorRec)
        {
            return TryResolveContext<IMajorRecordCommon, IMajorRecordCommonGetter>(formKey, out majorRec);
        }

        /// <inheritdoc />
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public bool TryResolveContext(string editorId, [MaybeNullWhen(false)] out IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter> majorRec)
        {
            return TryResolveContext<IMajorRecordCommon, IMajorRecordCommonGetter>(editorId, out majorRec);
        }

        /// <inheritdoc />
        public bool TryResolveContext<TMajor, TMajorGetter>(FormKey formKey, [MaybeNullWhen(false)] out IModContext<TMod, TModGetter, TMajor, TMajorGetter> majorRec)
            where TMajor : class, IMajorRecordCommon, TMajorGetter
            where TMajorGetter : class, IMajorRecordCommonGetter
        {
            if (!TryResolveContext(formKey, typeof(TMajorGetter), out var commonRec)
                || !(commonRec.Record is TMajorGetter))
            {
                majorRec = default;
                return false;
            }

            majorRec = commonRec.AsType<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter, TMajor, TMajorGetter>();
            return majorRec != null;
        }

        /// <inheritdoc />
        public bool TryResolveContext<TMajor, TMajorGetter>(string editorId, [MaybeNullWhen(false)] out IModContext<TMod, TModGetter, TMajor, TMajorGetter> majorRec)
            where TMajor : class, IMajorRecordCommon, TMajorGetter
            where TMajorGetter : class, IMajorRecordCommonGetter
        {
            if (!TryResolveContext(editorId, typeof(TMajorGetter), out var commonRec)
                || !(commonRec.Record is TMajorGetter))
            {
                majorRec = default;
                return false;
            }

            majorRec = commonRec.AsType<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter, TMajor, TMajorGetter>();
            return majorRec != null;
        }

        /// <inheritdoc />
        public bool TryResolveContext(FormKey formKey, Type type, [MaybeNullWhen(false)] out IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter> majorRec)
        {
            return _formKeyContextCache.TryResolveContext(formKey, formKey.ModKey, type, out majorRec);
        }

        /// <inheritdoc />
        public bool TryResolveContext(string editorId, Type type, [MaybeNullWhen(false)] out IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter> majorRec)
        {
            if (!_hasAny || string.IsNullOrWhiteSpace(editorId))
            {
                majorRec = default;
                return false;
            }

            return _editorIdContextCache.TryResolveContext(editorId, default(ModKey?), type, out majorRec);
        }

        /// <inheritdoc />
        public IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter> ResolveContext(FormKey formKey)
        {
            if (TryResolveContext<IMajorRecordCommon, IMajorRecordCommonGetter>(formKey, out var commonRec)) return commonRec;
            throw new MissingRecordException(formKey, typeof(IMajorRecordCommonGetter));
        }

        /// <inheritdoc />
        public IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter> ResolveContext(string editorId)
        {
            if (TryResolveContext<IMajorRecordCommon, IMajorRecordCommonGetter>(editorId, out var commonRec)) return commonRec;
            throw new MissingRecordException(editorId, typeof(IMajorRecordCommonGetter));
        }

        /// <inheritdoc />
        public IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter> ResolveContext(FormKey formKey, Type type)
        {
            if (TryResolveContext(formKey, type, out var commonRec)) return commonRec;
            throw new MissingRecordException(formKey, type);
        }

        /// <inheritdoc />
        public IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter> ResolveContext(string editorId, Type type)
        {
            if (TryResolveContext(editorId, type, out var commonRec)) return commonRec;
            throw new MissingRecordException(editorId, type);
        }

        /// <inheritdoc />
        public IModContext<TMod, TModGetter, TMajor, TMajorGetter> ResolveContext<TMajor, TMajorGetter>(FormKey formKey)
            where TMajor : class, IMajorRecordCommon, TMajorGetter
            where TMajorGetter : class, IMajorRecordCommonGetter
        {
            if (TryResolveContext<TMajor, TMajorGetter>(formKey, out var commonRec)) return commonRec;
            throw new MissingRecordException(formKey, typeof(TMajorGetter));
        }

        /// <inheritdoc />
        public IModContext<TMod, TModGetter, TMajor, TMajorGetter> ResolveContext<TMajor, TMajorGetter>(string editorId)
            where TMajor : class, IMajorRecordCommon, TMajorGetter
            where TMajorGetter : class, IMajorRecordCommonGetter
        {
            if (TryResolveContext<TMajor, TMajorGetter>(editorId, out var commonRec)) return commonRec;
            throw new MissingRecordException(editorId, typeof(TMajorGetter));
        }

        /// <inheritdoc />
        public IEnumerable<IModContext<TMod, TModGetter, TMajor, TMajorGetter>> ResolveAllContexts<TMajor, TMajorGetter>(FormKey formKey)
            where TMajor : class, IMajorRecordCommon, TMajorGetter
            where TMajorGetter : class, IMajorRecordCommonGetter
        {
            return ResolveAllContexts(formKey, typeof(TMajorGetter))
                .Select(c => c.AsType<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter, TMajor, TMajorGetter>());
        }

        /// <inheritdoc />
        public IEnumerable<IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter>> ResolveAllContexts(FormKey formKey, Type type)
        {
            // Break early if no content
            if (!_hasAny || formKey.IsNull)
            {
                return Enumerable.Empty<IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter>>();
            }

            return _formKeyContextCache.ResolveAllContexts(formKey, formKey.ModKey, type);
        }

        /// <inheritdoc />
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public IEnumerable<IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter>> ResolveAllContexts(FormKey formKey)
        {
            return ResolveAllContexts(formKey, typeof(IMajorRecordCommonGetter));
        }
    }

    internal class ImmutableLoadOrderLinkCacheContextCategory<TMod, TModGetter, K>
        where K : notnull
        where TMod : class, IContextMod<TMod, TModGetter>, TModGetter
        where TModGetter : class, IContextGetterMod<TMod, TModGetter>
    {
        private readonly ImmutableLoadOrderLinkCache<TMod, TModGetter> _parent;
        private readonly Func<IMajorRecordCommonGetter, TryGet<K>> _keyGetter;
        private readonly Func<K, bool> _shortCircuit;
        private readonly Dictionary<Type, DepthCache<K, IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter>>> _winningContexts
            = new Dictionary<Type, DepthCache<K, IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter>>>();
        private readonly Dictionary<Type, DepthCache<K, ImmutableList<IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter>>>> _allContexts
            = new Dictionary<Type, DepthCache<K, ImmutableList<IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter>>>>();

        public ImmutableLoadOrderLinkCacheContextCategory(
            ImmutableLoadOrderLinkCache<TMod, TModGetter> parent,
            Func<IMajorRecordCommonGetter, TryGet<K>> keyGetter,
            Func<K, bool> shortCircuit)
        {
            _parent = parent;
            _keyGetter = keyGetter;
            _shortCircuit = shortCircuit;
        }

        public bool TryResolveContext(
            K key,
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

            DepthCache<K, IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter>>? cache;
            lock (_winningContexts)
            {
                // Get cache object by type
                if (!_winningContexts.TryGetValue(type, out cache))
                {
                    cache = new DepthCache<K, IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter>>();
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
            K key,
            ModKey? modKey,
            Type type)
        {
            if (!_parent._hasAny || _shortCircuit(key))
            {
                yield break;
            }

            // Grab the type cache
            DepthCache<K, ImmutableList<IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter>>> cache;
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

    internal class DepthCache<K, T>
        where K : notnull
    {
        private readonly Dictionary<K, T> _dictionary = new Dictionary<K, T>();
        public HashSet<ModKey> PassedMods = new HashSet<ModKey>();
        public int Depth;
        public bool Done;

        public IReadOnlyCollection<T> Values => _dictionary.Values;

        public bool TryGetValue(K key, [MaybeNullWhen(false)] out T value)
        {
            return _dictionary.TryGetValue(key, out value);
        }

        public void AddIfMissing(K key, T item)
        {
            if (!_dictionary.ContainsKey(key))
            {
                _dictionary[key] = item;
            }
        }

        public void Add(K key, T item)
        {
            _dictionary.Add(key, item);
        }

        public void Set(K key, T item)
        {
            _dictionary[key] = item;
        }
    }
}
