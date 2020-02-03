using Loqui;
using Noggog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Mutagen.Bethesda
{
    public class ModListLinkCache<TMod> : ILinkCache<TMod>
        where TMod : IModGetter
    {
        class InternalTypedCache
        {
            public Dictionary<FormKey, object> Dictionary { get; } = new Dictionary<FormKey, object>();
            public int Depth;
        }

        private readonly ModList<TMod> _modList;

        private int _processedUntypedDepth = 0;
        private readonly Cache<IMajorRecordCommonGetter, FormKey> _modListUntypedMajorRecords;
        private readonly Dictionary<Type, InternalTypedCache> _modListMajorRecords;

        public ModListLinkCache(ModList<TMod> modList)
        {
            this._modList = modList;
            this._modListUntypedMajorRecords = new Cache<IMajorRecordCommonGetter, FormKey>(m => m.FormKey);
            this._modListMajorRecords = new Dictionary<Type, InternalTypedCache>();
        }

        public bool TryGetMajorRecord(FormKey formKey, out IMajorRecordCommonGetter majorRec)
        {
            lock (this._modListUntypedMajorRecords)
            {
                if (this._modListUntypedMajorRecords.TryGetValue(formKey, out majorRec)) return true;
                if (this._processedUntypedDepth >= this._modList.Count) return false;
                while (this._processedUntypedDepth < this._modList.Count)
                {
                    // Get next unprocessed mod
                    var targetIndex = this._modList.Count - _processedUntypedDepth - 1;
                    var targetMod = this._modList[targetIndex];
                    this._processedUntypedDepth++;
                    if (!targetMod.Loaded) continue;
                    // Add records from that mod that aren't already cached
                    foreach (var record in targetMod.Mod.EnumerateMajorRecords())
                    {
                        if (!_modListUntypedMajorRecords.ContainsKey(record.FormKey))
                        {
                            _modListUntypedMajorRecords.Set(record);
                        }
                    }
                    // Check again
                    if (this._modListUntypedMajorRecords.TryGetValue(formKey, out majorRec)) return true;
                }
                // Record doesn't exist
                return false;
            }
        }

        public bool TryGetMajorRecord<TMajor>(FormKey formKey, [MaybeNullWhen(false)] out TMajor majorRec)
            where TMajor : class, IMajorRecordCommonGetter
        {
            lock (this._modListMajorRecords)
            {
                // Get cache object by type
                if (!this._modListMajorRecords.TryGetValue(typeof(TMajor), out InternalTypedCache cache))
                {
                    cache = new InternalTypedCache();
                    if (typeof(TMajor).Equals(typeof(IMajorRecordCommon))
                        || typeof(TMajor).Equals(typeof(IMajorRecordCommonGetter)))
                    {
                        this._modListMajorRecords[typeof(IMajorRecordCommon)] = cache;
                        this._modListMajorRecords[typeof(IMajorRecordCommonGetter)] = cache;
                    }
                    else
                    {
                        var registration = LoquiRegistration.GetRegister(typeof(TMajor));
                        if (registration == null) throw new ArgumentException();
                        this._modListMajorRecords[registration.ClassType] = cache;
                        this._modListMajorRecords[registration.GetterType] = cache;
                        this._modListMajorRecords[registration.SetterType] = cache;
                        if (registration.InternalGetterType != null)
                        {
                            this._modListMajorRecords[registration.InternalGetterType] = cache;
                        }
                        if (registration.InternalSetterType != null)
                        {
                            this._modListMajorRecords[registration.InternalSetterType] = cache;
                        }
                    }
                }

                // Check for record
                if (cache.Dictionary.TryGetValue(formKey, out var majorRecObj))
                {
                    majorRec = (majorRecObj as TMajor)!;
                    return majorRec != null;
                }
                if (cache.Depth >= this._modList.Count)
                {
                    majorRec = default!;
                    return false;
                }

                while (cache.Depth < this._modList.Count)
                {
                    // Get next unprocessed mod
                    var targetIndex = this._modList.Count - cache.Depth - 1;
                    var targetMod = this._modList[targetIndex];
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
            foreach (var listing in this._modList)
            {
                if (listing.Loaded) yield return listing.Mod;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
