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
        private readonly GameCategory _category;

        protected readonly Lazy<IReadOnlyCache<IMajorRecordCommonGetter, FormKey>> _untypedFormKeyMajorRecords;
        protected readonly Dictionary<Type, IReadOnlyCache<IMajorRecordCommonGetter, FormKey>> _majorFormKeyRecords = new Dictionary<Type, IReadOnlyCache<IMajorRecordCommonGetter, FormKey>>();
        protected readonly Lazy<IReadOnlyCache<IMajorRecordCommonGetter, string>> _untypedEditorIdMajorRecords;
        protected readonly Dictionary<Type, IReadOnlyCache<IMajorRecordCommonGetter, string>> _majorEditorIdRecords = new Dictionary<Type, IReadOnlyCache<IMajorRecordCommonGetter, string>>();

        /// <inheritdoc />
        public IReadOnlyList<IModGetter> ListedOrder { get; }

        /// <inheritdoc />
        public IReadOnlyList<IModGetter> PriorityOrder => ListedOrder;

        public ImmutableModLinkCache(IModGetter sourceMod)
        {
            _sourceMod = sourceMod;
            _category = sourceMod.GameRelease.ToCategory();
            this._untypedFormKeyMajorRecords = new Lazy<IReadOnlyCache<IMajorRecordCommonGetter, FormKey>>(
                isThreadSafe: true,
                valueFactory: () => ConstructUntypedFormKeyCache());
            this._untypedEditorIdMajorRecords = new Lazy<IReadOnlyCache<IMajorRecordCommonGetter, string>>(
                isThreadSafe: true,
                valueFactory: () => ConstructUntypedEditorIdCache());
            this.ListedOrder = new List<IModGetter>()
            {
                sourceMod
            };
        }

        protected static IReadOnlyCache<IMajorRecordCommonGetter, K> ConstructCache<K>(
            Type type,
            IModGetter sourceMod,
            Func<IMajorRecordCommonGetter, TryGet<K>> keyGetter)
            where K : notnull
        {
            var cache = new Cache<IMajorRecordCommonGetter, K>(x => keyGetter(x).Value);
            // ToDo
            // Upgrade to call EnumerateGroups(), which will perform much better
            foreach (var majorRec in sourceMod.EnumerateMajorRecords(type))
            {
                var key = keyGetter(majorRec);
                if (key.Failed) continue;
                cache.Set(majorRec);
            }
            return cache;
        }

        protected IReadOnlyCache<IMajorRecordCommonGetter, FormKey> ConstructUntypedFormKeyCache()
        {
            var majorRecords = new Cache<IMajorRecordCommonGetter, FormKey>(x => x.FormKey);
            foreach (var majorRec in this._sourceMod.EnumerateMajorRecords())
            {
                majorRecords.Set(majorRec);
            }
            return majorRecords;
        }

        protected IReadOnlyCache<IMajorRecordCommonGetter, string> ConstructUntypedEditorIdCache()
        {
            var majorRecords = new Cache<IMajorRecordCommonGetter, string>(x => x.EditorID!);
            foreach (var majorRec in this._sourceMod.EnumerateMajorRecords())
            {
                if (majorRec.EditorID.IsNullOrWhitespace()) continue;
                majorRecords.Set(majorRec);
            }
            return majorRecords;
        }


        /// <inheritdoc />
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public bool TryResolve(FormKey formKey, [MaybeNullWhen(false)] out IMajorRecordCommonGetter majorRec)
        {
            if (formKey.IsNull)
            {
                majorRec = default;
                return false;
            }
            return _untypedFormKeyMajorRecords.Value.TryGetValue(formKey, out majorRec);
        }


        /// <inheritdoc />
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public bool TryResolve(string editorId, [MaybeNullWhen(false)] out IMajorRecordCommonGetter majorRec)
        {
            if (string.IsNullOrWhiteSpace(editorId))
            {
                majorRec = default;
                return false;
            }
            return _untypedEditorIdMajorRecords.Value.TryGetValue(editorId, out majorRec);
        }

        /// <inheritdoc />
        public bool TryResolve<TMajor>(FormKey formKey, [MaybeNullWhen(false)] out TMajor majorRec)
            where TMajor : class, IMajorRecordCommonGetter
        {
            if (formKey.IsNull)
            {
                majorRec = default;
                return false;
            }
            var cache = GetCache(typeof(TMajor), _category, _sourceMod, x => TryGet<FormKey>.Succeed(x.FormKey), _majorFormKeyRecords);
            if (!cache.TryGetValue(formKey, out var majorRecObj))
            {
                majorRec = default;
                return false;
            }
            majorRec = (majorRecObj as TMajor)!;
            return majorRec != null;
        }

        /// <inheritdoc />
        public bool TryResolve<TMajor>(string editorId, [MaybeNullWhen(false)] out TMajor majorRec)
            where TMajor : class, IMajorRecordCommonGetter
        {
            if (string.IsNullOrWhiteSpace(editorId))
            {
                majorRec = default;
                return false;
            }
            var cache = GetCache(
                typeof(TMajor),
                _category,
                _sourceMod,
                m =>
                {
                    var edid = m.EditorID;
                    return TryGet<string>.Create(successful: !string.IsNullOrWhiteSpace(edid), edid!);
                },
                _majorEditorIdRecords);
            if (!cache.TryGetValue(editorId, out var majorRecObj))
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
            if (formKey.IsNull)
            {
                majorRec = default;
                return false;
            }
            var cache = GetCache(type, _category, _sourceMod, x => TryGet<FormKey>.Succeed(x.FormKey), _majorFormKeyRecords);
            if (!cache.TryGetValue(formKey, out majorRec))
            {
                majorRec = default;
                return false;
            }
            return true;
        }

        /// <inheritdoc />
        public bool TryResolve(string editorId, Type type, [MaybeNullWhen(false)] out IMajorRecordCommonGetter majorRec)
        {
            if (editorId.IsNullOrWhitespace())
            {
                majorRec = default;
                return false;
            }
            var cache = GetCache(
                type,
                _category,
                _sourceMod,
                m =>
                {
                    var edid = m.EditorID;
                    return TryGet<string>.Create(successful: !string.IsNullOrWhiteSpace(edid), edid!);
                },
                _majorEditorIdRecords);
            if (!cache.TryGetValue(editorId, out majorRec))
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
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public IMajorRecordCommonGetter Resolve(string editorId)
        {
            if (TryResolve<IMajorRecordCommonGetter>(editorId, out var majorRec)) return majorRec;
            throw new KeyNotFoundException($"EditorID {editorId} could not be found.");
        }

        /// <inheritdoc />
        public IMajorRecordCommonGetter Resolve(FormKey formKey, Type type)
        {
            if (TryResolve(formKey, type, out var commonRec)) return commonRec;
            throw new KeyNotFoundException($"FormKey {formKey} could not be found.");
        }

        /// <inheritdoc />
        public IMajorRecordCommonGetter Resolve(string editorId, Type type)
        {
            if (TryResolve(editorId, type, out var commonRec)) return commonRec;
            throw new KeyNotFoundException($"EditorID {editorId} could not be found.");
        }

        /// <inheritdoc />
        public TMajor Resolve<TMajor>(FormKey formKey)
            where TMajor : class, IMajorRecordCommonGetter
        {
            if (TryResolve<TMajor>(formKey, out var commonRec)) return commonRec;
            throw new KeyNotFoundException($"FormKey {formKey} could not be found.");
        }

        /// <inheritdoc />
        public TMajor Resolve<TMajor>(string editorId)
            where TMajor : class, IMajorRecordCommonGetter
        {
            if (TryResolve<TMajor>(editorId, out var commonRec)) return commonRec;
            throw new KeyNotFoundException($"EditorID {editorId} could not be found.");
        }

        private static IReadOnlyCache<IMajorRecordCommonGetter, K> GetCache<K>(
            Type type,
            GameCategory category,
            IModGetter sourceMod,
            Func<IMajorRecordCommonGetter, TryGet<K>> keyGetter,
            Dictionary<Type, IReadOnlyCache<IMajorRecordCommonGetter, K>> typeCache)
            where K : notnull
        {
            lock (typeCache)
            {
                if (!typeCache.TryGetValue(type, out var cache))
                {
                    if (type.Equals(typeof(IMajorRecordCommon))
                        || type.Equals(typeof(IMajorRecordCommonGetter)))
                    {
                        cache = ConstructCache(type, sourceMod, keyGetter);
                        typeCache[typeof(IMajorRecordCommon)] = cache;
                        typeCache[typeof(IMajorRecordCommonGetter)] = cache;
                    }
                    else if (LoquiRegistration.TryGetRegister(type, out var registration))
                    {
                        cache = ConstructCache(type, sourceMod, keyGetter);
                        typeCache[registration.ClassType] = cache;
                        typeCache[registration.GetterType] = cache;
                        typeCache[registration.SetterType] = cache;
                        if (registration.InternalGetterType != null)
                        {
                            typeCache[registration.InternalGetterType] = cache;
                        }
                        if (registration.InternalSetterType != null)
                        {
                            typeCache[registration.InternalSetterType] = cache;
                        }
                    }
                    else
                    {
                        var interfaceMappings = LinkInterfaceMapping.InterfaceToObjectTypes(category);
                        if (!interfaceMappings.TryGetValue(type, out var objs))
                        {
                            throw new ArgumentException($"A lookup was queried for an unregistered type: {type.Name}");
                        }
                        var majorRecords = new Cache<IMajorRecordCommonGetter, K>(x => keyGetter(x).Value);
                        foreach (var objType in objs)
                        {
                            majorRecords.Set(
                                GetCache<K>(
                                    type: LoquiRegistration.GetRegister(objType).GetterType,
                                    category: category,
                                    typeCache: typeCache,
                                    keyGetter: keyGetter,
                                    sourceMod: sourceMod).Items);
                        }
                        typeCache[type] = majorRecords;
                        cache = majorRecords;
                    }
                }
                return cache;
            }
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
        public IEnumerable<TMajor> ResolveAll<TMajor>(string editorId)
            where TMajor : class, IMajorRecordCommonGetter
        {
            if (TryResolve<TMajor>(editorId, out var rec))
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
        public IEnumerable<IMajorRecordCommonGetter> ResolveAll(string editorId, Type type)
        {
            if (TryResolve(editorId, type, out var rec))
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
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public IEnumerable<IMajorRecordCommonGetter> ResolveAll(string editorId)
        {
            if (TryResolve(editorId, out var rec))
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
        public bool TryResolve(string editorId, [MaybeNullWhen(false)] out IMajorRecordCommonGetter majorRec, params Type[] types)
        {
            return TryResolve(editorId, (IEnumerable<Type>)types, out majorRec);
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
        public bool TryResolve(string editorId, IEnumerable<Type> types, [MaybeNullWhen(false)] out IMajorRecordCommonGetter majorRec)
        {
            foreach (var type in types)
            {
                if (TryResolve(editorId, type, out majorRec))
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
        public IMajorRecordCommonGetter Resolve(string editorId, params Type[] types)
        {
            return Resolve(editorId, (IEnumerable<Type>)types);
        }

        /// <inheritdoc />
        public IMajorRecordCommonGetter Resolve(FormKey formKey, IEnumerable<Type> types)
        {
            if (TryResolve(formKey, types, out var commonRec)) return commonRec;
            throw new KeyNotFoundException($"FormKey {formKey} could not be found.");
        }

        /// <inheritdoc />
        public IMajorRecordCommonGetter Resolve(string editorId, IEnumerable<Type> types)
        {
            if (TryResolve(editorId, types, out var commonRec)) return commonRec;
            throw new KeyNotFoundException($"EditorID {editorId} could not be found.");
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

        private readonly Lazy<IReadOnlyCache<IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter>, FormKey>> _untypedFormKeyContexts;
        private readonly Dictionary<Type, IReadOnlyCache<IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter>, FormKey>> _formKeyContexts
            = new Dictionary<Type, IReadOnlyCache<IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter>, FormKey>>();
        private readonly Lazy<IReadOnlyCache<IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter>, string>> _untypedEditorIdContexts;
        private readonly Dictionary<Type, IReadOnlyCache<IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter>, string>> _editorIdContexts
            = new Dictionary<Type, IReadOnlyCache<IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter>, string>>();

        /// <summary>
        /// Constructs a link cache around a target mod
        /// </summary>
        /// <param name="sourceMod">Mod to resolve against when linking</param>
        public ImmutableModLinkCache(TModGetter sourceMod)
            : base(sourceMod)
        {
            this._sourceMod = sourceMod;
            this._untypedFormKeyContexts = new Lazy<IReadOnlyCache<IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter>, FormKey>>(
                isThreadSafe: true,
                valueFactory: () => ConstructUntypedContextCache(x => TryGet<FormKey>.Succeed(x.FormKey)));
            this._untypedEditorIdContexts = new Lazy<IReadOnlyCache<IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter>, string>>(
                isThreadSafe: true,
                valueFactory: () => ConstructUntypedContextCache(m =>
                {
                    var edid = m.EditorID;
                    return TryGet<string>.Create(successful: !string.IsNullOrWhiteSpace(edid), edid!);
                }));
        }

        /// <inheritdoc />
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public bool TryResolveContext(FormKey formKey, [MaybeNullWhen(false)] out IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter> majorRec)
        {
            if (formKey.IsNull)
            {
                majorRec = default;
                return false;
            }
            return _untypedFormKeyContexts.Value.TryGetValue(formKey, out majorRec);
        }

        /// <inheritdoc />
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public bool TryResolveContext(string editorId, [MaybeNullWhen(false)] out IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter> majorRec)
        {
            if (editorId.IsNullOrWhitespace())
            {
                majorRec = default;
                return false;
            }
            return _untypedEditorIdContexts.Value.TryGetValue(editorId, out majorRec);
        }

        /// <inheritdoc />
        public bool TryResolveContext<TMajorSetter, TMajorGetter>(FormKey formKey, [MaybeNullWhen(false)] out IModContext<TMod, TMajorSetter, TMajorGetter> majorRec)
            where TMajorSetter : class, IMajorRecordCommon, TMajorGetter
            where TMajorGetter : class, IMajorRecordCommonGetter
        {
            if (formKey.IsNull)
            {
                majorRec = default;
                return false;
            }
            var cache = GetContextCache(
                typeof(TMajorGetter),
                sourceMod: _sourceMod,
                linkCache: this,
                keyGetter: m => TryGet<FormKey>.Succeed(m.FormKey),
                typedContexts: _formKeyContexts);
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
        public bool TryResolveContext<TMajorSetter, TMajorGetter>(string editorId, [MaybeNullWhen(false)] out IModContext<TMod, TMajorSetter, TMajorGetter> majorRec)
            where TMajorSetter : class, IMajorRecordCommon, TMajorGetter
            where TMajorGetter : class, IMajorRecordCommonGetter
        {
            if (editorId.IsNullOrWhitespace())
            {
                majorRec = default;
                return false;
            }
            var cache = GetContextCache(
                typeof(TMajorGetter),
                sourceMod: _sourceMod,
                linkCache: this,
                keyGetter: m =>
                {
                    var edid = m.EditorID;
                    return TryGet<string>.Create(successful: !string.IsNullOrWhiteSpace(edid), edid!);
                },
                typedContexts: _editorIdContexts);
            if (!cache.TryGetValue(editorId, out var majorRecObj)
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
            if (formKey.IsNull)
            {
                majorRec = default;
                return false;
            }
            var cache = GetContextCache(
                type,
                sourceMod: _sourceMod,
                linkCache: this,
                keyGetter: m => TryGet<FormKey>.Succeed(m.FormKey),
                typedContexts: _formKeyContexts);
            if (!cache.TryGetValue(formKey, out majorRec))
            {
                majorRec = default;
                return false;
            }
            return true;
        }

        /// <inheritdoc />
        public bool TryResolveContext(string editorId, Type type, [MaybeNullWhen(false)] out IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter> majorRec)
        {
            if (editorId.IsNullOrWhitespace())
            {
                majorRec = default;
                return false;
            }
            var cache = GetContextCache(
                type,
                sourceMod: _sourceMod,
                linkCache: this,
                keyGetter: m =>
                {
                    var edid = m.EditorID;
                    return TryGet<string>.Create(successful: !string.IsNullOrWhiteSpace(edid), edid!);
                },
                typedContexts: _editorIdContexts);
            if (!cache.TryGetValue(editorId, out majorRec))
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
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter> ResolveContext(string editorId)
        {
            if (TryResolveContext<IMajorRecordCommon, IMajorRecordCommonGetter>(editorId, out var majorRec)) return majorRec;
            throw new KeyNotFoundException($"EditorID {editorId} could not be found.");
        }

        /// <inheritdoc />
        public IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter> ResolveContext(FormKey formKey, Type type)
        {
            if (TryResolveContext(formKey, type, out var commonRec)) return commonRec;
            throw new KeyNotFoundException($"FormKey {formKey} could not be found.");
        }

        /// <inheritdoc />
        public IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter> ResolveContext(string editorId, Type type)
        {
            if (TryResolveContext(editorId, type, out var commonRec)) return commonRec;
            throw new KeyNotFoundException($"EditorID {editorId} could not be found.");
        }

        /// <inheritdoc />
        public IModContext<TMod, TMajorSetter, TMajorGetter> ResolveContext<TMajorSetter, TMajorGetter>(FormKey formKey)
            where TMajorSetter : class, IMajorRecordCommon, TMajorGetter
            where TMajorGetter : class, IMajorRecordCommonGetter
        {
            if (TryResolveContext<TMajorSetter, TMajorGetter>(formKey, out var commonRec)) return commonRec;
            throw new KeyNotFoundException($"FormKey {formKey} could not be found.");
        }

        /// <inheritdoc />
        public IModContext<TMod, TMajorSetter, TMajorGetter> ResolveContext<TMajorSetter, TMajorGetter>(string editorId)
            where TMajorSetter : class, IMajorRecordCommon, TMajorGetter
            where TMajorGetter : class, IMajorRecordCommonGetter
        {
            if (TryResolveContext<TMajorSetter, TMajorGetter>(editorId, out var commonRec)) return commonRec;
            throw new KeyNotFoundException($"EditorID {editorId} could not be found.");
        }

        private static IReadOnlyCache<IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter>, K> GetContextCache<K>(
            Type type,
            TModGetter sourceMod,
            ILinkCache linkCache,
            Func<IMajorRecordCommonGetter, TryGet<K>> keyGetter,
            Dictionary<Type, IReadOnlyCache<IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter>, K>> typedContexts)
            where K : notnull
        {
            lock (typedContexts)
            {
                if (!typedContexts.TryGetValue(type, out var cache))
                {
                    if (type.Equals(typeof(IMajorRecordCommon))
                        || type.Equals(typeof(IMajorRecordCommonGetter)))
                    {
                        cache = ConstructContextCache(type, sourceMod, linkCache, keyGetter);
                        typedContexts[typeof(IMajorRecordCommon)] = cache;
                        typedContexts[typeof(IMajorRecordCommonGetter)] = cache;
                    }
                    else if (LoquiRegistration.TryGetRegister(type, out var registration))
                    {
                        cache = ConstructContextCache(type, sourceMod, linkCache, keyGetter);
                        typedContexts[registration.ClassType] = cache;
                        typedContexts[registration.GetterType] = cache;
                        typedContexts[registration.SetterType] = cache;
                        if (registration.InternalGetterType != null)
                        {
                            typedContexts[registration.InternalGetterType] = cache;
                        }
                        if (registration.InternalSetterType != null)
                        {
                            typedContexts[registration.InternalSetterType] = cache;
                        }
                    }
                    else
                    {
                        var interfaceMappings = LinkInterfaceMapping.InterfaceToObjectTypes(sourceMod.GameRelease.ToCategory());
                        if (!interfaceMappings.TryGetValue(type, out var objs))
                        {
                            throw new ArgumentException($"A lookup was queried for an unregistered type: {type.Name}");
                        }
                        var majorRecords = new Cache<IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter>, K>(x => keyGetter(x.Record).Value);
                        foreach (var objType in objs)
                        {
                            majorRecords.Set(
                                GetContextCache(
                                    LoquiRegistration.GetRegister(objType).GetterType,
                                    sourceMod: sourceMod,
                                    linkCache: linkCache,
                                    keyGetter: keyGetter,
                                    typedContexts: typedContexts).Items);
                        }
                        typedContexts[type] = majorRecords;
                        cache = majorRecords;
                    }
                }
                return cache;
            }
        }

        private static IReadOnlyCache<IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter>, K> ConstructContextCache<K>(
            Type type,
            TModGetter sourceMod,
            ILinkCache linkCache,
            Func<IMajorRecordCommonGetter, TryGet<K>> keyGetter)
            where K : notnull
        {
            var cache = new Cache<IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter>, K>(x => keyGetter(x.Record).Value);
            // ToDo
            // Upgrade to call EnumerateGroups(), which will perform much better
            foreach (var majorRec in sourceMod.EnumerateMajorRecordContexts(linkCache, type))
            {
                var key = keyGetter(majorRec.Record);
                if (key.Failed) continue;
                cache.Set(majorRec);
            }
            return cache;
        }

        private IReadOnlyCache<IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter>, K> ConstructUntypedContextCache<K>(Func<IMajorRecordCommonGetter, TryGet<K>> keyGetter)
            where K : notnull
        {
            var majorRecords = new Cache<IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter>, K>(x => keyGetter(x.Record).Value);
            foreach (var majorRec in this._sourceMod.EnumerateMajorRecordContexts<IMajorRecordCommon, IMajorRecordCommonGetter>(this))
            {
                var key = keyGetter(majorRec.Record);
                if (key.Failed) continue;
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
        public IEnumerable<IModContext<TMod, TMajorSetter, TMajorGetter>> ResolveAllContexts<TMajorSetter, TMajorGetter>(string editorId)
            where TMajorSetter : class, IMajorRecordCommon, TMajorGetter
            where TMajorGetter : class, IMajorRecordCommonGetter
        {
            if (TryResolveContext<TMajorSetter, TMajorGetter>(editorId, out var rec))
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
        public IEnumerable<IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter>> ResolveAllContexts(string editorId, Type type)
        {
            if (TryResolveContext(editorId, type, out var rec))
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

        /// <inheritdoc />
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public IEnumerable<IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter>> ResolveAllContexts(string editorId)
        {
            if (TryResolveContext(editorId, out var rec))
            {
                yield return rec;
            }
        }
    }
}
