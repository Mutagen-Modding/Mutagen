using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Loqui;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Records.Internals;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Cache.Implementations.Internal
{
    internal interface IImmutableModLinkCacheSimpleContextCategory<TKey>
        where TKey : notnull
    {
        bool TryResolveSimpleContext<TMajorGetter>(TKey key, [MaybeNullWhen(false)] out IModContext<TMajorGetter> majorRec)
            where TMajorGetter : class, IMajorRecordCommonGetter;

        bool TryResolveSimpleContext(TKey key, Type type, [MaybeNullWhen(false)] out IModContext<IMajorRecordCommonGetter> majorRec);

        bool TryResolveUntypedSimpleContext(TKey key, [MaybeNullWhen(false)] out IModContext<IMajorRecordCommonGetter> majorRec);
    }

    internal class ImmutableModLinkCacheSimpleContextCategory<TKey> : IImmutableModLinkCacheSimpleContextCategory<TKey>
        where TKey : notnull
    {
        private readonly bool _simple;
        private readonly ILinkCache _linkCache;
        private readonly GameCategory _category;
        private readonly IMajorRecordSimpleContextEnumerable _contextEnumerable;
        private readonly Func<IMajorRecordCommonGetter, TryGet<TKey>> _keyGetter;
        private readonly Func<TKey, bool> _shortCircuit;
        private readonly Lazy<IReadOnlyCache<IModContext<IMajorRecordCommonGetter>, TKey>> _untypedContexts;
        private readonly Dictionary<Type, IReadOnlyCache<IModContext<IMajorRecordCommonGetter>, TKey>> _contexts = new();

        public ImmutableModLinkCacheSimpleContextCategory(
            bool simple,
            ILinkCache linkCache,
            GameCategory category,
            IMajorRecordSimpleContextEnumerable contextEnumerable,
            Func<IMajorRecordCommonGetter, TryGet<TKey>> keyGetter,
            Func<TKey, bool> shortCircuit)
        {
            _simple = simple;
            _linkCache = linkCache;
            _category = category;
            _contextEnumerable = contextEnumerable;
            _keyGetter = keyGetter;
            _shortCircuit = shortCircuit;
            _untypedContexts = new Lazy<IReadOnlyCache<IModContext<IMajorRecordCommonGetter>, TKey>>(
                isThreadSafe: true,
                valueFactory: () => ConstructUntypedContextCache());
        }

        private IReadOnlyCache<IModContext<IMajorRecordCommonGetter>, TKey> ConstructUntypedContextCache()
        {
            var majorRecords = new Cache<IModContext<IMajorRecordCommonGetter>, TKey>(x => _keyGetter(x.Record).Value);
            foreach (var majorRec in _contextEnumerable.EnumerateMajorRecordSimpleContexts<IMajorRecordCommonGetter>(_linkCache))
            {
                var key = _keyGetter(majorRec.Record);
                if (key.Failed) continue;
                majorRecords.Set(majorRec);
            }
            return majorRecords;
        }

        public bool TryResolveSimpleContext<TMajorGetter>(TKey key, [MaybeNullWhen(false)] out IModContext<TMajorGetter> majorRec)
            where TMajorGetter : class, IMajorRecordCommonGetter
        {
            if (_shortCircuit(key))
            {
                majorRec = default;
                return false;
            }
            var cache = GetContextCache(typeof(TMajorGetter));
            if (!cache.TryGetValue(key, out var majorRecObj)
                || !(majorRecObj.Record is TMajorGetter))
            {
                majorRec = default;
                return false;
            }
            majorRec = majorRecObj.AsType<IMajorRecordCommonGetter, TMajorGetter>();
            return true;
        }

        public bool TryResolveSimpleContext(TKey key, Type type, [MaybeNullWhen(false)] out IModContext<IMajorRecordCommonGetter> majorRec)
        {
            if (_shortCircuit(key))
            {
                majorRec = default;
                return false;
            }
            var cache = GetContextCache(type);
            if (!cache.TryGetValue(key, out majorRec))
            {
                majorRec = default;
                return false;
            }
            return true;
        }

        public bool TryResolveUntypedSimpleContext(TKey key, [MaybeNullWhen(false)] out IModContext<IMajorRecordCommonGetter> majorRec)
        {
            return _untypedContexts.Value.TryGetValue(key, out majorRec);
        }

        private IReadOnlyCache<IModContext<IMajorRecordCommonGetter>, TKey> GetContextCache(Type type)
        {
            if (_simple)
            {
                throw new ArgumentException("Queried for record on a simple cache");
            }
            lock (_contexts)
            {
                if (!_contexts.TryGetValue(type, out var cache))
                {
                    if (type == typeof(IMajorRecordCommon)
                        || type == typeof(IMajorRecordCommonGetter))
                    {
                        cache = ConstructContextCache(type);
                        _contexts[typeof(IMajorRecordCommon)] = cache;
                        _contexts[typeof(IMajorRecordCommonGetter)] = cache;
                    }
                    else if (LoquiRegistration.TryGetRegister(type, out var registration))
                    {
                        cache = ConstructContextCache(type);
                        _contexts[registration.ClassType] = cache;
                        _contexts[registration.GetterType] = cache;
                        _contexts[registration.SetterType] = cache;
                        if (registration.InternalGetterType != null)
                        {
                            _contexts[registration.InternalGetterType] = cache;
                        }
                        if (registration.InternalSetterType != null)
                        {
                            _contexts[registration.InternalSetterType] = cache;
                        }
                    }
                    else
                    {
                        var interfaceMappings = LinkInterfaceMapping.InterfaceToObjectTypes(_category);
                        if (!interfaceMappings.TryGetValue(type, out var objs))
                        {
                            throw new ArgumentException($"A lookup was queried for an unregistered type: {type.Name}");
                        }
                        var majorRecords = new Cache<IModContext<IMajorRecordCommonGetter>, TKey>(x => _keyGetter(x.Record).Value);
                        foreach (var objType in objs)
                        {
                            majorRecords.Set(
                                GetContextCache(
                                    LoquiRegistration.GetRegister(objType).GetterType).Items);
                        }
                        _contexts[type] = majorRecords;
                        cache = majorRecords;
                    }
                }
                return cache;
            }
        }

        private IReadOnlyCache<IModContext<IMajorRecordCommonGetter>, TKey> ConstructContextCache(Type type)
        {
            var cache = new Cache<IModContext<IMajorRecordCommonGetter>, TKey>(x => _keyGetter(x.Record).Value);
            // ToDo
            // Upgrade to call EnumerateGroups(), which will perform much better
            foreach (var majorRec in _contextEnumerable.EnumerateMajorRecordSimpleContexts(_linkCache, type))
            {
                var key = _keyGetter(majorRec.Record);
                if (key.Failed) continue;
                cache.Set(majorRec);
            }
            return cache;
        }
    }
}