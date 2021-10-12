using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using Loqui;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Records.Internals;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Cache.Implementations
{
    class InternalImmutableModLinkCache : ILinkCache
    {
        internal readonly IModGetter _sourceMod;
        public GameCategory Category { get; }

        internal readonly bool _simple;
        private readonly ImmutableModLinkCacheCategory<FormKey> _formKeyCache;
        private readonly ImmutableModLinkCacheCategory<string> _editorIdCache;

        /// <inheritdoc />
        public IReadOnlyList<IModGetter> ListedOrder { get; }

        /// <inheritdoc />
        public IReadOnlyList<IModGetter> PriorityOrder => ListedOrder;

        public InternalImmutableModLinkCache(IModGetter sourceMod, LinkCachePreferences? prefs = null)
        {
            _sourceMod = sourceMod;
            Category = sourceMod.GameRelease.ToCategory();
            _simple = prefs is LinkCachePreferenceOnlyIdentifiers;
            _formKeyCache = new ImmutableModLinkCacheCategory<FormKey>(
                this, 
                x => TryGet<FormKey>.Succeed(x.FormKey),
                x => x.IsNull);
            _editorIdCache = new ImmutableModLinkCacheCategory<string>(
                this,
                m =>
                {
                    var edid = m.EditorID;
                    return TryGet<string>.Create(successful: !string.IsNullOrWhiteSpace(edid), edid!);
                },
                e => e.IsNullOrWhitespace());
            this.ListedOrder = new List<IModGetter>()
            {
                sourceMod
            };
        }

        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public bool TryResolve(FormKey formKey, [MaybeNullWhen(false)] out IMajorRecordCommonGetter majorRec, ResolveTarget target = ResolveTarget.Winner)
        {
            if (formKey.IsNull)
            {
                majorRec = default;
                return false;
            }
            
            if (target == ResolveTarget.Origin
                && formKey.ModKey != _sourceMod.ModKey)
            {
                majorRec = default;
                return false;
            }
            
            if (_formKeyCache._untypedMajorRecords.Value.TryGetValue(formKey, out var item))
            {
                majorRec = item.Record;
                return true;
            }
            majorRec = default;
            return false;
        }

        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public bool TryResolve(string editorId, [MaybeNullWhen(false)] out IMajorRecordCommonGetter majorRec)
        {
            if (string.IsNullOrWhiteSpace(editorId))
            {
                majorRec = default;
                return false;
            }
            if (_editorIdCache._untypedMajorRecords.Value.TryGetValue(editorId, out var item))
            {
                majorRec = item.Record;
                return true;
            }
            majorRec = default;
            return false;
        }

        public bool TryResolve<TMajor>(FormKey formKey, [MaybeNullWhen(false)] out TMajor majorRec, ResolveTarget target = ResolveTarget.Winner)
            where TMajor : class, IMajorRecordCommonGetter
        {
            if (target == ResolveTarget.Origin
                && formKey.ModKey != _sourceMod.ModKey)
            {
                majorRec = default;
                return false;
            }
            
            if (_formKeyCache.TryResolve(formKey, typeof(TMajor), out var item))
            {
                majorRec = item.Record as TMajor;
                return majorRec != null;
            }
            majorRec = default;
            return false;
        }

        public bool TryResolve<TMajor>(string editorId, [MaybeNullWhen(false)] out TMajor majorRec)
            where TMajor : class, IMajorRecordCommonGetter
        {
            if (_editorIdCache.TryResolve(editorId, typeof(TMajor), out var item))
            {
                majorRec = item.Record as TMajor;
                return majorRec != null;
            }
            majorRec = default;
            return false;
        }

        public bool TryResolve(FormKey formKey, Type type, [MaybeNullWhen(false)] out IMajorRecordCommonGetter majorRec, ResolveTarget target = ResolveTarget.Winner)
        {
            if (target == ResolveTarget.Origin
                && formKey.ModKey != _sourceMod.ModKey)
            {
                majorRec = default;
                return false;
            }

            if (_formKeyCache.TryResolve(formKey, type, out var item))
            {
                majorRec = item.Record;
                return true;
            }
            majorRec = default;
            return false;
        }

        public bool TryResolve(string editorId, Type type, [MaybeNullWhen(false)] out IMajorRecordCommonGetter majorRec)
        {
            if (_editorIdCache.TryResolve(editorId, type, out var item))
            {
                majorRec = item.Record;
                return true;
            }
            majorRec = default;
            return false;
        }

        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public IMajorRecordCommonGetter Resolve(FormKey formKey, ResolveTarget target = ResolveTarget.Winner)
        {
            if (TryResolve<IMajorRecordCommonGetter>(formKey, out var majorRec, target)) return majorRec;
            throw new MissingRecordException(formKey, typeof(IMajorRecordCommonGetter));
        }

        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public IMajorRecordCommonGetter Resolve(string editorId)
        {
            if (TryResolve<IMajorRecordCommonGetter>(editorId, out var majorRec)) return majorRec;
            throw new MissingRecordException(editorId, typeof(IMajorRecordCommonGetter));
        }

        public IMajorRecordCommonGetter Resolve(FormKey formKey, Type type, ResolveTarget target = ResolveTarget.Winner)
        {
            if (TryResolve(formKey, type, out var commonRec, target)) return commonRec;
            throw new MissingRecordException(formKey, type);
        }

        public IMajorRecordCommonGetter Resolve(string editorId, Type type)
        {
            if (TryResolve(editorId, type, out var commonRec)) return commonRec;
            throw new MissingRecordException(editorId, type);
        }

        public TMajor Resolve<TMajor>(FormKey formKey, ResolveTarget target = ResolveTarget.Winner)
            where TMajor : class, IMajorRecordCommonGetter
        {
            if (TryResolve<TMajor>(formKey, out var commonRec, target)) return commonRec;
            throw new MissingRecordException(formKey, typeof(TMajor));
        }

        public TMajor Resolve<TMajor>(string editorId)
            where TMajor : class, IMajorRecordCommonGetter
        {
            if (TryResolve<TMajor>(editorId, out var commonRec)) return commonRec;
            throw new MissingRecordException(editorId, typeof(TMajor));
        }

        public IEnumerable<TMajor> ResolveAll<TMajor>(FormKey formKey, ResolveTarget target = ResolveTarget.Winner)
            where TMajor : class, IMajorRecordCommonGetter
        {
            if (TryResolve<TMajor>(formKey, out var rec, target))
            {
                yield return rec;
            }
        }

        public IEnumerable<IMajorRecordCommonGetter> ResolveAll(FormKey formKey, Type type, ResolveTarget target = ResolveTarget.Winner)
        {
            if (TryResolve(formKey, type, out var rec, target))
            {
                yield return rec;
            }
        }

        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public IEnumerable<IMajorRecordCommonGetter> ResolveAll(FormKey formKey, ResolveTarget target = ResolveTarget.Winner)
        {
            if (TryResolve(formKey, out var rec, target))
            {
                yield return rec;
            }
        }

        public bool TryResolve(FormKey formKey, [MaybeNullWhen(false)] out IMajorRecordCommonGetter majorRec, params Type[] types)
        {
            return TryResolve(formKey, (IEnumerable<Type>)types, out majorRec);
        }

        public bool TryResolve(string editorId, [MaybeNullWhen(false)] out IMajorRecordCommonGetter majorRec, params Type[] types)
        {
            return TryResolve(editorId, (IEnumerable<Type>)types, out majorRec);
        }

        public bool TryResolve(FormKey formKey, IEnumerable<Type> types, [MaybeNullWhen(false)] out IMajorRecordCommonGetter majorRec, ResolveTarget target = ResolveTarget.Winner)
        {
            if (target == ResolveTarget.Origin
                && formKey.ModKey != _sourceMod.ModKey)
            {
                majorRec = default;
                return false;
            }
            
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

        public IMajorRecordCommonGetter Resolve(FormKey formKey, params Type[] types)
        {
            return Resolve(formKey, (IEnumerable<Type>)types);
        }

        public IMajorRecordCommonGetter Resolve(string editorId, params Type[] types)
        {
            return Resolve(editorId, (IEnumerable<Type>)types);
        }

        public IMajorRecordCommonGetter Resolve(FormKey formKey, IEnumerable<Type> types, ResolveTarget target = ResolveTarget.Winner)
        {
            if (TryResolve(formKey, types, out var commonRec, target)) return commonRec;
            throw new MissingRecordException(formKey, types.ToArray());
        }

        public IMajorRecordCommonGetter Resolve(string editorId, IEnumerable<Type> types)
        {
            if (TryResolve(editorId, types, out var commonRec)) return commonRec;
            throw new MissingRecordException(editorId, types.ToArray());
        }

        public bool TryResolveIdentifier(FormKey formKey, [MaybeNullWhen(false)] out string? editorId, ResolveTarget target = ResolveTarget.Winner)
        {
            if (formKey.IsNull)
            {
                editorId = default;
                return false;
            }
            
            if (target == ResolveTarget.Origin
                && formKey.ModKey != _sourceMod.ModKey)
            {
                editorId = default;
                return false;
            }
            
            if (_formKeyCache._untypedMajorRecords.Value.TryGetValue(formKey, out var item))
            {
                editorId = item.EditorID;
                return true;
            }
            editorId = default;
            return false;
        }

        public bool TryResolveIdentifier(string editorId, [MaybeNullWhen(false)] out FormKey formKey)
        {
            if (string.IsNullOrWhiteSpace(editorId))
            {
                formKey = default;
                return false;
            }
            if (_editorIdCache._untypedMajorRecords.Value.TryGetValue(editorId, out var item))
            {
                formKey = item.FormKey;
                return true;
            }
            formKey = default;
            return false;
        }

        public bool TryResolveIdentifier(FormKey formKey, Type type, [MaybeNullWhen(false)] out string? editorId, ResolveTarget target = ResolveTarget.Winner)
        {
            if (target == ResolveTarget.Origin
                && formKey.ModKey != _sourceMod.ModKey)
            {
                editorId = default;
                return false;
            }
            
            if (_formKeyCache.TryResolve(formKey, type, out var item))
            {
                editorId = item.EditorID;
                return true;
            }
            editorId = default;
            return false;
        }

        public bool TryResolveIdentifier(string editorId, Type type, [MaybeNullWhen(false)] out FormKey formKey)
        {
            if (_editorIdCache.TryResolve(editorId, type, out var item))
            {
                formKey = item.FormKey;
                return true;
            }
            formKey = default;
            return false;
        }

        public bool TryResolveIdentifier<TMajor>(FormKey formKey, out string? editorId, ResolveTarget target = ResolveTarget.Winner)
            where TMajor : class, IMajorRecordCommonGetter
        {
            if (target == ResolveTarget.Origin
                && formKey.ModKey != _sourceMod.ModKey)
            {
                editorId = default;
                return false;
            }
            
            if (_formKeyCache.TryResolve(formKey, typeof(TMajor), out var item))
            {
                editorId = item.EditorID;
                return true;
            }
            editorId = default;
            return false;
        }

        public bool TryResolveIdentifier<TMajor>(string editorId, out FormKey formKey)
            where TMajor : class, IMajorRecordCommonGetter
        {
            if (_editorIdCache.TryResolve(editorId, typeof(TMajor), out var item))
            {
                formKey = item.FormKey;
                return true;
            }
            formKey = default;
            return false;
        }

        public bool TryResolveIdentifier(FormKey formKey, [MaybeNullWhen(false)] out string? editorId, params Type[] types)
        {
            return TryResolveIdentifier(formKey, (IEnumerable<Type>)types, out editorId);
        }

        public bool TryResolveIdentifier(string editorId, [MaybeNullWhen(false)] out FormKey formKey, params Type[] types)
        {
            return TryResolveIdentifier(editorId, (IEnumerable<Type>)types, out formKey);
        }

        public bool TryResolveIdentifier(FormKey formKey, IEnumerable<Type> types, [MaybeNullWhen(false)] out string? editorId, ResolveTarget target = ResolveTarget.Winner)
        {
            if (target == ResolveTarget.Origin
                && formKey.ModKey != _sourceMod.ModKey)
            {
                editorId = default;
                return false;
            }

            foreach (var type in types)
            {
                if (TryResolveIdentifier(formKey, type, out editorId))
                {
                    return true;
                }
            }
            editorId = default;
            return false;
        }

        public bool TryResolveIdentifier(string editorId, IEnumerable<Type> types, [MaybeNullWhen(false)] out FormKey formKey)
        {
            foreach (var type in types)
            {
                if (TryResolveIdentifier(editorId, type, out formKey))
                {
                    return true;
                }
            }
            formKey = default;
            return false;
        }

        public IEnumerable<IMajorRecordIdentifier> AllIdentifiers(Type type, CancellationToken? cancel = null)
        {
            return _formKeyCache.AllIdentifiers(type, cancel);
        }

        public IEnumerable<IMajorRecordIdentifier> AllIdentifiers<TMajor>(CancellationToken? cancel = null)
            where TMajor : class, IMajorRecordCommonGetter
        {
            return _formKeyCache.AllIdentifiers(typeof(TMajor), cancel);
        }

        public IEnumerable<IMajorRecordIdentifier> AllIdentifiers(params Type[] types)
        {
            return AllIdentifiers((IEnumerable<Type>)types, CancellationToken.None);
        }

        public IEnumerable<IMajorRecordIdentifier> AllIdentifiers(IEnumerable<Type> types, CancellationToken? cancel = null)
        {
            return types.SelectMany(type => AllIdentifiers(type, cancel))
                .Distinct(x => x.FormKey);
        }

        public void Dispose()
        {
        }

        public void Warmup(Type type)
        {
            _formKeyCache.Warmup(type);
        }

        public void Warmup<TMajor>()
        {
            _formKeyCache.Warmup(typeof(TMajor));
        }

        public void Warmup(params Type[] types)
        {
            Warmup((IEnumerable<Type>)types);
        }

        public void Warmup(IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                _formKeyCache.Warmup(type);
            }
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
    public class ImmutableModLinkCache : ILinkCache
    {
        private readonly InternalImmutableModLinkCache _cache;

        public ImmutableModLinkCache(IModGetter sourceMod, LinkCachePreferences? prefs = null)
        {
            _cache = new InternalImmutableModLinkCache(sourceMod, prefs);
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
        public bool TryResolveIdentifier<TMajor>(FormKey formKey, out string? editorId, ResolveTarget target = ResolveTarget.Winner) where TMajor : class, IMajorRecordCommonGetter
        {
            return _cache.TryResolveIdentifier<TMajor>(formKey, out editorId, target);
        }

        /// <inheritdoc />
        public bool TryResolveIdentifier<TMajor>(string editorId, out FormKey formKey) where TMajor : class, IMajorRecordCommonGetter
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
        public bool TryResolve(FormKey formKey, [MaybeNullWhen(false)] out IMajorRecordCommonGetter majorRec, ResolveTarget target = ResolveTarget.Winner)
        {
            return _cache.TryResolve(formKey, out majorRec, target);
        }

        /// <inheritdoc />
        public bool TryResolve(string editorId, [MaybeNullWhen(false)] out IMajorRecordCommonGetter majorRec)
        {
            return _cache.TryResolve(editorId, out majorRec);
        }

        /// <inheritdoc />
        public bool TryResolve<TMajor>(FormKey formKey, [MaybeNullWhen(false)] out TMajor majorRec, ResolveTarget target = ResolveTarget.Winner) where TMajor : class, IMajorRecordCommonGetter
        {
            return _cache.TryResolve<TMajor>(formKey, out majorRec, target);
        }

        /// <inheritdoc />
        public bool TryResolve<TMajor>(string editorId, [MaybeNullWhen(false)] out TMajor majorRec) where TMajor : class, IMajorRecordCommonGetter
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
        public IMajorRecordCommonGetter Resolve(FormKey formKey, ResolveTarget target = ResolveTarget.Winner)
        {
            return _cache.Resolve(formKey, target);
        }

        /// <inheritdoc />
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
        public IEnumerable<IMajorRecordCommonGetter> ResolveAll(FormKey formKey, ResolveTarget target = ResolveTarget.Winner)
        {
            return _cache.ResolveAll(formKey, target);
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

    internal class ImmutableModLinkCacheCategory<TKey>
        where TKey : notnull
    {
        private readonly InternalImmutableModLinkCache _parent;
        private readonly Func<LinkCacheItem, TryGet<TKey>> _keyGetter;
        private readonly Func<TKey, bool> _shortCircuit;
        internal readonly Lazy<IReadOnlyCache<LinkCacheItem, TKey>> _untypedMajorRecords;
        private readonly Dictionary<Type, IReadOnlyCache<LinkCacheItem, TKey>> _majorRecords = new();

        public ImmutableModLinkCacheCategory(
            InternalImmutableModLinkCache parent,
            Func<LinkCacheItem, TryGet<TKey>> keyGetter,
            Func<TKey, bool> shortCircuit)
        {
            _parent = parent;
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
                    if (type.Equals(typeof(IMajorRecordCommon))
                        || type.Equals(typeof(IMajorRecordCommonGetter)))
                    {
                        cache = ConstructTypedCache(type, sourceMod);
                        _majorRecords[typeof(IMajorRecordCommon)] = cache;
                        _majorRecords[typeof(IMajorRecordCommonGetter)] = cache;
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
                        var interfaceMappings = LinkInterfaceMapping.InterfaceToObjectTypes(category);
                        if (!interfaceMappings.TryGetValue(type, out var objs))
                        {
                            throw new ArgumentException($"A lookup was queried for an unregistered type: {type.Name}");
                        }
                        var majorRecords = new Cache<LinkCacheItem, TKey>(x => _keyGetter(x).Value);
                        foreach (var objType in objs)
                        {
                            majorRecords.Set(
                                GetCache(
                                    type: LoquiRegistration.GetRegister(objType).GetterType,
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
        where TMod : class, IContextMod<TMod, TModGetter>, TModGetter
        where TModGetter : class, IContextGetterMod<TMod, TModGetter>
    {
        internal readonly TModGetter _sourceMod;
        internal readonly bool _simple;

        private readonly IImmutableModLinkCacheContextCategory<TMod, TModGetter, FormKey> _formKeyContexts;
        private readonly IImmutableModLinkCacheContextCategory<TMod, TModGetter, string> _editorIdContexts;

        /// <summary>
        /// Constructs a link cache around a target mod
        /// </summary>
        /// <param name="sourceMod">Mod to resolve against when linking</param>
        public ImmutableModLinkCache(TModGetter sourceMod, LinkCachePreferences prefs)
            : base(sourceMod, prefs)
        {
            this._sourceMod = sourceMod;
            _simple = prefs is LinkCachePreferenceOnlyIdentifiers;
            _formKeyContexts = new ImmutableModLinkCacheContextCategory<TMod, TModGetter, FormKey>(
                parent: this,
                keyGetter: m => TryGet<FormKey>.Succeed(m.FormKey),
                shortCircuit: f => f.IsNull);
            _editorIdContexts = new ImmutableModLinkCacheContextCategory<TMod, TModGetter, string>(
                parent: this,
                keyGetter: m =>
                {
                    var edid = m.EditorID;
                    return TryGet<string>.Create(successful: !string.IsNullOrWhiteSpace(edid), edid!);
                },
                shortCircuit: e => e.IsNullOrWhitespace());
        }

        /// <inheritdoc />
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public bool TryResolveContext(FormKey formKey, [MaybeNullWhen(false)] out IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter> majorRec, ResolveTarget target = ResolveTarget.Winner)
        {
            if (formKey.IsNull)
            {
                majorRec = default;
                return false;
            }
            
            if (target == ResolveTarget.Origin
                && formKey.ModKey != _sourceMod.ModKey)
            {
                majorRec = default;
                return false;
            }
            
            return _formKeyContexts.TryResolveUntypedContext(formKey, out majorRec);
        }

        /// <inheritdoc />
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public bool TryResolveContext(string editorId, [MaybeNullWhen(false)] out IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter> majorRec)
        {
            if (editorId.IsNullOrWhitespace())
            {
                majorRec = default;
                return false;
            }
            return _editorIdContexts.TryResolveUntypedContext(editorId, out majorRec);
        }

        /// <inheritdoc />
        public bool TryResolveContext<TMajor, TMajorGetter>(FormKey formKey, [MaybeNullWhen(false)] out IModContext<TMod, TModGetter, TMajor, TMajorGetter> majorRec, ResolveTarget target = ResolveTarget.Winner)
            where TMajor : class, IMajorRecordCommon, TMajorGetter
            where TMajorGetter : class, IMajorRecordCommonGetter
        {
            if (target == ResolveTarget.Origin
                && formKey.ModKey != _sourceMod.ModKey)
            {
                majorRec = default;
                return false;
            }
            
            return _formKeyContexts.TryResolveContext(formKey, out majorRec);
        }

        /// <inheritdoc />
        public bool TryResolveContext<TMajor, TMajorGetter>(string editorId, [MaybeNullWhen(false)] out IModContext<TMod, TModGetter, TMajor, TMajorGetter> majorRec)
            where TMajor : class, IMajorRecordCommon, TMajorGetter
            where TMajorGetter : class, IMajorRecordCommonGetter
        {
            return _editorIdContexts.TryResolveContext(editorId, out majorRec);
        }

        /// <inheritdoc />
        public bool TryResolveContext(FormKey formKey, Type type, [MaybeNullWhen(false)] out IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter> majorRec, ResolveTarget target = ResolveTarget.Winner)
        {
            if (target == ResolveTarget.Origin
                && formKey.ModKey != _sourceMod.ModKey)
            {
                majorRec = default;
                return false;
            }

            return _formKeyContexts.TryResolveContext(formKey, type, out majorRec);
        }

        /// <inheritdoc />
        public bool TryResolveContext(string editorId, Type type, [MaybeNullWhen(false)] out IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter> majorRec)
        {
            return _editorIdContexts.TryResolveContext(editorId, type, out majorRec);
        }

        /// <inheritdoc />
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter> ResolveContext(FormKey formKey, ResolveTarget target = ResolveTarget.Winner)
        {
            if (TryResolveContext<IMajorRecordCommon, IMajorRecordCommonGetter>(formKey, out var majorRec, target)) return majorRec;
            throw new MissingRecordException(formKey, typeof(IMajorRecordCommonGetter));
        }

        /// <inheritdoc />
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter> ResolveContext(string editorId)
        {
            if (TryResolveContext<IMajorRecordCommon, IMajorRecordCommonGetter>(editorId, out var majorRec)) return majorRec;
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
            if (TryResolveContext<TMajor, TMajorGetter>(formKey, out var rec, target))
            {
                yield return rec;
            }
        }
        
        /// <inheritdoc />
        public IEnumerable<IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter>> ResolveAllContexts(FormKey formKey, Type type, ResolveTarget target = ResolveTarget.Winner)
        {
            if (TryResolveContext(formKey, type, out var rec, target))
            {
                yield return rec;
            }
        }

        /// <inheritdoc />
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public IEnumerable<IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter>> ResolveAllContexts(FormKey formKey, ResolveTarget target = ResolveTarget.Winner)
        {
            if (TryResolveContext(formKey, out var rec, target))
            {
                yield return rec;
            }
        }
    }

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
