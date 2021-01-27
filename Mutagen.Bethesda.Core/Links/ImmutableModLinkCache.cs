using Loqui;
using Mutagen.Bethesda.Core;
using Noggog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Mutagen.Bethesda
{
    /// <summary>
    /// A Link Cache using a single mod as its link target. <br/>
    /// <br/>
    /// Internal caching will only occur for the types required to serve the requested link. <br/>
    /// <br/>
    /// All functionality is multithread safe. <br/>
    /// <br/>
    /// Modification of the target Mod is not safe.  Internal caches can become incorrect if 
    /// modifications occur on content already cached.
    /// </summary>
    public class ImmutableModLinkCache : ILinkCache
    {
        private readonly IModGetter _sourceMod;

        protected readonly Lazy<IReadOnlyCache<IMajorRecordCommonGetter, FormKey>> _untypedMajorRecords;
        protected readonly Dictionary<Type, IReadOnlyCache<IMajorRecordCommonGetter, FormKey>> _majorRecords = new Dictionary<Type, IReadOnlyCache<IMajorRecordCommonGetter, FormKey>>();

        /// <inheritdoc />
        public IReadOnlyList<IModGetter> ListedOrder { get; }

        /// <inheritdoc />
        public IReadOnlyList<IModGetter> PriorityOrder => ListedOrder;

        public ImmutableModLinkCache(IModGetter sourceMod)
        {
            _sourceMod = sourceMod;
            this._untypedMajorRecords = new Lazy<IReadOnlyCache<IMajorRecordCommonGetter, FormKey>>(
                isThreadSafe: true,
                valueFactory: () => ConstructCache());
            this.ListedOrder = new List<IModGetter>()
            {
                sourceMod
            };
        }

        protected IReadOnlyCache<IMajorRecordCommonGetter, FormKey> ConstructCache(Type type)
        {
            var cache = new Cache<IMajorRecordCommonGetter, FormKey>(x => x.FormKey);
            // ToDo
            // Upgrade to call EnumerateGroups(), which will perform much better
            foreach (var majorRec in this._sourceMod.EnumerateMajorRecords(type))
            {
                cache.Set(majorRec);
            }
            return cache;
        }

        protected IReadOnlyCache<IMajorRecordCommonGetter, FormKey> ConstructCache()
        {
            var majorRecords = new Cache<IMajorRecordCommonGetter, FormKey>(x => x.FormKey);
            foreach (var majorRec in this._sourceMod.EnumerateMajorRecords())
            {
                majorRecords.Set(majorRec);
            }
            return majorRecords;
        }


        /// <inheritdoc />
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public bool TryResolve(FormKey formKey, [MaybeNullWhen(false)] out IMajorRecordCommonGetter majorRec)
        {
            if (formKey == null)
            {
                majorRec = default;
                return false;
            }
            return _untypedMajorRecords.Value.TryGetValue(formKey, out majorRec);
        }

        /// <inheritdoc />
        public bool TryResolve<TMajor>(FormKey formKey, [MaybeNullWhen(false)] out TMajor majorRec)
            where TMajor : class, IMajorRecordCommonGetter
        {
            if (formKey == null)
            {
                majorRec = default;
                return false;
            }
            IReadOnlyCache<IMajorRecordCommonGetter, FormKey> cache;
            lock (_majorRecords)
            {
                cache = GetCache(typeof(TMajor));
            }
            if (!cache.TryGetValue(formKey, out var majorRecObj))
            {
                majorRec = default;
                return false;
            }
            majorRec = (majorRecObj as TMajor)!;
            return majorRec != null;
        }

        /// <inheritdoc />
        public bool TryResolve(FormKey formKey, Type type, [MaybeNullWhen(false)] out IMajorRecordCommonGetter majorRec)
        {
            if (formKey == null)
            {
                majorRec = default;
                return false;
            }
            IReadOnlyCache<IMajorRecordCommonGetter, FormKey> cache;
            lock (_majorRecords)
            {
                cache = GetCache(type);
            }
            if (!cache.TryGetValue(formKey, out majorRec))
            {
                majorRec = default;
                return false;
            }
            return true;
        }

        /// <inheritdoc />
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public IMajorRecordCommonGetter Resolve(FormKey formKey)
        {
            if (TryResolve<IMajorRecordCommonGetter>(formKey, out var majorRec)) return majorRec;
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

        private IReadOnlyCache<IMajorRecordCommonGetter, FormKey> GetCache(Type type)
        {
            if (!_majorRecords.TryGetValue(type, out var cache))
            {
                if (type.Equals(typeof(IMajorRecordCommon))
                    || type.Equals(typeof(IMajorRecordCommonGetter)))
                {
                    cache = ConstructCache(type);
                    _majorRecords[typeof(IMajorRecordCommon)] = cache;
                    _majorRecords[typeof(IMajorRecordCommonGetter)] = cache;
                }
                else if (LoquiRegistration.TryGetRegister(type, out var registration))
                {
                    cache = ConstructCache(type);
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
                    var interfaceMappings = LinkInterfaceMapping.InterfaceToObjectTypes(_sourceMod.GameRelease.ToCategory());
                    if (!interfaceMappings.TryGetValue(type, out var objs))
                    {
                        throw new ArgumentException($"A lookup was queried for an unregistered type: {type.Name}");
                    }
                    var majorRecords = new Cache<IMajorRecordCommonGetter, FormKey>(x => x.FormKey);
                    foreach (var objType in objs)
                    {
                        majorRecords.Set(GetCache(LoquiRegistration.GetRegister(objType).GetterType).Items);
                    }
                    _majorRecords[type] = majorRecords;
                    cache = majorRecords;
                }
            }
            return cache;
        }

        /// <inheritdoc />
        public IEnumerable<TMajor> ResolveAll<TMajor>(FormKey formKey)
            where TMajor : class, IMajorRecordCommonGetter
        {
            if (TryResolve<TMajor>(formKey, out var rec))
            {
                yield return rec;
            }
        }

        /// <inheritdoc />
        public IEnumerable<IMajorRecordCommonGetter> ResolveAll(FormKey formKey, Type type)
        {
            if (TryResolve(formKey, type, out var rec))
            {
                yield return rec;
            }
        }

        /// <inheritdoc />
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public IEnumerable<IMajorRecordCommonGetter> ResolveAll(FormKey formKey)
        {
            if (TryResolve(formKey, out var rec))
            {
                yield return rec;
            }
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
    /// A Link Cache using a single mod as its link target. <br/>
    /// <br/>
    /// Internal caching will only occur for the types required to serve the requested link. <br/>
    /// <br/>
    /// All functionality is multithread safe. <br/>
    /// <br/>
    /// Modification of the target Mod is not safe.  Internal caches can become incorrect if 
    /// modifications occur on content already cached.
    /// </summary>
    public class ImmutableModLinkCache<TMod, TModGetter> : ImmutableModLinkCache, ILinkCache<TMod, TModGetter>
        where TMod : class, IContextMod<TMod>, TModGetter
        where TModGetter : class, IContextGetterMod<TMod>
    {
        private readonly TModGetter _sourceMod;

        private readonly Lazy<IReadOnlyCache<IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter>, FormKey>> _untypedContexts;
        private readonly Dictionary<Type, IReadOnlyCache<IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter>, FormKey>> _contexts = new Dictionary<Type, IReadOnlyCache<IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter>, FormKey>>();


        /// <summary>
        /// Constructs a link cache around a target mod
        /// </summary>
        /// <param name="sourceMod">Mod to resolve against when linking</param>
        public ImmutableModLinkCache(TModGetter sourceMod)
            : base(sourceMod)
        {
            this._sourceMod = sourceMod;
            this._untypedContexts = new Lazy<IReadOnlyCache<IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter>, FormKey>>(
                isThreadSafe: true,
                valueFactory: () => ConstructContextCache());
        }

        /// <inheritdoc />
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public bool TryResolveContext(FormKey formKey, [MaybeNullWhen(false)] out IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter> majorRec)
        {
            if (formKey == null)
            {
                majorRec = default;
                return false;
            }
            return _untypedContexts.Value.TryGetValue(formKey, out majorRec);
        }

        /// <inheritdoc />
        public bool TryResolveContext<TMajorSetter, TMajorGetter>(FormKey formKey, [MaybeNullWhen(false)] out IModContext<TMod, TMajorSetter, TMajorGetter> majorRec)
            where TMajorSetter : class, IMajorRecordCommon, TMajorGetter
            where TMajorGetter : class, IMajorRecordCommonGetter
        {
            if (formKey == null)
            {
                majorRec = default;
                return false;
            }
            IReadOnlyCache<IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter>, FormKey> cache;
            lock (_contexts)
            {
                cache = GetContextCache(typeof(TMajorGetter));
            }
            if (!cache.TryGetValue(formKey, out var majorRecObj)
                || !(majorRecObj.Record is TMajorGetter))
            {
                majorRec = default;
                return false;
            }
            majorRec = majorRecObj.AsType<TMod, IMajorRecordCommon, IMajorRecordCommonGetter, TMajorSetter, TMajorGetter>();
            return true;
        }

        /// <inheritdoc />
        public bool TryResolveContext(FormKey formKey, Type type, [MaybeNullWhen(false)] out IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter> majorRec)
        {
            if (formKey == null)
            {
                majorRec = default;
                return false;
            }
            IReadOnlyCache<IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter>, FormKey> cache;
            lock (_contexts)
            {
                cache = GetContextCache(type);
            }
            if (!cache.TryGetValue(formKey, out majorRec))
            {
                majorRec = default;
                return false;
            }
            return true;
        }

        /// <inheritdoc />
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter> ResolveContext(FormKey formKey)
        {
            if (TryResolveContext<IMajorRecordCommon, IMajorRecordCommonGetter>(formKey, out var majorRec)) return majorRec;
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

        private IReadOnlyCache<IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter>, FormKey> GetContextCache(Type type)
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
                    var interfaceMappings = LinkInterfaceMapping.InterfaceToObjectTypes(_sourceMod.GameRelease.ToCategory());
                    if (!interfaceMappings.TryGetValue(type, out var objs))
                    {
                        throw new ArgumentException($"A lookup was queried for an unregistered type: {type.Name}");
                    }
                    var majorRecords = new Cache<IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter>, FormKey>(x => x.Record.FormKey);
                    foreach (var objType in objs)
                    {
                        majorRecords.Set(GetContextCache(LoquiRegistration.GetRegister(objType).GetterType).Items);
                    }
                    _contexts[type] = majorRecords;
                    cache = majorRecords;
                }
            }
            return cache;
        }

        private IReadOnlyCache<IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter>, FormKey> ConstructContextCache(Type type)
        {
            var cache = new Cache<IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter>, FormKey>(x => x.Record.FormKey);
            // ToDo
            // Upgrade to call EnumerateGroups(), which will perform much better
            foreach (var majorRec in this._sourceMod.EnumerateMajorRecordContexts(this, type))
            {
                cache.Set(majorRec);
            }
            return cache;
        }

        private IReadOnlyCache<IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter>, FormKey> ConstructContextCache()
        {
            var majorRecords = new Cache<IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter>, FormKey>(x => x.Record.FormKey);
            foreach (var majorRec in this._sourceMod.EnumerateMajorRecordContexts<IMajorRecordCommon, IMajorRecordCommonGetter>(this))
            {
                majorRecords.Set(majorRec);
            }
            return majorRecords;
        }

        /// <inheritdoc />
        public IEnumerable<IModContext<TMod, TMajorSetter, TMajorGetter>> ResolveAllContexts<TMajorSetter, TMajorGetter>(FormKey formKey)
            where TMajorSetter : class, IMajorRecordCommon, TMajorGetter
            where TMajorGetter : class, IMajorRecordCommonGetter
        {
            if (TryResolveContext<TMajorSetter, TMajorGetter>(formKey, out var rec))
            {
                yield return rec;
            }
        }

        /// <inheritdoc />
        public IEnumerable<IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter>> ResolveAllContexts(FormKey formKey, Type type)
        {
            if (TryResolveContext(formKey, type, out var rec))
            {
                yield return rec;
            }
        }

        /// <inheritdoc />
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public IEnumerable<IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter>> ResolveAllContexts(FormKey formKey)
        {
            if (TryResolveContext(formKey, out var rec))
            {
                yield return rec;
            }
        }
    }
}
