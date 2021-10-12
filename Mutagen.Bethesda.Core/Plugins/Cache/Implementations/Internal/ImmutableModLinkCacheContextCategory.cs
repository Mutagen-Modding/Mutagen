using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Loqui;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Records.Internals;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Cache.Implementations.Internal
{
    internal interface IImmutableModLinkCacheContextCategory<TMod, TModGetter, TKey>
        where TMod : class, IContextMod<TMod, TModGetter>, TModGetter
        where TModGetter : class, IContextGetterMod<TMod, TModGetter> 
        where TKey : notnull
    {
        bool TryResolveContext<TMajor, TMajorGetter>(TKey key, [MaybeNullWhen(false)] out IModContext<TMod, TModGetter, TMajor, TMajorGetter> majorRec)
            where TMajor : class, IMajorRecordCommon, TMajorGetter
            where TMajorGetter : class, IMajorRecordCommonGetter;

        bool TryResolveContext(TKey key, Type type, [MaybeNullWhen(false)] out IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter> majorRec);
        
        bool TryResolveUntypedContext(TKey key, [MaybeNullWhen(false)] out IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter> majorRec);
    }

    internal class ImmutableModLinkCacheContextCategory<TMod, TModGetter, TKey> : IImmutableModLinkCacheContextCategory<TMod, TModGetter, TKey> where TMod : class, IContextMod<TMod, TModGetter>, TModGetter
        where TModGetter : class, IContextGetterMod<TMod, TModGetter>
        where TKey : notnull
    {
        private readonly ImmutableModLinkCache<TMod, TModGetter> _parent;
        private readonly Func<IMajorRecordCommonGetter, TryGet<TKey>> _keyGetter;
        private readonly Func<TKey, bool> _shortCircuit;
        private readonly Lazy<IReadOnlyCache<IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter>, TKey>> _untypedContexts;
        private readonly Dictionary<Type, IReadOnlyCache<IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter>, TKey>> _contexts = new();

        public ImmutableModLinkCacheContextCategory(
            ImmutableModLinkCache<TMod, TModGetter> parent,
            Func<IMajorRecordCommonGetter, TryGet<TKey>> keyGetter,
            Func<TKey, bool> shortCircuit)
        {
            _parent = parent;
            _keyGetter = keyGetter;
            _shortCircuit = shortCircuit;
            _untypedContexts = new Lazy<IReadOnlyCache<IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter>, TKey>>(
                isThreadSafe: true,
                valueFactory: () => ConstructUntypedContextCache());
        }

        private IReadOnlyCache<IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter>, TKey> ConstructUntypedContextCache()
        {
            var majorRecords = new Cache<IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter>, TKey>(x => _keyGetter(x.Record).Value);
            foreach (var majorRec in this._parent._sourceMod.EnumerateMajorRecordContexts<IMajorRecordCommon, IMajorRecordCommonGetter>(_parent))
            {
                var key = _keyGetter(majorRec.Record);
                if (key.Failed) continue;
                majorRecords.Set(majorRec);
            }
            return majorRecords;
        }

        public bool TryResolveContext<TMajor, TMajorGetter>(TKey key, [MaybeNullWhen(false)] out IModContext<TMod, TModGetter, TMajor, TMajorGetter> majorRec)
            where TMajor : class, IMajorRecordCommon, TMajorGetter
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
            majorRec = majorRecObj.AsType<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter, TMajor, TMajorGetter>();
            return true;
        }

        public bool TryResolveContext(TKey key, Type type, [MaybeNullWhen(false)] out IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter> majorRec)
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

        public bool TryResolveUntypedContext(TKey key, [MaybeNullWhen(false)] out IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter> majorRec)
        {
            return _untypedContexts.Value.TryGetValue(key, out majorRec);
        }

        private IReadOnlyCache<IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter>, TKey> GetContextCache(Type type)
        {
            if (_parent._simple)
            {
                throw new ArgumentException("Queried for record on a simple cache");
            }
            lock (_contexts)
            {
                if (!_contexts.TryGetValue(type, out var cache))
                {
                    if (type.Equals(typeof(IMajorRecordCommon))
                        || type.Equals(typeof(IMajorRecordCommonGetter)))
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
                        var interfaceMappings = LinkInterfaceMapping.InterfaceToObjectTypes(_parent._sourceMod.GameRelease.ToCategory());
                        if (!interfaceMappings.TryGetValue(type, out var objs))
                        {
                            throw new ArgumentException($"A lookup was queried for an unregistered type: {type.Name}");
                        }
                        var majorRecords = new Cache<IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter>, TKey>(x => _keyGetter(x.Record).Value);
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

        private IReadOnlyCache<IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter>, TKey> ConstructContextCache(Type type)
        {
            var cache = new Cache<IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter>, TKey>(x => _keyGetter(x.Record).Value);
            // ToDo
            // Upgrade to call EnumerateGroups(), which will perform much better
            foreach (var majorRec in _parent._sourceMod.EnumerateMajorRecordContexts(_parent, type))
            {
                var key = _keyGetter(majorRec.Record);
                if (key.Failed) continue;
                cache.Set(majorRec);
            }
            return cache;
        }
    }
}