using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using Mutagen.Bethesda.Plugins.Cache.Internals.Implementations.Internal;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Cache.Internals.Implementations
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
        private readonly InternalImmutableLoadOrderLinkCache _cache;
        private readonly IImmutableLoadOrderLinkCacheSimpleContextCategory<FormKey> _formKeyContexts;
        private readonly IImmutableLoadOrderLinkCacheSimpleContextCategory<string> _editorIdContexts;
        private readonly IReadOnlyDictionary<ModKey, ILinkCache> _modsByKey;
        private readonly bool _hasAny;

        public ImmutableLoadOrderLinkCache(
            IEnumerable<IModGetter> loadOrder, 
            GameCategory? gameCategory, 
            LinkCachePreferences? prefs)
        {
            var loadOrderArr = loadOrder.ToArray();
            var firstMod = loadOrderArr.FirstOrDefault();
            _hasAny = firstMod != null;
            var simple = prefs is LinkCachePreferenceOnlyIdentifiers;
            gameCategory ??= firstMod?.GameRelease.ToCategory() ?? throw new ArgumentException($"Could not get {nameof(GameCategory)} via generic type or first mod");
            _cache = new InternalImmutableLoadOrderLinkCache(
                loadOrderArr,
                gameCategory.Value,
                hasAny: _hasAny,
                simple: simple,
                prefs);
            _formKeyContexts = new ImmutableLoadOrderLinkCacheSimpleContextCategory<FormKey>(
                gameCategory.Value,
                simple: simple,
                hasAny: _hasAny,
                this,
                loadOrderArr,
                m => TryGet<FormKey>.Succeed(m.FormKey),
                f => f.IsNull);
            _editorIdContexts = new ImmutableLoadOrderLinkCacheSimpleContextCategory<string>(
                gameCategory.Value,
                simple: simple,
                hasAny: _hasAny,
                this,
                loadOrderArr,
                m =>
                {
                    var edid = m.EditorID;
                    return TryGet<string>.Create(successful: !string.IsNullOrWhiteSpace(edid), edid!);
                },
                e => e.IsNullOrWhitespace());
            
            var modsByKey = new Dictionary<ModKey, ILinkCache>();
            foreach (var modGetter in loadOrderArr) 
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

        public void Dispose()
        {
            _cache.Dispose();
        }

        /// <inheritdoc />
        public bool TryResolveIdentifier(FormKey formKey, out string? editorId, ResolveTarget target = ResolveTarget.Winner)
        {
            return _cache.TryResolveIdentifier(formKey, out editorId, target);
        }

        /// <inheritdoc />
        public bool TryResolveIdentifier(string editorId, out FormKey formKey)
        {
            return _cache.TryResolveIdentifier(editorId, out formKey);
        }

        /// <inheritdoc />
        public bool TryResolveIdentifier(FormKey formKey, Type type, out string? editorId,
            ResolveTarget target = ResolveTarget.Winner)
        {
            return _cache.TryResolveIdentifier(formKey, type, out editorId, target);
        }

        /// <inheritdoc />
        public bool TryResolveIdentifier(string editorId, Type type, out FormKey formKey)
        {
            return _cache.TryResolveIdentifier(editorId, type, out formKey);
        }

        /// <inheritdoc />
        public bool TryResolveIdentifier<TMajor>(FormKey formKey, out string? editorId, ResolveTarget target = ResolveTarget.Winner)
            where TMajor : class, IMajorRecordCommonGetter
        {
            return _cache.TryResolveIdentifier<TMajor>(formKey, out editorId, target);
        }

        /// <inheritdoc />
        public bool TryResolveIdentifier<TMajor>(string editorId, out FormKey formKey) 
            where TMajor : class, IMajorRecordCommonGetter
        {
            return _cache.TryResolveIdentifier<TMajor>(editorId, out formKey);
        }

        /// <inheritdoc />
        public bool TryResolveIdentifier(FormKey formKey, out string? editorId, params Type[] types)
        {
            return _cache.TryResolveIdentifier(formKey, out editorId, types);
        }

        /// <inheritdoc />
        public bool TryResolveIdentifier(string editorId, out FormKey formKey, params Type[] types)
        {
            return _cache.TryResolveIdentifier(editorId, out formKey, types);
        }

        /// <inheritdoc />
        public bool TryResolveIdentifier(FormKey formKey, IEnumerable<Type> types, out string? editorId,
            ResolveTarget target = ResolveTarget.Winner)
        {
            return _cache.TryResolveIdentifier(formKey, types, out editorId, target);
        }

        /// <inheritdoc />
        public bool TryResolveIdentifier(string editorId, IEnumerable<Type> types, out FormKey formKey)
        {
            return _cache.TryResolveIdentifier(editorId, types, out formKey);
        }

        /// <inheritdoc />
        public IEnumerable<IMajorRecordIdentifier> AllIdentifiers(Type type, CancellationToken? cancel = null)
        {
            return _cache.AllIdentifiers(type, cancel);
        }

        /// <inheritdoc />
        public IEnumerable<IMajorRecordIdentifier> AllIdentifiers<TMajor>(CancellationToken? cancel = null) where TMajor : class, IMajorRecordCommonGetter
        {
            return _cache.AllIdentifiers<TMajor>(cancel);
        }

        /// <inheritdoc />
        public IEnumerable<IMajorRecordIdentifier> AllIdentifiers(IEnumerable<Type> types, CancellationToken? cancel = null)
        {
            return _cache.AllIdentifiers(types, cancel);
        }

        /// <inheritdoc />
        public IEnumerable<IMajorRecordIdentifier> AllIdentifiers(params Type[] types)
        {
            return _cache.AllIdentifiers(types);
        }

        /// <inheritdoc />
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public bool TryResolve(FormKey formKey, [MaybeNullWhen(false)] out IMajorRecordCommonGetter majorRec, ResolveTarget target = ResolveTarget.Winner)
        {
            return _cache.TryResolve(formKey, out majorRec, target);
        }

        /// <inheritdoc />
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public bool TryResolve(string editorId, [MaybeNullWhen(false)] out IMajorRecordCommonGetter majorRec)
        {
            return _cache.TryResolve(editorId, out majorRec);
        }

        /// <inheritdoc />
        public bool TryResolve<TMajor>(FormKey formKey, [MaybeNullWhen(false)] out TMajor majorRec, ResolveTarget target = ResolveTarget.Winner) 
            where TMajor : class, IMajorRecordCommonGetter
        {
            return _cache.TryResolve<TMajor>(formKey, out majorRec, target);
        }

        /// <inheritdoc />
        public bool TryResolve<TMajor>(string editorId, [MaybeNullWhen(false)] out TMajor majorRec) 
            where TMajor : class, IMajorRecordCommonGetter
        {
            return _cache.TryResolve<TMajor>(editorId, out majorRec);
        }

        /// <inheritdoc />
        public bool TryResolve(FormKey formKey, Type type, [MaybeNullWhen(false)] out IMajorRecordCommonGetter majorRec,
            ResolveTarget target = ResolveTarget.Winner)
        {
            return _cache.TryResolve(formKey, type, out majorRec, target);
        }

        /// <inheritdoc />
        public bool TryResolve(string editorId, Type type, [MaybeNullWhen(false)] out IMajorRecordCommonGetter majorRec)
        {
            return _cache.TryResolve(editorId, type, out majorRec);
        }

        /// <inheritdoc />
        public bool TryResolve(FormKey formKey, [MaybeNullWhen(false)] out IMajorRecordCommonGetter majorRec, params Type[] types)
        {
            return _cache.TryResolve(formKey, out majorRec, types);
        }

        /// <inheritdoc />
        public bool TryResolve(string editorId, [MaybeNullWhen(false)] out IMajorRecordCommonGetter majorRec, params Type[] types)
        {
            return _cache.TryResolve(editorId, out majorRec, types);
        }

        /// <inheritdoc />
        public bool TryResolve(FormKey formKey, IEnumerable<Type> types, [MaybeNullWhen(false)] out IMajorRecordCommonGetter majorRec,
            ResolveTarget target = ResolveTarget.Winner)
        {
            return _cache.TryResolve(formKey, types, out majorRec, target);
        }

        /// <inheritdoc />
        public bool TryResolve(string editorId, IEnumerable<Type> types, [MaybeNullWhen(false)] out IMajorRecordCommonGetter majorRec)
        {
            return _cache.TryResolve(editorId, types, out majorRec);
        }

        /// <inheritdoc />
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public IMajorRecordCommonGetter Resolve(FormKey formKey, ResolveTarget target = ResolveTarget.Winner)
        {
            return _cache.Resolve(formKey, target);
        }

        /// <inheritdoc />
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public IMajorRecordCommonGetter Resolve(string editorId)
        {
            return _cache.Resolve(editorId);
        }

        /// <inheritdoc />
        public IMajorRecordCommonGetter Resolve(FormKey formKey, Type type, ResolveTarget target = ResolveTarget.Winner)
        {
            return _cache.Resolve(formKey, type, target);
        }

        /// <inheritdoc />
        public IMajorRecordCommonGetter Resolve(string editorId, Type type)
        {
            return _cache.Resolve(editorId, type);
        }

        /// <inheritdoc />
        public IMajorRecordCommonGetter Resolve(FormKey formKey, params Type[] types)
        {
            return _cache.Resolve(formKey, types);
        }

        /// <inheritdoc />
        public IMajorRecordCommonGetter Resolve(string editorId, params Type[] types)
        {
            return _cache.Resolve(editorId, types);
        }

        /// <inheritdoc />
        public IMajorRecordCommonGetter Resolve(FormKey formKey, IEnumerable<Type> types, ResolveTarget target = ResolveTarget.Winner)
        {
            return _cache.Resolve(formKey, types, target);
        }

        /// <inheritdoc />
        public IMajorRecordCommonGetter Resolve(string editorId, IEnumerable<Type> types)
        {
            return _cache.Resolve(editorId, types);
        }

        /// <inheritdoc />
        public TMajor Resolve<TMajor>(FormKey formKey, ResolveTarget target = ResolveTarget.Winner) where TMajor : class, IMajorRecordCommonGetter
        {
            return _cache.Resolve<TMajor>(formKey, target);
        }

        /// <inheritdoc />
        public TMajor Resolve<TMajor>(string editorId) where TMajor : class, IMajorRecordCommonGetter
        {
            return _cache.Resolve<TMajor>(editorId);
        }

        /// <inheritdoc />
        public IEnumerable<TMajor> ResolveAll<TMajor>(FormKey formKey, ResolveTarget target = ResolveTarget.Winner) where TMajor : class, IMajorRecordCommonGetter
        {
            return _cache.ResolveAll<TMajor>(formKey, target);
        }

        /// <inheritdoc />
        public IEnumerable<IMajorRecordCommonGetter> ResolveAll(FormKey formKey, Type type, ResolveTarget target = ResolveTarget.Winner)
        {
            return _cache.ResolveAll(formKey, type, target);
        }

        /// <inheritdoc />
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public IEnumerable<IMajorRecordCommonGetter> ResolveAll(FormKey formKey, ResolveTarget target = ResolveTarget.Winner)
        {
            return _cache.ResolveAll(formKey, target);
        }

        /// <inheritdoc />
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public bool TryResolveSimpleContext(FormKey formKey, [MaybeNullWhen(false)] out IModContext<IMajorRecordCommonGetter> majorRec, ResolveTarget target = ResolveTarget.Winner)
        {
            if (target == ResolveTarget.Origin)
            {
                if (!_modsByKey.TryGetValue(formKey.ModKey, out var origMod))
                {
                    majorRec = default;
                    return false;
                }

                return origMod.TryResolveSimpleContext(formKey, out majorRec);
            }
            
            return _formKeyContexts.TryResolveSimpleContext(formKey, formKey.ModKey, typeof(IMajorRecordCommonGetter), out majorRec);
        }

        /// <inheritdoc />
        public bool TryResolveSimpleContext(string editorId, [MaybeNullWhen(false)] out IModContext<IMajorRecordCommonGetter> majorRec)
        {
            if (!_hasAny || string.IsNullOrWhiteSpace(editorId))
            {
                majorRec = default;
                return false;
            }
            
            return _editorIdContexts.TryResolveSimpleContext(editorId, default(ModKey?), typeof(IMajorRecordCommonGetter), out majorRec);
        }

        /// <inheritdoc />
        public bool TryResolveSimpleContext<TMajor>(FormKey formKey, [MaybeNullWhen(false)] out IModContext<TMajor> majorRec,
            ResolveTarget target = ResolveTarget.Winner) where TMajor : class, IMajorRecordCommonGetter
        {
            if (!TryResolveSimpleContext(formKey, typeof(TMajor), out var commonRec, target)
                || !(commonRec.Record is TMajor))
            {
                majorRec = default;
                return false;
            }

            majorRec = commonRec.AsType<IMajorRecordCommonGetter, TMajor>();
            return true;
        }

        /// <inheritdoc />
        public bool TryResolveSimpleContext<TMajor>(string editorId, [MaybeNullWhen(false)] out IModContext<TMajor> majorRec) where TMajor : class, IMajorRecordCommonGetter
        {
            if (!TryResolveSimpleContext(editorId, typeof(TMajor), out var commonRec)
                || !(commonRec.Record is TMajor))
            {
                majorRec = default;
                return false;
            }

            majorRec = commonRec.AsType<IMajorRecordCommonGetter, TMajor>();
            return true;
        }

        /// <inheritdoc />
        public bool TryResolveSimpleContext(FormKey formKey, Type type, [MaybeNullWhen(false)] out IModContext<IMajorRecordCommonGetter> majorRec,
            ResolveTarget target = ResolveTarget.Winner)
        {
            if (target == ResolveTarget.Origin)
            {
                if (!_modsByKey.TryGetValue(formKey.ModKey, out var origMod))
                {
                    majorRec = default;
                    return false;
                }

                return origMod.TryResolveSimpleContext(formKey, type, out majorRec);
            }
            
            return _formKeyContexts.TryResolveSimpleContext(formKey, formKey.ModKey, type, out majorRec);
        }

        /// <inheritdoc />
        public bool TryResolveSimpleContext(string editorId, Type type, [MaybeNullWhen(false)] out IModContext<IMajorRecordCommonGetter> majorRec)
        {
            if (!_hasAny || string.IsNullOrWhiteSpace(editorId))
            {
                majorRec = default;
                return false;
            }

            return _editorIdContexts.TryResolveSimpleContext(editorId, default(ModKey?), type, out majorRec);
        }

        /// <inheritdoc />
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public IModContext<IMajorRecordCommonGetter> ResolveSimpleContext(FormKey formKey, ResolveTarget target = ResolveTarget.Winner)
        {
            if (TryResolveSimpleContext(formKey, out var commonRec, target)) return commonRec;
            throw new MissingRecordException(formKey, typeof(IMajorRecordCommonGetter));
        }

        /// <inheritdoc />
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public IModContext<IMajorRecordCommonGetter> ResolveSimpleContext(string editorId)
        {
            if (TryResolveSimpleContext(editorId, out var commonRec)) return commonRec;
            throw new MissingRecordException(editorId, typeof(IMajorRecordCommonGetter));
        }

        /// <inheritdoc />
        public IModContext<IMajorRecordCommonGetter> ResolveSimpleContext(FormKey formKey, Type type, ResolveTarget target = ResolveTarget.Winner)
        {
            if (TryResolveSimpleContext(formKey, type, out var commonRec, target)) return commonRec;
            throw new MissingRecordException(formKey, type);
        }

        /// <inheritdoc />
        public IModContext<IMajorRecordCommonGetter> ResolveSimpleContext(string editorId, Type type)
        {
            if (TryResolveSimpleContext(editorId, type, out var commonRec)) return commonRec;
            throw new MissingRecordException(editorId, type);
        }

        /// <inheritdoc />
        public IModContext<TMajor> ResolveSimpleContext<TMajor>(FormKey formKey, ResolveTarget target = ResolveTarget.Winner) where TMajor : class, IMajorRecordCommonGetter
        {
            if (TryResolveSimpleContext<TMajor>(formKey, out var commonRec, target)) return commonRec;
            throw new MissingRecordException(formKey, typeof(TMajor));
        }

        /// <inheritdoc />
        public IModContext<TMajor> ResolveSimpleContext<TMajor>(string editorId) where TMajor : class, IMajorRecordCommonGetter
        {
            if (TryResolveSimpleContext<TMajor>(editorId, out var commonRec)) return commonRec;
            throw new MissingRecordException(editorId, typeof(TMajor));
        }

        /// <inheritdoc />
        public IEnumerable<IModContext<TMajor>> ResolveAllSimpleContexts<TMajor>(FormKey formKey, ResolveTarget target = ResolveTarget.Winner) where TMajor : class, IMajorRecordCommonGetter
        {
            return ResolveAllSimpleContexts(formKey, typeof(TMajor), target)
                .Select(c => c.AsType<IMajorRecordCommonGetter, TMajor>());
        }

        /// <inheritdoc />
        public IEnumerable<IModContext<IMajorRecordCommonGetter>> ResolveAllSimpleContexts(FormKey formKey, Type type, ResolveTarget target = ResolveTarget.Winner)
        {
            // Break early if no content
            if (!_hasAny || formKey.IsNull)
            {
                return Enumerable.Empty<IModContext<IMajorRecordCommonGetter>>();
            }

            switch (target)
            {
                case ResolveTarget.Origin:
                    return _formKeyContexts.ResolveAllSimpleContexts(formKey, formKey.ModKey, type).Reverse();
                case ResolveTarget.Winner:
                    return _formKeyContexts.ResolveAllSimpleContexts(formKey, formKey.ModKey, type);
                default:
                    throw new NotImplementedException();
            }
        }

        /// <inheritdoc />
        public IEnumerable<IModContext<IMajorRecordCommonGetter>> ResolveAllSimpleContexts(FormKey formKey, ResolveTarget target = ResolveTarget.Winner)
        {
            return ResolveAllSimpleContexts(formKey, typeof(IMajorRecordCommonGetter), target);
        }

        /// <inheritdoc />
        public void Warmup(Type type)
        {
            _cache.Warmup(type);
        }

        /// <inheritdoc />
        public void Warmup<TMajor>()
        {
            _cache.Warmup<TMajor>();
        }

        /// <inheritdoc />
        public void Warmup(params Type[] types)
        {
            _cache.Warmup(types);
        }

        /// <inheritdoc />
        public void Warmup(IEnumerable<Type> types)
        {
            _cache.Warmup(types);
        }

        /// <inheritdoc />
        public IReadOnlyList<IModGetter> ListedOrder => _cache.ListedOrder;

        /// <inheritdoc />
        public IReadOnlyList<IModGetter> PriorityOrder => _cache.PriorityOrder;
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
    public class ImmutableLoadOrderLinkCache<TMod, TModGetter> : ILinkCache<TMod, TModGetter>
        where TMod : class, IContextMod<TMod, TModGetter>, TModGetter
        where TModGetter : class, IContextGetterMod<TMod, TModGetter>
    {
        public static readonly ImmutableLoadOrderLinkCache<TMod, TModGetter> Empty = new(Enumerable.Empty<TModGetter>(), LinkCachePreferences.Default);

        private readonly bool _hasAny;
        private readonly InternalImmutableLoadOrderLinkCache _cache;
        private readonly IImmutableLoadOrderLinkCacheContextCategory<TMod, TModGetter, FormKey> _formKeyContextCache;
        private readonly IImmutableLoadOrderLinkCacheContextCategory<TMod, TModGetter, string> _editorIdContextCache;

        private readonly IReadOnlyDictionary<ModKey, ILinkCache<TMod, TModGetter>> _modsByKey;

        /// <summary>
        /// Constructs a LoadOrderLinkCache around a target load order
        /// </summary>
        /// <param name="loadOrder">LoadOrder to resolve against when linking</param>
        public ImmutableLoadOrderLinkCache(IEnumerable<TModGetter> loadOrder, LinkCachePreferences prefs)
        {
            var listedOrder = loadOrder.ToList();
            var simple = prefs is LinkCachePreferenceOnlyIdentifiers;
            var firstMod = listedOrder.FirstOrDefault();
            _hasAny = firstMod != null;
            var gameCategory = GameCategoryHelper.TryFromModType<TModGetter>() ?? firstMod?.GameRelease.ToCategory() ?? throw new ArgumentException($"Could not get {nameof(GameCategory)} via generic type or first mod");

            _cache = new InternalImmutableLoadOrderLinkCache(
                listedOrder,
                gameCategory,
                hasAny: _hasAny,
                simple: simple,
                prefs);
            
            _formKeyContextCache = new ImmutableLoadOrderLinkCacheContextCategory<TMod, TModGetter, FormKey>(
                category: gameCategory,
                simple: simple,
                hasAny: _hasAny,
                linkCache: this,
                listedOrder: listedOrder,
                m => TryGet<FormKey>.Succeed(m.FormKey),
                f => f.IsNull);
            _editorIdContextCache = new ImmutableLoadOrderLinkCacheContextCategory<TMod, TModGetter, string>(
                category: gameCategory,
                simple: simple,
                hasAny: _hasAny,
                linkCache: this,
                listedOrder: listedOrder,
                m =>
                {
                    var edid = m.EditorID;
                    return TryGet<string>.Create(successful: !string.IsNullOrWhiteSpace(edid), edid!);
                },
                e => e.IsNullOrWhitespace());
            
            var modsByKey = new Dictionary<ModKey, ILinkCache<TMod, TModGetter>>();
            foreach (var modGetter in listedOrder) 
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

        public void Dispose()
        {
            _cache.Dispose();
        }

        /// <inheritdoc />
        public bool TryResolveIdentifier(FormKey formKey, out string? editorId, ResolveTarget target = ResolveTarget.Winner)
        {
            return _cache.TryResolveIdentifier(formKey, out editorId, target);
        }

        /// <inheritdoc />
        public bool TryResolveIdentifier(string editorId, out FormKey formKey)
        {
            return _cache.TryResolveIdentifier(editorId, out formKey);
        }

        /// <inheritdoc />
        public bool TryResolveIdentifier(FormKey formKey, Type type, out string? editorId,
            ResolveTarget target = ResolveTarget.Winner)
        {
            return _cache.TryResolveIdentifier(formKey, type, out editorId, target);
        }

        /// <inheritdoc />
        public bool TryResolveIdentifier(string editorId, Type type, out FormKey formKey)
        {
            return _cache.TryResolveIdentifier(editorId, type, out formKey);
        }

        /// <inheritdoc />
        public bool TryResolveIdentifier<TMajor>(FormKey formKey, out string? editorId, ResolveTarget target = ResolveTarget.Winner)
            where TMajor : class, IMajorRecordCommonGetter
        {
            return _cache.TryResolveIdentifier<TMajor>(formKey, out editorId, target);
        }

        /// <inheritdoc />
        public bool TryResolveIdentifier<TMajor>(string editorId, out FormKey formKey) 
            where TMajor : class, IMajorRecordCommonGetter
        {
            return _cache.TryResolveIdentifier<TMajor>(editorId, out formKey);
        }

        /// <inheritdoc />
        public bool TryResolveIdentifier(FormKey formKey, out string? editorId, params Type[] types)
        {
            return _cache.TryResolveIdentifier(formKey, out editorId, types);
        }

        /// <inheritdoc />
        public bool TryResolveIdentifier(string editorId, out FormKey formKey, params Type[] types)
        {
            return _cache.TryResolveIdentifier(editorId, out formKey, types);
        }

        /// <inheritdoc />
        public bool TryResolveIdentifier(FormKey formKey, IEnumerable<Type> types, out string? editorId,
            ResolveTarget target = ResolveTarget.Winner)
        {
            return _cache.TryResolveIdentifier(formKey, types, out editorId, target);
        }

        /// <inheritdoc />
        public bool TryResolveIdentifier(string editorId, IEnumerable<Type> types, out FormKey formKey)
        {
            return _cache.TryResolveIdentifier(editorId, types, out formKey);
        }

        /// <inheritdoc />
        public IEnumerable<IMajorRecordIdentifier> AllIdentifiers(Type type, CancellationToken? cancel = null)
        {
            return _cache.AllIdentifiers(type, cancel);
        }

        /// <inheritdoc />
        public IEnumerable<IMajorRecordIdentifier> AllIdentifiers<TMajor>(CancellationToken? cancel = null) where TMajor : class, IMajorRecordCommonGetter
        {
            return _cache.AllIdentifiers<TMajor>(cancel);
        }

        /// <inheritdoc />
        public IEnumerable<IMajorRecordIdentifier> AllIdentifiers(IEnumerable<Type> types, CancellationToken? cancel = null)
        {
            return _cache.AllIdentifiers(types, cancel);
        }

        /// <inheritdoc />
        public IEnumerable<IMajorRecordIdentifier> AllIdentifiers(params Type[] types)
        {
            return _cache.AllIdentifiers(types);
        }

        /// <inheritdoc />
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public bool TryResolve(FormKey formKey, [MaybeNullWhen(false)] out IMajorRecordCommonGetter majorRec, ResolveTarget target = ResolveTarget.Winner)
        {
            return _cache.TryResolve(formKey, out majorRec, target);
        }

        /// <inheritdoc />
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public bool TryResolve(string editorId, [MaybeNullWhen(false)] out IMajorRecordCommonGetter majorRec)
        {
            return _cache.TryResolve(editorId, out majorRec);
        }

        /// <inheritdoc />
        public bool TryResolve<TMajor>(FormKey formKey, [MaybeNullWhen(false)] out TMajor majorRec, ResolveTarget target = ResolveTarget.Winner) 
            where TMajor : class, IMajorRecordCommonGetter
        {
            return _cache.TryResolve<TMajor>(formKey, out majorRec, target);
        }

        /// <inheritdoc />
        public bool TryResolve<TMajor>(string editorId, [MaybeNullWhen(false)] out TMajor majorRec) 
            where TMajor : class, IMajorRecordCommonGetter
        {
            return _cache.TryResolve<TMajor>(editorId, out majorRec);
        }

        /// <inheritdoc />
        public bool TryResolve(FormKey formKey, Type type, [MaybeNullWhen(false)] out IMajorRecordCommonGetter majorRec,
            ResolveTarget target = ResolveTarget.Winner)
        {
            return _cache.TryResolve(formKey, type, out majorRec, target);
        }

        /// <inheritdoc />
        public bool TryResolve(string editorId, Type type, [MaybeNullWhen(false)] out IMajorRecordCommonGetter majorRec)
        {
            return _cache.TryResolve(editorId, type, out majorRec);
        }

        /// <inheritdoc />
        public bool TryResolve(FormKey formKey, [MaybeNullWhen(false)] out IMajorRecordCommonGetter majorRec, params Type[] types)
        {
            return _cache.TryResolve(formKey, out majorRec, types);
        }

        /// <inheritdoc />
        public bool TryResolve(string editorId, [MaybeNullWhen(false)] out IMajorRecordCommonGetter majorRec, params Type[] types)
        {
            return _cache.TryResolve(editorId, out majorRec, types);
        }

        /// <inheritdoc />
        public bool TryResolve(FormKey formKey, IEnumerable<Type> types, [MaybeNullWhen(false)] out IMajorRecordCommonGetter majorRec,
            ResolveTarget target = ResolveTarget.Winner)
        {
            return _cache.TryResolve(formKey, types, out majorRec, target);
        }

        /// <inheritdoc />
        public bool TryResolve(string editorId, IEnumerable<Type> types, [MaybeNullWhen(false)] out IMajorRecordCommonGetter majorRec)
        {
            return _cache.TryResolve(editorId, types, out majorRec);
        }

        /// <inheritdoc />
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public IMajorRecordCommonGetter Resolve(FormKey formKey, ResolveTarget target = ResolveTarget.Winner)
        {
            return _cache.Resolve(formKey, target);
        }

        /// <inheritdoc />
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public IMajorRecordCommonGetter Resolve(string editorId)
        {
            return _cache.Resolve(editorId);
        }

        /// <inheritdoc />
        public IMajorRecordCommonGetter Resolve(FormKey formKey, Type type, ResolveTarget target = ResolveTarget.Winner)
        {
            return _cache.Resolve(formKey, type, target);
        }

        /// <inheritdoc />
        public IMajorRecordCommonGetter Resolve(string editorId, Type type)
        {
            return _cache.Resolve(editorId, type);
        }

        /// <inheritdoc />
        public IMajorRecordCommonGetter Resolve(FormKey formKey, params Type[] types)
        {
            return _cache.Resolve(formKey, types);
        }

        /// <inheritdoc />
        public IMajorRecordCommonGetter Resolve(string editorId, params Type[] types)
        {
            return _cache.Resolve(editorId, types);
        }

        /// <inheritdoc />
        public IMajorRecordCommonGetter Resolve(FormKey formKey, IEnumerable<Type> types, ResolveTarget target = ResolveTarget.Winner)
        {
            return _cache.Resolve(formKey, types, target);
        }

        /// <inheritdoc />
        public IMajorRecordCommonGetter Resolve(string editorId, IEnumerable<Type> types)
        {
            return _cache.Resolve(editorId, types);
        }

        /// <inheritdoc />
        public TMajor Resolve<TMajor>(FormKey formKey, ResolveTarget target = ResolveTarget.Winner) where TMajor : class, IMajorRecordCommonGetter
        {
            return _cache.Resolve<TMajor>(formKey, target);
        }

        /// <inheritdoc />
        public TMajor Resolve<TMajor>(string editorId) where TMajor : class, IMajorRecordCommonGetter
        {
            return _cache.Resolve<TMajor>(editorId);
        }

        /// <inheritdoc />
        public IEnumerable<TMajor> ResolveAll<TMajor>(FormKey formKey, ResolveTarget target = ResolveTarget.Winner) where TMajor : class, IMajorRecordCommonGetter
        {
            return _cache.ResolveAll<TMajor>(formKey, target);
        }

        /// <inheritdoc />
        public IEnumerable<IMajorRecordCommonGetter> ResolveAll(FormKey formKey, Type type, ResolveTarget target = ResolveTarget.Winner)
        {
            return _cache.ResolveAll(formKey, type, target);
        }

        /// <inheritdoc />
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public IEnumerable<IMajorRecordCommonGetter> ResolveAll(FormKey formKey, ResolveTarget target = ResolveTarget.Winner)
        {
            return _cache.ResolveAll(formKey, target);
        }

        /// <inheritdoc />
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public bool TryResolveSimpleContext(FormKey formKey, [MaybeNullWhen(false)] out IModContext<IMajorRecordCommonGetter> majorRec, ResolveTarget target = ResolveTarget.Winner)
        {
            if (TryResolveContext(formKey, out var resolve, target))
            {
                majorRec = resolve;
                return true;
            }

            majorRec = default;
            return false;
        }

        /// <inheritdoc />
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public bool TryResolveSimpleContext(string editorId, [MaybeNullWhen(false)] out IModContext<IMajorRecordCommonGetter> majorRec)
        {
            if (TryResolveContext(editorId, out var resolve))
            {
                majorRec = resolve;
                return true;
            }

            majorRec = default;
            return false;
        }

        /// <inheritdoc />
        public bool TryResolveSimpleContext<TMajor>(FormKey formKey, [MaybeNullWhen(false)] out IModContext<TMajor> majorRec,
            ResolveTarget target = ResolveTarget.Winner) where TMajor : class, IMajorRecordCommonGetter
        {
            if (TryResolveContext(formKey, typeof(TMajor), out var resolve, target))
            {
                majorRec = resolve.AsType<IMajorRecordCommonGetter, TMajor>();
                return true;
            }

            majorRec = default;
            return false;
        }

        /// <inheritdoc />
        public bool TryResolveSimpleContext<TMajor>(string editorId, [MaybeNullWhen(false)] out IModContext<TMajor> majorRec) where TMajor : class, IMajorRecordCommonGetter
        {
            if (TryResolveContext(editorId, typeof(TMajor), out var resolve))
            {
                majorRec = resolve.AsType<IMajorRecordCommonGetter, TMajor>();
                return true;
            }

            majorRec = default;
            return false;
        }

        /// <inheritdoc />
        public bool TryResolveSimpleContext(FormKey formKey, Type type, [MaybeNullWhen(false)] out IModContext<IMajorRecordCommonGetter> majorRec,
            ResolveTarget target = ResolveTarget.Winner)
        {
            if (TryResolveContext(formKey, type, out var resolve, target))
            {
                majorRec = resolve;
                return true;
            }

            majorRec = default;
            return false;
        }

        /// <inheritdoc />
        public bool TryResolveSimpleContext(string editorId, Type type, [MaybeNullWhen(false)] out IModContext<IMajorRecordCommonGetter> majorRec)
        {
            if (TryResolveContext(editorId, type, out var resolve))
            {
                majorRec = resolve;
                return true;
            }

            majorRec = default;
            return false;
        }

        /// <inheritdoc />
        public IModContext<IMajorRecordCommonGetter> ResolveSimpleContext(FormKey formKey, ResolveTarget target = ResolveTarget.Winner)
        {
            return ResolveContext(formKey, target);
        }

        /// <inheritdoc />
        public IModContext<IMajorRecordCommonGetter> ResolveSimpleContext(string editorId)
        {
            return ResolveContext(editorId);
        }

        /// <inheritdoc />
        public IModContext<IMajorRecordCommonGetter> ResolveSimpleContext(FormKey formKey, Type type, ResolveTarget target = ResolveTarget.Winner)
        {
            return ResolveContext(formKey, type, target);
        }

        /// <inheritdoc />
        public IModContext<IMajorRecordCommonGetter> ResolveSimpleContext(string editorId, Type type)
        {
            return ResolveContext(editorId, type);
        }

        /// <inheritdoc />
        public IModContext<TMajor> ResolveSimpleContext<TMajor>(FormKey formKey, ResolveTarget target = ResolveTarget.Winner) where TMajor : class, IMajorRecordCommonGetter
        {
            return ResolveContext(formKey, typeof(TMajor), target).AsType<IMajorRecordCommonGetter, TMajor>();
        }

        /// <inheritdoc />
        public IModContext<TMajor> ResolveSimpleContext<TMajor>(string editorId) where TMajor : class, IMajorRecordCommonGetter
        {
            return ResolveContext(editorId, typeof(TMajor)).AsType<IMajorRecordCommonGetter, TMajor>();
        }

        /// <inheritdoc />
        public IEnumerable<IModContext<TMajor>> ResolveAllSimpleContexts<TMajor>(FormKey formKey, ResolveTarget target = ResolveTarget.Winner) where TMajor : class, IMajorRecordCommonGetter
        {
            return ResolveAllContexts(formKey, typeof(TMajor), target).Select(x => x.AsType<IMajorRecordCommonGetter, TMajor>());
        }

        /// <inheritdoc />
        public IEnumerable<IModContext<IMajorRecordCommonGetter>> ResolveAllSimpleContexts(FormKey formKey, Type type, ResolveTarget target = ResolveTarget.Winner)
        {
            return ResolveAllContexts(formKey, type, target);
        }

        /// <inheritdoc />
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public IEnumerable<IModContext<IMajorRecordCommonGetter>> ResolveAllSimpleContexts(FormKey formKey, ResolveTarget target = ResolveTarget.Winner)
        {
            return ResolveAllContexts(formKey, target);
        }

        /// <inheritdoc />
        public void Warmup(Type type)
        {
            _cache.Warmup(type);
        }

        /// <inheritdoc />
        public void Warmup<TMajor>()
        {
            _cache.Warmup<TMajor>();
        }

        /// <inheritdoc />
        public void Warmup(params Type[] types)
        {
            _cache.Warmup(types);
        }

        /// <inheritdoc />
        public void Warmup(IEnumerable<Type> types)
        {
            _cache.Warmup(types);
        }

        /// <inheritdoc />
        public IReadOnlyList<IModGetter> ListedOrder => _cache.ListedOrder;

        /// <inheritdoc />
        public IReadOnlyList<IModGetter> PriorityOrder => _cache.PriorityOrder;
        
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
            return true;
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
            return true;
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
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter> ResolveContext(FormKey formKey, ResolveTarget target = ResolveTarget.Winner)
        {
            if (TryResolveContext(formKey, out var commonRec, target)) return commonRec;
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
