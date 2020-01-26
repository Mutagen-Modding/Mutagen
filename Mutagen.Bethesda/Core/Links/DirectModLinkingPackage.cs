using Loqui;
using Noggog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda
{
    public class DirectModLinkingPackage<TMod> : ILinkingPackage<TMod>
        where TMod : IModGetter
    {
        private readonly TMod _sourceMod;

        private readonly Lazy<IReadOnlyCache<IMajorRecordCommonGetter, FormKey>> _untypedMajorRecords;
        private readonly Dictionary<Type, IReadOnlyCache<object, FormKey>> _majorRecords = new Dictionary<Type, IReadOnlyCache<object, FormKey>>();

        public DirectModLinkingPackage(TMod sourceMod)
        {
            this._sourceMod = sourceMod;
            this._untypedMajorRecords = new Lazy<IReadOnlyCache<IMajorRecordCommonGetter, FormKey>>(
                isThreadSafe: true,
                valueFactory: () => GetCache());
        }

        public bool TryGetMajorRecord(FormKey formKey, out IMajorRecordCommonGetter majorRec)
        {
            return _untypedMajorRecords.Value.TryGetValue(formKey, out majorRec);
        }

        public bool TryGetMajorRecord<TMajor>(FormKey formKey, out TMajor majorRec)
            where TMajor : class, IMajorRecordCommonGetter
        {
            IReadOnlyCache<object, FormKey> cache;
            lock (_majorRecords)
            {
                if (!_majorRecords.TryGetValue(typeof(TMajor), out cache))
                {
                    cache = GetCache<TMajor>();
                    if (typeof(TMajor).Equals(typeof(IMajorRecordCommon))
                        || typeof(TMajor).Equals(typeof(IMajorRecordCommonGetter)))
                    {
                        _majorRecords[typeof(IMajorRecordCommon)] = cache;
                        _majorRecords[typeof(IMajorRecordCommonGetter)] = cache;
                    }
                    else
                    {
                        var registration = LoquiRegistration.GetRegister(typeof(TMajor));
                        _majorRecords[registration.ClassType] = cache;
                        _majorRecords[registration.GetterType] = cache;
                        _majorRecords[registration.SetterType] = cache;
                        if (registration.InternalGetterType != null)
                        {
                            _majorRecords[registration.InternalGetterType] = cache;
                        }
                        if (registration.InternalSetterType != null)
                        {
                            _majorRecords[registration.InternalSetterType] = cache;
                        }
                    }
                }
            }
            if (!cache.TryGetValue(formKey, out var majorRecObj))
            {
                majorRec = default;
                return false;
            }
            majorRec = majorRecObj as TMajor;
            return majorRec != null;
        }

        private IReadOnlyCache<IMajorRecordCommonGetter, FormKey> GetCache()
        {
            var majorRecords = new Cache<IMajorRecordCommonGetter, FormKey>(x => x.FormKey);
            foreach (var majorRec in this._sourceMod.EnumerateMajorRecords())
            {
                majorRecords.Set(majorRec);
            }
            return majorRecords;
        }

        private IReadOnlyCache<TMajor, FormKey> GetCache<TMajor>()
            where TMajor : class, IMajorRecordCommonGetter
        {
            var cache = new Cache<TMajor, FormKey>(x => x.FormKey);
            foreach (var majorRec in this._sourceMod.EnumerateMajorRecords<TMajor>())
            {
                cache.Set(majorRec);
            }
            return cache;
        }

        public IEnumerator<TMod> GetEnumerator()
        {
            yield return this._sourceMod;
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}
