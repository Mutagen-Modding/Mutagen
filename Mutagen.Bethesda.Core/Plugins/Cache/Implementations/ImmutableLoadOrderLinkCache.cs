using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using Loqui;
using Mutagen.Bethesda.Plugins.Cache.Implementations.Internal;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Records.Internals;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Cache.Implementations
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
    public class ImmutableLoadOrderLinkCache : ILinkCache
    {
        internal readonly bool _hasAny;
        protected readonly GameCategory _gameCategory;
        internal bool _simple;

        private readonly IReadOnlyList<IModGetter> _listedOrder;
        private readonly IReadOnlyList<IModGetter> _priorityOrder;
        private readonly ImmutableLoadOrderLinkCacheCategory<FormKey> _formKeyCache;
        private readonly ImmutableLoadOrderLinkCacheCategory<string> _editorIdCache;
        internal readonly IReadOnlyDictionary<Type, Type[]> _linkInterfaces;
        private readonly IReadOnlyDictionary<ModKey, ILinkCache> _modsByKey;

        /// <inheritdoc />
        public IReadOnlyList<IModGetter> ListedOrder => _listedOrder;

        /// <inheritdoc />
        public IReadOnlyList<IModGetter> PriorityOrder => _priorityOrder;

        public ImmutableLoadOrderLinkCache(IEnumerable<IModGetter> loadOrder, GameCategory? gameCategory, LinkCachePreferences? prefs)
        {
            prefs ??= LinkCachePreferences.Default;
            this._listedOrder = loadOrder.ToList();
            this._priorityOrder = _listedOrder.Reverse().ToList();
            var firstMod = _listedOrder.FirstOrDefault();
            this._hasAny = firstMod != null;
            this._simple = prefs is LinkCachePreferenceOnlyIdentifiers;
            this._gameCategory = gameCategory ?? firstMod?.GameRelease.ToCategory() ?? throw new ArgumentException($"Could not get {nameof(GameCategory)} via generic type or first mod");
            this._linkInterfaces = LinkInterfaceMapping.InterfaceToObjectTypes(_gameCategory);
            this._formKeyCache = new ImmutableLoadOrderLinkCacheCategory<FormKey>(
                this,
                m => TryGet<FormKey>.Succeed(m.FormKey),
                f => f.IsNull);
            this._editorIdCache = new ImmutableLoadOrderLinkCacheCategory<string>(
                this,
                m =>
                {
                    var edid = m.EditorID;
                    return TryGet<string>.Create(successful: !string.IsNullOrWhiteSpace(edid), edid!);
                },
                e => e.IsNullOrWhitespace());
            
            var modsByKey = new Dictionary<ModKey, ILinkCache>();
            foreach (var modGetter in _listedOrder) 
            {
                try
                {
                    modsByKey.Add(modGetter.ModKey, modGetter.ToUntypedImmutableLinkCache(prefs));
                }
                catch (Exception e)
                {
                    throw new ArgumentException(
                        $"Mods with duplicate ModKeys were passed into the Link Cache: {modGetter.ModKey}");
                }
            }

            _modsByKey = modsByKey;
        }

        internal static bool ShouldStopQuery<K, T>(ModKey? modKey, int modCount, DepthCache<K, T> cache)
            where K : notnull
        {
            if (cache.Depth >= modCount)
            {
                cache.Done = true;
                return true;
            }

            // If we're going deeper than the originating mod of the target FormKey, we can stop
            if (modKey != null && cache.PassedMods.Contains(modKey.Value))
            {
                return true;
            }

            return false;
        }

        /// <inheritdoc />
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public bool TryResolve(FormKey formKey, [MaybeNullWhen(false)] out IMajorRecordCommonGetter majorRec, ResolveTarget target = ResolveTarget.Winner)
        {
            return TryResolve<IMajorRecordCommonGetter>(formKey, out majorRec, target);
        }

        /// <inheritdoc />
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public bool TryResolve(string editorId, [MaybeNullWhen(false)] out IMajorRecordCommonGetter majorRec)
        {
            return TryResolve<IMajorRecordCommonGetter>(editorId, out majorRec);
        }

        /// <inheritdoc />
        public bool TryResolve<TMajor>(FormKey formKey, [MaybeNullWhen(false)] out TMajor majorRec, ResolveTarget target = ResolveTarget.Winner)
            where TMajor : class, IMajorRecordCommonGetter
        {
            if (!TryResolve(formKey, typeof(TMajor), out var commonRec, target))
            {
                majorRec = default;
                return false;
            }

            majorRec = commonRec as TMajor;
            return majorRec != null;
        }

        /// <inheritdoc />
        public bool TryResolve<TMajor>(string editorId, [MaybeNullWhen(false)] out TMajor majorRec)
            where TMajor : class, IMajorRecordCommonGetter
        {
            if (!TryResolve(editorId, typeof(TMajor), out var commonRec))
            {
                majorRec = default;
                return false;
            }

            majorRec = commonRec as TMajor;
            return majorRec != null;
        }

        /// <inheritdoc />
        public bool TryResolve(FormKey formKey, Type type, [MaybeNullWhen(false)] out IMajorRecordCommonGetter majorRec, ResolveTarget target = ResolveTarget.Winner)
        {
            if (target == ResolveTarget.Origin)
            {
                if (!_modsByKey.TryGetValue(formKey.ModKey, out var origMod))
                {
                    majorRec = default;
                    return false;
                }

                return origMod.TryResolve(formKey, type, out majorRec);
            }
            
            if (_formKeyCache.TryResolve(formKey, formKey.ModKey, type, out var item))
            {
                majorRec = item.Record;
                return true;
            }
            
            majorRec = default;
            return false;
        }

        /// <inheritdoc />
        public bool TryResolve(string editorId, Type type, [MaybeNullWhen(false)] out IMajorRecordCommonGetter majorRec)
        {
            if (_editorIdCache.TryResolve(editorId, default(ModKey?), type, out var item))
            {
                majorRec = item.Record;
                return true;
            }
            majorRec = default;
            return false;
        }

        /// <inheritdoc />
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public IMajorRecordCommonGetter Resolve(FormKey formKey, ResolveTarget target = ResolveTarget.Winner)
        {
            if (TryResolve<IMajorRecordCommonGetter>(formKey, out var commonRec, target)) return commonRec;
            throw new MissingRecordException(formKey, typeof(IMajorRecordCommonGetter));
        }

        /// <inheritdoc />
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public IMajorRecordCommonGetter Resolve(string editorId)
        {
            if (TryResolve<IMajorRecordCommonGetter>(editorId, out var commonRec)) return commonRec;
            throw new MissingRecordException(editorId, typeof(IMajorRecordCommonGetter));
        }

        /// <inheritdoc />
        public IMajorRecordCommonGetter Resolve(FormKey formKey, Type type, ResolveTarget target = ResolveTarget.Winner)
        {
            if (TryResolve(formKey, type, out var commonRec, target)) return commonRec;
            throw new MissingRecordException(formKey, type);
        }

        /// <inheritdoc />
        public IMajorRecordCommonGetter Resolve(string editorId, Type type)
        {
            if (TryResolve(editorId, type, out var commonRec)) return commonRec;
            throw new MissingRecordException(editorId, type);
        }

        /// <inheritdoc />
        public TMajor Resolve<TMajor>(FormKey formKey, ResolveTarget target = ResolveTarget.Winner)
            where TMajor : class, IMajorRecordCommonGetter
        {
            if (TryResolve<TMajor>(formKey, out var commonRec, target)) return commonRec;
            throw new MissingRecordException(formKey, typeof(TMajor));
        }

        /// <inheritdoc />
        public TMajor Resolve<TMajor>(string editorId)
            where TMajor : class, IMajorRecordCommonGetter
        {
            if (TryResolve<TMajor>(editorId, out var commonRec)) return commonRec;
            throw new MissingRecordException(editorId, typeof(TMajor));
        }

        /// <inheritdoc />
        public IEnumerable<TMajor> ResolveAll<TMajor>(FormKey formKey, ResolveTarget target = ResolveTarget.Winner)
            where TMajor : class, IMajorRecordCommonGetter
        {
            return ResolveAll(formKey, typeof(TMajor), target).Cast<TMajor>();
        }

        /// <inheritdoc />
        public IEnumerable<IMajorRecordCommonGetter> ResolveAll(FormKey formKey, Type type, ResolveTarget target = ResolveTarget.Winner)
        {
            switch (target)
            {
                case ResolveTarget.Origin:
                    return _formKeyCache.ResolveAll(formKey, formKey.ModKey, type).Reverse().Select(i => i.Record);
                case ResolveTarget.Winner:
                    return _formKeyCache.ResolveAll(formKey, formKey.ModKey, type).Select(i => i.Record);
                default:
                    throw new NotImplementedException();
            }
        }

        /// <inheritdoc />
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public IEnumerable<IMajorRecordCommonGetter> ResolveAll(FormKey formKey, ResolveTarget target = ResolveTarget.Winner)
        {
            return ResolveAll(formKey, typeof(IMajorRecordCommonGetter), target);
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
        public bool TryResolve(FormKey formKey, IEnumerable<Type> types, [MaybeNullWhen(false)] out IMajorRecordCommonGetter majorRec, ResolveTarget target = ResolveTarget.Winner)
        {
            foreach (var type in types)
            {
                if (TryResolve(formKey, type, out majorRec, target))
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
        public IMajorRecordCommonGetter Resolve(FormKey formKey, IEnumerable<Type> types, ResolveTarget target = ResolveTarget.Winner)
        {
            if (TryResolve(formKey, types, out var commonRec, target)) return commonRec;
            throw new MissingRecordException(formKey, types.ToArray());
        }

        /// <inheritdoc />
        public IMajorRecordCommonGetter Resolve(string editorId, IEnumerable<Type> types)
        {
            if (TryResolve(editorId, types, out var commonRec)) return commonRec;
            throw new MissingRecordException(editorId, types.ToArray());
        }

        /// <inheritdoc />
        public bool TryResolveIdentifier(FormKey formKey, [MaybeNullWhen(false)] out string? editorId, ResolveTarget target = ResolveTarget.Winner)
        {
            if (target == ResolveTarget.Origin)
            {
                if (!_modsByKey.TryGetValue(formKey.ModKey, out var origMod))
                {
                    editorId = default;
                    return false;
                }

                return origMod.TryResolveIdentifier(formKey, out editorId);
            }

            if (_formKeyCache.TryResolve(formKey, formKey.ModKey, typeof(IMajorRecordCommonGetter), out var rec))
            {
                editorId = rec.EditorID;
                return true;
            }

            editorId = default;
            return false;
        }

        /// <inheritdoc />
        public bool TryResolveIdentifier(string editorId, [MaybeNullWhen(false)] out FormKey formKey)
        {
            if (_editorIdCache.TryResolve(editorId, default, typeof(IMajorRecordCommonGetter), out var rec))
            {
                formKey = rec.FormKey;
                return true;
            }

            formKey = default;
            return false;
        }

        /// <inheritdoc />
        public bool TryResolveIdentifier(FormKey formKey, Type type, [MaybeNullWhen(false)] out string? editorId, ResolveTarget target = ResolveTarget.Winner)
        {
            if (target == ResolveTarget.Origin)
            {
                if (!_modsByKey.TryGetValue(formKey.ModKey, out var origMod))
                {
                    editorId = default;
                    return false;
                }

                return origMod.TryResolveIdentifier(formKey, type, out editorId);
            }

            if (_formKeyCache.TryResolve(formKey, formKey.ModKey, type, out var rec))
            {
                editorId = rec.EditorID;
                return true;
            }

            editorId = default;
            return false;
        }

        /// <inheritdoc />
        public bool TryResolveIdentifier(string editorId, Type type, [MaybeNullWhen(false)] out FormKey formKey)
        {
            if (_editorIdCache.TryResolve(editorId, default, type, out var rec))
            {
                formKey = rec.FormKey;
                return true;
            }

            formKey = default;
            return false;
        }

        /// <inheritdoc />
        public bool TryResolveIdentifier<TMajor>(FormKey formKey, [MaybeNullWhen(false)] out string? editorId, ResolveTarget target = ResolveTarget.Winner)
            where TMajor : class, IMajorRecordCommonGetter
        {
            if (target == ResolveTarget.Origin)
            {
                if (!_modsByKey.TryGetValue(formKey.ModKey, out var origMod))
                {
                    editorId = default;
                    return false;
                }

                return origMod.TryResolveIdentifier<TMajor>(formKey, out editorId);
            }

            if (_formKeyCache.TryResolve(formKey, formKey.ModKey, typeof(TMajor), out var rec))
            {
                editorId = rec.EditorID;
                return true;
            }

            editorId = default;
            return false;
        }

        /// <inheritdoc />
        public bool TryResolveIdentifier<TMajor>(string editorId, [MaybeNullWhen(false)] out FormKey formKey)
            where TMajor : class, IMajorRecordCommonGetter
        {
            if (_editorIdCache.TryResolve(editorId, default, typeof(TMajor), out var rec))
            {
                formKey = rec.FormKey;
                return true;
            }

            formKey = default;
            return false;
        }

        /// <inheritdoc />
        public bool TryResolveIdentifier(FormKey formKey, [MaybeNullWhen(false)] out string? editorId, params Type[] types)
        {
            return TryResolveIdentifier(formKey, (IEnumerable<Type>)types, out editorId);
        }

        /// <inheritdoc />
        public bool TryResolveIdentifier(string editorId, [MaybeNullWhen(false)] out FormKey formKey, params Type[] types)
        {
            return TryResolveIdentifier(editorId, (IEnumerable<Type>)types, out formKey);
        }

        /// <inheritdoc />
        public bool TryResolveIdentifier(FormKey formKey, IEnumerable<Type> types, [MaybeNullWhen(false)] out string? editorId, ResolveTarget target = ResolveTarget.Winner)
        {
            if (target == ResolveTarget.Origin)
            {
                if (!_modsByKey.TryGetValue(formKey.ModKey, out var origMod))
                {
                    editorId = default;
                    return false;
                }

                foreach (var type in types)
                {
                    if (origMod.TryResolveIdentifier(formKey, out editorId, type))
                    {
                        return true;
                    }
                }
            }
            else
            {
                foreach (var type in types)
                {
                    if (_formKeyCache.TryResolve(formKey, formKey.ModKey, type, out var rec))
                    {
                        editorId = rec.EditorID;
                        return true;
                    }
                }
            }

            editorId = default;
            return false;
        }

        /// <inheritdoc />
        public bool TryResolveIdentifier(string editorId, IEnumerable<Type> types, [MaybeNullWhen(false)] out FormKey formKey)
        {
            foreach (var type in types)
            {
                if (_editorIdCache.TryResolve(editorId, default, type, out var rec))
                {
                    formKey = rec.FormKey;
                    return true;
                }
            }

            formKey = default;
            return false;
        }

        /// <inheritdoc />
        public IEnumerable<IMajorRecordIdentifier> AllIdentifiers(Type type, CancellationToken? cancel = null)
        {
            return _formKeyCache.AllIdentifiers(type, cancel);
        }

        /// <inheritdoc />
        public IEnumerable<IMajorRecordIdentifier> AllIdentifiers<TMajor>(CancellationToken? cancel = null)
            where TMajor : class, IMajorRecordCommonGetter
        {
            return _formKeyCache.AllIdentifiers(typeof(TMajor), cancel);
        }

        /// <inheritdoc />
        public IEnumerable<IMajorRecordIdentifier> AllIdentifiers(params Type[] types)
        {
            return AllIdentifiers((IEnumerable<Type>)types, CancellationToken.None);
        }

        /// <inheritdoc />
        public IEnumerable<IMajorRecordIdentifier> AllIdentifiers(IEnumerable<Type> types, CancellationToken? cancel = null)
        {
            return types.SelectMany(type => AllIdentifiers(type, cancel))
                .Distinct(x => x.FormKey);
        }

        /// <inheritdoc />
        public void Dispose()
        {
        }

        /// <inheritdoc />
        public void Warmup(Type type)
        {
            _formKeyCache.Warmup(type);
        }

        /// <inheritdoc />
        public void Warmup<TMajor>()
        {
            _formKeyCache.Warmup(typeof(TMajor));
        }

        /// <inheritdoc />
        public void Warmup(params Type[] types)
        {
            Warmup((IEnumerable<Type>)types);
        }

        /// <inheritdoc />
        public void Warmup(IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                _formKeyCache.Warmup(type);
            }
        }
    }

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
    /// <typeparam name="TMod">Mod setter type</typeparam>
    /// <typeparam name="TModGetter">Mod getter type</typeparam>
    public class ImmutableLoadOrderLinkCache<TMod, TModGetter> : ImmutableLoadOrderLinkCache, ILinkCache<TMod, TModGetter>
        where TMod : class, IContextMod<TMod, TModGetter>, TModGetter
        where TModGetter : class, IContextGetterMod<TMod, TModGetter>
    {
        public static readonly ImmutableLoadOrderLinkCache<TMod, TModGetter> Empty = new(Enumerable.Empty<TModGetter>(), LinkCachePreferences.Default);

        private readonly ImmutableLoadOrderLinkCacheContextCategory<TMod, TModGetter, FormKey> _formKeyContextCache;
        private readonly ImmutableLoadOrderLinkCacheContextCategory<TMod, TModGetter, string> _editorIdContextCache;

        internal readonly IReadOnlyList<TModGetter> _listedOrder;
        private readonly IReadOnlyDictionary<ModKey, ILinkCache<TMod, TModGetter>> _modsByKey;

        /// <summary>
        /// Constructs a LoadOrderLinkCache around a target load order
        /// </summary>
        /// <param name="loadOrder">LoadOrder to resolve against when linking</param>
        public ImmutableLoadOrderLinkCache(IEnumerable<TModGetter> loadOrder, LinkCachePreferences prefs)
            : base(loadOrder, GameCategoryHelper.TryFromModType<TModGetter>(), prefs)
        {
            this._listedOrder = loadOrder.ToList();
            this._formKeyContextCache = new ImmutableLoadOrderLinkCacheContextCategory<TMod, TModGetter, FormKey>(
                this,
                m => TryGet<FormKey>.Succeed(m.FormKey),
                f => f.IsNull);
            this._editorIdContextCache = new ImmutableLoadOrderLinkCacheContextCategory<TMod, TModGetter, string>(
                this,
                m =>
                {
                    var edid = m.EditorID;
                    return TryGet<string>.Create(successful: !string.IsNullOrWhiteSpace(edid), edid!);
                },
                e => e.IsNullOrWhitespace());
            
            var modsByKey = new Dictionary<ModKey, ILinkCache<TMod, TModGetter>>();
            foreach (var modGetter in _listedOrder) 
            {
                try
                {
                    modsByKey.Add(modGetter.ModKey, modGetter.ToImmutableLinkCache<TMod, TModGetter>(prefs));
                }
                catch (Exception e)
                {
                    throw new ArgumentException(
                        $"Mods with duplicate ModKeys were passed into the Link Cache: {modGetter.ModKey}");
                }
            }

            _modsByKey = modsByKey;
        }

        /// <inheritdoc />
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public bool TryResolveContext(FormKey formKey, [MaybeNullWhen(false)] out IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter> majorRec, ResolveTarget target = ResolveTarget.Winner)
        {
            return TryResolveContext<IMajorRecordCommon, IMajorRecordCommonGetter>(formKey, out majorRec, target);
        }

        /// <inheritdoc />
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public bool TryResolveContext(string editorId, [MaybeNullWhen(false)] out IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter> majorRec)
        {
            return TryResolveContext<IMajorRecordCommon, IMajorRecordCommonGetter>(editorId, out majorRec);
        }

        /// <inheritdoc />
        public bool TryResolveContext<TMajor, TMajorGetter>(FormKey formKey, [MaybeNullWhen(false)] out IModContext<TMod, TModGetter, TMajor, TMajorGetter> majorRec, ResolveTarget target = ResolveTarget.Winner)
            where TMajor : class, IMajorRecordCommon, TMajorGetter
            where TMajorGetter : class, IMajorRecordCommonGetter
        {
            if (!TryResolveContext(formKey, typeof(TMajorGetter), out var commonRec, target)
                || !(commonRec.Record is TMajorGetter))
            {
                majorRec = default;
                return false;
            }

            majorRec = commonRec.AsType<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter, TMajor, TMajorGetter>();
            return majorRec != null;
        }

        /// <inheritdoc />
        public bool TryResolveContext<TMajor, TMajorGetter>(string editorId, [MaybeNullWhen(false)] out IModContext<TMod, TModGetter, TMajor, TMajorGetter> majorRec)
            where TMajor : class, IMajorRecordCommon, TMajorGetter
            where TMajorGetter : class, IMajorRecordCommonGetter
        {
            if (!TryResolveContext(editorId, typeof(TMajorGetter), out var commonRec)
                || !(commonRec.Record is TMajorGetter))
            {
                majorRec = default;
                return false;
            }

            majorRec = commonRec.AsType<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter, TMajor, TMajorGetter>();
            return majorRec != null;
        }

        /// <inheritdoc />
        public bool TryResolveContext(FormKey formKey, Type type, [MaybeNullWhen(false)] out IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter> majorRec, ResolveTarget target = ResolveTarget.Winner)
        {
            if (target == ResolveTarget.Origin)
            {
                if (!_modsByKey.TryGetValue(formKey.ModKey, out var origMod))
                {
                    majorRec = default;
                    return false;
                }

                return origMod.TryResolveContext(formKey, type, out majorRec);
            }
            
            return _formKeyContextCache.TryResolveContext(formKey, formKey.ModKey, type, out majorRec);
        }

        /// <inheritdoc />
        public bool TryResolveContext(string editorId, Type type, [MaybeNullWhen(false)] out IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter> majorRec)
        {
            if (!_hasAny || string.IsNullOrWhiteSpace(editorId))
            {
                majorRec = default;
                return false;
            }

            return _editorIdContextCache.TryResolveContext(editorId, default(ModKey?), type, out majorRec);
        }

        /// <inheritdoc />
        public IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter> ResolveContext(FormKey formKey, ResolveTarget target = ResolveTarget.Winner)
        {
            if (TryResolveContext<IMajorRecordCommon, IMajorRecordCommonGetter>(formKey, out var commonRec, target)) return commonRec;
            throw new MissingRecordException(formKey, typeof(IMajorRecordCommonGetter));
        }

        /// <inheritdoc />
        public IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter> ResolveContext(string editorId)
        {
            if (TryResolveContext<IMajorRecordCommon, IMajorRecordCommonGetter>(editorId, out var commonRec)) return commonRec;
            throw new MissingRecordException(editorId, typeof(IMajorRecordCommonGetter));
        }

        /// <inheritdoc />
        public IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter> ResolveContext(FormKey formKey, Type type, ResolveTarget target = ResolveTarget.Winner)
        {
            if (TryResolveContext(formKey, type, out var commonRec, target)) return commonRec;
            throw new MissingRecordException(formKey, type);
        }

        /// <inheritdoc />
        public IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter> ResolveContext(string editorId, Type type)
        {
            if (TryResolveContext(editorId, type, out var commonRec)) return commonRec;
            throw new MissingRecordException(editorId, type);
        }

        /// <inheritdoc />
        public IModContext<TMod, TModGetter, TMajor, TMajorGetter> ResolveContext<TMajor, TMajorGetter>(FormKey formKey, ResolveTarget target = ResolveTarget.Winner)
            where TMajor : class, IMajorRecordCommon, TMajorGetter
            where TMajorGetter : class, IMajorRecordCommonGetter
        {
            if (TryResolveContext<TMajor, TMajorGetter>(formKey, out var commonRec, target)) return commonRec;
            throw new MissingRecordException(formKey, typeof(TMajorGetter));
        }

        /// <inheritdoc />
        public IModContext<TMod, TModGetter, TMajor, TMajorGetter> ResolveContext<TMajor, TMajorGetter>(string editorId)
            where TMajor : class, IMajorRecordCommon, TMajorGetter
            where TMajorGetter : class, IMajorRecordCommonGetter
        {
            if (TryResolveContext<TMajor, TMajorGetter>(editorId, out var commonRec)) return commonRec;
            throw new MissingRecordException(editorId, typeof(TMajorGetter));
        }

        /// <inheritdoc />
        public IEnumerable<IModContext<TMod, TModGetter, TMajor, TMajorGetter>> ResolveAllContexts<TMajor, TMajorGetter>(FormKey formKey, ResolveTarget target = ResolveTarget.Winner)
            where TMajor : class, IMajorRecordCommon, TMajorGetter
            where TMajorGetter : class, IMajorRecordCommonGetter
        {
            return ResolveAllContexts(formKey, typeof(TMajorGetter), target)
                .Select(c => c.AsType<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter, TMajor, TMajorGetter>());
        }

        /// <inheritdoc />
        public IEnumerable<IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter>> ResolveAllContexts(FormKey formKey, Type type, ResolveTarget target = ResolveTarget.Winner)
        {
            // Break early if no content
            if (!_hasAny || formKey.IsNull)
            {
                return Enumerable.Empty<IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter>>();
            }

            switch (target)
            {
                case ResolveTarget.Origin:
                    return _formKeyContextCache.ResolveAllContexts(formKey, formKey.ModKey, type).Reverse();
                case ResolveTarget.Winner:
                    return _formKeyContextCache.ResolveAllContexts(formKey, formKey.ModKey, type);
                default:
                    throw new NotImplementedException();
            }
        }

        /// <inheritdoc />
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public IEnumerable<IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter>> ResolveAllContexts(FormKey formKey, ResolveTarget target = ResolveTarget.Winner)
        {
            return ResolveAllContexts(formKey, typeof(IMajorRecordCommonGetter), target);
        }
    }

}
