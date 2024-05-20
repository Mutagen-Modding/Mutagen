using System.Diagnostics.CodeAnalysis;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Records.Mapping;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Cache.Internals.Implementations.Internal;

internal sealed class InternalImmutableModLinkCache
{
    internal readonly IModGetter _sourceMod;
    public GameCategory Category { get; }

    internal readonly bool _simple;
    private readonly ImmutableModLinkCacheCategory<FormKey> _formKeyCache;
    private readonly ImmutableModLinkCacheCategory<string> _editorIdCache;

    public IReadOnlyList<IModGetter> ListedOrder { get; }

    public IReadOnlyList<IModGetter> PriorityOrder => ListedOrder;

    public InternalImmutableModLinkCache(IModGetter sourceMod, LinkCachePreferences? prefs = null)
    {
        _sourceMod = sourceMod;
        Category = sourceMod.GameRelease.ToCategory();
        _simple = (prefs ?? LinkCachePreferences.Default).Retention == LinkCachePreferences.RetentionType.OnlyIdentifiers;
        _formKeyCache = new ImmutableModLinkCacheCategory<FormKey>(
            this, 
            prefs?.MetaInterfaceMapGetterOverride ?? MetaInterfaceMapping.Instance,
            x => TryGet<FormKey>.Succeed(x.FormKey),
            x => x.IsNull);
        _editorIdCache = new ImmutableModLinkCacheCategory<string>(
            this,
            prefs?.MetaInterfaceMapGetterOverride ?? MetaInterfaceMapping.Instance,
            m =>
            {
                var edid = m.EditorID;
                return TryGet<string>.Create(successful: !string.IsNullOrWhiteSpace(edid), edid!);
            },
            e => e.IsNullOrWhitespace());
        ListedOrder = new List<IModGetter>()
        {
            sourceMod
        };
    }

