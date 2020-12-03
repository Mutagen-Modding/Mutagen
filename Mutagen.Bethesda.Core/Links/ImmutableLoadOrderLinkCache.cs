using Loqui;
using Mutagen.Bethesda.Core;
using Noggog;
using System;
using System.Collections;
using System.Collections.Generic;
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
    /// <typeparam name="TMod">Mod setter type</typeparam>
    /// <typeparam name="TModGetter">Mod getter type</typeparam>
    public class ImmutableLoadOrderLinkCache<TMod, TModGetter> : ILinkCache<TMod, TModGetter>
        where TMod : class, IContextMod<TMod>, TModGetter
        where TModGetter : class, IContextGetterMod<TMod>
    {
        class InternalTypedCache
        {
            public Dictionary<FormKey, IMajorRecordCommonGetter> Dictionary { get; } = new Dictionary<FormKey, IMajorRecordCommonGetter>();
            public int Depth;
        }
        class InternalTypedContextCache
        {
            public Dictionary<FormKey, IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter>> Dictionary { get; } = new Dictionary<FormKey, IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter>>();
            public int Depth;
        }
        private readonly bool _hasAny;
        private readonly GameCategory _gameCategory;

        private readonly IReadOnlyList<TModGetter> _listedOrder;
        private readonly IReadOnlyList<TModGetter> _priorityOrder;
        private int _processedUntypedDepth = 0;
        private int _processedContextUntypedDepth = 0;
        private readonly Cache<IMajorRecordCommonGetter, FormKey> _loadOrderUntypedMajorRecords;
        private readonly Cache<IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter>, FormKey> _loadOrderUntypedContexts;
        private readonly Dictionary<Type, InternalTypedCache> _loadOrderMajorRecords;
        private readonly Dictionary<Type, InternalTypedContextCache> _loadOrderContexts;

        /// <inheritdoc />
        public IReadOnlyList<IModGetter> ListedOrder => _listedOrder;

        /// <inheritdoc />
        public IReadOnlyList<IModGetter> PriorityOrder => _priorityOrder;

        /// <summary>
        /// Constructs a LoadOrderLinkCache around a target load order
        /// </summary>
        /// <param name="loadOrder">LoadOrder to resolve against when linking</param>
        public ImmutableLoadOrderLinkCache(IEnumerable<TModGetter> loadOrder)
        {
            this._listedOrder = loadOrder.ToList();
            this._priorityOrder = _listedOrder.Reverse().ToList();
            this._loadOrderUntypedMajorRecords = new Cache<IMajorRecordCommonGetter, FormKey>(m => m.FormKey);
            this._loadOrderUntypedContexts = new Cache<IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter>, FormKey>(m => m.Record.FormKey);
            this._loadOrderMajorRecords = new Dictionary<Type, InternalTypedCache>();
            this._loadOrderContexts = new Dictionary<Type, InternalTypedContextCache>();
            var firstMod = _listedOrder.FirstOrDefault();
            this._hasAny = firstMod != null;
            // ToDo
            // Upgrade to bounce off ModInstantiator systems
            this._gameCategory = firstMod?.GameRelease.ToCategory() ?? GameCategory.Oblivion;
        }

        /// <inheritdoc />
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public bool TryResolve(FormKey formKey, [MaybeNullWhen(false)] out IMajorRecordCommonGetter majorRec)
        {
            if (!_hasAny)
            {
                majorRec = default;
                return false;
            }

            lock (this._loadOrderUntypedMajorRecords)
            {
                if (this._loadOrderUntypedMajorRecords.TryGetValue(formKey, out majorRec)) return true;
                if (this._processedUntypedDepth >= this._listedOrder.Count) return false;
                while (this._processedUntypedDepth < this._listedOrder.Count)
                {
                    // Get next unprocessed mod
                    var targetIndex = this._listedOrder.Count - _processedUntypedDepth - 1;
                    var targetMod = this._listedOrder[targetIndex];
                    this._processedUntypedDepth++;
                    // Add records from that mod that aren't already cached
                    foreach (var record in targetMod.EnumerateMajorRecords())
                    {
                        if (!_loadOrderUntypedMajorRecords.ContainsKey(record.FormKey))
                        {
                            _loadOrderUntypedMajorRecords.Set(record);
                        }
                    }
                    // Check again
                    if (this._loadOrderUntypedMajorRecords.TryGetValue(formKey, out majorRec)) return true;
                }
                // Record doesn't exist
                return false;
            }
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
            if (!_hasAny)
            {
                majorRec = default;
                return false;
            }

            lock (this._loadOrderMajorRecords)
            {
                // Get cache object by type
                if (!this._loadOrderMajorRecords.TryGetValue(type, out var cache))
                {
                    cache = new InternalTypedCache();
                    if (type.Equals(typeof(IMajorRecordCommon))
                        || type.Equals(typeof(IMajorRecordCommonGetter)))
                    {
                        this._loadOrderMajorRecords[typeof(IMajorRecordCommon)] = cache;
                        this._loadOrderMajorRecords[typeof(IMajorRecordCommonGetter)] = cache;
                    }
                    else if (LoquiRegistration.TryGetRegister(type, out var registration))
                    {
                        this._loadOrderMajorRecords[registration.ClassType] = cache;
                        this._loadOrderMajorRecords[registration.GetterType] = cache;
                        this._loadOrderMajorRecords[registration.SetterType] = cache;
                        if (registration.InternalGetterType != null)
                        {
                            this._loadOrderMajorRecords[registration.InternalGetterType] = cache;
                        }
                        if (registration.InternalSetterType != null)
                        {
                            this._loadOrderMajorRecords[registration.InternalSetterType] = cache;
                        }
                    }
                    else
                    {
                        var interfaceMappings = LinkInterfaceMapping.InterfaceToObjectTypes(_gameCategory);
                        if (!interfaceMappings.TryGetValue(type, out var objs))
                        {
                            throw new ArgumentException($"A lookup was queried for an unregistered type: {type.Name}");
                        }
                        this._loadOrderMajorRecords[type] = cache;
                    }
                }

                // Check for record
                if (cache.Dictionary.TryGetValue(formKey, out majorRec))
                {
                    return true;
                }
                if (cache.Depth >= this._listedOrder.Count)
                {
                    majorRec = default!;
                    return false;
                }

                while (cache.Depth < this._listedOrder.Count)
                {
                    // Get next unprocessed mod
                    var targetIndex = this._listedOrder.Count - cache.Depth - 1;
                    var targetMod = this._listedOrder[targetIndex];
                    cache.Depth++;

                    void AddRecords(TModGetter mod, Type type)
                    {
                        foreach (var record in mod.EnumerateMajorRecords(type))
                        {
                            if (!cache.Dictionary.ContainsKey(record.FormKey))
                            {
                                cache.Dictionary[record.FormKey] = record;
                            }
                        }
                    }

                    // Add records from that mod that aren't already cached
                    if (LinkInterfaceMapping.InterfaceToObjectTypes(_gameCategory).TryGetValue(type, out var objs))
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
                    if (cache.Dictionary.TryGetValue(formKey, out majorRec))
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
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public IMajorRecordCommonGetter Resolve(FormKey formKey)
        {
            if (TryResolve<IMajorRecordCommonGetter>(formKey, out var commonRec)) return commonRec;
            throw new KeyNotFoundException($"Form ID {formKey.ID} could not be found.");
        }

        /// <inheritdoc />
        public IMajorRecordCommonGetter Resolve(FormKey formKey, Type type)
        {
            if (TryResolve(formKey, type, out var commonRec)) return commonRec;
            throw new KeyNotFoundException($"Form ID {formKey.ID} could not be found.");
        }

        /// <inheritdoc />
        public TMajor Resolve<TMajor>(FormKey formKey)
            where TMajor : class, IMajorRecordCommonGetter
        {
            if (TryResolve<TMajor>(formKey, out var commonRec)) return commonRec;
            throw new KeyNotFoundException($"Form ID {formKey.ID} could not be found.");
        }

        /// <inheritdoc />
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public bool TryResolveContext(FormKey formKey, [MaybeNullWhen(false)] out IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter> majorRec)
        {
            if (!_hasAny)
            {
                majorRec = default;
                return false;
            }

            lock (this._loadOrderUntypedContexts)
            {
                if (this._loadOrderUntypedContexts.TryGetValue(formKey, out majorRec)) return true;
                if (this._processedContextUntypedDepth >= this._listedOrder.Count) return false;
                while (this._processedContextUntypedDepth < this._listedOrder.Count)
                {
                    // Get next unprocessed mod
                    var targetIndex = this._listedOrder.Count - _processedContextUntypedDepth - 1;
                    var targetMod = this._listedOrder[targetIndex];
                    this._processedContextUntypedDepth++;
                    // Add records from that mod that aren't already cached
                    foreach (var record in targetMod.EnumerateMajorRecordContexts<IMajorRecordCommon, IMajorRecordCommonGetter>(this))
                    {
                        if (!_loadOrderUntypedContexts.ContainsKey(record.Record.FormKey))
                        {
                            _loadOrderUntypedContexts.Set(record);
                        }
                    }
                    // Check again
                    if (this._loadOrderUntypedContexts.TryGetValue(formKey, out majorRec)) return true;
                }
                // Record doesn't exist
                return false;
            }
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
            if (!_hasAny)
            {
                majorRec = default;
                return false;
            }

            lock (this._loadOrderContexts)
            {
                // Get cache object by type
                if (!this._loadOrderContexts.TryGetValue(type, out var cache))
                {
                    cache = new InternalTypedContextCache();
                    if (type.Equals(typeof(IMajorRecordCommon))
                        || type.Equals(typeof(IMajorRecordCommonGetter)))
                    {
                        this._loadOrderContexts[typeof(IMajorRecordCommon)] = cache;
                        this._loadOrderContexts[typeof(IMajorRecordCommonGetter)] = cache;
                    }
                    else if (LoquiRegistration.TryGetRegister(type, out var registration))
                    {
                        this._loadOrderContexts[registration.ClassType] = cache;
                        this._loadOrderContexts[registration.GetterType] = cache;
                        this._loadOrderContexts[registration.SetterType] = cache;
                        if (registration.InternalGetterType != null)
                        {
                            this._loadOrderContexts[registration.InternalGetterType] = cache;
                        }
                        if (registration.InternalSetterType != null)
                        {
                            this._loadOrderContexts[registration.InternalSetterType] = cache;
                        }
                    }
                    else
                    {
                        var interfaceMappings = LinkInterfaceMapping.InterfaceToObjectTypes(_gameCategory);
                        if (!interfaceMappings.TryGetValue(type, out var objs))
                        {
                            throw new ArgumentException($"A lookup was queried for an unregistered type: {type.Name}");
                        }
                        this._loadOrderContexts[type] = cache;
                    }
                }

                // Check for record
                if (cache.Dictionary.TryGetValue(formKey, out majorRec))
                {
                    return true;
                }
                if (cache.Depth >= this._listedOrder.Count)
                {
                    majorRec = default!;
                    return false;
                }

                while (cache.Depth < this._listedOrder.Count)
                {
                    // Get next unprocessed mod
                    var targetIndex = this._listedOrder.Count - cache.Depth - 1;
                    var targetMod = this._listedOrder[targetIndex];
                    cache.Depth++;

                    void AddRecords(TModGetter mod, Type type)
                    {
                        foreach (var record in mod.EnumerateMajorRecordContexts(this, type))
                        {
                            if (!cache.Dictionary.ContainsKey(record.Record.FormKey))
                            {
                                cache.Dictionary[record.Record.FormKey] = record;
                            }
                        }
                    }

                    // Add records from that mod that aren't already cached
                    if (LinkInterfaceMapping.InterfaceToObjectTypes(_gameCategory).TryGetValue(type, out var objs))
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
                    if (cache.Dictionary.TryGetValue(formKey, out majorRec))
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
            throw new KeyNotFoundException($"Form ID {formKey.ID} could not be found.");
        }

        /// <inheritdoc />
        public IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter> ResolveContext(FormKey formKey, Type type)
        {
            if (TryResolveContext(formKey, type, out var commonRec)) return commonRec;
            throw new KeyNotFoundException($"Form ID {formKey.ID} could not be found.");
        }

        /// <inheritdoc />
        public IModContext<TMod, TMajorSetter, TMajorGetter> ResolveContext<TMajorSetter, TMajorGetter>(FormKey formKey)
            where TMajorSetter : class, IMajorRecordCommon, TMajorGetter
            where TMajorGetter : class, IMajorRecordCommonGetter
        {
            if (TryResolveContext<TMajorSetter, TMajorGetter>(formKey, out var commonRec)) return commonRec;
            throw new KeyNotFoundException($"Form ID {formKey.ID} could not be found.");
        }
    }
}
