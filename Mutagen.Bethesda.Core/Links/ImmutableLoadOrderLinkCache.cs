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
        private readonly Dictionary<Type, DepthCache<IMajorRecordCommonGetter>> _winningRecords = new Dictionary<Type, DepthCache<IMajorRecordCommonGetter>>();
        private readonly Dictionary<Type, DepthCache<ImmutableList<IMajorRecordCommonGetter>>> _allRecords = new Dictionary<Type, DepthCache<ImmutableList<IMajorRecordCommonGetter>>>();
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

        internal bool ShouldStopQuery<T>(FormKey targetKey, DepthCache<T> cache)
        {
            if (cache.Depth >= this._listedOrder.Count)
            {
                return true;
            }

            // If we're going deeper than the originating mod of the target FormKey, we can stop
            if (cache.PassedMods.Contains(targetKey.ModKey))
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
            if (!_hasAny || formKey.IsNull)
            {
                majorRec = default;
                depth = default;
                return false;
            }

            DepthCache<IMajorRecordCommonGetter>? cache;
            lock (this._winningRecords)
            {
                // Get cache object by type
                if (!this._winningRecords.TryGetValue(type, out cache))
                {
                    cache = new DepthCache<IMajorRecordCommonGetter>();
                    if (type.Equals(typeof(IMajorRecordCommon))
                        || type.Equals(typeof(IMajorRecordCommonGetter)))
                    {
                        this._winningRecords[typeof(IMajorRecordCommon)] = cache;
                        this._winningRecords[typeof(IMajorRecordCommonGetter)] = cache;
                    }
                    else if (LoquiRegistration.TryGetRegister(type, out var registration))
                    {
                        this._winningRecords[registration.ClassType] = cache;
                        this._winningRecords[registration.GetterType] = cache;
                        this._winningRecords[registration.SetterType] = cache;
                        if (registration.InternalGetterType != null)
                        {
                            this._winningRecords[registration.InternalGetterType] = cache;
                        }
                        if (registration.InternalSetterType != null)
                        {
                            this._winningRecords[registration.InternalSetterType] = cache;
                        }
                    }
                    else
                    {
                        if (!_linkInterfaces.TryGetValue(type, out var objs))
                        {
                            throw new ArgumentException($"A lookup was queried for an unregistered type: {type.Name}");
                        }
                        this._winningRecords[type] = cache;
                    }
                }
            }

            lock (cache)
            {
                // Check for record
                if (cache.TryGetValue(formKey, out majorRec))
                {
                    depth = cache.Depth;
                    return true;
                }
                if (ShouldStopQuery(formKey, cache))
                {
                    depth = default;
                    majorRec = default!;
                    return false;
                }

                while (!ShouldStopQuery(formKey, cache))
                {
                    // Get next unprocessed mod
                    var targetIndex = this._listedOrder.Count - cache.Depth - 1;
                    var targetMod = this._listedOrder[targetIndex];
                    cache.Depth++;
                    cache.PassedMods.Add(targetMod.ModKey);

                    void AddRecords(IModGetter mod, Type type)
                    {
                        foreach (var record in mod.EnumerateMajorRecords(type))
                        {
                            cache.AddIfMissing(record.FormKey, record);
                        }
                    }

                    // Add records from that mod that aren't already cached
                    if (_linkInterfaces.TryGetValue(type, out var objs))
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
                    if (cache.TryGetValue(formKey, out majorRec))
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
                yield break;
            }

            // Grab the type cache
            DepthCache<ImmutableList<IMajorRecordCommonGetter>> cache;
            lock (_allRecords)
            {
                cache = _allRecords.GetOrAdd(type);
            }

            // Grab the formkey's list
            ImmutableList<IMajorRecordCommonGetter>? list;
            int consideredDepth;
            lock (cache)
            {
                if (!cache.TryGetValue(formKey, out list))
                {
                    list = ImmutableList<IMajorRecordCommonGetter>.Empty;
                    cache.Add(formKey, list);
                }
                consideredDepth = cache.Depth;
            }

            // Return everyhing we have already
            foreach (var item in list)
            {
                yield return item;
            }

            int iteratedCount = list.Count;
            bool more = !ShouldStopQuery(formKey, cache);

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
                        var targetIndex = this._listedOrder.Count - cache.Depth - 1;
                        var targetMod = this._listedOrder[targetIndex];
                        cache.Depth++;
                        cache.PassedMods.Add(targetMod.ModKey);

                        void AddRecords(IModGetter mod, Type type)
                        {
                            foreach (var item in mod.EnumerateMajorRecords(type))
                            {
                                var iterFormKey = item.FormKey;
                                if (!cache.TryGetValue(iterFormKey, out var targetList))
                                {
                                    targetList = ImmutableList<IMajorRecordCommonGetter>.Empty;
                                }
                                cache.Set(iterFormKey, targetList.Add(item));
                            }
                            if (cache.TryGetValue(formKey, out var requeriedList))
                            {
                                list = requeriedList;
                            }
                        }

                        // Add records from that mod that aren't already cached
                        if (_linkInterfaces.TryGetValue(type, out var objs))
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
                    more = !ShouldStopQuery(formKey, cache);
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
        private readonly Dictionary<Type, DepthCache<IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter>>> _winningContexts = new Dictionary<Type, DepthCache<IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter>>>();
        private readonly Dictionary<Type, DepthCache<ImmutableList<IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter>>>> _allContexts = new Dictionary<Type, DepthCache<ImmutableList<IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter>>>>();

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

            DepthCache<IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter>>? cache;
            lock (this._winningContexts)
            {
                // Get cache object by type
                if (!this._winningContexts.TryGetValue(type, out cache))
                {
                    cache = new DepthCache<IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter>>();
                    if (type.Equals(typeof(IMajorRecordCommon))
                        || type.Equals(typeof(IMajorRecordCommonGetter)))
                    {
                        this._winningContexts[typeof(IMajorRecordCommon)] = cache;
                        this._winningContexts[typeof(IMajorRecordCommonGetter)] = cache;
                    }
                    else if (LoquiRegistration.TryGetRegister(type, out var registration))
                    {
                        this._winningContexts[registration.ClassType] = cache;
                        this._winningContexts[registration.GetterType] = cache;
                        this._winningContexts[registration.SetterType] = cache;
                        if (registration.InternalGetterType != null)
                        {
                            this._winningContexts[registration.InternalGetterType] = cache;
                        }
                        if (registration.InternalSetterType != null)
                        {
                            this._winningContexts[registration.InternalSetterType] = cache;
                        }
                    }
                    else
                    {
                        if (!_linkInterfaces.TryGetValue(type, out var objs))
                        {
                            throw new ArgumentException($"A lookup was queried for an unregistered type: {type.Name}");
                        }
                        this._winningContexts[type] = cache;
                    }
                }
            }

            lock (cache)
            {
                // Check for record
                if (cache.TryGetValue(formKey, out majorRec))
                {
                    return true;
                }
                if (ShouldStopQuery(formKey, cache))
                {
                    majorRec = default!;
                    return false;
                }

                while (!ShouldStopQuery(formKey, cache))
                {
                    // Get next unprocessed mod
                    var targetIndex = this._listedOrder.Count - cache.Depth - 1;
                    var targetMod = this._listedOrder[targetIndex];
                    cache.Depth++;
                    cache.PassedMods.Add(targetMod.ModKey);

                    void AddRecords(TModGetter mod, Type type)
                    {
                        foreach (var record in mod.EnumerateMajorRecordContexts(this, type))
                        {
                            cache.AddIfMissing(record.Record.FormKey, record);
                        }
                    }

                    // Add records from that mod that aren't already cached
                    if (_linkInterfaces.TryGetValue(type, out var objs))
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
                    if (cache.TryGetValue(formKey, out majorRec))
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
                yield break;
            }

            // Grab the type cache
            DepthCache<ImmutableList<IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter>>> cache;
            lock (_allContexts)
            {
                cache = _allContexts.GetOrAdd(type);
            }

            // Grab the formkey's list
            ImmutableList<IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter>>? list;
            int consideredDepth;
            lock (cache)
            {
                if (!cache.TryGetValue(formKey, out list))
                {
                    list = ImmutableList<IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter>>.Empty;
                    cache.Add(formKey, list);
                }
                consideredDepth = cache.Depth;
            }

            // Return everyhing we have already
            foreach (var item in list)
            {
                yield return item;
            }

            int iteratedCount = list.Count;
            bool more = !ShouldStopQuery(formKey, cache);

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
                        var targetIndex = this._listedOrder.Count - cache.Depth - 1;
                        var targetMod = this._listedOrder[targetIndex];
                        cache.Depth++;
                        cache.PassedMods.Add(targetMod.ModKey);

                        void AddRecords(TModGetter mod, Type type)
                        {
                            foreach (var item in mod.EnumerateMajorRecordContexts(this, type))
                            {
                                var iterFormKey = item.Record.FormKey;
                                if (!cache.TryGetValue(iterFormKey, out var targetList))
                                {
                                    targetList = ImmutableList<IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter>>.Empty;
                                }
                                cache.Set(iterFormKey, targetList.Add(item));
                            }
                            if (cache.TryGetValue(formKey, out var requeriedList))
                            {
                                list = requeriedList;
                            }
                        }

                        // Add records from that mod that aren't already cached
                        if (_linkInterfaces.TryGetValue(type, out var objs))
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
                    more = !ShouldStopQuery(formKey, cache);
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

    internal class DepthCache<T>
    {
        private readonly Dictionary<FormKey, T> _dictionary = new Dictionary<FormKey, T>();
        public HashSet<ModKey> PassedMods = new HashSet<ModKey>();
        public int Depth;

        public bool TryGetValue(FormKey key, [MaybeNullWhen(false)] out T value)
        {
            return _dictionary.TryGetValue(key, out value);
        }

        public void AddIfMissing(FormKey key, T item)
        {
            if (!_dictionary.ContainsKey(key))
            {
                _dictionary[key] = item;
            }
        }

        public void Add(FormKey key, T item)
        {
            _dictionary.Add(key, item);
        }

        public void Set(FormKey key, T item)
        {
            _dictionary[key] = item;
        }
    }
}