    [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
    public bool TryResolve(FormKey formKey, [MaybeNullWhen(false)] out IMajorRecordGetter majorRec, ResolveTarget target = ResolveTarget.Winner)
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
    public bool TryResolve(string editorId, [MaybeNullWhen(false)] out IMajorRecordGetter majorRec)
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
        where TMajor : class, IMajorRecordQueryableGetter
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
        where TMajor : class, IMajorRecordQueryableGetter
    {
        if (_editorIdCache.TryResolve(editorId, typeof(TMajor), out var item))
        {
            majorRec = item.Record as TMajor;
            return majorRec != null;
        }
        majorRec = default;
        return false;
    }

    public bool TryResolve(FormKey formKey, Type type, [MaybeNullWhen(false)] out IMajorRecordGetter majorRec, ResolveTarget target = ResolveTarget.Winner)
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

    public bool TryResolve(string editorId, Type type, [MaybeNullWhen(false)] out IMajorRecordGetter majorRec)
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
    public IMajorRecordGetter Resolve(FormKey formKey, ResolveTarget target = ResolveTarget.Winner)
    {
        if (TryResolve<IMajorRecordGetter>(formKey, out var majorRec, target)) return majorRec;
        throw new MissingRecordException(formKey, typeof(IMajorRecordGetter));
    }

    [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
    public IMajorRecordGetter Resolve(string editorId)
    {
        if (TryResolve<IMajorRecordGetter>(editorId, out var majorRec)) return majorRec;
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
        if (TryResolve<TMajor>(formKey, out var rec, target))
        {
            yield return rec;
        }
    }

    public IEnumerable<IMajorRecordGetter> ResolveAll(FormKey formKey, Type type, ResolveTarget target = ResolveTarget.Winner)
    {
        if (TryResolve(formKey, type, out var rec, target))
        {
            yield return rec;
        }
    }

    [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
    public IEnumerable<IMajorRecordGetter> ResolveAll(FormKey formKey, ResolveTarget target = ResolveTarget.Winner)
    {
        if (TryResolve(formKey, out var rec, target))
        {
            yield return rec;
        }
    }

    public bool TryResolve(FormKey formKey, [MaybeNullWhen(false)] out IMajorRecordGetter majorRec, [MaybeNullWhen(false)] out Type matchedType, params Type[] types)
    {
        return TryResolve(formKey, (IEnumerable<Type>)types, out majorRec, out matchedType);
    }

    public bool TryResolve(string editorId, [MaybeNullWhen(false)] out IMajorRecordGetter majorRec, [MaybeNullWhen(false)] out Type matchedType, params Type[] types)
    {
        return TryResolve(editorId, (IEnumerable<Type>)types, out majorRec, out matchedType);
    }

    public bool TryResolve(FormKey formKey, IEnumerable<Type> types, [MaybeNullWhen(false)] out IMajorRecordGetter majorRec, [MaybeNullWhen(false)] out Type matchedType, ResolveTarget target = ResolveTarget.Winner)
    {
        if (target == ResolveTarget.Origin
            && formKey.ModKey != _sourceMod.ModKey)
        {
            majorRec = default;
            matchedType = default;
            return false;
        }
            
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

    public bool TryResolve(string editorId, IEnumerable<Type> types, [MaybeNullWhen(false)] out IMajorRecordGetter majorRec, [MaybeNullWhen(false)] out Type matchedType)
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

    public IMajorRecordGetter Resolve(FormKey formKey, [MaybeNullWhen(false)] out Type matchedType, params Type[] types)
    {
        return Resolve(formKey, (IEnumerable<Type>)types, out matchedType);
    }

    public IMajorRecordGetter Resolve(string editorId, [MaybeNullWhen(false)] out Type matchedType, params Type[] types)
    {
        return Resolve(editorId, (IEnumerable<Type>)types, out matchedType);
    }

    public IMajorRecordGetter Resolve(FormKey formKey, IEnumerable<Type> types, [MaybeNullWhen(false)] out Type matchedType, ResolveTarget target = ResolveTarget.Winner)
    {
        if (TryResolve(formKey, types, out var commonRec, out matchedType!, target)) return commonRec!;
        throw new MissingRecordException(formKey, types.ToArray());
    }

    public IMajorRecordGetter Resolve(string editorId, IEnumerable<Type> types, [MaybeNullWhen(false)] out Type matchedType)
    {
        if (TryResolve(editorId, types, out var commonRec, out matchedType!)) return commonRec!;
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
        where TMajor : class, IMajorRecordQueryableGetter
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
        where TMajor : class, IMajorRecordQueryableGetter
    {
        if (_editorIdCache.TryResolve(editorId, typeof(TMajor), out var item))
        {
            formKey = item.FormKey;
            return true;
        }
        formKey = default;
        return false;
    }

    public bool TryResolveIdentifier(FormKey formKey, [MaybeNullWhen(false)] out string? editorId, [MaybeNullWhen(false)] out Type matchedType, params Type[] types)
    {
        return TryResolveIdentifier(formKey, (IEnumerable<Type>)types, out editorId, out matchedType);
    }

    public bool TryResolveIdentifier(string editorId, [MaybeNullWhen(false)] out FormKey formKey, [MaybeNullWhen(false)] out Type matchedType, params Type[] types)
    {
        return TryResolveIdentifier(editorId, (IEnumerable<Type>)types, out formKey, out matchedType);
    }

    public bool TryResolveIdentifier(FormKey formKey, IEnumerable<Type> types, [MaybeNullWhen(false)] out string? editorId, [MaybeNullWhen(false)] out Type matchedType, ResolveTarget target = ResolveTarget.Winner)
    {
        if (target == ResolveTarget.Origin
            && formKey.ModKey != _sourceMod.ModKey)
        {
            editorId = default;
            matchedType = default;
            return false;
        }

        foreach (var type in types)
        {
            if (TryResolveIdentifier(formKey, type, out editorId))
            {
                matchedType = type;
                return true;
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
            if (TryResolveIdentifier(editorId, type, out formKey))
            {
                matchedType = type;
                return true;
            }
        }

        matchedType = default;
        formKey = default;
        return false;
    }

    public IEnumerable<IMajorRecordIdentifier> AllIdentifiers(Type type, CancellationToken? cancel = null)
    {
        return _formKeyCache.AllIdentifiers(type, cancel);
    }

    public IEnumerable<IMajorRecordIdentifier> AllIdentifiers<TMajor>(CancellationToken? cancel = null)
        where TMajor : class, IMajorRecordQueryableGetter
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