using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Loqui;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Records.Internals;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Cache.Internals.Implementations.Internal
{
    internal class ImmutableModLinkCacheCategory<TKey>
        where TKey : notnull
    {
        private readonly InternalImmutableModLinkCache _parent;
        private readonly IMetaInterfaceMapGetter _metaInterfaceMapGetter;
        private readonly Func<LinkCacheItem, TryGet<TKey>> _keyGetter;
        private readonly Func<TKey, bool> _shortCircuit;
        internal readonly Lazy<IReadOnlyCache<LinkCacheItem, TKey>> _untypedMajorRecords;
        private readonly Dictionary<Type, IReadOnlyCache<LinkCacheItem, TKey>> _majorRecords = new();

        public ImmutableModLinkCacheCategory(
            InternalImmutableModLinkCache parent,
            IMetaInterfaceMapGetter metaInterfaceMapGetter,
            Func<LinkCacheItem, TryGet<TKey>> keyGetter,
            Func<TKey, bool> shortCircuit)
        {
            _parent = parent;
            _metaInterfaceMapGetter = metaInterfaceMapGetter;
            _keyGetter = keyGetter;
            _shortCircuit = shortCircuit;
            _untypedMajorRecords = new Lazy<IReadOnlyCache<LinkCacheItem, TKey>>(
                isThreadSafe: true,
                valueFactory: () => ConstructUntypedCache());
        }

        protected IReadOnlyCache<LinkCacheItem, TKey> ConstructUntypedCache()
        {
            var majorRecords = new Cache<LinkCacheItem, TKey>(x => _keyGetter(x).Value);
            foreach (var majorRec in _parent._sourceMod.EnumerateMajorRecords()
                // ToDo
                // Capture and expose errors optionally via TryResolve /w out param
                .Catch((Exception ex) => { }))
            {
                var item = LinkCacheItem.Factory(majorRec, _parent._simple);
                var key = _keyGetter(item);
                if (key.Failed) continue;
                majorRecords.Set(item);
            }
            return majorRecords;
        }

        private IReadOnlyCache<LinkCacheItem, TKey> ConstructTypedCache(
            Type type,
            IModGetter sourceMod)
        {
            var cache = new Cache<LinkCacheItem, TKey>(x => _keyGetter(x).Value);
            foreach (var majorRec in sourceMod.EnumerateMajorRecords(type)
                // ToDo
                // Capture and expose errors optionally via TryResolve /w out param
                .Catch((Exception ex) => { }))
            {
                var item = LinkCacheItem.Factory(majorRec, _parent._simple);
                var key = _keyGetter(item);
                if (key.Failed) continue;
                cache.Set(item);
            }
            return cache;
        }

        public IReadOnlyCache<LinkCacheItem, TKey> GetCache(
            Type type,
            GameCategory category,
            IModGetter sourceMod)
        {
            lock (_majorRecords)
            {
                if (!_majorRecords.TryGetValue(type, out var cache))
                {
                    if (type == typeof(IMajorRecord)
                        || type == typeof(IMajorRecordGetter))
                    {
                        cache = ConstructTypedCache(type, sourceMod);
                        _majorRecords[typeof(IMajorRecord)] = cache;
                        _majorRecords[typeof(IMajorRecordGetter)] = cache;
                    }
                    else if (LoquiRegistration.TryGetRegister(type, out var registration))
                    {
                        cache = ConstructTypedCache(type, sourceMod);
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
                    else
                    {
                        if (!_metaInterfaceMapGetter.TryGetRegistrationsForInterface(category, type, out var objs))
                        {
                            throw new ArgumentException($"A lookup was queried for an unregistered type: {type.Name}");
                        }
                        var majorRecords = new Cache<LinkCacheItem, TKey>(x => _keyGetter(x).Value);
                        foreach (var regis in objs.Registrations)
                        {
                            majorRecords.Set(
                                GetCache(
                                    type: regis.GetterType,
                                    category: category,
                                    sourceMod: sourceMod).Items);
                        }
                        _majorRecords[type] = majorRecords;
                        cache = majorRecords;
                    }
                }
                return cache;
            }
        }

        public bool TryResolve(TKey key, Type type, [MaybeNullWhen(false)] out LinkCacheItem majorRec)
        {
            if (_shortCircuit(key))
            {
                majorRec = default;
                return false;
            }
            var cache = GetCache(type, _parent.Category, _parent._sourceMod);
            if (!cache.TryGetValue(key, out majorRec))
            {
                majorRec = default;
                return false;
            }
            return true;
        }

        public IEnumerable<LinkCacheItem> AllIdentifiers(Type type, CancellationToken? cancel)
        {
            return GetCache(type, _parent.Category, _parent._sourceMod).Items;
        }

        public void Warmup(Type type)
        {
            GetCache(type, _parent.Category, _parent._sourceMod);
        }
    }
}