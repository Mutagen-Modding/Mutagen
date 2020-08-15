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
    /// <typeparam name="TMod">Mod type</typeparam>
    public class ImmutableLoadOrderLinkCache<TMod> : ILinkCache
        where TMod : class, IModGetter
    {
        class InternalTypedCache
        {
            public Dictionary<FormKey, object> Dictionary { get; } = new Dictionary<FormKey, object>();
            public int Depth;
        }
        private readonly bool _hasAny;
        private readonly GameCategory _gameCategory;

        private readonly IReadOnlyList<TMod> _listedOrder;
        private readonly IReadOnlyList<TMod> _priorityOrder;
        private int _processedUntypedDepth = 0;
        private readonly Cache<IMajorRecordCommonGetter, FormKey> _loadOrderUntypedMajorRecords;
        private readonly Dictionary<Type, InternalTypedCache> _loadOrderMajorRecords;

        /// <inheritdoc />
        public IReadOnlyList<IModGetter> ListedOrder => _listedOrder;

        /// <inheritdoc />
        public IReadOnlyList<IModGetter> PriorityOrder => _priorityOrder;

        /// <summary>
        /// Constructs a LoadOrderLinkCache around a target load order
        /// </summary>
        /// <param name="loadOrder">LoadOrder to resolve against when linking</param>
        public ImmutableLoadOrderLinkCache(IEnumerable<TMod> loadOrder)
        {
            this._listedOrder = loadOrder.ToList();
            this._priorityOrder = _listedOrder.Reverse().ToList();
            this._loadOrderUntypedMajorRecords = new Cache<IMajorRecordCommonGetter, FormKey>(m => m.FormKey);
            this._loadOrderMajorRecords = new Dictionary<Type, InternalTypedCache>();
            var firstMod = _listedOrder.FirstOrDefault();
            this._hasAny = firstMod != null;
            // ToDo
            // Upgrade to bounce off ModInstantiator systems
            this._gameCategory = firstMod?.GameRelease.ToCategory() ?? GameCategory.Oblivion;
        }

        /// <inheritdoc />
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public bool TryLookup(FormKey formKey, [MaybeNullWhen(false)] out IMajorRecordCommonGetter majorRec)
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
        public bool TryLookup<TMajor>(FormKey formKey, [MaybeNullWhen(false)] out TMajor majorRec)
            where TMajor : class, IMajorRecordCommonGetter
        {
            if (!_hasAny)
            {
                majorRec = default;
                return false;
            }

            lock (this._loadOrderMajorRecords)
            {
                // Get cache object by type
                if (!this._loadOrderMajorRecords.TryGetValue(typeof(TMajor), out InternalTypedCache cache))
                {
                    cache = new InternalTypedCache();
                    if (typeof(TMajor).Equals(typeof(IMajorRecordCommon))
                        || typeof(TMajor).Equals(typeof(IMajorRecordCommonGetter)))
                    {
                        this._loadOrderMajorRecords[typeof(IMajorRecordCommon)] = cache;
                        this._loadOrderMajorRecords[typeof(IMajorRecordCommonGetter)] = cache;
                    }
                    else if (LoquiRegistration.TryGetRegister(typeof(TMajor), out var registration))
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
                        if (!interfaceMappings.TryGetValue(typeof(TMajor), out var objs))
                        {
                            throw new ArgumentException($"A lookup was queried for an unregistered type: {typeof(TMajor).Name}");
                        }
                        this._loadOrderMajorRecords[typeof(TMajor)] = cache;
                    }
                }

                // Check for record
                if (cache.Dictionary.TryGetValue(formKey, out var majorRecObj))
                {
                    majorRec = (majorRecObj as TMajor)!;
                    return majorRec != null;
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
                    
                    void AddRecords(TMod mod, Type type)
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
                    if (LinkInterfaceMapping.InterfaceToObjectTypes(_gameCategory).TryGetValue(typeof(TMajor), out var objs))
                    {
                        foreach (var objType in objs)
                        {
                            AddRecords(targetMod, LoquiRegistration.GetRegister(objType).GetterType);
                        }
                    }
                    else
                    {
                        AddRecords(targetMod, typeof(IMajorRecordGetter));
                    }
                    // Check again
                    if (cache.Dictionary.TryGetValue(formKey, out majorRecObj))
                    {
                        majorRec = (majorRecObj as TMajor)!;
                        return majorRec != null;
                    }
                }
                // Record doesn't exist
                majorRec = default!;
                return false;
            }
        }
    }
}
