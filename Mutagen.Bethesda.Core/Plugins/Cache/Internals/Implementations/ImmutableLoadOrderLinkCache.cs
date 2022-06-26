using System.Diagnostics.CodeAnalysis;
using Mutagen.Bethesda.Plugins.Cache.Internals.Implementations.Internal;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Records.Mapping;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Cache.Internals.Implementations;

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
    private bool _disposed;

    public ImmutableLoadOrderLinkCache(
        IEnumerable<IModGetter> loadOrder, 
        GameCategory? gameCategory, 
        LinkCachePreferences? prefs)
    {
        var loadOrderArr = loadOrder.ToArray();
        var firstMod = loadOrderArr.FirstOrDefault();
        _hasAny = firstMod != null;
        var simple = (prefs ?? LinkCachePreferences.Default).Retention == LinkCachePreferences.RetentionType.OnlyIdentifiers;
        gameCategory ??= firstMod?.GameRelease.ToCategory() ?? throw new ArgumentException($"Could not get {nameof(GameCategory)} via generic type or first mod");
        _cache = new InternalImmutableLoadOrderLinkCache(
            loadOrderArr,
            gameCategory.Value,
            hasAny: _hasAny,
            simple: simple,
            prefs);
        _formKeyContexts = new ImmutableLoadOrderLinkCacheSimpleContextCategory<FormKey>(
            gameCategory.Value,
            metaInterfaceMapGetter: prefs?.MetaInterfaceMapGetterOverride ?? MetaInterfaceMapping.Instance,
            simple: simple,
            hasAny: _hasAny,
            this,
            loadOrderArr,
            m => TryGet<FormKey>.Succeed(m.FormKey),
            f => f.IsNull);
        _editorIdContexts = new ImmutableLoadOrderLinkCacheSimpleContextCategory<string>(
            gameCategory.Value,
            metaInterfaceMapGetter: prefs?.MetaInterfaceMapGetterOverride ?? MetaInterfaceMapping.Instance,
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
        _disposed = true;
        _cache.Dispose();
    }

    private void CheckDisposal()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(ImmutableLoadOrderLinkCache));
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
    public bool TryResolveIdentifier(IFormLinkIdentifier formLink, out string? editorId,
        ResolveTarget target = ResolveTarget.Winner)
    {
        return TryResolveIdentifier(formLink.FormKey, formLink.Type, out editorId, target);
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
    [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
    public bool TryResolve(FormKey formKey, [MaybeNullWhen(false)] out IMajorRecordGetter majorRec, ResolveTarget target = ResolveTarget.Winner)
    {
        CheckDisposal();
        return _cache.TryResolve(formKey, out majorRec, target);
    }

    /// <inheritdoc />
    [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
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
    public bool TryResolve(IFormLinkIdentifier formLink, [MaybeNullWhen(false)] out IMajorRecordGetter majorRec,
        ResolveTarget target = ResolveTarget.Winner)
    {
        return TryResolve(formLink.FormKey, formLink.Type, out majorRec, target);
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
    [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
    public IMajorRecordGetter Resolve(FormKey formKey, ResolveTarget target = ResolveTarget.Winner)
    {
        CheckDisposal();
        return _cache.Resolve(formKey, target);
    }

    /// <inheritdoc />
    [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
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
    public IMajorRecordGetter Resolve(IFormLinkIdentifier formLink, ResolveTarget target = ResolveTarget.Winner)
    {
        return Resolve(formLink.FormKey, formLink.Type, target);
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
    public IEnumerable<IMajorRecordGetter> ResolveAll(IFormLinkIdentifier formLink, ResolveTarget target = ResolveTarget.Winner)
    {
        return ResolveAll(formLink.FormKey, formLink.Type, target);
    }

    /// <inheritdoc />
    [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
    public IEnumerable<IMajorRecordGetter> ResolveAll(FormKey formKey, ResolveTarget target = ResolveTarget.Winner)
    {
        CheckDisposal();
        return _cache.ResolveAll(formKey, target);
    }

    /// <inheritdoc />
    [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
    public bool TryResolveSimpleContext(FormKey formKey, [MaybeNullWhen(false)] out IModContext<IMajorRecordGetter> majorRec, ResolveTarget target = ResolveTarget.Winner)
    {
        CheckDisposal();
            
        if (target == ResolveTarget.Origin)
        {
            if (!_modsByKey.TryGetValue(formKey.ModKey, out var origMod))
            {
                majorRec = default;
                return false;
            }

            return origMod.TryResolveSimpleContext(formKey, out majorRec);
        }
            
        return _formKeyContexts.TryResolveSimpleContext(formKey, formKey.ModKey, typeof(IMajorRecordGetter), out majorRec);
    }

    /// <inheritdoc />
    public bool TryResolveSimpleContext(string editorId, [MaybeNullWhen(false)] out IModContext<IMajorRecordGetter> majorRec)
    {
        CheckDisposal();
            
        if (!_hasAny || string.IsNullOrWhiteSpace(editorId))
        {
            majorRec = default;
            return false;
        }
            
        return _editorIdContexts.TryResolveSimpleContext(editorId, default(ModKey?), typeof(IMajorRecordGetter), out majorRec);
    }

    /// <inheritdoc />
    public bool TryResolveSimpleContext<TMajor>(
        FormKey formKey, 
        [MaybeNullWhen(false)] out IModContext<TMajor> majorRec,
        ResolveTarget target = ResolveTarget.Winner) 
        where TMajor : class, IMajorRecordQueryableGetter
    {
        CheckDisposal();
            
        if (!TryResolveSimpleContext(formKey, typeof(TMajor), out var commonRec, target)
            || !(commonRec.Record is TMajor))
        {
            majorRec = default;
            return false;
        }

        majorRec = commonRec.AsType<IMajorRecordQueryableGetter, TMajor>();
        return true;
    }

    /// <inheritdoc />
    public bool TryResolveSimpleContext<TMajor>(string editorId, [MaybeNullWhen(false)] out IModContext<TMajor> majorRec) 
        where TMajor : class, IMajorRecordQueryableGetter
    {
        CheckDisposal();
            
        if (!TryResolveSimpleContext(editorId, typeof(TMajor), out var commonRec)
            || !(commonRec.Record is TMajor))
        {
            majorRec = default;
            return false;
        }

        majorRec = commonRec.AsType<IMajorRecordQueryableGetter, TMajor>();
        return true;
    }

    /// <inheritdoc />
    public bool TryResolveSimpleContext(
        FormKey formKey, Type type,
        [MaybeNullWhen(false)] out IModContext<IMajorRecordGetter> majorRec,
        ResolveTarget target = ResolveTarget.Winner)
    {
        CheckDisposal();
            
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
    public bool TryResolveSimpleContext(
        IFormLinkIdentifier formLink,
        [MaybeNullWhen(false)] out IModContext<IMajorRecordGetter> majorRec,
        ResolveTarget target = ResolveTarget.Winner)
    {
        return TryResolveSimpleContext(formLink.FormKey, formLink.Type, out majorRec, target);
    }

    /// <inheritdoc />
    public bool TryResolveSimpleContext(string editorId, Type type, [MaybeNullWhen(false)] out IModContext<IMajorRecordGetter> majorRec)
    {
        CheckDisposal();
            
        if (!_hasAny || string.IsNullOrWhiteSpace(editorId))
        {
            majorRec = default;
            return false;
        }

        return _editorIdContexts.TryResolveSimpleContext(editorId, default(ModKey?), type, out majorRec);
    }

    /// <inheritdoc />
    [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
    public IModContext<IMajorRecordGetter> ResolveSimpleContext(FormKey formKey, ResolveTarget target = ResolveTarget.Winner)
    {
        if (TryResolveSimpleContext(formKey, out var commonRec, target)) return commonRec;
        throw new MissingRecordException(formKey, typeof(IMajorRecordGetter));
    }

    /// <inheritdoc />
    [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
    public IModContext<IMajorRecordGetter> ResolveSimpleContext(string editorId)
    {
        if (TryResolveSimpleContext(editorId, out var commonRec)) return commonRec;
        throw new MissingRecordException(editorId, typeof(IMajorRecordGetter));
    }

    /// <inheritdoc />
    public IModContext<IMajorRecordGetter> ResolveSimpleContext(FormKey formKey, Type type, ResolveTarget target = ResolveTarget.Winner)
    {
        if (TryResolveSimpleContext(formKey, type, out var commonRec, target)) return commonRec;
        throw new MissingRecordException(formKey, type);
    }

    /// <inheritdoc />
    public IModContext<IMajorRecordGetter> ResolveSimpleContext(IFormLinkIdentifier formLink, ResolveTarget target = ResolveTarget.Winner)
    {
        return ResolveSimpleContext(formLink.FormKey, formLink.Type, target);
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
        return ResolveAllSimpleContexts(formKey, typeof(TMajor), target)
            .Select(c => c.AsType<IMajorRecordQueryableGetter, TMajor>());
    }

    /// <inheritdoc />
    public IEnumerable<IModContext<IMajorRecordGetter>> ResolveAllSimpleContexts(FormKey formKey, Type type, ResolveTarget target = ResolveTarget.Winner)
    {
        CheckDisposal();
            
        // Break early if no content
        if (!_hasAny || formKey.IsNull)
        {
            return Enumerable.Empty<IModContext<IMajorRecordGetter>>();
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
    public IEnumerable<IModContext<IMajorRecordGetter>> ResolveAllSimpleContexts(IFormLinkIdentifier formLink, ResolveTarget target = ResolveTarget.Winner)
    {
        return ResolveAllSimpleContexts(formLink.FormKey, formLink.Type, target);
    }

    /// <inheritdoc />
    public IEnumerable<IModContext<IMajorRecordGetter>> ResolveAllSimpleContexts(FormKey formKey, ResolveTarget target = ResolveTarget.Winner)
    {
        return ResolveAllSimpleContexts(formKey, typeof(IMajorRecordGetter), target);
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
    private bool _disposed;

    /// <summary>
    /// Constructs a LoadOrderLinkCache around a target load order
    /// </summary>
    /// <param name="loadOrder">LoadOrder to resolve against when linking</param>
    public ImmutableLoadOrderLinkCache(IEnumerable<TModGetter> loadOrder, LinkCachePreferences prefs)
    {
        var listedOrder = loadOrder.ToList();
        var simple = prefs.Retention == LinkCachePreferences.RetentionType.OnlyIdentifiers;
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
            metaInterfaceMapGetter: prefs.MetaInterfaceMapGetterOverride ?? MetaInterfaceMapping.Instance,
            simple: simple,
            hasAny: _hasAny,
            linkCache: this,
            listedOrder: listedOrder,
            m => TryGet<FormKey>.Succeed(m.FormKey),
            f => f.IsNull);
        _editorIdContextCache = new ImmutableLoadOrderLinkCacheContextCategory<TMod, TModGetter, string>(
            category: gameCategory,
            metaInterfaceMapGetter: prefs.MetaInterfaceMapGetterOverride ?? MetaInterfaceMapping.Instance,
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

    private void CheckDisposal()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException($"{nameof(ImmutableLoadOrderLinkCache)}<{typeof(TMod)}, {typeof(TModGetter)}>");
        }
    }

    public void Dispose()
    {
        _disposed = true;
        _cache.Dispose();
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
    public bool TryResolveIdentifier(IFormLinkIdentifier formLink, out string? editorId,
        ResolveTarget target = ResolveTarget.Winner)
    {
        return TryResolveIdentifier(formLink.FormKey, formLink.Type, out editorId, target);
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
    [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
    public bool TryResolve(FormKey formKey, [MaybeNullWhen(false)] out IMajorRecordGetter majorRec, ResolveTarget target = ResolveTarget.Winner)
    {
        CheckDisposal();
        return _cache.TryResolve(formKey, out majorRec, target);
    }

    /// <inheritdoc />
    [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
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
    public bool TryResolve(IFormLinkIdentifier formLink, [MaybeNullWhen(false)] out IMajorRecordGetter majorRec,
        ResolveTarget target = ResolveTarget.Winner)
    {
        return TryResolve(formLink.FormKey, formLink.Type, out majorRec, target);
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
    [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
    public IMajorRecordGetter Resolve(FormKey formKey, ResolveTarget target = ResolveTarget.Winner)
    {
        CheckDisposal();
        return _cache.Resolve(formKey, target);
    }

    /// <inheritdoc />
    [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
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
    public IMajorRecordGetter Resolve(IFormLinkIdentifier formLink, ResolveTarget target = ResolveTarget.Winner)
    {
        return Resolve(formLink.FormKey, formLink.Type, target);
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
    public IEnumerable<IMajorRecordGetter> ResolveAll(IFormLinkIdentifier formLink, ResolveTarget target = ResolveTarget.Winner)
    {
        return ResolveAll(formLink.FormKey, formLink.Type, target);
    }

    /// <inheritdoc />
    [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
    public IEnumerable<IMajorRecordGetter> ResolveAll(FormKey formKey, ResolveTarget target = ResolveTarget.Winner)
    {
        CheckDisposal();
        return _cache.ResolveAll(formKey, target);
    }

    /// <inheritdoc />
    [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
    public bool TryResolveSimpleContext(FormKey formKey, [MaybeNullWhen(false)] out IModContext<IMajorRecordGetter> majorRec, ResolveTarget target = ResolveTarget.Winner)
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
    public bool TryResolveSimpleContext(string editorId, [MaybeNullWhen(false)] out IModContext<IMajorRecordGetter> majorRec)
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
    public bool TryResolveSimpleContext<TMajor>(
        FormKey formKey, 
        [MaybeNullWhen(false)] out IModContext<TMajor> majorRec,
        ResolveTarget target = ResolveTarget.Winner)
        where TMajor : class, IMajorRecordQueryableGetter
    {
        if (TryResolveContext(formKey, typeof(TMajor), out var resolve, target))
        {
            majorRec = resolve.AsType<IMajorRecordQueryableGetter, TMajor>();
            return true;
        }

        majorRec = default;
        return false;
    }

    /// <inheritdoc />
    public bool TryResolveSimpleContext<TMajor>(string editorId, [MaybeNullWhen(false)] out IModContext<TMajor> majorRec) 
        where TMajor : class, IMajorRecordQueryableGetter
    {
        if (TryResolveContext(editorId, typeof(TMajor), out var resolve))
        {
            majorRec = resolve.AsType<IMajorRecordQueryableGetter, TMajor>();
            return true;
        }

        majorRec = default;
        return false;
    }

    /// <inheritdoc />
    public bool TryResolveSimpleContext(FormKey formKey, Type type, [MaybeNullWhen(false)] out IModContext<IMajorRecordGetter> majorRec,
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
    public bool TryResolveSimpleContext(IFormLinkIdentifier formLink, [MaybeNullWhen(false)] out IModContext<IMajorRecordGetter> majorRec,
        ResolveTarget target = ResolveTarget.Winner)
    {
        return TryResolveSimpleContext(formLink.FormKey, formLink.Type, out majorRec, target);
    }

    /// <inheritdoc />
    public bool TryResolveSimpleContext(string editorId, Type type, [MaybeNullWhen(false)] out IModContext<IMajorRecordGetter> majorRec)
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
    public IModContext<IMajorRecordGetter> ResolveSimpleContext(FormKey formKey, ResolveTarget target = ResolveTarget.Winner)
    {
        return ResolveContext(formKey, target);
    }

    /// <inheritdoc />
    public IModContext<IMajorRecordGetter> ResolveSimpleContext(string editorId)
    {
        return ResolveContext(editorId);
    }

    /// <inheritdoc />
    public IModContext<IMajorRecordGetter> ResolveSimpleContext(FormKey formKey, Type type, ResolveTarget target = ResolveTarget.Winner)
    {
        return ResolveContext(formKey, type, target);
    }

    /// <inheritdoc />
    public IModContext<IMajorRecordGetter> ResolveSimpleContext(IFormLinkIdentifier formLink, ResolveTarget target = ResolveTarget.Winner)
    {
        return ResolveSimpleContext(formLink.FormKey, formLink.Type, target);
    }

    /// <inheritdoc />
    public IModContext<IMajorRecordGetter> ResolveSimpleContext(string editorId, Type type)
    {
        return ResolveContext(editorId, type);
    }

    /// <inheritdoc />
    public IModContext<TMajor> ResolveSimpleContext<TMajor>(FormKey formKey, ResolveTarget target = ResolveTarget.Winner) 
        where TMajor : class, IMajorRecordQueryableGetter
    {
        return ResolveContext(formKey, typeof(TMajor), target).AsType<IMajorRecordQueryableGetter, TMajor>();
    }

    /// <inheritdoc />
    public IModContext<TMajor> ResolveSimpleContext<TMajor>(string editorId) 
        where TMajor : class, IMajorRecordQueryableGetter
    {
        return ResolveContext(editorId, typeof(TMajor)).AsType<IMajorRecordQueryableGetter, TMajor>();
    }

    /// <inheritdoc />
    public IEnumerable<IModContext<TMajor>> ResolveAllSimpleContexts<TMajor>(FormKey formKey, ResolveTarget target = ResolveTarget.Winner)
        where TMajor : class, IMajorRecordQueryableGetter
    {
        return ResolveAllContexts(formKey, typeof(TMajor), target).Select(x => x.AsType<IMajorRecordQueryableGetter, TMajor>());
    }

    /// <inheritdoc />
    public IEnumerable<IModContext<IMajorRecordGetter>> ResolveAllSimpleContexts(FormKey formKey, Type type, ResolveTarget target = ResolveTarget.Winner)
    {
        return ResolveAllContexts(formKey, type, target);
    }

    /// <inheritdoc />
    public IEnumerable<IModContext<IMajorRecordGetter>> ResolveAllSimpleContexts(IFormLinkIdentifier formLink, ResolveTarget target = ResolveTarget.Winner)
    {
        return ResolveAllSimpleContexts(formLink.FormKey, formLink.Type, target);
    }

    /// <inheritdoc />
    [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
    public IEnumerable<IModContext<IMajorRecordGetter>> ResolveAllSimpleContexts(FormKey formKey, ResolveTarget target = ResolveTarget.Winner)
    {
        return ResolveAllContexts(formKey, target);
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
            return  _cache.ListedOrder;
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
        
    /// <inheritdoc />
    [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
    public bool TryResolveContext(FormKey formKey, [MaybeNullWhen(false)] out IModContext<TMod, TModGetter, IMajorRecord, IMajorRecordGetter> majorRec, ResolveTarget target = ResolveTarget.Winner)
    {
        return TryResolveContext<IMajorRecord, IMajorRecordGetter>(formKey, out majorRec, target);
    }

    /// <inheritdoc />
    [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
    public bool TryResolveContext(string editorId, [MaybeNullWhen(false)] out IModContext<TMod, TModGetter, IMajorRecord, IMajorRecordGetter> majorRec)
    {
        return TryResolveContext<IMajorRecord, IMajorRecordGetter>(editorId, out majorRec);
    }

    /// <inheritdoc />
    public bool TryResolveContext<TMajor, TMajorGetter>(FormKey formKey, [MaybeNullWhen(false)] out IModContext<TMod, TModGetter, TMajor, TMajorGetter> majorRec, ResolveTarget target = ResolveTarget.Winner)
        where TMajor : class, IMajorRecordQueryable, TMajorGetter
        where TMajorGetter : class, IMajorRecordQueryableGetter
    {
        CheckDisposal();
            
        if (!TryResolveContext(formKey, typeof(TMajorGetter), out var commonRec, target)
            || !(commonRec.Record is TMajorGetter))
        {
            majorRec = default;
            return false;
        }

        majorRec = commonRec.AsType<TMod, TModGetter, IMajorRecordQueryable, IMajorRecordQueryableGetter, TMajor, TMajorGetter>();
        return true;
    }

    /// <inheritdoc />
    public bool TryResolveContext<TMajor, TMajorGetter>(string editorId, [MaybeNullWhen(false)] out IModContext<TMod, TModGetter, TMajor, TMajorGetter> majorRec)
        where TMajor : class, IMajorRecordQueryable, TMajorGetter
        where TMajorGetter : class, IMajorRecordQueryableGetter
    {
        CheckDisposal();
            
        if (!TryResolveContext(editorId, typeof(TMajorGetter), out var commonRec)
            || !(commonRec.Record is TMajorGetter))
        {
            majorRec = default;
            return false;
        }

        majorRec = commonRec.AsType<TMod, TModGetter, IMajorRecordQueryable, IMajorRecordQueryableGetter, TMajor, TMajorGetter>();
        return true;
    }

    /// <inheritdoc />
    public bool TryResolveContext(FormKey formKey, Type type, [MaybeNullWhen(false)] out IModContext<TMod, TModGetter, IMajorRecord, IMajorRecordGetter> majorRec, ResolveTarget target = ResolveTarget.Winner)
    {
        CheckDisposal();

        if (formKey.IsNull)
        {
            majorRec = default;
            return false;
        }
            
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
    public bool TryResolveContext(IFormLinkIdentifier formLink, [MaybeNullWhen(false)] out IModContext<TMod, TModGetter, IMajorRecord, IMajorRecordGetter> majorRec, ResolveTarget target = ResolveTarget.Winner)
    {
        return TryResolveContext(formLink.FormKey, formLink.Type, out majorRec, target);
    }

    /// <inheritdoc />
    public bool TryResolveContext(string editorId, Type type, [MaybeNullWhen(false)] out IModContext<TMod, TModGetter, IMajorRecord, IMajorRecordGetter> majorRec)
    {
        CheckDisposal();
            
        if (string.IsNullOrWhiteSpace(editorId))
        {
            majorRec = default;
            return false;
        }

        return _editorIdContextCache.TryResolveContext(editorId, default(ModKey?), type, out majorRec);
    }

    /// <inheritdoc />
    [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
    public IModContext<TMod, TModGetter, IMajorRecord, IMajorRecordGetter> ResolveContext(FormKey formKey, ResolveTarget target = ResolveTarget.Winner)
    {
        if (TryResolveContext(formKey, out var commonRec, target)) return commonRec;
        throw new MissingRecordException(formKey, typeof(IMajorRecordGetter));
    }

    /// <inheritdoc />
    public IModContext<TMod, TModGetter, IMajorRecord, IMajorRecordGetter> ResolveContext(string editorId)
    {
        if (TryResolveContext<IMajorRecord, IMajorRecordGetter>(editorId, out var commonRec)) return commonRec;
        throw new MissingRecordException(editorId, typeof(IMajorRecordGetter));
    }

    /// <inheritdoc />
    public IModContext<TMod, TModGetter, IMajorRecord, IMajorRecordGetter> ResolveContext(FormKey formKey, Type type, ResolveTarget target = ResolveTarget.Winner)
    {
        if (TryResolveContext(formKey, type, out var commonRec, target)) return commonRec;
        throw new MissingRecordException(formKey, type);
    }

    /// <inheritdoc />
    public IModContext<TMod, TModGetter, IMajorRecord, IMajorRecordGetter> ResolveContext(IFormLinkIdentifier formLink, ResolveTarget target = ResolveTarget.Winner)
    {
        return ResolveContext(formLink.FormKey, formLink.Type, target);
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
        return ResolveAllContexts(formKey, typeof(TMajorGetter), target)
            .Select(c => c.AsType<TMod, TModGetter, IMajorRecordQueryable, IMajorRecordQueryableGetter, TMajor, TMajorGetter>());
    }

    /// <inheritdoc />
    public IEnumerable<IModContext<TMod, TModGetter, IMajorRecord, IMajorRecordGetter>> ResolveAllContexts(FormKey formKey, Type type, ResolveTarget target = ResolveTarget.Winner)
    {
        CheckDisposal();
            
        // Break early if no content
        if (!_hasAny || formKey.IsNull)
        {
            return Enumerable.Empty<IModContext<TMod, TModGetter, IMajorRecord, IMajorRecordGetter>>();
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
    public IEnumerable<IModContext<TMod, TModGetter, IMajorRecord, IMajorRecordGetter>> ResolveAllContexts(
        IFormLinkIdentifier formLink,
        ResolveTarget target = ResolveTarget.Winner)
    {
        return ResolveAllContexts(formLink.FormKey, formLink.Type, target);
    }

    /// <inheritdoc />
    [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
    public IEnumerable<IModContext<TMod, TModGetter, IMajorRecord, IMajorRecordGetter>> ResolveAllContexts(FormKey formKey, ResolveTarget target = ResolveTarget.Winner)
    {
        return ResolveAllContexts(formKey, typeof(IMajorRecordGetter), target);
    }
}