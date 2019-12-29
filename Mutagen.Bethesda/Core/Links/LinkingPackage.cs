using DynamicData;
using Loqui;
using Noggog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda
{
    public interface ILinkingPackage<TMod>
        where TMod : IModGetter
    {
        TMod SourceMod { get; }
        ModList<TMod> ModList { get; }
        bool TryGetMajorRecord(FormKey formKey, out IMajorRecordCommonGetter majorRec);
        bool TryGetMajorRecord<TMajor>(FormKey formKey, out TMajor majorRec)
            where TMajor : class, IMajorRecordCommonGetter;
    }

    public class LinkingPackage<TMod> : ILinkingPackage<TMod>
        where TMod : IModGetter
    {
        public TMod SourceMod { get; private set; }
        private IReadOnlyCache<IMajorRecordCommonGetter, FormKey> _sourceModUntypedMajorRecords;
        private Dictionary<Type, object> _sourceModMajorRecords = new Dictionary<Type, object>();
        private readonly IReadOnlyCache<IMajorRecordCommonGetter, FormKey>[] _modListUntypedMajorRecords;
        private readonly Dictionary<Type, object>[] _modListMajorRecords;
        public ModList<TMod> ModList { get; }

        public LinkingPackage(TMod sourceMod, ModList<TMod> modList)
        {
            this.SourceMod = sourceMod;
            this.ModList = modList;
            if (this.ModList != null)
            {
                this._modListUntypedMajorRecords = new IReadOnlyCache<IMajorRecordCommonGetter, FormKey>[this.ModList.Count];
                this._modListMajorRecords = new Dictionary<Type, object>[this.ModList.Count];
            }
        }

        public LinkingPackage(ModList<TMod> modList)
            : this(default, modList)
        {
        }

        public bool TryGetMajorRecord(FormKey formKey, out IMajorRecordCommonGetter majorRec)
        {
            if (!TryGetMajorRecords(formKey.ModKey, out var cache))
            {
                majorRec = default;
                return false;
            }
            return cache.TryGetValue(formKey, out majorRec);
        }

        public bool TryGetMajorRecord<TMajor>(FormKey formKey, out TMajor majorRec)
            where TMajor : class, IMajorRecordCommonGetter
        {
            if (!TryGetMajorRecords<TMajor>(formKey.ModKey, out var cache))
            {
                majorRec = default;
                return false;
            }
            return cache.TryGetValue(formKey, out majorRec);
        }

        public void SetSourceMod(TMod sourceMod)
        {
            this.SourceMod = sourceMod;
            this._sourceModUntypedMajorRecords = null;
        }

        private bool TryGetMajorRecords(ModKey modKey, out IReadOnlyCache<IMajorRecordCommonGetter, FormKey> dict)
        {
            if (this.ModList != null && this.ModList.TryGetMod(modKey, out var mod))
            {
                dict = GetCacheFromModID(mod.Index);
                return true;
            }
            else if (modKey == SourceMod.ModKey)
            {
                if (_sourceModUntypedMajorRecords == null)
                {
                    _sourceModUntypedMajorRecords = GetCacheFromMod(SourceMod);
                }
                dict = _sourceModUntypedMajorRecords;
                return true;
            }
            dict = default;
            return false;
        }

        private bool TryGetMajorRecords<TMajor>(ModKey modKey, out IReadOnlyCache<TMajor, FormKey> cache)
            where TMajor : class, IMajorRecordCommonGetter
        {
            if (this.ModList != null && this.ModList.TryGetMod(modKey, out var mod))
            {
                cache = GetCacheFromModID<TMajor>(mod.Index);
                return true;
            }
            else if (modKey == SourceMod.ModKey)
            {
                if (!_sourceModMajorRecords.TryGetValue(typeof(TMajor), out object cacheObj))
                {
                    cache = GetCacheFromMod<TMajor>(SourceMod);
                    var registration = LoquiRegistration.GetRegister(typeof(TMajor));
                    _sourceModMajorRecords[registration.ClassType] = cache;
                    _sourceModMajorRecords[registration.GetterType] = cache;
                    _sourceModMajorRecords[registration.SetterType] = cache;
                    if (registration.InternalGetterType != null)
                    {
                        _sourceModMajorRecords[registration.InternalGetterType] = cache;
                    }
                    if (registration.InternalSetterType != null)
                    {
                        _sourceModMajorRecords[registration.InternalSetterType] = cache;
                    }
                    cacheObj = cache;
                }
                cache = cacheObj as IReadOnlyCache<TMajor, FormKey>;
                return true;
            }
            cache = default;
            return false;
        }

        private IReadOnlyCache<IMajorRecordCommonGetter, FormKey> GetCacheFromModID(ModID modIndex)
        {
            if (!_modListUntypedMajorRecords.InRange(modIndex.ID))
            {
                throw new ArgumentException();
            }
            if (!_modListUntypedMajorRecords.TryGet(modIndex.ID, out var cache))
            {
                cache = GetCacheFromMod(this.ModList.GetIndex(modIndex).Mod);
                _modListUntypedMajorRecords[modIndex.ID] = cache;
            }
            return cache;
        }

        private IReadOnlyCache<TMajor, FormKey> GetCacheFromModID<TMajor>(ModID modIndex)
            where TMajor : class, IMajorRecordCommonGetter
        {
            if (!_modListMajorRecords.InRange(modIndex.ID))
            {
                throw new ArgumentException();
            }
            if (!_modListMajorRecords.TryGet(modIndex.ID, out var typeDict))
            {
                typeDict = new Dictionary<Type, object>();
                _modListMajorRecords[modIndex.ID] = typeDict;
            }
            if (typeDict.TryGetValue(typeof(TMajor), out object cacheObj))
            {
                var cache = GetCacheFromMod<TMajor>(SourceMod);
                var registration = LoquiRegistration.GetRegister(typeof(TMajor));
                typeDict[registration.ClassType] = cache;
                typeDict[registration.GetterType] = cache;
                typeDict[registration.SetterType] = cache;
                if (registration.InternalGetterType != null)
                {
                    typeDict[registration.InternalGetterType] = cache;
                }
                if (registration.InternalSetterType != null)
                {
                    typeDict[registration.InternalSetterType] = cache;
                }
                cacheObj = cache;
            }
            return cacheObj as IReadOnlyCache<TMajor, FormKey>;
        }

        private IReadOnlyCache<IMajorRecordCommonGetter, FormKey> GetCacheFromMod(TMod mod)
        {
            var majorRecords = new Cache<IMajorRecordCommonGetter, FormKey>(x => x.FormKey);
            foreach (var majorRec in mod.EnumerateMajorRecords())
            {
                majorRecords.Set(majorRec);
            }
            return majorRecords;
        }

        private IReadOnlyCache<TMajor, FormKey> GetCacheFromMod<TMajor>(TMod mod)
            where TMajor : class, IMajorRecordCommonGetter
        {
            var cache = new Cache<TMajor, FormKey>(x => x.FormKey);
            foreach (var majorRec in mod.EnumerateMajorRecords<TMajor>())
            {
                cache.Set(majorRec);
            }
            return cache;
        }
    }
}
