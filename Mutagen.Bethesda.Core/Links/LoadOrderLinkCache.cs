using Loqui;
using Noggog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Mutagen.Bethesda
{
    public class LoadOrderLinkCache<TMod> : ILinkCache<TMod>
        where TMod : IModGetter
    {
        class InternalTypedCache
        {
            public Dictionary<FormKey, object> Dictionary { get; } = new Dictionary<FormKey, object>();
            public int Depth;
        }

        private readonly LoadOrder<TMod> _loadOrder;

        private int _processedUntypedDepth = 0;
        private readonly Cache<IMajorRecordCommonGetter, FormKey> _loadOrderUntypedMajorRecords;
        private readonly Dictionary<Type, InternalTypedCache> _loadOrderMajorRecords;

        public LoadOrderLinkCache(LoadOrder<TMod> loadOrder)
        {
            this._loadOrder = loadOrder;
            this._loadOrderUntypedMajorRecords = new Cache<IMajorRecordCommonGetter, FormKey>(m => m.FormKey);
            this._loadOrderMajorRecords = new Dictionary<Type, InternalTypedCache>();
        }

        public bool TryLookup(FormKey formKey, out IMajorRecordCommonGetter majorRec)
        {
            lock (this._loadOrderUntypedMajorRecords)
            {
                if (this._loadOrderUntypedMajorRecords.TryGetValue(formKey, out majorRec)) return true;
                if (this._processedUntypedDepth >= this._loadOrder.Count) return false;
                while (this._processedUntypedDepth < this._loadOrder.Count)
                {
                    // Get next unprocessed mod
                    var targetIndex = this._loadOrder.Count - _processedUntypedDepth - 1;
                    var targetMod = this._loadOrder[targetIndex];
                    this._processedUntypedDepth++;
                    if (!targetMod.Loaded) continue;
                    // Add records from that mod that aren't already cached
                    foreach (var record in targetMod.Mod.EnumerateMajorRecords())
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

        public bool TryLookup<TMajor>(FormKey formKey, [MaybeNullWhen(false)] out TMajor majorRec)
            where TMajor : class, IMajorRecordCommonGetter
        {
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
                    else
                    {
                        var registration = LoquiRegistration.GetRegister(typeof(TMajor));
                        if (registration == null) throw new ArgumentException();
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
                }

                // Check for record
                if (cache.Dictionary.TryGetValue(formKey, out var majorRecObj))
                {
                    majorRec = (majorRecObj as TMajor)!;
                    return majorRec != null;
                }
                if (cache.Depth >= this._loadOrder.Count)
                {
                    majorRec = default!;
                    return false;
                }

                while (cache.Depth < this._loadOrder.Count)
                {
                    // Get next unprocessed mod
                    var targetIndex = this._loadOrder.Count - cache.Depth - 1;
                    var targetMod = this._loadOrder[targetIndex];
                    cache.Depth++;
                    if (!targetMod.Loaded) continue;
                    // Add records from that mod that aren't already cached
                    foreach (var record in targetMod.Mod.EnumerateMajorRecords<TMajor>())
                    {
                        if (!cache.Dictionary.ContainsKey(record.FormKey))
                        {
                            cache.Dictionary[record.FormKey] = record;
                        }
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

        public IEnumerator<TMod> GetEnumerator()
        {
            foreach (var listing in this._loadOrder)
            {
                if (listing.Loaded) yield return listing.Mod;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
