using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Mutagen.Bethesda.Plugins.Cache.Internals.Implementations.Internal;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Records.Mapping;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Cache.Internals.Implementations;

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
    private readonly InternalImmutableModLinkCache _cache;
    private readonly IImmutableModLinkCacheSimpleContextCategory<FormKey> _formKeyContexts;
    private readonly IImmutableModLinkCacheSimpleContextCategory<string> _editorIdContexts;
    private bool _disposed;

    public ImmutableModLinkCache(IModGetter sourceMod, LinkCachePreferences? prefs = null)
    {
        _sourceMod = sourceMod;
        var simple = (prefs ?? LinkCachePreferences.Default).Retention == LinkCachePreferences.RetentionType.OnlyIdentifiers;
        var category = sourceMod.GameRelease.ToCategory();
        _cache = new InternalImmutableModLinkCache(sourceMod, prefs);
        _formKeyContexts = new ImmutableModLinkCacheSimpleContextCategory<FormKey>(
            simple: simple,
            linkCache: this,
            category: category,
            metaInterfaceMapGetter: prefs?.MetaInterfaceMapGetterOverride ?? MetaInterfaceMapping.Instance,
            contextEnumerable: sourceMod,
            keyGetter: m => TryGet<FormKey>.Succeed(m.FormKey),
            shortCircuit: f => f.IsNull);
        _editorIdContexts = new ImmutableModLinkCacheSimpleContextCategory<string>(
            simple: simple,
            linkCache: this,
            category: category,
            metaInterfaceMapGetter: prefs?.MetaInterfaceMapGetterOverride ?? MetaInterfaceMapping.Instance,
            contextEnumerable: sourceMod,
            keyGetter: m =>
            {
                var edid = m.EditorID;
                return TryGet<string>.Create(successful: !string.IsNullOrWhiteSpace(edid), edid!);
            },
            shortCircuit: e => e.IsNullOrWhitespace());
    }

    public void Dispose()
    {
        _disposed = true;
        _cache.Dispose();
    }

    private void CheckDisposal()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(ImmutableModLinkCache));
        }
    }

    /// <inheritdoc />
    public bool TryResolveIdentifier(FormKey formKey, out string? editorId, ResolveTarget target = ResolveTarget.Winner)
    {
        CheckDisposal();
        return _cache.TryResolveIdentifier(formKey, out editorId, target);
    }

    /// <inheritdoc />
    public bool TryResolveIdentifier(string editorId, out FormKey formKey)
    {
        CheckDisposal();
        return _cache.TryResolveIdentifier(editorId, out formKey);
    }

    /// <inheritdoc />
    public bool TryResolveIdentifier(FormKey formKey, Type type, out string? editorId,
        ResolveTarget target = ResolveTarget.Winner)
    {
        CheckDisposal();
        return _cache.TryResolveIdentifier(formKey, type, out editorId, target);
    }

    /// <inheritdoc />
    public bool TryResolveIdentifier(string editorId, Type type, out FormKey formKey)
    {
        CheckDisposal();
        return _cache.TryResolveIdentifier(editorId, type, out formKey);
    }

    /// <inheritdoc />
    public bool TryResolveIdentifier<TMajor>(FormKey formKey, out string? editorId, ResolveTarget target = ResolveTarget.Winner) 
        where TMajor : class, IMajorRecordQueryableGetter
    {
        CheckDisposal();
        return _cache.TryResolveIdentifier<TMajor>(formKey, out editorId, target);
    }

    /// <inheritdoc />
    public bool TryResolveIdentifier<TMajor>(string editorId, out FormKey formKey) 
        where TMajor : class, IMajorRecordQueryableGetter
    {
        CheckDisposal();
        return _cache.TryResolveIdentifier<TMajor>(editorId, out formKey);
    }

    /// <inheritdoc />
    public bool TryResolveIdentifier(FormKey formKey, out string? editorId, params Type[] types)
    {
        CheckDisposal();
        return _cache.TryResolveIdentifier(formKey, out editorId, types);
    }

    /// <inheritdoc />
    public bool TryResolveIdentifier(string editorId, out FormKey formKey, params Type[] types)
    {
        CheckDisposal();
        return _cache.TryResolveIdentifier(editorId, out formKey, types);
    }

    /// <inheritdoc />
    public bool TryResolveIdentifier(FormKey formKey, IEnumerable<Type> types, out string? editorId,
        ResolveTarget target = ResolveTarget.Winner)
    {
        CheckDisposal();
        return _cache.TryResolveIdentifier(formKey, types, out editorId, target);
    }

    /// <inheritdoc />
    public bool TryResolveIdentifier(string editorId, IEnumerable<Type> types, out FormKey formKey)
    {
        CheckDisposal();
        return _cache.TryResolveIdentifier(editorId, types, out formKey);
    }

    /// <inheritdoc />
    public IEnumerable<IMajorRecordIdentifier> AllIdentifiers(Type type, CancellationToken? cancel = null)
    {
        CheckDisposal();
        return _cache.AllIdentifiers(type, cancel);
    }

    /// <inheritdoc />
    public IEnumerable<IMajorRecordIdentifier> AllIdentifiers<TMajor>(CancellationToken? cancel = null) 
        where TMajor : class, IMajorRecordQueryableGetter
    {
        CheckDisposal();
        return _cache.AllIdentifiers<TMajor>(cancel);
    }

    /// <inheritdoc />
    public IEnumerable<IMajorRecordIdentifier> AllIdentifiers(IEnumerable<Type> types, CancellationToken? cancel = null)
    {
        CheckDisposal();
        return _cache.AllIdentifiers(types, cancel);
    }

    /// <inheritdoc />
    public IEnumerable<IMajorRecordIdentifier> AllIdentifiers(params Type[] types)
    {
        CheckDisposal();
        return _cache.AllIdentifiers(types);
    }

    /// <inheritdoc />
    public bool TryResolve(FormKey formKey, [MaybeNullWhen(false)] out IMajorRecordGetter majorRec, ResolveTarget target = ResolveTarget.Winner)
    {
        CheckDisposal();
        return _cache.TryResolve(formKey, out majorRec, target);
    }

    /// <inheritdoc />
    public bool TryResolve(string editorId, [MaybeNullWhen(false)] out IMajorRecordGetter majorRec)
    {
        CheckDisposal();
        return _cache.TryResolve(editorId, out majorRec);
    }

    /// <inheritdoc />
    public bool TryResolve<TMajor>(FormKey formKey, [MaybeNullWhen(false)] out TMajor majorRec, ResolveTarget target = ResolveTarget.Winner)
        where TMajor : class, IMajorRecordQueryableGetter
    {
        CheckDisposal();
        return _cache.TryResolve<TMajor>(formKey, out majorRec, target);
    }

    /// <inheritdoc />
    public bool TryResolve<TMajor>(string editorId, [MaybeNullWhen(false)] out TMajor majorRec)
        where TMajor : class, IMajorRecordQueryableGetter
    {
        CheckDisposal();
        return _cache.TryResolve<TMajor>(editorId, out majorRec);
    }

    /// <inheritdoc />
    public bool TryResolve(FormKey formKey, Type type, [MaybeNullWhen(false)] out IMajorRecordGetter majorRec,
        ResolveTarget target = ResolveTarget.Winner)
    {
        CheckDisposal();
        return _cache.TryResolve(formKey, type, out majorRec, target);
    }

    /// <inheritdoc />
    public bool TryResolve(string editorId, Type type, [MaybeNullWhen(false)] out IMajorRecordGetter majorRec)
    {
        CheckDisposal();
        return _cache.TryResolve(editorId, type, out majorRec);
    }

    /// <inheritdoc />
    public bool TryResolve(FormKey formKey, [MaybeNullWhen(false)] out IMajorRecordGetter majorRec, params Type[] types)
    {
        CheckDisposal();
        return _cache.TryResolve(formKey, out majorRec, types);
    }

    /// <inheritdoc />
    public bool TryResolve(string editorId, [MaybeNullWhen(false)] out IMajorRecordGetter majorRec, params Type[] types)
    {
        CheckDisposal();
        return _cache.TryResolve(editorId, out majorRec, types);
    }

    /// <inheritdoc />
    public bool TryResolve(FormKey formKey, IEnumerable<Type> types, [MaybeNullWhen(false)] out IMajorRecordGetter majorRec,
        ResolveTarget target = ResolveTarget.Winner)
    {
        CheckDisposal();
        return _cache.TryResolve(formKey, types, out majorRec, target);
    }

    /// <inheritdoc />
    public bool TryResolve(string editorId, IEnumerable<Type> types, [MaybeNullWhen(false)] out IMajorRecordGetter majorRec)
    {
        CheckDisposal();
        return _cache.TryResolve(editorId, types, out majorRec);
    }

    /// <inheritdoc />
    public IMajorRecordGetter Resolve(FormKey formKey, ResolveTarget target = ResolveTarget.Winner)
    {
        CheckDisposal();
        return _cache.Resolve(formKey, target);
    }

    /// <inheritdoc />
    public IMajorRecordGetter Resolve(string editorId)
    {
        CheckDisposal();
        return _cache.Resolve(editorId);
    }

    /// <inheritdoc />
    public IMajorRecordGetter Resolve(FormKey formKey, Type type, ResolveTarget target = ResolveTarget.Winner)
    {
        CheckDisposal();
        return _cache.Resolve(formKey, type, target);
    }

    /// <inheritdoc />
    public IMajorRecordGetter Resolve(string editorId, Type type)
    {
        CheckDisposal();
        return _cache.Resolve(editorId, type);
    }

    /// <inheritdoc />
    public IMajorRecordGetter Resolve(FormKey formKey, params Type[] types)
    {
        CheckDisposal();
        return _cache.Resolve(formKey, types);
    }

    /// <inheritdoc />
    public IMajorRecordGetter Resolve(string editorId, params Type[] types)
    {
        CheckDisposal();
        return _cache.Resolve(editorId, types);
    }

    /// <inheritdoc />
    public IMajorRecordGetter Resolve(FormKey formKey, IEnumerable<Type> types, ResolveTarget target = ResolveTarget.Winner)
    {
        CheckDisposal();
        return _cache.Resolve(formKey, types, target);
    }

    /// <inheritdoc />
    public IMajorRecordGetter Resolve(string editorId, IEnumerable<Type> types)
    {
        CheckDisposal();
        return _cache.Resolve(editorId, types);
    }

    /// <inheritdoc />
    public TMajor Resolve<TMajor>(FormKey formKey, ResolveTarget target = ResolveTarget.Winner) 
        where TMajor : class, IMajorRecordQueryableGetter
    {
        CheckDisposal();
        return _cache.Resolve<TMajor>(formKey, target);
    }

    /// <inheritdoc />
    public TMajor Resolve<TMajor>(string editorId)
        where TMajor : class, IMajorRecordQueryableGetter
    {
        CheckDisposal();
        return _cache.Resolve<TMajor>(editorId);
    }

    /// <inheritdoc />
    public IEnumerable<TMajor> ResolveAll<TMajor>(FormKey formKey, ResolveTarget target = ResolveTarget.Winner) 
        where TMajor : class, IMajorRecordQueryableGetter
    {
        CheckDisposal();
        return _cache.ResolveAll<TMajor>(formKey, target);
    }

    /// <inheritdoc />
    public IEnumerable<IMajorRecordGetter> ResolveAll(FormKey formKey, Type type, ResolveTarget target = ResolveTarget.Winner)
    {
        CheckDisposal();
        return _cache.ResolveAll(formKey, type, target);
    }

    /// <inheritdoc />
    public IEnumerable<IMajorRecordGetter> ResolveAll(FormKey formKey, ResolveTarget target = ResolveTarget.Winner)
    {
        CheckDisposal();
        return _cache.ResolveAll(formKey, target);
    }

    /// <inheritdoc />
    public bool TryResolveSimpleContext(FormKey formKey, [MaybeNullWhen(false)] out IModContext<IMajorRecordGetter> majorRec, ResolveTarget target = ResolveTarget.Winner)
    {
        CheckDisposal();
            
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
            
        return _formKeyContexts.TryResolveUntypedSimpleContext(formKey, out majorRec);
    }

    /// <inheritdoc />
    public bool TryResolveSimpleContext(string editorId, [MaybeNullWhen(false)] out IModContext<IMajorRecordGetter> majorRec)
    {
        CheckDisposal();
            
        if (editorId.IsNullOrWhitespace())
        {
            majorRec = default;
            return false;
        }
        return _editorIdContexts.TryResolveUntypedSimpleContext(editorId, out majorRec);
    }

    /// <inheritdoc />
    public bool TryResolveSimpleContext<TMajor>(
        FormKey formKey, 
        [MaybeNullWhen(false)] out IModContext<TMajor> majorRec,
        ResolveTarget target = ResolveTarget.Winner)
        where TMajor : class, IMajorRecordQueryableGetter
    {
        CheckDisposal();
            
        if (target == ResolveTarget.Origin
            && formKey.ModKey != _sourceMod.ModKey)
        {
            majorRec = default;
            return false;
        }
            
        return _formKeyContexts.TryResolveSimpleContext(formKey, out majorRec);
    }

    /// <inheritdoc />
    public bool TryResolveSimpleContext<TMajor>(string editorId, [MaybeNullWhen(false)] out IModContext<TMajor> majorRec)
        where TMajor : class, IMajorRecordQueryableGetter
    {
        CheckDisposal();
        return _editorIdContexts.TryResolveSimpleContext(editorId, out majorRec);
    }

    /// <inheritdoc />
    public bool TryResolveSimpleContext(FormKey formKey, Type type, [MaybeNullWhen(false)] out IModContext<IMajorRecordGetter> majorRec,
        ResolveTarget target = ResolveTarget.Winner)
    {
        CheckDisposal();
            
        if (target == ResolveTarget.Origin
            && formKey.ModKey != _sourceMod.ModKey)
        {
            majorRec = default;
            return false;
        }

        return _formKeyContexts.TryResolveSimpleContext(formKey, type, out majorRec);
    }

    /// <inheritdoc />
    public bool TryResolveSimpleContext(string editorId, Type type, [MaybeNullWhen(false)] out IModContext<IMajorRecordGetter> majorRec)
    {
        CheckDisposal();
        return _editorIdContexts.TryResolveSimpleContext(editorId, type, out majorRec);
    }

    /// <inheritdoc />
    public IModContext<IMajorRecordGetter> ResolveSimpleContext(FormKey formKey, ResolveTarget target = ResolveTarget.Winner)
    {
        if (TryResolveSimpleContext(formKey, out var majorRec, target)) return majorRec;
        throw new MissingRecordException(formKey, typeof(IMajorRecordGetter));
    }

    /// <inheritdoc />
    public IModContext<IMajorRecordGetter> ResolveSimpleContext(string editorId)
    {
        if (TryResolveSimpleContext(editorId, out var majorRec)) return majorRec;
        throw new MissingRecordException(editorId, typeof(IMajorRecordGetter));
    }

    /// <inheritdoc />
    public IModContext<IMajorRecordGetter> ResolveSimpleContext(FormKey formKey, Type type, ResolveTarget target = ResolveTarget.Winner)
    {
        if (TryResolveSimpleContext(formKey, type, out var commonRec, target)) return commonRec;
        throw new MissingRecordException(formKey, type);
    }

    /// <inheritdoc />
    public IModContext<IMajorRecordGetter> ResolveSimpleContext(string editorId, Type type)
    {
        if (TryResolveSimpleContext(editorId, type, out var commonRec)) return commonRec;
        throw new MissingRecordException(editorId, type);
    }

    /// <inheritdoc />
    public IModContext<TMajor> ResolveSimpleContext<TMajor>(FormKey formKey, ResolveTarget target = ResolveTarget.Winner) 
        where TMajor : class, IMajorRecordQueryableGetter
    {
        if (TryResolveSimpleContext<TMajor>(formKey, out var commonRec, target)) return commonRec;
        throw new MissingRecordException(formKey, typeof(TMajor));
    }

    /// <inheritdoc />
    public IModContext<TMajor> ResolveSimpleContext<TMajor>(string editorId) 
        where TMajor : class, IMajorRecordQueryableGetter
    {
        if (TryResolveSimpleContext<TMajor>(editorId, out var commonRec)) return commonRec;
        throw new MissingRecordException(editorId, typeof(TMajor));
    }

    /// <inheritdoc />
    public IEnumerable<IModContext<TMajor>> ResolveAllSimpleContexts<TMajor>(FormKey formKey, ResolveTarget target = ResolveTarget.Winner)
        where TMajor : class, IMajorRecordQueryableGetter
    {
        if (TryResolveSimpleContext<TMajor>(formKey, out var rec, target))
        {
            yield return rec;
        }
    }

    /// <inheritdoc />
    public IEnumerable<IModContext<IMajorRecordGetter>> ResolveAllSimpleContexts(FormKey formKey, Type type, ResolveTarget target = ResolveTarget.Winner)
    {
        if (TryResolveSimpleContext(formKey, type, out var rec, target))
        {
            yield return rec;
        }
    }

    /// <inheritdoc />
    public IEnumerable<IModContext<IMajorRecordGetter>> ResolveAllSimpleContexts(FormKey formKey, ResolveTarget target = ResolveTarget.Winner)
    {
        if (TryResolveSimpleContext(formKey, out var rec, target))
        {
            yield return rec;
        }
    }

    /// <inheritdoc />
    public void Warmup(Type type)
    {
        CheckDisposal();
        _cache.Warmup(type);
    }

    /// <inheritdoc />
    public void Warmup<TMajor>()
    {
        CheckDisposal();
        _cache.Warmup<TMajor>();
    }

    /// <inheritdoc />
    public void Warmup(params Type[] types)
    {
        CheckDisposal();
        _cache.Warmup(types);
    }

    /// <inheritdoc />
    public void Warmup(IEnumerable<Type> types)
    {
        CheckDisposal();
        _cache.Warmup(types);
    }

    /// <inheritdoc />
    public IReadOnlyList<IModGetter> ListedOrder
    {
        get
        {
            CheckDisposal();
            return _cache.ListedOrder;
        }
    }

    /// <inheritdoc />
    public IReadOnlyList<IModGetter> PriorityOrder
    {
        get
        {
            CheckDisposal();
            return _cache.PriorityOrder;
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
public class ImmutableModLinkCache<TMod, TModGetter> : ILinkCache<TMod, TModGetter>
    where TMod : class, IContextMod<TMod, TModGetter>, TModGetter
    where TModGetter : class, IContextGetterMod<TMod, TModGetter>
{
    internal readonly TModGetter _sourceMod;
    internal readonly bool _simple;
    private bool _disposed;

    private readonly InternalImmutableModLinkCache _cache;
    private readonly IImmutableModLinkCacheContextCategory<TMod, TModGetter, FormKey> _formKeyContexts;
    private readonly IImmutableModLinkCacheContextCategory<TMod, TModGetter, string> _editorIdContexts;

    /// <summary>
    /// Constructs a link cache around a target mod
    /// </summary>
    /// <param name="sourceMod">Mod to resolve against when linking</param>
    public ImmutableModLinkCache(TModGetter sourceMod, LinkCachePreferences prefs)
    {
        this._sourceMod = sourceMod;
        _cache = new InternalImmutableModLinkCache(sourceMod, prefs);
        _simple = prefs.Retention == LinkCachePreferences.RetentionType.OnlyIdentifiers;
        _formKeyContexts = new ImmutableModLinkCacheContextCategory<TMod, TModGetter, FormKey>(
            parent: this,
            metaInterfaceMapGetter: prefs.MetaInterfaceMapGetterOverride ?? MetaInterfaceMapping.Instance,
            keyGetter: m => TryGet<FormKey>.Succeed(m.FormKey),
            shortCircuit: f => f.IsNull);
        _editorIdContexts = new ImmutableModLinkCacheContextCategory<TMod, TModGetter, string>(
            parent: this,
            metaInterfaceMapGetter: prefs.MetaInterfaceMapGetterOverride ?? MetaInterfaceMapping.Instance,
            keyGetter: m =>
            {
                var edid = m.EditorID;
                return TryGet<string>.Create(successful: !string.IsNullOrWhiteSpace(edid), edid!);
            },
            shortCircuit: e => e.IsNullOrWhitespace());
    }

    private void CheckDisposal()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException($"{nameof(ImmutableModLinkCache)}<{typeof(TMod)}, {typeof(TModGetter)}>");
        }
    }

    /// <inheritdoc />
    [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
    public bool TryResolveContext(FormKey formKey, [MaybeNullWhen(false)] out IModContext<TMod, TModGetter, IMajorRecord, IMajorRecordGetter> majorRec, ResolveTarget target = ResolveTarget.Winner)
    {
        CheckDisposal();
            
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
    public bool TryResolveContext(string editorId, [MaybeNullWhen(false)] out IModContext<TMod, TModGetter, IMajorRecord, IMajorRecordGetter> majorRec)
    {
        CheckDisposal();
            
        if (editorId.IsNullOrWhitespace())
        {
            majorRec = default;
            return false;
        }
        return _editorIdContexts.TryResolveUntypedContext(editorId, out majorRec);
    }

    /// <inheritdoc />
    public bool TryResolveContext<TMajor, TMajorGetter>(FormKey formKey, [MaybeNullWhen(false)] out IModContext<TMod, TModGetter, TMajor, TMajorGetter> majorRec, ResolveTarget target = ResolveTarget.Winner)
        where TMajor : class, IMajorRecordQueryable, TMajorGetter
        where TMajorGetter : class, IMajorRecordQueryableGetter
    {
        CheckDisposal();
            
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
        where TMajor : class, IMajorRecordQueryable, TMajorGetter
        where TMajorGetter : class, IMajorRecordQueryableGetter
    {
        CheckDisposal();
        return _editorIdContexts.TryResolveContext(editorId, out majorRec);
    }

    /// <inheritdoc />
    public bool TryResolveContext(FormKey formKey, Type type, [MaybeNullWhen(false)] out IModContext<TMod, TModGetter, IMajorRecord, IMajorRecordGetter> majorRec, ResolveTarget target = ResolveTarget.Winner)
    {
        CheckDisposal();
            
        if (target == ResolveTarget.Origin
            && formKey.ModKey != _sourceMod.ModKey)
        {
            majorRec = default;
            return false;
        }

        return _formKeyContexts.TryResolveContext(formKey, type, out majorRec);
    }

    /// <inheritdoc />
    public bool TryResolveContext(string editorId, Type type, [MaybeNullWhen(false)] out IModContext<TMod, TModGetter, IMajorRecord, IMajorRecordGetter> majorRec)
    {
        CheckDisposal();
        return _editorIdContexts.TryResolveContext(editorId, type, out majorRec);
    }

    /// <inheritdoc />
    [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
    public IModContext<TMod, TModGetter, IMajorRecord, IMajorRecordGetter> ResolveContext(FormKey formKey, ResolveTarget target = ResolveTarget.Winner)
    {
        if (TryResolveContext<IMajorRecord, IMajorRecordGetter>(formKey, out var majorRec, target)) return majorRec;
        throw new MissingRecordException(formKey, typeof(IMajorRecordGetter));
    }

    /// <inheritdoc />
    [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
    public IModContext<TMod, TModGetter, IMajorRecord, IMajorRecordGetter> ResolveContext(string editorId)
    {
        if (TryResolveContext<IMajorRecord, IMajorRecordGetter>(editorId, out var majorRec)) return majorRec;
        throw new MissingRecordException(editorId, typeof(IMajorRecordGetter));
    }

    /// <inheritdoc />
    public IModContext<TMod, TModGetter, IMajorRecord, IMajorRecordGetter> ResolveContext(FormKey formKey, Type type, ResolveTarget target = ResolveTarget.Winner)
    {
        if (TryResolveContext(formKey, type, out var commonRec, target)) return commonRec;
        throw new MissingRecordException(formKey, type);
    }

    /// <inheritdoc />
    public IModContext<TMod, TModGetter, IMajorRecord, IMajorRecordGetter> ResolveContext(string editorId, Type type)
    {
        if (TryResolveContext(editorId, type, out var commonRec)) return commonRec;
        throw new MissingRecordException(editorId, type);
    }

    /// <inheritdoc />
    public IModContext<TMod, TModGetter, TMajor, TMajorGetter> ResolveContext<TMajor, TMajorGetter>(FormKey formKey, ResolveTarget target = ResolveTarget.Winner)
        where TMajor : class, IMajorRecordQueryable, TMajorGetter
        where TMajorGetter : class, IMajorRecordQueryableGetter
    {
        if (TryResolveContext<TMajor, TMajorGetter>(formKey, out var commonRec, target)) return commonRec;
        throw new MissingRecordException(formKey, typeof(TMajorGetter));
    }

    /// <inheritdoc />
    public IModContext<TMod, TModGetter, TMajor, TMajorGetter> ResolveContext<TMajor, TMajorGetter>(string editorId)
        where TMajor : class, IMajorRecordQueryable, TMajorGetter
        where TMajorGetter : class, IMajorRecordQueryableGetter
    {
        if (TryResolveContext<TMajor, TMajorGetter>(editorId, out var commonRec)) return commonRec;
        throw new MissingRecordException(editorId, typeof(TMajorGetter));
    }

    /// <inheritdoc />
    public IEnumerable<IModContext<TMod, TModGetter, TMajor, TMajorGetter>> ResolveAllContexts<TMajor, TMajorGetter>(FormKey formKey, ResolveTarget target = ResolveTarget.Winner)
        where TMajor : class, IMajorRecordQueryable, TMajorGetter
        where TMajorGetter : class, IMajorRecordQueryableGetter
    {
        if (TryResolveContext<TMajor, TMajorGetter>(formKey, out var rec, target))
        {
            yield return rec;
        }
    }
        
    /// <inheritdoc />
    public IEnumerable<IModContext<TMod, TModGetter, IMajorRecord, IMajorRecordGetter>> ResolveAllContexts(FormKey formKey, Type type, ResolveTarget target = ResolveTarget.Winner)
    {
        if (TryResolveContext(formKey, type, out var rec, target))
        {
            yield return rec;
        }
    }

    /// <inheritdoc />
    [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
    public IEnumerable<IModContext<TMod, TModGetter, IMajorRecord, IMajorRecordGetter>> ResolveAllContexts(FormKey formKey, ResolveTarget target = ResolveTarget.Winner)
    {
        if (TryResolveContext(formKey, out var rec, target))
        {
            yield return rec;
        }
    }

    public void Dispose()
    {
        _disposed = true;
        _cache.Dispose();
    }

    public bool TryResolveIdentifier(FormKey formKey, out string? editorId, ResolveTarget target = ResolveTarget.Winner)
    {
        CheckDisposal();
        return _cache.TryResolveIdentifier(formKey, out editorId, target);
    }

    public bool TryResolveIdentifier(string editorId, out FormKey formKey)
    {
        CheckDisposal();
        return _cache.TryResolveIdentifier(editorId, out formKey);
    }

    public bool TryResolveIdentifier(FormKey formKey, Type type, out string? editorId,
        ResolveTarget target = ResolveTarget.Winner)
    {
        CheckDisposal();
        return _cache.TryResolveIdentifier(formKey, type, out editorId, target);
    }

    public bool TryResolveIdentifier(string editorId, Type type, out FormKey formKey)
    {
        CheckDisposal();
        return _cache.TryResolveIdentifier(editorId, type, out formKey);
    }

    public bool TryResolveIdentifier<TMajor>(FormKey formKey, out string? editorId, ResolveTarget target = ResolveTarget.Winner) 
        where TMajor : class, IMajorRecordQueryableGetter
    {
        CheckDisposal();
        return _cache.TryResolveIdentifier<TMajor>(formKey, out editorId, target);
    }

    public bool TryResolveIdentifier<TMajor>(string editorId, out FormKey formKey) 
        where TMajor : class, IMajorRecordQueryableGetter
    {
        CheckDisposal();
        return _cache.TryResolveIdentifier<TMajor>(editorId, out formKey);
    }

    public bool TryResolveIdentifier(FormKey formKey, out string? editorId, params Type[] types)
    {
        CheckDisposal();
        return _cache.TryResolveIdentifier(formKey, out editorId, types);
    }

    public bool TryResolveIdentifier(string editorId, out FormKey formKey, params Type[] types)
    {
        CheckDisposal();
        return _cache.TryResolveIdentifier(editorId, out formKey, types);
    }

    public bool TryResolveIdentifier(FormKey formKey, IEnumerable<Type> types, out string? editorId,
        ResolveTarget target = ResolveTarget.Winner)
    {
        CheckDisposal();
        return _cache.TryResolveIdentifier(formKey, types, out editorId, target);
    }

    public bool TryResolveIdentifier(string editorId, IEnumerable<Type> types, out FormKey formKey)
    {
        CheckDisposal();
        return _cache.TryResolveIdentifier(editorId, types, out formKey);
    }

    public IEnumerable<IMajorRecordIdentifier> AllIdentifiers(Type type, CancellationToken? cancel = null)
    {
        CheckDisposal();
        return _cache.AllIdentifiers(type, cancel);
    }

    public IEnumerable<IMajorRecordIdentifier> AllIdentifiers<TMajor>(CancellationToken? cancel = null) 
        where TMajor : class, IMajorRecordQueryableGetter
    {
        CheckDisposal();
        return _cache.AllIdentifiers<TMajor>(cancel);
    }

    public IEnumerable<IMajorRecordIdentifier> AllIdentifiers(IEnumerable<Type> types, CancellationToken? cancel = null)
    {
        CheckDisposal();
        return _cache.AllIdentifiers(types, cancel);
    }

    public IEnumerable<IMajorRecordIdentifier> AllIdentifiers(params Type[] types)
    {
        CheckDisposal();
        return _cache.AllIdentifiers(types);
    }

    public bool TryResolve(FormKey formKey, [MaybeNullWhen(false)] out IMajorRecordGetter majorRec, ResolveTarget target = ResolveTarget.Winner)
    {
        CheckDisposal();
        return _cache.TryResolve(formKey, out majorRec, target);
    }

    public bool TryResolve(string editorId, [MaybeNullWhen(false)] out IMajorRecordGetter majorRec)
    {
        CheckDisposal();
        return _cache.TryResolve(editorId, out majorRec);
    }

    public bool TryResolve<TMajor>(FormKey formKey, [MaybeNullWhen(false)] out TMajor majorRec, ResolveTarget target = ResolveTarget.Winner) 
        where TMajor : class, IMajorRecordQueryableGetter
    {
        CheckDisposal();
        return _cache.TryResolve<TMajor>(formKey, out majorRec, target);
    }

    public bool TryResolve<TMajor>(string editorId, [MaybeNullWhen(false)] out TMajor majorRec)
        where TMajor : class, IMajorRecordQueryableGetter
    {
        CheckDisposal();
        return _cache.TryResolve<TMajor>(editorId, out majorRec);
    }

    public bool TryResolve(FormKey formKey, Type type, [MaybeNullWhen(false)] out IMajorRecordGetter majorRec,
        ResolveTarget target = ResolveTarget.Winner)
    {
        CheckDisposal();
        return _cache.TryResolve(formKey, type, out majorRec, target);
    }

    public bool TryResolve(string editorId, Type type, [MaybeNullWhen(false)] out IMajorRecordGetter majorRec)
    {
        CheckDisposal();
        return _cache.TryResolve(editorId, type, out majorRec);
    }

    public bool TryResolve(FormKey formKey, [MaybeNullWhen(false)] out IMajorRecordGetter majorRec, params Type[] types)
    {
        CheckDisposal();
        return _cache.TryResolve(formKey, out majorRec, types);
    }

    public bool TryResolve(string editorId, [MaybeNullWhen(false)] out IMajorRecordGetter majorRec, params Type[] types)
    {
        CheckDisposal();
        return _cache.TryResolve(editorId, out majorRec, types);
    }

    public bool TryResolve(FormKey formKey, IEnumerable<Type> types, [MaybeNullWhen(false)] out IMajorRecordGetter majorRec,
        ResolveTarget target = ResolveTarget.Winner)
    {
        CheckDisposal();
        return _cache.TryResolve(formKey, types, out majorRec, target);
    }

    public bool TryResolve(string editorId, IEnumerable<Type> types, [MaybeNullWhen(false)] out IMajorRecordGetter majorRec)
    {
        CheckDisposal();
        return _cache.TryResolve(editorId, types, out majorRec);
    }

    public IMajorRecordGetter Resolve(FormKey formKey, ResolveTarget target = ResolveTarget.Winner)
    {
        CheckDisposal();
        return _cache.Resolve(formKey, target);
    }

    public IMajorRecordGetter Resolve(string editorId)
    {
        CheckDisposal();
        return _cache.Resolve(editorId);
    }

    public IMajorRecordGetter Resolve(FormKey formKey, Type type, ResolveTarget target = ResolveTarget.Winner)
    {
        CheckDisposal();
        return _cache.Resolve(formKey, type, target);
    }

    public IMajorRecordGetter Resolve(string editorId, Type type)
    {
        CheckDisposal();
        return _cache.Resolve(editorId, type);
    }

    public IMajorRecordGetter Resolve(FormKey formKey, params Type[] types)
    {
        CheckDisposal();
        return _cache.Resolve(formKey, types);
    }

    public IMajorRecordGetter Resolve(string editorId, params Type[] types)
    {
        CheckDisposal();
        return _cache.Resolve(editorId, types);
    }

    public IMajorRecordGetter Resolve(FormKey formKey, IEnumerable<Type> types, ResolveTarget target = ResolveTarget.Winner)
    {
        CheckDisposal();
        return _cache.Resolve(formKey, types, target);
    }

    public IMajorRecordGetter Resolve(string editorId, IEnumerable<Type> types)
    {
        CheckDisposal();
        return _cache.Resolve(editorId, types);
    }

    public TMajor Resolve<TMajor>(FormKey formKey, ResolveTarget target = ResolveTarget.Winner) 
        where TMajor : class, IMajorRecordQueryableGetter
    {
        CheckDisposal();
        return _cache.Resolve<TMajor>(formKey, target);
    }

    public TMajor Resolve<TMajor>(string editorId) 
        where TMajor : class, IMajorRecordQueryableGetter
    {
        CheckDisposal();
        return _cache.Resolve<TMajor>(editorId);
    }

    public IEnumerable<TMajor> ResolveAll<TMajor>(FormKey formKey, ResolveTarget target = ResolveTarget.Winner)
        where TMajor : class, IMajorRecordQueryableGetter
    {
        CheckDisposal();
        return _cache.ResolveAll<TMajor>(formKey, target);
    }

    public IEnumerable<IMajorRecordGetter> ResolveAll(FormKey formKey, Type type, ResolveTarget target = ResolveTarget.Winner)
    {
        CheckDisposal();
        return _cache.ResolveAll(formKey, type, target);
    }

    public IEnumerable<IMajorRecordGetter> ResolveAll(FormKey formKey, ResolveTarget target = ResolveTarget.Winner)
    {
        CheckDisposal();
        return _cache.ResolveAll(formKey, target);
    }

    public bool TryResolveSimpleContext(FormKey formKey, [MaybeNullWhen(false)] out IModContext<IMajorRecordGetter> majorRec, ResolveTarget target = ResolveTarget.Winner)
    {
        if (TryResolveContext(formKey, out var context, target))
        {
            majorRec = context;
            return true;
        }

        majorRec = default;
        return false;
    }

    public bool TryResolveSimpleContext(string editorId, [MaybeNullWhen(false)] out IModContext<IMajorRecordGetter> majorRec)
    {
        if (TryResolveContext(editorId, out var context))
        {
            majorRec = context;
            return true;
        }

        majorRec = default;
        return false;
    }

    public bool TryResolveSimpleContext<TMajor>(
        FormKey formKey, 
        [MaybeNullWhen(false)] out IModContext<TMajor> majorRec,
        ResolveTarget target = ResolveTarget.Winner) 
        where TMajor : class, IMajorRecordQueryableGetter
    {
        CheckDisposal();
            
        if (target == ResolveTarget.Origin
            && formKey.ModKey != _sourceMod.ModKey)
        {
            majorRec = default;
            return false;
        }
            
        return _formKeyContexts.TryResolveSimpleContext(formKey, out majorRec);
    }

    public bool TryResolveSimpleContext<TMajor>(string editorId, [MaybeNullWhen(false)] out IModContext<TMajor> majorRec) 
        where TMajor : class, IMajorRecordQueryableGetter
    {
        CheckDisposal();
        return _editorIdContexts.TryResolveSimpleContext(editorId, out majorRec);
    }

    public bool TryResolveSimpleContext(FormKey formKey, Type type, [MaybeNullWhen(false)] out IModContext<IMajorRecordGetter> majorRec,
        ResolveTarget target = ResolveTarget.Winner)
    {
        if (TryResolveContext(formKey, type, out var context))
        {
            majorRec = context;
            return true;
        }

        majorRec = default;
        return false;
    }

    public bool TryResolveSimpleContext(string editorId, Type type, [MaybeNullWhen(false)] out IModContext<IMajorRecordGetter> majorRec)
    {
        if (TryResolveContext(editorId, type, out var context))
        {
            majorRec = context;
            return true;
        }

        majorRec = default;
        return false;
    }

    public IModContext<IMajorRecordGetter> ResolveSimpleContext(FormKey formKey, ResolveTarget target)
    {
        return ResolveContext(formKey, target);
    }

    public IModContext<IMajorRecordGetter> ResolveSimpleContext(string editorId)
    {
        return ResolveContext(editorId);
    }

    public IModContext<IMajorRecordGetter> ResolveSimpleContext(FormKey formKey, Type type, ResolveTarget target)
    {
        return ResolveContext(formKey, type, target);
    }

    public IModContext<IMajorRecordGetter> ResolveSimpleContext(string editorId, Type type)
    {
        return ResolveContext(editorId, type);
    }

    public IModContext<TMajor> ResolveSimpleContext<TMajor>(FormKey formKey, ResolveTarget target = ResolveTarget.Winner) 
        where TMajor : class, IMajorRecordQueryableGetter
    {
        if (TryResolveSimpleContext<TMajor>(formKey, out var commonRec, target)) return commonRec;
        throw new MissingRecordException(formKey, typeof(TMajor));
    }

    public IModContext<TMajor> ResolveSimpleContext<TMajor>(string editorId)
        where TMajor : class, IMajorRecordQueryableGetter
    {
        if (TryResolveSimpleContext<TMajor>(editorId, out var commonRec)) return commonRec;
        throw new MissingRecordException(editorId, typeof(TMajor));
    }

    public IEnumerable<IModContext<TMajor>> ResolveAllSimpleContexts<TMajor>(FormKey formKey, ResolveTarget target = ResolveTarget.Winner)
        where TMajor : class, IMajorRecordQueryableGetter
    {
        if (TryResolveSimpleContext<TMajor>(formKey, out var rec, target))
        {
            yield return rec;
        }
    }

    public IEnumerable<IModContext<IMajorRecordGetter>> ResolveAllSimpleContexts(FormKey formKey, Type type, ResolveTarget target)
    {
        return ResolveAllContexts(formKey, type, target);
    }

    public IEnumerable<IModContext<IMajorRecordGetter>> ResolveAllSimpleContexts(FormKey formKey, ResolveTarget target)
    {
        return ResolveAllContexts(formKey, target);
    }

    public void Warmup(Type type)
    {
        CheckDisposal();
        _cache.Warmup(type);
    }

    public void Warmup<TMajor>()
    {
        CheckDisposal();
        _cache.Warmup<TMajor>();
    }

    public void Warmup(params Type[] types)
    {
        CheckDisposal();
        _cache.Warmup(types);
    }

    public void Warmup(IEnumerable<Type> types)
    {
        CheckDisposal();
        _cache.Warmup(types);
    }

    public IReadOnlyList<IModGetter> ListedOrder
    {
        get
        {
            CheckDisposal();
            return _cache.ListedOrder;
        }
    }

    public IReadOnlyList<IModGetter> PriorityOrder
    {
        get
        {
            CheckDisposal();
            return _cache.PriorityOrder;
        }
    }
}