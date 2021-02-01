using Loqui;
using Mutagen.Bethesda.Core;
using Noggog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Mutagen.Bethesda
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
        protected readonly bool _hasAny;
        protected readonly GameCategory _gameCategory;

        private readonly IReadOnlyList<IModGetter> _listedOrder;
        private readonly IReadOnlyList<IModGetter> _priorityOrder;
        private readonly Dictionary<Type, DepthCache<FormKey, IMajorRecordCommonGetter>> _winningFormKeyRecords
            = new Dictionary<Type, DepthCache<FormKey, IMajorRecordCommonGetter>>();
        private readonly Dictionary<Type, DepthCache<FormKey, ImmutableList<IMajorRecordCommonGetter>>> _allFormKeyRecords 
            = new Dictionary<Type, DepthCache<FormKey, ImmutableList<IMajorRecordCommonGetter>>>();
        protected readonly IReadOnlyDictionary<Type, Type[]> _linkInterfaces;

        /// <inheritdoc />
        public IReadOnlyList<IModGetter> ListedOrder => _listedOrder;

        /// <inheritdoc />
        public IReadOnlyList<IModGetter> PriorityOrder => _priorityOrder;

        public ImmutableLoadOrderLinkCache(IEnumerable<IModGetter> loadOrder)
        {
            this._listedOrder = loadOrder.ToList();
            this._priorityOrder = _listedOrder.Reverse().ToList();
            var firstMod = _listedOrder.FirstOrDefault();
            this._hasAny = firstMod != null;
            // ToDo
            // Upgrade to bounce off ModInstantiator systems
            this._gameCategory = firstMod?.GameRelease.ToCategory() ?? GameCategory.Oblivion;
            this._linkInterfaces = LinkInterfaceMapping.InterfaceToObjectTypes(_gameCategory);
        }

        internal static bool ShouldStopQuery<K, T>(K targetKey, ModKey? modKey, int modCount, DepthCache<K, T> cache)
            where K : notnull
        {
            if (cache.Depth >= modCount)
            {
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
        public bool TryResolve(FormKey formKey, Type type, [MaybeNullWhen(false)] out IMajorRecordCommonGetter majorRec)
        {
            return TryResolve(formKey, type, out majorRec, out _);
        }

        private bool TryResolve(FormKey formKey, Type type, [MaybeNullWhen(false)] out IMajorRecordCommonGetter majorRec, [MaybeNullWhen(false)] out int depth)
        {
            if (formKey.IsNull)
            {
                majorRec = default;
                depth = default;
                return false;
            }

            return TryResolve(
                key: formKey,
                modKey: formKey.ModKey,
                keyGetter: m => m.FormKey,
                type: type,
                winningRecs: _winningFormKeyRecords,
                linkInterfaces: _linkInterfaces,
                listedOrder: _listedOrder,
                majorRec: out majorRec,
                depth: out depth);
        }

        private static bool TryResolve<K>(
            K key,
            ModKey modKey,
            Type type,
            Func<IMajorRecordCommonGetter, K> keyGetter,
            Dictionary<Type, DepthCache<K, IMajorRecordCommonGetter>> winningRecs,
            IReadOnlyDictionary<Type, Type[]> linkInterfaces,
            IReadOnlyList<IModGetter> listedOrder,
            [MaybeNullWhen(false)] out IMajorRecordCommonGetter majorRec,
            [MaybeNullWhen(false)] out int depth)
            where K : notnull
        {
            DepthCache<K, IMajorRecordCommonGetter>? cache;
            lock (winningRecs)
            {
                // Get cache object by type 
                if (!winningRecs.TryGetValue(type, out cache))
                {
                    cache = new DepthCache<K, IMajorRecordCommonGetter>();
                    if (type.Equals(typeof(IMajorRecordCommon))
                        || type.Equals(typeof(IMajorRecordCommonGetter)))
                    {
                        winningRecs[typeof(IMajorRecordCommon)] = cache;
                        winningRecs[typeof(IMajorRecordCommonGetter)] = cache;
                    }
                    else if (LoquiRegistration.TryGetRegister(type, out var registration))
                    {
                        winningRecs[registration.ClassType] = cache;
                        winningRecs[registration.GetterType] = cache;
                        winningRecs[registration.SetterType] = cache;
                        if (registration.InternalGetterType != null)
                        {
                            winningRecs[registration.InternalGetterType] = cache;
                        }
                        if (registration.InternalSetterType != null)
                        {
                            winningRecs[registration.InternalSetterType] = cache;
                        }
                    }
                    else
                    {
                        if (!linkInterfaces.TryGetValue(type, out var objs))
                        {
                            throw new ArgumentException($"A lookup was queried for an unregistered type: {type.Name}");
                        }
                        winningRecs[type] = cache;
                    }
                }
            }

            lock (cache)
            {
                // Check for record 
                if (cache.TryGetValue(key, out majorRec))
                {
                    depth = cache.Depth;
                    return true;
                }
                if (ShouldStopQuery(key, modKey, listedOrder.Count, cache))
                {
                    depth = default;
                    majorRec = default!;
                    return false;
                }

                while (!ShouldStopQuery(key, modKey, listedOrder.Count, cache))
                {
                    // Get next unprocessed mod 
                    var targetIndex = listedOrder.Count - cache.Depth - 1;
                    var targetMod = listedOrder[targetIndex];
                    cache.Depth++;
                    cache.PassedMods.Add(targetMod.ModKey);

                    void AddRecords(IModGetter mod, Type type)
                    {
                        foreach (var record in mod.EnumerateMajorRecords(type))
                        {
                            cache.AddIfMissing(keyGetter(record), record);
                        }
                    }

                    // Add records from that mod that aren't already cached 
                    if (linkInterfaces.TryGetValue(type, out var objs))
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
                        depth = cache.Depth;
                        return true;
                    }
                }
                // Record doesn't exist 
                majorRec = default;
                depth = default;
                return false;
            }
        }

        /// <inheritdoc />
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public IMajorRecordCommonGetter Resolve(FormKey formKey)
        {
            if (TryResolve<IMajorRecordCommonGetter>(formKey, out var commonRec)) return commonRec;
            throw new KeyNotFoundException($"FormKey {formKey} could not be found.");
        }

        /// <inheritdoc />
        public IMajorRecordCommonGetter Resolve(FormKey formKey, Type type)
        {
            if (TryResolve(formKey, type, out var commonRec)) return commonRec;
            throw new KeyNotFoundException($"FormKey {formKey} could not be found.");
        }

        /// <inheritdoc />
        public TMajor Resolve<TMajor>(FormKey formKey)
            where TMajor : class, IMajorRecordCommonGetter
        {
            if (TryResolve<TMajor>(formKey, out var commonRec)) return commonRec;
            throw new KeyNotFoundException($"FormKey {formKey} could not be found.");
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
            // Break early if no content
            if (!_hasAny || formKey.IsNull)
            {
                return Enumerable.Empty<IMajorRecordCommonGetter>();
            }

            return ResolveAll(
                key: formKey,
                modKey: formKey.ModKey,
                type: type,
                keyGetter: m => m.FormKey,
                allRecs: _allFormKeyRecords,
                linkInterfaces: _linkInterfaces,
                listedOrder: _listedOrder);
        }

        private static IEnumerable<IMajorRecordCommonGetter> ResolveAll<K>(
            K key,
            ModKey modKey,
            Type type,
            Func<IMajorRecordCommonGetter, K> keyGetter,
            Dictionary<Type, DepthCache<K, ImmutableList<IMajorRecordCommonGetter>>> allRecs,
            IReadOnlyDictionary<Type, Type[]> linkInterfaces,
            IReadOnlyList<IModGetter> listedOrder)
            where K : notnull
        {
            // Grab the type cache
            DepthCache<K, ImmutableList<IMajorRecordCommonGetter>> cache;
            lock (allRecs)
            {
                cache = allRecs.GetOrAdd(type);
            }

            // Grab the formkey's list
            ImmutableList<IMajorRecordCommonGetter>? list;
            int consideredDepth;
            lock (cache)
            {
                if (!cache.TryGetValue(key, out list))
                {
                    list = ImmutableList<IMajorRecordCommonGetter>.Empty;
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
            bool more = !ShouldStopQuery(key, modKey, listedOrder.Count, cache);

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
                        var targetIndex = listedOrder.Count - cache.Depth - 1;
                        var targetMod = listedOrder[targetIndex];
                        cache.Depth++;
                        cache.PassedMods.Add(targetMod.ModKey);

                        void AddRecords(IModGetter mod, Type type)
                        {
                            foreach (var item in mod.EnumerateMajorRecords(type))
                            {
                                var iterKey = keyGetter(item);
                                if (!cache.TryGetValue(iterKey, out var targetList))
                                {
                                    targetList = ImmutableList<IMajorRecordCommonGetter>.Empty;
                                }
                                cache.Set(iterKey, targetList.Add(item));
                            }
                            if (cache.TryGetValue(key, out var requeriedList))
                            {
                                list = requeriedList;
                            }
                        }

                        // Add records from that mod that aren't already cached
                        if (linkInterfaces.TryGetValue(type, out var objs))
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
                    more = !ShouldStopQuery(key, modKey, listedOrder.Count, cache);
                }

                // Return any new data
                for (int i = iteratedCount; i < list.Count; i++)
                {
                    yield return list[i];
                }
                iteratedCount = list.Count;
            }
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
        public IMajorRecordCommonGetter Resolve(FormKey formKey, params Type[] types)
        {
            return Resolve(formKey, (IEnumerable<Type>)types);
        }

        /// <inheritdoc />
        public IMajorRecordCommonGetter Resolve(FormKey formKey, IEnumerable<Type> types)
        {
            if (TryResolve(formKey, types, out var commonRec)) return commonRec;
            throw new KeyNotFoundException($"FormKey {formKey} could not be found.");
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
        where TMod : class, IContextMod<TMod>, TModGetter
        where TModGetter : class, IContextGetterMod<TMod>
    {
        private readonly Dictionary<Type, DepthCache<FormKey, IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter>>> _winningFormKeyContexts 
            = new Dictionary<Type, DepthCache<FormKey, IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter>>>();
        private readonly Dictionary<Type, DepthCache<FormKey, ImmutableList<IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter>>>> _allFormKeyContexts
            = new Dictionary<Type, DepthCache<FormKey, ImmutableList<IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter>>>>();

        private readonly IReadOnlyList<TModGetter> _listedOrder;

        /// <summary>
        /// Constructs a LoadOrderLinkCache around a target load order
        /// </summary>
        /// <param name="loadOrder">LoadOrder to resolve against when linking</param>
        public ImmutableLoadOrderLinkCache(IEnumerable<TModGetter> loadOrder)
            : base(loadOrder)
        {
            this._listedOrder = loadOrder.ToList();
        }

        /// <inheritdoc />
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public bool TryResolveContext(FormKey formKey, [MaybeNullWhen(false)] out IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter> majorRec)
        {
            return TryResolveContext<IMajorRecordCommon, IMajorRecordCommonGetter>(formKey, out majorRec);
        }

        /// <inheritdoc />
        public bool TryResolveContext<TMajorSetter, TMajorGetter>(FormKey formKey, [MaybeNullWhen(false)] out IModContext<TMod, TMajorSetter, TMajorGetter> majorRec)
            where TMajorSetter : class, IMajorRecordCommon, TMajorGetter
            where TMajorGetter : class, IMajorRecordCommonGetter
        {
            if (!TryResolveContext(formKey, typeof(TMajorGetter), out var commonRec)
                || !(commonRec.Record is TMajorGetter))
            {
                majorRec = default;
                return false;
            }

            majorRec = commonRec.AsType<TMod, IMajorRecordCommon, IMajorRecordCommonGetter, TMajorSetter, TMajorGetter>();
            return majorRec != null;
        }

        /// <inheritdoc />
        public bool TryResolveContext(FormKey formKey, Type type, [MaybeNullWhen(false)] out IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter> majorRec)
        {
            if (!_hasAny || formKey.IsNull)
            {
                majorRec = default;
                return false;
            }

            return TryResolveContext(
                key: formKey,
                modKey: formKey.ModKey,
                type: type,
                keyGetter: m => m.FormKey,
                winningContexts: _winningFormKeyContexts,
                linkInterfaces: _linkInterfaces,
                listedOrder: _listedOrder,
                linkCache: this,
                majorRec: out majorRec);
        }

        private static bool TryResolveContext<K>(
            K key,
            ModKey modKey,
            Type type,
            Func<IMajorRecordCommonGetter, K> keyGetter,
            Dictionary<Type, DepthCache<K, IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter>>> winningContexts,
            IReadOnlyDictionary<Type, Type[]> linkInterfaces,
            IReadOnlyList<TModGetter> listedOrder,
            ILinkCache linkCache,
            [MaybeNullWhen(false)] out IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter> majorRec)
            where K : notnull
        {
            DepthCache<K, IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter>>? cache;
            lock (winningContexts)
            {
                // Get cache object by type
                if (!winningContexts.TryGetValue(type, out cache))
                {
                    cache = new DepthCache<K, IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter>>();
                    if (type.Equals(typeof(IMajorRecordCommon))
                        || type.Equals(typeof(IMajorRecordCommonGetter)))
                    {
                        winningContexts[typeof(IMajorRecordCommon)] = cache;
                        winningContexts[typeof(IMajorRecordCommonGetter)] = cache;
                    }
                    else if (LoquiRegistration.TryGetRegister(type, out var registration))
                    {
                        winningContexts[registration.ClassType] = cache;
                        winningContexts[registration.GetterType] = cache;
                        winningContexts[registration.SetterType] = cache;
                        if (registration.InternalGetterType != null)
                        {
                            winningContexts[registration.InternalGetterType] = cache;
                        }
                        if (registration.InternalSetterType != null)
                        {
                            winningContexts[registration.InternalSetterType] = cache;
                        }
                    }
                    else
                    {
                        if (!linkInterfaces.TryGetValue(type, out var objs))
                        {
                            throw new ArgumentException($"A lookup was queried for an unregistered type: {type.Name}");
                        }
                        winningContexts[type] = cache;
                    }
                }
            }

            lock (cache)
            {
                // Check for record
                if (cache.TryGetValue(key, out majorRec))
                {
                    return true;
                }
                if (ShouldStopQuery(key, modKey, listedOrder.Count, cache))
                {
                    majorRec = default!;
                    return false;
                }

                while (!ShouldStopQuery(key, modKey, listedOrder.Count, cache))
                {
                    // Get next unprocessed mod
                    var targetIndex = listedOrder.Count - cache.Depth - 1;
                    var targetMod = listedOrder[targetIndex];
                    cache.Depth++;
                    cache.PassedMods.Add(targetMod.ModKey);

                    void AddRecords(TModGetter mod, Type type)
                    {
                        foreach (var record in mod.EnumerateMajorRecordContexts(linkCache, type))
                        {
                            cache.AddIfMissing(keyGetter(record.Record), record);
                        }
                    }

                    // Add records from that mod that aren't already cached
                    if (linkInterfaces.TryGetValue(type, out var objs))
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

        /// <inheritdoc />
        public IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter> ResolveContext(FormKey formKey)
        {
            if (TryResolveContext<IMajorRecordCommon, IMajorRecordCommonGetter>(formKey, out var commonRec)) return commonRec;
            throw new KeyNotFoundException($"FormKey {formKey} could not be found.");
        }

        /// <inheritdoc />
        public IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter> ResolveContext(FormKey formKey, Type type)
        {
            if (TryResolveContext(formKey, type, out var commonRec)) return commonRec;
            throw new KeyNotFoundException($"FormKey {formKey} could not be found.");
        }

        /// <inheritdoc />
        public IModContext<TMod, TMajorSetter, TMajorGetter> ResolveContext<TMajorSetter, TMajorGetter>(FormKey formKey)
            where TMajorSetter : class, IMajorRecordCommon, TMajorGetter
            where TMajorGetter : class, IMajorRecordCommonGetter
        {
            if (TryResolveContext<TMajorSetter, TMajorGetter>(formKey, out var commonRec)) return commonRec;
            throw new KeyNotFoundException($"FormKey {formKey} could not be found.");
        }

        /// <inheritdoc />
        public IEnumerable<IModContext<TMod, TMajorSetter, TMajorGetter>> ResolveAllContexts<TMajorSetter, TMajorGetter>(FormKey formKey)
            where TMajorSetter : class, IMajorRecordCommon, TMajorGetter
            where TMajorGetter : class, IMajorRecordCommonGetter
        {
            return ResolveAllContexts(formKey, typeof(TMajorGetter))
                .Select(c => c.AsType<TMod, IMajorRecordCommon, IMajorRecordCommonGetter, TMajorSetter, TMajorGetter>());
        }

        /// <inheritdoc />
        public IEnumerable<IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter>> ResolveAllContexts(FormKey formKey, Type type)
        {
            // Break early if no content
            if (!_hasAny || formKey.IsNull)
            {
                return Enumerable.Empty<IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter>>();
            }

            return ResolveAllContexts(
                key: formKey,
                modKey: formKey.ModKey,
                type: type,
                keyGetter: m => m.FormKey,
                allContexts: _allFormKeyContexts,
                linkInterfaces: _linkInterfaces,
                listedOrder: _listedOrder,
                linkCache: this);
        }

        private static IEnumerable<IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter>> ResolveAllContexts<K>(
            K key, 
            ModKey modKey,
            Type type,
            Func<IMajorRecordCommonGetter, K> keyGetter,
            Dictionary<Type, DepthCache<K, ImmutableList<IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter>>>> allContexts,
            IReadOnlyDictionary<Type, Type[]> linkInterfaces,
            IReadOnlyList<TModGetter> listedOrder,
            ILinkCache linkCache)
            where K : notnull
        {
            // Grab the type cache
            DepthCache<K, ImmutableList<IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter>>> cache;
            lock (allContexts)
            {
                cache = allContexts.GetOrAdd(type);
            }

            // Grab the formkey's list
            ImmutableList<IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter>>? list;
            int consideredDepth;
            lock (cache)
            {
                if (!cache.TryGetValue(key, out list))
                {
                    list = ImmutableList<IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter>>.Empty;
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
            bool more = !ShouldStopQuery(key, modKey, listedOrder.Count, cache);

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
                        var targetIndex = listedOrder.Count - cache.Depth - 1;
                        var targetMod = listedOrder[targetIndex];
                        cache.Depth++;
                        cache.PassedMods.Add(targetMod.ModKey);

                        void AddRecords(TModGetter mod, Type type)
                        {
                            foreach (var item in mod.EnumerateMajorRecordContexts(linkCache, type))
                            {
                                var iterKey = keyGetter(item.Record);
                                if (!cache.TryGetValue(iterKey, out var targetList))
                                {
                                    targetList = ImmutableList<IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter>>.Empty;
                                }
                                cache.Set(iterKey, targetList.Add(item));
                            }
                            if (cache.TryGetValue(key, out var requeriedList))
                            {
                                list = requeriedList;
                            }
                        }

                        // Add records from that mod that aren't already cached
                        if (linkInterfaces.TryGetValue(type, out var objs))
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
                    more = !ShouldStopQuery(key, modKey, listedOrder.Count, cache);
                }

                // Return any new data
                for (int i = iteratedCount; i < list.Count; i++)
                {
                    yield return list[i];
                }
                iteratedCount = list.Count;
            }
        }

        /// <inheritdoc />
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public IEnumerable<IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter>> ResolveAllContexts(FormKey formKey)
        {
            return ResolveAllContexts(formKey, typeof(IMajorRecordCommonGetter));
        }
    }

    internal class DepthCache<K, T>
        where K : notnull
    {
        private readonly Dictionary<K, T> _dictionary = new Dictionary<K, T>();
        public HashSet<ModKey> PassedMods = new HashSet<ModKey>();
        public int Depth;

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
