﻿using System.Diagnostics.CodeAnalysis;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Records.Mapping;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Cache.Internals.Implementations.Internal;

internal sealed class InternalImmutableLoadOrderLinkCache
{
    private readonly bool _simple;
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
        _simple = simple;
        prefs ??= LinkCachePreferences.Default;
        _listedOrder = loadOrder.ToList();
        _priorityOrder = _listedOrder.Reverse().ToList();
        _formKeyCache = new ImmutableLoadOrderLinkCacheCategory<FormKey>(
            gameCategory,
            hasAny: hasAny,
            simple: simple,
            listedOrder: _listedOrder,
            metaInterfaceMapGetter: prefs?.MetaInterfaceMapGetterOverride ?? MetaInterfaceMapping.Instance,
            m => TryGet<FormKey>.Succeed(m.FormKey),
            f => f.IsNull,
            equalityComparer: null);
        _editorIdCache = new ImmutableLoadOrderLinkCacheCategory<string>(
            gameCategory,
            hasAny: hasAny,
            simple: simple,
            listedOrder: _listedOrder,
            metaInterfaceMapGetter: prefs?.MetaInterfaceMapGetterOverride ?? MetaInterfaceMapping.Instance,
            m =>
            {
                var edid = m.EditorID;
                return TryGet<string>.Create(successful: !string.IsNullOrWhiteSpace(edid), edid!);
            },
            e => e.IsNullOrWhitespace(),
            equalityComparer: StringComparer.OrdinalIgnoreCase);
            
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
        where TMajor : class, IMajorRecordQueryableGetter
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
        where TMajor : class, IMajorRecordQueryableGetter
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
        if (formKey.IsNull)
        {
            majorRec = default;
            return false;
        }
            
        if (_simple)
        {
            throw new ArgumentException("Queried for record on a simple cache");
        }
            
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
        if (editorId.IsNullOrEmpty())
        {
            majorRec = default;
            return false;
        }
            
        if (_simple)
        {
            throw new ArgumentException("Queried for record on a simple cache");
        }
            
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
        where TMajor : class, IMajorRecordQueryableGetter
    {
        if (TryResolve<TMajor>(formKey, out var commonRec, target)) return commonRec;
        throw new MissingRecordException(formKey, typeof(TMajor));
    }

    public TMajor Resolve<TMajor>(string editorId)
        where TMajor : class, IMajorRecordQueryableGetter
    {
        if (TryResolve<TMajor>(editorId, out var commonRec)) return commonRec;
        throw new MissingRecordException(editorId, typeof(TMajor));
    }

    public IEnumerable<TMajor> ResolveAll<TMajor>(FormKey formKey, ResolveTarget target = ResolveTarget.Winner)
        where TMajor : class, IMajorRecordQueryableGetter
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

    public bool TryResolve(FormKey formKey, [MaybeNullWhen(false)] out IMajorRecordGetter majorRec,
        [MaybeNullWhen(false)] out Type matchedType, params Type[] types)
    {
        return TryResolve(formKey, (IEnumerable<Type>)types, out majorRec, out matchedType);
    }

    public bool TryResolve(string editorId, [MaybeNullWhen(false)] out IMajorRecordGetter majorRec,
        [MaybeNullWhen(false)] out Type matchedType, params Type[] types)
    {
        return TryResolve(editorId, (IEnumerable<Type>)types, out majorRec, out matchedType);
    }

    public bool TryResolve(FormKey formKey, IEnumerable<Type> types, [MaybeNullWhen(false)] out IMajorRecordGetter majorRec, 
        [MaybeNullWhen(false)] out Type matchedType, ResolveTarget target = ResolveTarget.Winner)
    {
        foreach (var type in types)
        {
            if (TryResolve(formKey, type, out majorRec, target))
            {
                matchedType = type;
                return true;
            }
        }

        matchedType = default;
        majorRec = default;
        return false;
    }

    public bool TryResolve(string editorId, IEnumerable<Type> types, [MaybeNullWhen(false)] out IMajorRecordGetter majorRec, 
        [MaybeNullWhen(false)] out Type matchedType)
    {
        foreach (var type in types)
        {
            if (TryResolve(editorId, type, out majorRec))
            {
                matchedType = type;
                return true;
            }
        }
        
        matchedType = default;
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
        if (TryResolve(formKey, types, out var commonRec, out var matchedType, target)) return commonRec;
        throw new MissingRecordException(formKey, types.ToArray());
    }

    public IMajorRecordGetter Resolve(string editorId, IEnumerable<Type> types)
    {
        if (TryResolve(editorId, types, out var commonRec, out var matchedType)) return commonRec;
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
        where TMajor : class, IMajorRecordQueryableGetter
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
        where TMajor : class, IMajorRecordQueryableGetter
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
        return TryResolveIdentifier(formKey, (IEnumerable<Type>)types, out editorId, out var matchedType);
    }

    public bool TryResolveIdentifier(string editorId, [MaybeNullWhen(false)] out FormKey formKey, params Type[] types)
    {
        return TryResolveIdentifier(editorId, (IEnumerable<Type>)types, out formKey, out var matchedType);
    }

    public bool TryResolveIdentifier(FormKey formKey, IEnumerable<Type> types, [MaybeNullWhen(false)] out string? editorId, [MaybeNullWhen(false)] out Type matchedType, ResolveTarget target = ResolveTarget.Winner)
    {
        if (target == ResolveTarget.Origin)
        {
            if (!_modsByKey.TryGetValue(formKey.ModKey, out var origMod))
            {
                matchedType = default;
                editorId = default;
                return false;
            }

            foreach (var type in types)
            {
                if (origMod.TryResolveIdentifier(formKey, out editorId, type))
                {
                    matchedType = type;
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
                    matchedType = type;
                    editorId = rec.EditorID;
                    return true;
                }
            }
        }

        matchedType = default;
        editorId = default;
        return false;
    }

    public bool TryResolveIdentifier(string editorId, IEnumerable<Type> types, [MaybeNullWhen(false)] out FormKey formKey, [MaybeNullWhen(false)] out Type matchedType)
    {
        foreach (var type in types)
        {
            if (_editorIdCache.TryResolve(editorId, default, type, out var rec))
            {
                formKey = rec.FormKey;
                matchedType = type;
                return true;
            }
        }

        matchedType = default;
        formKey = default;
        return false;
    }

    public IEnumerable<IMajorRecordIdentifierGetter> AllIdentifiers(Type type, CancellationToken? cancel = null)
    {
        return _formKeyCache.AllIdentifiers(type, cancel);
    }

    public IEnumerable<IMajorRecordIdentifierGetter> AllIdentifiers<TMajor>(CancellationToken? cancel = null)
        where TMajor : class, IMajorRecordQueryableGetter
    {
        return _formKeyCache.AllIdentifiers(typeof(TMajor), cancel);
    }

    public IEnumerable<IMajorRecordIdentifierGetter> AllIdentifiers(params Type[] types)
    {
        return AllIdentifiers((IEnumerable<Type>)types, CancellationToken.None);
    }

    public IEnumerable<IMajorRecordIdentifierGetter> AllIdentifiers(IEnumerable<Type> types, CancellationToken? cancel = null)
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