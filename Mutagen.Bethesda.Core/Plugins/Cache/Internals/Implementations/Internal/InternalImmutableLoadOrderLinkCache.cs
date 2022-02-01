using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Records.Internals;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Cache.Internals.Implementations.Internal
{
    public class InternalImmutableLoadOrderLinkCache
    {
        private readonly IReadOnlyList<IModGetter> _listedOrder;
        private readonly IReadOnlyList<IModGetter> _priorityOrder;
        private readonly ImmutableLoadOrderLinkCacheCategory<FormKey> _formKeyCache;
        private readonly ImmutableLoadOrderLinkCacheCategory<string> _editorIdCache;
        private readonly IReadOnlyDictionary<ModKey, ILinkCache> _modsByKey;

        public IReadOnlyList<IModGetter> ListedOrder => _listedOrder;

        public IReadOnlyList<IModGetter> PriorityOrder => _priorityOrder;

        public InternalImmutableLoadOrderLinkCache(
            IEnumerable<IModGetter> loadOrder,
            GameCategory gameCategory,
            bool hasAny,
            bool simple,
            LinkCachePreferences? prefs)
        {
            prefs ??= LinkCachePreferences.Default;
            _listedOrder = loadOrder.ToList();
            _priorityOrder = _listedOrder.Reverse().ToList();
            _formKeyCache = new ImmutableLoadOrderLinkCacheCategory<FormKey>(
                gameCategory,
                hasAny: hasAny,
                simple: simple,
                listedOrder: _listedOrder,
                linkInterfaceMapGetter: prefs?.LinkInterfaceMapGetterOverride ?? LinkInterfaceMapping.Instance,
                m => TryGet<FormKey>.Succeed(m.FormKey),
                f => f.IsNull);
            this._editorIdCache = new ImmutableLoadOrderLinkCacheCategory<string>(
                gameCategory,
                hasAny: hasAny,
                simple: simple,
                listedOrder: _listedOrder,
                linkInterfaceMapGetter: prefs?.LinkInterfaceMapGetterOverride ?? LinkInterfaceMapping.Instance,
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

        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public bool TryResolve(FormKey formKey, [MaybeNullWhen(false)] out IMajorRecordGetter majorRec, ResolveTarget target = ResolveTarget.Winner)
        {
            return TryResolve<IMajorRecordGetter>(formKey, out majorRec, target);
        }

        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public bool TryResolve(string editorId, [MaybeNullWhen(false)] out IMajorRecordGetter majorRec)
        {
            return TryResolve<IMajorRecordGetter>(editorId, out majorRec);
        }

        public bool TryResolve<TMajor>(FormKey formKey, [MaybeNullWhen(false)] out TMajor majorRec, ResolveTarget target = ResolveTarget.Winner)
            where TMajor : class, IMajorRecordGetter
        {
            if (!TryResolve(formKey, typeof(TMajor), out var commonRec, target))
            {
                majorRec = default;
                return false;
            }

            majorRec = commonRec as TMajor;
            return majorRec != null;
        }

        public bool TryResolve<TMajor>(string editorId, [MaybeNullWhen(false)] out TMajor majorRec)
            where TMajor : class, IMajorRecordGetter
        {
            if (!TryResolve(editorId, typeof(TMajor), out var commonRec))
            {
                majorRec = default;
                return false;
            }

            majorRec = commonRec as TMajor;
            return majorRec != null;
        }

        public bool TryResolve(FormKey formKey, Type type, [MaybeNullWhen(false)] out IMajorRecordGetter majorRec, ResolveTarget target = ResolveTarget.Winner)
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

        public bool TryResolve(string editorId, Type type, [MaybeNullWhen(false)] out IMajorRecordGetter majorRec)
        {
            if (_editorIdCache.TryResolve(editorId, default(ModKey?), type, out var item))
            {
                majorRec = item.Record;
                return true;
            }
            majorRec = default;
            return false;
        }

        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public IMajorRecordGetter Resolve(FormKey formKey, ResolveTarget target = ResolveTarget.Winner)
        {
            if (TryResolve<IMajorRecordGetter>(formKey, out var commonRec, target)) return commonRec;
            throw new MissingRecordException(formKey, typeof(IMajorRecordGetter));
        }

        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public IMajorRecordGetter Resolve(string editorId)
        {
            if (TryResolve<IMajorRecordGetter>(editorId, out var commonRec)) return commonRec;
            throw new MissingRecordException(editorId, typeof(IMajorRecordGetter));
        }

        public IMajorRecordGetter Resolve(FormKey formKey, Type type, ResolveTarget target = ResolveTarget.Winner)
        {
            if (TryResolve(formKey, type, out var commonRec, target)) return commonRec;
            throw new MissingRecordException(formKey, type);
        }

        public IMajorRecordGetter Resolve(string editorId, Type type)
        {
            if (TryResolve(editorId, type, out var commonRec)) return commonRec;
            throw new MissingRecordException(editorId, type);
        }

        public TMajor Resolve<TMajor>(FormKey formKey, ResolveTarget target = ResolveTarget.Winner)
            where TMajor : class, IMajorRecordGetter
        {
            if (TryResolve<TMajor>(formKey, out var commonRec, target)) return commonRec;
            throw new MissingRecordException(formKey, typeof(TMajor));
        }

        public TMajor Resolve<TMajor>(string editorId)
            where TMajor : class, IMajorRecordGetter
        {
            if (TryResolve<TMajor>(editorId, out var commonRec)) return commonRec;
            throw new MissingRecordException(editorId, typeof(TMajor));
        }

        public IEnumerable<TMajor> ResolveAll<TMajor>(FormKey formKey, ResolveTarget target = ResolveTarget.Winner)
            where TMajor : class, IMajorRecordGetter
        {
            return ResolveAll(formKey, typeof(TMajor), target).Cast<TMajor>();
        }

        public IEnumerable<IMajorRecordGetter> ResolveAll(FormKey formKey, Type type, ResolveTarget target = ResolveTarget.Winner)
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

        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public IEnumerable<IMajorRecordGetter> ResolveAll(FormKey formKey, ResolveTarget target = ResolveTarget.Winner)
        {
            return ResolveAll(formKey, typeof(IMajorRecordGetter), target);
        }

        public bool TryResolve(FormKey formKey, [MaybeNullWhen(false)] out IMajorRecordGetter majorRec, params Type[] types)
        {
            return TryResolve(formKey, (IEnumerable<Type>)types, out majorRec);
        }

        public bool TryResolve(string editorId, [MaybeNullWhen(false)] out IMajorRecordGetter majorRec, params Type[] types)
        {
            return TryResolve(editorId, (IEnumerable<Type>)types, out majorRec);
        }

        public bool TryResolve(FormKey formKey, IEnumerable<Type> types, [MaybeNullWhen(false)] out IMajorRecordGetter majorRec, ResolveTarget target = ResolveTarget.Winner)
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

        public bool TryResolve(string editorId, IEnumerable<Type> types, [MaybeNullWhen(false)] out IMajorRecordGetter majorRec)
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

        public IMajorRecordGetter Resolve(FormKey formKey, params Type[] types)
        {
            return Resolve(formKey, (IEnumerable<Type>)types);
        }

        public IMajorRecordGetter Resolve(string editorId, params Type[] types)
        {
            return Resolve(editorId, (IEnumerable<Type>)types);
        }

        public IMajorRecordGetter Resolve(FormKey formKey, IEnumerable<Type> types, ResolveTarget target = ResolveTarget.Winner)
        {
            if (TryResolve(formKey, types, out var commonRec, target)) return commonRec;
            throw new MissingRecordException(formKey, types.ToArray());
        }

        public IMajorRecordGetter Resolve(string editorId, IEnumerable<Type> types)
        {
            if (TryResolve(editorId, types, out var commonRec)) return commonRec;
            throw new MissingRecordException(editorId, types.ToArray());
        }

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

            if (_formKeyCache.TryResolve(formKey, formKey.ModKey, typeof(IMajorRecordGetter), out var rec))
            {
                editorId = rec.EditorID;
                return true;
            }

            editorId = default;
            return false;
        }

        public bool TryResolveIdentifier(string editorId, [MaybeNullWhen(false)] out FormKey formKey)
        {
            if (_editorIdCache.TryResolve(editorId, default, typeof(IMajorRecordGetter), out var rec))
            {
                formKey = rec.FormKey;
                return true;
            }

            formKey = default;
            return false;
        }

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

        public bool TryResolveIdentifier<TMajor>(FormKey formKey, [MaybeNullWhen(false)] out string? editorId, ResolveTarget target = ResolveTarget.Winner)
            where TMajor : class, IMajorRecordGetter
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

        public bool TryResolveIdentifier<TMajor>(string editorId, [MaybeNullWhen(false)] out FormKey formKey)
            where TMajor : class, IMajorRecordGetter
        {
            if (_editorIdCache.TryResolve(editorId, default, typeof(TMajor), out var rec))
            {
                formKey = rec.FormKey;
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

        public IEnumerable<IMajorRecordIdentifier> AllIdentifiers(Type type, CancellationToken? cancel = null)
        {
            return _formKeyCache.AllIdentifiers(type, cancel);
        }

        public IEnumerable<IMajorRecordIdentifier> AllIdentifiers<TMajor>(CancellationToken? cancel = null)
            where TMajor : class, IMajorRecordGetter
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
}