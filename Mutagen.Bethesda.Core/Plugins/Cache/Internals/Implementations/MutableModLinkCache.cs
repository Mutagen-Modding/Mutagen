using System.Diagnostics.CodeAnalysis;
using DynamicData;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Cache.Internals.Implementations;

/// <summary>
/// A Link Cache using a single mod as its link target.  Mod is allowed to be modified afterwards, but
/// this comes at a performance cost of not allowing much caching to be done.  If the mod is not expected to
/// be modified afterwards, use ImmutableModLinkCache instead.<br/>
/// <br/>
/// If being used in a multithreaded scenario,<br/>
/// this cache must be locked alongside any mutations to the mod the cache wraps
/// </summary>
public sealed class MutableModLinkCache : ILinkCache
{
    private readonly IModGetter _sourceMod;
    private bool _disposed;

    /// <inheritdoc />
    public IReadOnlyList<IModGetter> ListedOrder { get; }

    /// <inheritdoc />
    public IReadOnlyList<IModGetter> PriorityOrder => ListedOrder;

    public IModGetter SourceMod => _sourceMod;

    /// <summary>
    /// Constructs a link cache around a target mod
    /// </summary>
    /// <param name="sourceMod">Mod to resolve against when linking</param>
    public MutableModLinkCache(IModGetter sourceMod)
    {
        _sourceMod = sourceMod;
        ListedOrder = new List<IModGetter>()
        {
            sourceMod
        };
    }

    private void CheckDisposal()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException($"MutableModLinkCache");
        }
    }

    /// <inheritdoc />
    [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
    public bool TryResolve(FormKey formKey, [MaybeNullWhen(false)] out IMajorRecordGetter majorRec, ResolveTarget target = ResolveTarget.Winner)
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
            
        // ToDo
        // Upgrade to call EnumerateGroups(), which will perform much better
        foreach (var item in _sourceMod.EnumerateMajorRecords()
                     // ToDo
                     // Capture and expose errors optionally via TryResolve /w out param
                     .Catch((Exception ex) => { }))
        {
            if (item.FormKey == formKey)
            {
                majorRec = item;
                return true;
            }
        }
        majorRec = default;
        return false;
    }

    /// <inheritdoc />
    [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
    public bool TryResolve(string editorId, [MaybeNullWhen(false)] out IMajorRecordGetter majorRec)
    {
        CheckDisposal();
            
        if (editorId.IsNullOrWhitespace())
        {
            majorRec = default;
            return false;
        }
            
        // ToDo
        // Upgrade to call EnumerateGroups(), which will perform much better
        foreach (var item in _sourceMod.EnumerateMajorRecords()
                     // ToDo
                     // Capture and expose errors optionally via TryResolve /w out param
                     .Catch((Exception ex) => { }))
        {
            if (editorId.Equals(item.EditorID))
            {
                majorRec = item;
                return true;
            }
        }
        majorRec = default;
        return false;
    }

    /// <inheritdoc />
    public bool TryResolve<TMajor>(FormKey formKey, [MaybeNullWhen(false)] out TMajor majorRec, ResolveTarget target = ResolveTarget.Winner)
        where TMajor : class, IMajorRecordQueryableGetter
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
            
        // ToDo
        // Upgrade to EnumerateGroups<TMajor>()
        foreach (var item in _sourceMod.EnumerateMajorRecords<TMajor>()
                     // ToDo
                     // Capture and expose errors optionally via TryResolve /w out param
                     .Catch((Exception ex) => { }))
        {
            if (item is IMajorRecordGetter majRec
                && majRec.FormKey == formKey)
            {
                majorRec = item;
                return true;
            }
        }

        majorRec = default;
        return false;
    }

    /// <inheritdoc />
    public bool TryResolve<TMajor>(string editorId, [MaybeNullWhen(false)] out TMajor majorRec)
        where TMajor : class, IMajorRecordQueryableGetter
    {
        CheckDisposal();
            
        if (editorId.IsNullOrWhitespace())
        {
            majorRec = default;
            return false;
        }
            
        // ToDo
        // Upgrade to EnumerateGroups<TMajor>()
        foreach (var item in _sourceMod.EnumerateMajorRecords<TMajor>()
                     // ToDo
                     // Capture and expose errors optionally via TryResolve /w out param
                     .Catch((Exception ex) => { }))
        {
            if (item is IMajorRecordGetter majRec
                && editorId.Equals(majRec.EditorID))
            {
                majorRec = item;
                return true;
            }
        }

        majorRec = default;
        return false;
    }

    /// <inheritdoc />
    public bool TryResolve(FormKey formKey, Type type, [MaybeNullWhen(false)] out IMajorRecordGetter majorRec, ResolveTarget target = ResolveTarget.Winner)
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
            
        // ToDo
        // Upgrade to EnumerateGroups<TMajor>()
        foreach (var major in _sourceMod.EnumerateMajorRecords(type)
                     // ToDo
                     // Capture and expose errors optionally via TryResolve /w out param
                     .Catch((Exception ex) => { }))
        {
            if (major.FormKey == formKey)
            {
                majorRec = major;
                return true;
            }
        }

        majorRec = default;
        return false;
    }

    /// <inheritdoc />
    public bool TryResolve(IFormLinkIdentifier formLink, [MaybeNullWhen(false)] out IMajorRecordGetter majorRec, ResolveTarget target = ResolveTarget.Winner)
    {
        return TryResolve(formLink.FormKey, formLink.Type, out majorRec, target);
    }

    /// <inheritdoc />
    public bool TryResolve(string editorId, Type type, [MaybeNullWhen(false)] out IMajorRecordGetter majorRec)
    {
        CheckDisposal();
            
        if (editorId.IsNullOrWhitespace())
        {
            majorRec = default;
            return false;
        }
            
        // ToDo
        // Upgrade to EnumerateGroups<TMajor>()
        foreach (var major in _sourceMod.EnumerateMajorRecords(type)
                     // ToDo
                     // Capture and expose errors optionally via TryResolve /w out param
                     .Catch((Exception ex) => { }))
        {
            if (editorId.Equals(major.EditorID))
            {
                majorRec = major;
                return true;
            }
        }

        majorRec = default;
        return false;
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
    public IMajorRecordGetter Resolve(FormKey formKey, ResolveTarget target = ResolveTarget.Winner)
    {
        if (TryResolve<IMajorRecordGetter>(formKey, out var majorRec, target)) return majorRec;
        throw new MissingRecordException(formKey, typeof(IMajorRecordGetter));
    }

    /// <inheritdoc />
    public IMajorRecordGetter Resolve(string editorId)
    {
        if (TryResolve<IMajorRecordGetter>(editorId, out var majorRec)) return majorRec;
        throw new MissingRecordException(editorId, typeof(IMajorRecordGetter));
    }

    /// <inheritdoc />
    public IMajorRecordGetter Resolve(FormKey formKey, Type type, ResolveTarget target = ResolveTarget.Winner)
    {
        if (TryResolve(formKey, type, out var commonRec, target)) return commonRec;
        throw new MissingRecordException(formKey, type);
    }

    /// <inheritdoc />
    public IMajorRecordGetter Resolve(IFormLinkIdentifier formLink, ResolveTarget target = ResolveTarget.Winner)
    {
        return Resolve(formLink.FormKey, formLink.Type, target);
    }

    /// <inheritdoc />
    public IMajorRecordGetter Resolve(string editorId, Type type)
    {
        if (TryResolve(editorId, type, out var commonRec)) return commonRec;
        throw new MissingRecordException(editorId, type);
    }

    /// <inheritdoc />
    public TMajor Resolve<TMajor>(FormKey formKey, ResolveTarget target = ResolveTarget.Winner)
        where TMajor : class, IMajorRecordQueryableGetter
    {
        if (TryResolve<TMajor>(formKey, out var commonRec, target)) return commonRec;
        throw new MissingRecordException(formKey, typeof(TMajor));
    }

    /// <inheritdoc />
    public TMajor Resolve<TMajor>(string editorId)
        where TMajor : class, IMajorRecordQueryableGetter
    {
        if (TryResolve<TMajor>(editorId, out var commonRec)) return commonRec;
        throw new MissingRecordException(editorId, typeof(TMajor));
    }

    /// <inheritdoc />
    public IEnumerable<TMajor> ResolveAll<TMajor>(FormKey formKey, ResolveTarget target = ResolveTarget.Winner) where TMajor : class, IMajorRecordQueryableGetter
    {
        if (TryResolve<TMajor>(formKey, out var rec, target))
        {
            yield return rec;
        }
    }

    /// <inheritdoc />
    public IEnumerable<IMajorRecordGetter> ResolveAll(FormKey formKey, Type type, ResolveTarget target = ResolveTarget.Winner)
    {
        if (TryResolve(formKey, type, out var rec, target))
        {
            yield return rec;
        }
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
        if (TryResolve(formKey, out var rec, target))
        {
            yield return rec;
        }
    }

    /// <inheritdoc />
    [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
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
            
        // ToDo
        // Upgrade to call EnumerateGroups(), which will perform much better
        foreach (var item in _sourceMod.EnumerateMajorRecordSimpleContexts<IMajorRecordGetter>()
                     // ToDo
                     // Capture and expose errors optionally via TryResolve /w out param
                     .Catch((Exception ex) => { }))
        {
            if (item.Record.FormKey == formKey)
            {
                majorRec = item;
                return true;
            }
        }
        majorRec = default;
        return false;
    }

    /// <inheritdoc />
    [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
    public bool TryResolveSimpleContext(string editorId, [MaybeNullWhen(false)] out IModContext<IMajorRecordGetter> majorRec)
    {
        CheckDisposal();
            
        if (editorId.IsNullOrWhitespace())
        {
            majorRec = default;
            return false;
        }
            
        // ToDo
        // Upgrade to call EnumerateGroups(), which will perform much better
        foreach (var item in _sourceMod.EnumerateMajorRecordSimpleContexts<IMajorRecordGetter>()
                     // ToDo
                     // Capture and expose errors optionally via TryResolve /w out param
                     .Catch((Exception ex) => { }))
        {
            if (editorId.Equals(item.Record.EditorID))
            {
                majorRec = item;
                return true;
            }
        }
        majorRec = default;
        return false;
    }

    /// <inheritdoc />
    public bool TryResolveSimpleContext<TMajor>(FormKey formKey, [MaybeNullWhen(false)] out IModContext<TMajor> majorRec, ResolveTarget target = ResolveTarget.Winner)
        where TMajor : class, IMajorRecordQueryableGetter
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
            
        // ToDo
        // Upgrade to EnumerateGroups<TMajor>()
        foreach (var context in _sourceMod.EnumerateMajorRecordSimpleContexts<TMajor>()
                     // ToDo
                     // Capture and expose errors optionally via TryResolve /w out param
                     .Catch((Exception ex) => { }))
        {
            if (context.Record is IMajorRecordGetter majRec && majRec.FormKey == formKey)
            {
                majorRec = context;
                return true;
            }
        }

        majorRec = default;
        return false;
    }

    /// <inheritdoc />
    public bool TryResolveSimpleContext<TMajor>(string editorId, [MaybeNullWhen(false)] out IModContext<TMajor> majorRec) 
        where TMajor : class, IMajorRecordQueryableGetter
    {
        CheckDisposal();
            
        if (editorId.IsNullOrWhitespace())
        {
            majorRec = default;
            return false;
        }
            
        // ToDo
        // Upgrade to EnumerateGroups<TMajor>()
        foreach (var context in _sourceMod.EnumerateMajorRecordSimpleContexts<TMajor>()
                     // ToDo
                     // Capture and expose errors optionally via TryResolve /w out param
                     .Catch((Exception ex) => { }))
        {
            if (context.Record is IMajorRecordGetter majRec && editorId.Equals(majRec.EditorID))
            {
                majorRec = context;
                return true;
            }
        }

        majorRec = default;
        return false;
    }

    /// <inheritdoc />
    public bool TryResolveSimpleContext(FormKey formKey, Type type, [MaybeNullWhen(false)] out IModContext<IMajorRecordGetter> majorRec, ResolveTarget target = ResolveTarget.Winner)
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
            
        // ToDo
        // Upgrade to EnumerateGroups<TMajor>()
        foreach (var major in _sourceMod.EnumerateMajorRecordSimpleContexts(type)
                     // ToDo
                     // Capture and expose errors optionally via TryResolve /w out param
                     .Catch((Exception ex) => { }))
        {
            if (major.Record.FormKey == formKey)
            {
                majorRec = major;
                return true;
            }
        }
        
        majorRec = default;
        return false;
    }

    /// <inheritdoc />
    public bool TryResolveSimpleContext(IFormLinkIdentifier formLink, [MaybeNullWhen(false)] out IModContext<IMajorRecordGetter> majorRec, ResolveTarget target = ResolveTarget.Winner)
    {
        return TryResolveSimpleContext(formLink.FormKey, formLink.Type, out majorRec, target);
    }

    /// <inheritdoc />
    public bool TryResolveSimpleContext(string editorId, Type type, [MaybeNullWhen(false)] out IModContext<IMajorRecordGetter> majorRec)
    {
        CheckDisposal();
            
        if (editorId.IsNullOrWhitespace())
        {
            majorRec = default;
            return false;
        }
        // ToDo
        // Upgrade to EnumerateGroups<TMajor>()
        foreach (var major in _sourceMod.EnumerateMajorRecordSimpleContexts(type)
                     // ToDo
                     // Capture and expose errors optionally via TryResolve /w out param
                     .Catch((Exception ex) => { }))
        {
            if (editorId.Equals(major.Record.EditorID))
            {
                majorRec = major;
                return true;
            }
        }
        
        majorRec = default;
        return false;
    }

    /// <inheritdoc />
    [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
    public IModContext<IMajorRecordGetter> ResolveSimpleContext(FormKey formKey, ResolveTarget target = ResolveTarget.Winner)
    {
        if (TryResolveSimpleContext<IMajorRecordGetter>(formKey, out var majorRec, target)) return majorRec;
        throw new MissingRecordException(formKey, typeof(IMajorRecordGetter));
    }

    /// <inheritdoc />
    [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
    public IModContext<IMajorRecordGetter> ResolveSimpleContext(string editorId)
    {
        if (TryResolveSimpleContext<IMajorRecordGetter>(editorId, out var majorRec)) return majorRec;
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
    public IEnumerable<IModContext<IMajorRecordGetter>> ResolveAllSimpleContexts(IFormLinkIdentifier formLink, ResolveTarget target = ResolveTarget.Winner)
    {
        return ResolveAllSimpleContexts(formLink.FormKey, formLink.Type, target);
    }

    /// <inheritdoc />
    [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
    public IEnumerable<IModContext<IMajorRecordGetter>> ResolveAllSimpleContexts(FormKey formKey, ResolveTarget target = ResolveTarget.Winner)
    {
        if (TryResolveSimpleContext(formKey, out var rec, target))
        {
            yield return rec;
        }
    }

    /// <inheritdoc />
    public bool TryResolve(FormKey formKey, [MaybeNullWhen(false)] out IMajorRecordGetter majorRec, params Type[] types)
    {
        return TryResolve(formKey, (IEnumerable<Type>)types, out majorRec);
    }

    /// <inheritdoc />
    public bool TryResolve(string editorId, [MaybeNullWhen(false)] out IMajorRecordGetter majorRec, params Type[] types)
    {
        return TryResolve(editorId, (IEnumerable<Type>)types, out majorRec);
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
    public bool TryResolve(FormKey formKey, IEnumerable<Type> types, [MaybeNullWhen(false)] out IMajorRecordGetter majorRec, [MaybeNullWhen(false)] out Type matchedType,
        ResolveTarget target = ResolveTarget.Winner)
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

    /// <inheritdoc />
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

    /// <inheritdoc />
    public IMajorRecordGetter Resolve(FormKey formKey, params Type[] types)
    {
        return Resolve(formKey, (IEnumerable<Type>)types);
    }

    /// <inheritdoc />
    public IMajorRecordGetter Resolve(string editorId, params Type[] types)
    {
        return Resolve(editorId, (IEnumerable<Type>)types);
    }

    /// <inheritdoc />
    public IMajorRecordGetter Resolve(FormKey formKey, IEnumerable<Type> types, ResolveTarget target = ResolveTarget.Winner)
    {
        if (TryResolve(formKey, types, out var commonRec, target)) return commonRec;
        throw new MissingRecordException(formKey, types.ToArray());
    }

    /// <inheritdoc />
    public IMajorRecordGetter Resolve(string editorId, IEnumerable<Type> types)
    {
        if (TryResolve(editorId, types, out var commonRec)) return commonRec;
        throw new MissingRecordException(editorId, types.ToArray());
    }

    /// <inheritdoc />
    public bool TryResolveIdentifier(FormKey formKey, [MaybeNullWhen(false)] out string? editorId, ResolveTarget target = ResolveTarget.Winner)
    {
        if (TryResolve(formKey, out var rec, target))
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
        if (TryResolve(editorId, out var rec))
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
        if (TryResolve(formKey, type, out var rec, target))
        {
            editorId = rec.EditorID;
            return true;
        }
        editorId = default;
        return false;
    }

    /// <inheritdoc />
    public bool TryResolveIdentifier(IFormLinkIdentifier formLink, [MaybeNullWhen(false)] out string? editorId, ResolveTarget target = ResolveTarget.Winner)
    {
        return TryResolveIdentifier(formLink.FormKey, formLink.Type, out editorId, target);
    }

    /// <inheritdoc />
    public bool TryResolveIdentifier(string editorId, Type type, [MaybeNullWhen(false)] out FormKey formKey)
    {
        if (TryResolve(editorId, type, out var rec))
        {
            formKey = rec.FormKey;
            return true;
        }
        formKey = default;
        return false;
    }

    /// <inheritdoc />
    public bool TryResolveIdentifier<TMajor>(FormKey formKey, out string? editorId, ResolveTarget target = ResolveTarget.Winner)
        where TMajor : class, IMajorRecordQueryableGetter
    {
        if (TryResolve<TMajor>(formKey, out var rec, target)
            && rec is IMajorRecordGetter majRec)
        {
            editorId = majRec.EditorID;
            return true;
        }
        editorId = default;
        return false;
    }

    /// <inheritdoc />
    public bool TryResolveIdentifier<TMajor>(string editorId, out FormKey formKey)
        where TMajor : class, IMajorRecordQueryableGetter
    {
        if (TryResolve<TMajor>(editorId, out var rec) 
            && rec is IMajorRecordGetter majRec)
        {
            formKey = majRec.FormKey;
            return true;
        }
        formKey = default;
        return false;
    }

    /// <inheritdoc />
    public bool TryResolveIdentifier(FormKey formKey, [MaybeNullWhen(false)] out string? editorId, params Type[] types)
    {
        if (TryResolve(formKey, out var rec, types))
        {
            editorId = rec.EditorID;
            return true;
        }
        editorId = default;
        return false;
    }

    /// <inheritdoc />
    public bool TryResolveIdentifier(string editorId, [MaybeNullWhen(false)] out FormKey formKey, params Type[] types)
    {
        if (TryResolve(editorId, out var rec, types))
        {
            formKey = rec.FormKey;
            return true;
        }
        formKey = default;
        return false;
    }

    /// <inheritdoc />
    public bool TryResolveIdentifier(FormKey formKey, IEnumerable<Type> types, [MaybeNullWhen(false)] out string? editorId, ResolveTarget target = ResolveTarget.Winner)
    {
        if (TryResolve(formKey, types, out var rec, target))
        {
            editorId = rec.EditorID;
            return true;
        }
        editorId = default;
        return false;
    }

    /// <inheritdoc />
    public bool TryResolveIdentifier(FormKey formKey, IEnumerable<Type> types, [MaybeNullWhen(false)] out string? editorId, [MaybeNullWhen(false)] out Type matchedType,
        ResolveTarget target = ResolveTarget.Winner)
    {
        if (TryResolve(formKey, types, out var rec, out matchedType, target))
        {
            editorId = rec.EditorID;
            return true;
        }
        editorId = default;
        return false;
    }

    /// <inheritdoc />
    public bool TryResolveIdentifier(string editorId, IEnumerable<Type> types, [MaybeNullWhen(false)] out FormKey formKey)
    {
        if (TryResolve(editorId, types, out var rec))
        {
            formKey = rec.FormKey;
            return true;
        }
        formKey = default;
        return false;
    }

    /// <inheritdoc />
    public bool TryResolveIdentifier(string editorId, IEnumerable<Type> types, out FormKey formKey, [MaybeNullWhen(false)] out Type matchedType)
    {
        if (TryResolve(editorId, types, out var rec, out matchedType))
        {
            formKey = rec.FormKey;
            return true;
        }
        formKey = default;
        return false;
    }

    /// <inheritdoc />
    public string? ResolveIdentifier(FormKey formKey, ResolveTarget target = ResolveTarget.Winner)
    {
        if (TryResolveIdentifier(formKey, out var edid, target)) return edid;
        throw new MissingRecordException(formKey, typeof(IMajorRecordGetter));
    }

    public FormKey ResolveIdentifier(string editorId)
    {
        if (TryResolveIdentifier(editorId, out var formKey)) return formKey;
        throw new MissingRecordException(editorId, typeof(IMajorRecordGetter));
    }

    public string? ResolveIdentifier(FormKey formKey, Type type, ResolveTarget target = ResolveTarget.Winner)
    {
        if (TryResolveIdentifier(formKey, type, out var edid, target)) return edid;
        throw new MissingRecordException(formKey, typeof(IMajorRecordGetter));
    }

    public string? ResolveIdentifier(IFormLinkIdentifier formLink, ResolveTarget target = ResolveTarget.Winner)
    {
        if (TryResolveIdentifier(formLink.FormKey, out var edid, target)) return edid;
        throw new MissingRecordException(formLink);
    }

    public FormKey ResolveIdentifier(string editorId, Type type)
    {
        if (TryResolveIdentifier(editorId, type, out var formKey)) return formKey;
        throw new MissingRecordException(editorId, type);
    }

    public string? ResolveIdentifier<TMajor>(FormKey formKey, ResolveTarget target = ResolveTarget.Winner) where TMajor : class, IMajorRecordQueryableGetter
    {
        if (TryResolveIdentifier(formKey, out var edid, target)) return edid;
        throw new MissingRecordException(formKey, typeof(TMajor));
    }

    public FormKey ResolveIdentifier<TMajor>(string editorId) where TMajor : class, IMajorRecordQueryableGetter
    {
        if (TryResolveIdentifier(editorId, out var formKey)) return formKey;
        throw new MissingRecordException(editorId, typeof(TMajor));
    }

    public string? ResolveIdentifier(FormKey formKey, params Type[] types)
    {
        if (TryResolveIdentifier(formKey, types, out var editorId)) return editorId;
        throw new MissingRecordException(formKey, types);
    }

    public FormKey ResolveIdentifier(string editorId, params Type[] types)
    {
        if (TryResolveIdentifier(editorId, types, out var formKey)) return formKey;
        throw new MissingRecordException(editorId, types);
    }

    public string? ResolveIdentifier(FormKey formKey, IEnumerable<Type> types, ResolveTarget target = ResolveTarget.Winner)
    {
        if (TryResolveIdentifier(formKey, types, out var editorId)) return editorId;
        throw new MissingRecordException(formKey, types.ToArray());
    }

    public FormKey ResolveIdentifier(string editorId, IEnumerable<Type> types)
    {
        if (TryResolveIdentifier(editorId, types, out var formKey)) return formKey;
        throw new MissingRecordException(editorId, types.ToArray());
    }

    /// <inheritdoc />
    public IEnumerable<IMajorRecordIdentifierGetter> AllIdentifiers(Type type, CancellationToken? cancel = null)
    {
        return AllIdentifiersNoUniqueness(type, cancel)
            .Distinct(x => x.FormKey);
    }

    internal IEnumerable<IMajorRecordIdentifierGetter> AllIdentifiersNoUniqueness(Type type, CancellationToken? cancel)
    {
        CheckDisposal();
            
        return _sourceMod.EnumerateMajorRecords(type);
    }

    /// <inheritdoc />
    public IEnumerable<IMajorRecordIdentifierGetter> AllIdentifiers<TMajor>(CancellationToken? cancel = null)
        where TMajor : class, IMajorRecordQueryableGetter
    {
        return AllIdentifiers(typeof(TMajor), cancel);
    }

    /// <inheritdoc />
    public IEnumerable<IMajorRecordIdentifierGetter> AllIdentifiers(params Type[] types)
    {
        return AllIdentifiers((IEnumerable<Type>)types, CancellationToken.None);
    }

    /// <inheritdoc />
    public IEnumerable<IMajorRecordIdentifierGetter> AllIdentifiers(IEnumerable<Type> types, CancellationToken? cancel = null)
    {
        return types.SelectMany(type => AllIdentifiersNoUniqueness(type, cancel))
            .Distinct(x => x.FormKey);
    }

    /// <inheritdoc />
    public bool TryResolve<TMajor>(IFormLinkGetter<TMajor> formLink, [MaybeNullWhen(false)] out TMajor majorRec, ResolveTarget target = ResolveTarget.Winner)
        where TMajor : class, IMajorRecordGetter
    {
        return TryResolve<TMajor>(formLink.FormKey, out majorRec, target);
    }

    /// <inheritdoc />
    public TMajor Resolve<TMajor>(IFormLinkGetter<TMajor> formLink, ResolveTarget target = ResolveTarget.Winner)
        where TMajor : class, IMajorRecordGetter
    {
        return Resolve<TMajor>(formLink.FormKey, target);
    }

    /// <inheritdoc />
    public bool TryResolveSimpleContext<TMajor>(IFormLinkGetter<TMajor> formLink, [MaybeNullWhen(false)] out IModContext<TMajor> majorRec, ResolveTarget target = ResolveTarget.Winner)
        where TMajor : class, IMajorRecordGetter
    {
        return TryResolveSimpleContext<TMajor>(formLink.FormKey, out majorRec, target);
    }

    /// <inheritdoc />
    public IModContext<TMajor> ResolveSimpleContext<TMajor>(IFormLinkGetter<TMajor> formLink, ResolveTarget target = ResolveTarget.Winner)
        where TMajor : class, IMajorRecordGetter
    {
        if (TryResolveSimpleContext<TMajor>(formLink.FormKey, out var commonRec, target)) return commonRec;
        throw new MissingRecordException(formLink.FormKey, typeof(TMajor));
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _disposed = true;
    }

    public void Warmup(Type type)
    {
        CheckDisposal();
    }

    public void Warmup<TMajor>()
    {
        CheckDisposal();
    }

    public void Warmup(params Type[] types)
    {
        CheckDisposal();
    }

    public void Warmup(IEnumerable<Type> types)
    {
        CheckDisposal();
    }
}

/// <summary>
/// A Link Cache using a single mod as its link target.  Mod is allowed to be modified afterwards, but
/// this comes at a performance cost of not allowing much caching to be done.  If the mod is not expected to
/// be modified afterwards, use ImmutableModLinkCache instead.<br/>
/// <br/>
/// If being used in a multithreaded scenario,<br/>
/// this cache must be locked alongside any mutations to the mod the cache wraps
/// </summary>
public sealed class MutableModLinkCache<TMod, TModGetter> : ILinkCache<TMod, TModGetter>
    where TMod : class, IContextMod<TMod, TModGetter>, TModGetter
    where TModGetter : class, IContextGetterMod<TMod, TModGetter>
{
    private readonly TModGetter _sourceMod;
    private bool _disposed;

    /// <inheritdoc />
    public IReadOnlyList<IModGetter> ListedOrder { get; }

    /// <inheritdoc />
    public IReadOnlyList<IModGetter> PriorityOrder => ListedOrder;

    public TModGetter SourceMod => _sourceMod;

    /// <summary>
    /// Constructs a link cache around a target mod
    /// </summary>
    /// <param name="sourceMod">Mod to resolve against when linking</param>
    public MutableModLinkCache(TModGetter sourceMod)
    {
        _sourceMod = sourceMod;
        ListedOrder = new List<IModGetter>()
        {
            sourceMod
        };
    }

    private void CheckDisposal()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException($"MutableModLinkCache<{typeof(TMod)}, {typeof(TModGetter)}>");
        }
    }

    /// <inheritdoc />
    [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
    public bool TryResolve(FormKey formKey, [MaybeNullWhen(false)] out IMajorRecordGetter majorRec, ResolveTarget target = ResolveTarget.Winner)
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
            
        // ToDo
        // Upgrade to call EnumerateGroups(), which will perform much better
        foreach (var item in _sourceMod.EnumerateMajorRecords()
                     // ToDo
                     // Capture and expose errors optionally via TryResolve /w out param
                     .Catch((Exception ex) => { }))
        {
            if (item.FormKey == formKey)
            {
                majorRec = item;
                return true;
            }
        }
        majorRec = default;
        return false;
    }

    /// <inheritdoc />
    [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
    public bool TryResolve(string editorId, [MaybeNullWhen(false)] out IMajorRecordGetter majorRec)
    {
        CheckDisposal();
            
        if (editorId.IsNullOrWhitespace())
        {
            majorRec = default;
            return false;
        }
            
        // ToDo
        // Upgrade to call EnumerateGroups(), which will perform much better
        foreach (var item in _sourceMod.EnumerateMajorRecords()
                     // ToDo
                     // Capture and expose errors optionally via TryResolve /w out param
                     .Catch((Exception ex) => { }))
        {
            if (editorId.Equals(item.EditorID))
            {
                majorRec = item;
                return true;
            }
        }
        majorRec = default;
        return false;
    }

    /// <inheritdoc />
    public bool TryResolve<TMajor>(FormKey formKey, [MaybeNullWhen(false)] out TMajor majorRec, ResolveTarget target = ResolveTarget.Winner)
        where TMajor : class, IMajorRecordQueryableGetter
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
            
        // ToDo
        // Upgrade to EnumerateGroups<TMajor>()
        foreach (var item in _sourceMod.EnumerateMajorRecords<TMajor>()
                     // ToDo
                     // Capture and expose errors optionally via TryResolve /w out param
                     .Catch((Exception ex) => { }))
        {
            if (item is IMajorRecordGetter majRec
                && majRec.FormKey == formKey)
            {
                majorRec = item;
                return true;
            }
        }

        majorRec = default;
        return false;
    }

    /// <inheritdoc />
    public bool TryResolve<TMajor>(string editorId, [MaybeNullWhen(false)] out TMajor majorRec)
        where TMajor : class, IMajorRecordQueryableGetter
    {
        CheckDisposal();
            
        if (editorId.IsNullOrWhitespace())
        {
            majorRec = default;
            return false;
        }
            
        // ToDo
        // Upgrade to EnumerateGroups<TMajor>()
        foreach (var item in _sourceMod.EnumerateMajorRecords<TMajor>()
                     // ToDo
                     // Capture and expose errors optionally via TryResolve /w out param
                     .Catch((Exception ex) => { }))
        {
            if (item is IMajorRecordGetter majRec
                && editorId.Equals(majRec.EditorID, StringComparison.OrdinalIgnoreCase))
            {
                majorRec = item;
                return true;
            }
        }

        majorRec = default;
        return false;
    }

    /// <inheritdoc />
    public bool TryResolve(FormKey formKey, Type type, [MaybeNullWhen(false)] out IMajorRecordGetter majorRec, ResolveTarget target = ResolveTarget.Winner)
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
            
        // ToDo
        // Upgrade to EnumerateGroups<TMajor>()
        foreach (var major in _sourceMod.EnumerateMajorRecords(type)
                     // ToDo
                     // Capture and expose errors optionally via TryResolve /w out param
                     .Catch((Exception ex) => { }))
        {
            if (major.FormKey == formKey)
            {
                majorRec = major;
                return true;
            }
        }

        majorRec = default;
        return false;
    }

    /// <inheritdoc />
    public bool TryResolve(IFormLinkIdentifier formLink, [MaybeNullWhen(false)] out IMajorRecordGetter majorRec, ResolveTarget target = ResolveTarget.Winner)
    {
        return TryResolve(formLink.FormKey, formLink.Type, out majorRec, target);
    }

    /// <inheritdoc />
    public bool TryResolve(string editorId, Type type, [MaybeNullWhen(false)] out IMajorRecordGetter majorRec)
    {
        CheckDisposal();
            
        if (editorId.IsNullOrWhitespace())
        {
            majorRec = default;
            return false;
        }
            
        // ToDo
        // Upgrade to EnumerateGroups<TMajor>()
        foreach (var major in _sourceMod.EnumerateMajorRecords(type)
                     // ToDo
                     // Capture and expose errors optionally via TryResolve /w out param
                     .Catch((Exception ex) => { }))
        {
            if (editorId.Equals(major.EditorID))
            {
                majorRec = major;
                return true;
            }
        }

        majorRec = default;
        return false;
    }

    public bool TryResolve(string editorId, IEnumerable<Type> types, out IMajorRecordGetter majorRec, out Type matchedType)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public IMajorRecordGetter Resolve(FormKey formKey, ResolveTarget target = ResolveTarget.Winner)
    {
        if (TryResolve<IMajorRecordGetter>(formKey, out var majorRec, target)) return majorRec;
        throw new MissingRecordException(formKey, typeof(IMajorRecordGetter));
    }

    /// <inheritdoc />
    public IMajorRecordGetter Resolve(string editorId)
    {
        if (TryResolve<IMajorRecordGetter>(editorId, out var majorRec)) return majorRec;
        throw new MissingRecordException(editorId, typeof(IMajorRecordGetter));
    }

    /// <inheritdoc />
    public IMajorRecordGetter Resolve(FormKey formKey, Type type, ResolveTarget target = ResolveTarget.Winner)
    {
        if (TryResolve(formKey, type, out var commonRec, target)) return commonRec;
        throw new MissingRecordException(formKey, type);
    }

    /// <inheritdoc />
    public IMajorRecordGetter Resolve(IFormLinkIdentifier formLink, ResolveTarget target = ResolveTarget.Winner)
    {
        return Resolve(formLink.FormKey, formLink.Type, target);
    }

    /// <inheritdoc />
    public IMajorRecordGetter Resolve(string editorId, Type type)
    {
        if (TryResolve(editorId, type, out var commonRec)) return commonRec;
        throw new MissingRecordException(editorId, type);
    }

    /// <inheritdoc />
    public TMajor Resolve<TMajor>(FormKey formKey, ResolveTarget target = ResolveTarget.Winner)
        where TMajor : class, IMajorRecordQueryableGetter
    {
        if (TryResolve<TMajor>(formKey, out var commonRec, target)) return commonRec;
        throw new MissingRecordException(formKey, typeof(TMajor));
    }

    /// <inheritdoc />
    public TMajor Resolve<TMajor>(string editorId)
        where TMajor : class, IMajorRecordQueryableGetter
    {
        if (TryResolve<TMajor>(editorId, out var commonRec)) return commonRec;
        throw new MissingRecordException(editorId, typeof(TMajor));
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
            
        // ToDo
        // Upgrade to call EnumerateGroups(), which will perform much better
        foreach (var item in _sourceMod.EnumerateMajorRecordContexts<IMajorRecord, IMajorRecordGetter>(this)
                     // ToDo
                     // Capture and expose errors optionally via TryResolve /w out param
                     .Catch((Exception ex) => { }))
        {
            if (item.Record.FormKey == formKey)
            {
                majorRec = item;
                return true;
            }
        }
        majorRec = default;
        return false;
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
            
        // ToDo
        // Upgrade to call EnumerateGroups(), which will perform much better
        foreach (var item in _sourceMod.EnumerateMajorRecordContexts<IMajorRecord, IMajorRecordGetter>(this)
                     // ToDo
                     // Capture and expose errors optionally via TryResolve /w out param
                     .Catch((Exception ex) => { }))
        {
            if (editorId.Equals(item.Record.EditorID))
            {
                majorRec = item;
                return true;
            }
        }
        majorRec = default;
        return false;
    }

    /// <inheritdoc />
    public bool TryResolveContext<TMajor, TMajorGetter>(FormKey formKey, [MaybeNullWhen(false)] out IModContext<TMod, TModGetter, TMajor, TMajorGetter> majorRec, ResolveTarget target = ResolveTarget.Winner)
        where TMajor : class, IMajorRecordQueryable, TMajorGetter
        where TMajorGetter : class, IMajorRecordQueryableGetter
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
            
        // ToDo
        // Upgrade to EnumerateGroups<TMajor>()
        foreach (var context in _sourceMod.EnumerateMajorRecordContexts<TMajor, TMajorGetter>(this)
                     // ToDo
                     // Capture and expose errors optionally via TryResolve /w out param
                     .Catch((Exception ex) => { }))
        {
            if (context.Record is IMajorRecordGetter majRec
                && majRec.FormKey == formKey)
            {
                majorRec = context;
                return true;
            }
        }

        majorRec = default;
        return false;
    }

    /// <inheritdoc />
    public bool TryResolveContext<TMajor, TMajorGetter>(string editorId, [MaybeNullWhen(false)] out IModContext<TMod, TModGetter, TMajor, TMajorGetter> majorRec)
        where TMajor : class, IMajorRecordQueryable, TMajorGetter
        where TMajorGetter : class, IMajorRecordQueryableGetter
    {
        CheckDisposal();
            
        if (editorId.IsNullOrWhitespace())
        {
            majorRec = default;
            return false;
        }
            
        // ToDo
        // Upgrade to EnumerateGroups<TMajor>()
        foreach (var context in _sourceMod.EnumerateMajorRecordContexts<TMajor, TMajorGetter>(this)
                     // ToDo
                     // Capture and expose errors optionally via TryResolve /w out param
                     .Catch((Exception ex) => { }))
        {
            if (context.Record is IMajorRecordGetter majRec
                && editorId.Equals(majRec.EditorID))
            {
                majorRec = context;
                return true;
            }
        }

        majorRec = default;
        return false;
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

        if (target == ResolveTarget.Origin
            && formKey.ModKey != _sourceMod.ModKey)
        {
            majorRec = default;
            return false;
        }
            
        // ToDo
        // Upgrade to EnumerateGroups<TMajor>()
        foreach (var major in _sourceMod.EnumerateMajorRecordContexts(this, type)
                     // ToDo
                     // Capture and expose errors optionally via TryResolve /w out param
                     .Catch((Exception ex) => { }))
        {
            if (major.Record.FormKey == formKey)
            {
                majorRec = major;
                return true;
            }
        }

        majorRec = default;
        return false;
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
            
        if (editorId.IsNullOrWhitespace())
        {
            majorRec = default;
            return false;
        }
        // ToDo
        // Upgrade to EnumerateGroups<TMajor>()
        foreach (var major in _sourceMod.EnumerateMajorRecordContexts(this, type)
                     // ToDo
                     // Capture and expose errors optionally via TryResolve /w out param
                     .Catch((Exception ex) => { }))
        {
            if (editorId.Equals(major.Record.EditorID))
            {
                majorRec = major;
                return true;
            }
        }

        majorRec = default;
        return false;
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
    public IEnumerable<TMajor> ResolveAll<TMajor>(FormKey formKey, ResolveTarget target = ResolveTarget.Winner)
        where TMajor : class, IMajorRecordQueryableGetter
    {
        if (TryResolve<TMajor>(formKey, out var rec, target))
        {
            yield return rec;
        }
    }

    /// <inheritdoc />
    public IEnumerable<IMajorRecordGetter> ResolveAll(FormKey formKey, Type type, ResolveTarget target = ResolveTarget.Winner)
    {
        if (TryResolve(formKey, type, out var rec, target))
        {
            yield return rec;
        }
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
        if (TryResolve(formKey, out var rec, target))
        {
            yield return rec;
        }
    }

    /// <inheritdoc />
    [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
    public bool TryResolveSimpleContext(FormKey formKey, [MaybeNullWhen(false)] out IModContext<IMajorRecordGetter> majorRec, ResolveTarget target = ResolveTarget.Winner)
    {
        if (TryResolveContext(formKey, out var simple, target))
        {
            majorRec = simple;
            return true;
        }

        majorRec = default;
        return false;
    }

    /// <inheritdoc />
    [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
    public bool TryResolveSimpleContext(string editorId, [MaybeNullWhen(false)] out IModContext<IMajorRecordGetter> majorRec)
    {
        if (TryResolveContext(editorId, out var simple))
        {
            majorRec = simple;
            return true;
        }

        majorRec = default;
        return false;
    }

    /// <inheritdoc />
    public bool TryResolveSimpleContext<TMajor>(FormKey formKey, [MaybeNullWhen(false)] out IModContext<TMajor> majorRec, ResolveTarget target = ResolveTarget.Winner)
        where TMajor : class, IMajorRecordQueryableGetter
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
            
        // ToDo
        // Upgrade to EnumerateGroups<TMajor>()
        foreach (var context in _sourceMod.EnumerateMajorRecordSimpleContexts<TMajor>()
                     // ToDo
                     // Capture and expose errors optionally via TryResolve /w out param
                     .Catch((Exception ex) => { }))
        {
            if (context.Record is IMajorRecordGetter majRec && majRec.FormKey == formKey)
            {
                majorRec = context;
                return true;
            }
        }

        majorRec = default;
        return false;
    }

    /// <inheritdoc />
    public bool TryResolveSimpleContext<TMajor>(string editorId, [MaybeNullWhen(false)] out IModContext<TMajor> majorRec) 
        where TMajor : class, IMajorRecordQueryableGetter
    {
        CheckDisposal();
            
        if (editorId.IsNullOrWhitespace())
        {
            majorRec = default;
            return false;
        }
            
        // ToDo
        // Upgrade to EnumerateGroups<TMajor>()
        foreach (var context in _sourceMod.EnumerateMajorRecordSimpleContexts<TMajor>()
                     // ToDo
                     // Capture and expose errors optionally via TryResolve /w out param
                     .Catch((Exception ex) => { }))
        {
            if (context.Record is IMajorRecordGetter majRec && editorId.Equals(majRec.EditorID))
            {
                majorRec = context;
                return true;
            }
        }

        majorRec = default;
        return false;
    }

    /// <inheritdoc />
    public bool TryResolveSimpleContext(FormKey formKey, Type type, [MaybeNullWhen(false)] out IModContext<IMajorRecordGetter> majorRec, ResolveTarget target = ResolveTarget.Winner)
    {
        if (TryResolveContext(formKey, type, out var simple, target))
        {
            majorRec = simple;
            return true;
        }

        majorRec = default;
        return false;
    }

    /// <inheritdoc />
    public bool TryResolveSimpleContext(IFormLinkIdentifier formLink, [MaybeNullWhen(false)] out IModContext<IMajorRecordGetter> majorRec, ResolveTarget target = ResolveTarget.Winner)
    {
        return TryResolveSimpleContext(formLink.FormKey, formLink.Type, out majorRec, target);
    }

    /// <inheritdoc />
    public bool TryResolveSimpleContext(string editorId, Type type, [MaybeNullWhen(false)] out IModContext<IMajorRecordGetter> majorRec)
    {
        if (TryResolveContext(editorId, type, out var simple))
        {
            majorRec = simple;
            return true;
        }

        majorRec = default;
        return false;
    }

    /// <inheritdoc />
    [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
    public IModContext<IMajorRecordGetter> ResolveSimpleContext(FormKey formKey, ResolveTarget target = ResolveTarget.Winner)
    {
        return ResolveContext(formKey, target);
    }

    /// <inheritdoc />
    [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
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
    public IEnumerable<IModContext<IMajorRecordGetter>> ResolveAllSimpleContexts(IFormLinkIdentifier formLink, ResolveTarget target = ResolveTarget.Winner)
    {
        return ResolveAllSimpleContexts(formLink, target);
    }

    /// <inheritdoc />
    [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
    public IEnumerable<IModContext<IMajorRecordGetter>> ResolveAllSimpleContexts(FormKey formKey, ResolveTarget target = ResolveTarget.Winner)
    {
        if (TryResolveSimpleContext(formKey, out var rec, target))
        {
            yield return rec;
        }
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
    public IEnumerable<IModContext<TMod, TModGetter, IMajorRecord, IMajorRecordGetter>> ResolveAllContexts(IFormLinkIdentifier formLink, ResolveTarget target = ResolveTarget.Winner)
    {
        return ResolveAllContexts(formLink.FormKey, formLink.Type, target);
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

    /// <inheritdoc />
    public bool TryResolve(FormKey formKey, [MaybeNullWhen(false)] out IMajorRecordGetter majorRec, params Type[] types)
    {
        return TryResolve(formKey, (IEnumerable<Type>)types, out majorRec);
    }

    /// <inheritdoc />
    public bool TryResolve(string editorId, [MaybeNullWhen(false)] out IMajorRecordGetter majorRec, params Type[] types)
    {
        return TryResolve(editorId, (IEnumerable<Type>)types, out majorRec);
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
    public bool TryResolve(FormKey formKey, IEnumerable<Type> types, [MaybeNullWhen(false)] out IMajorRecordGetter majorRec, [MaybeNullWhen(false)] out Type matchedType,
        ResolveTarget target = ResolveTarget.Winner)
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

    /// <inheritdoc />
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

    /// <inheritdoc />
    public IMajorRecordGetter Resolve(FormKey formKey, params Type[] types)
    {
        return Resolve(formKey, (IEnumerable<Type>)types);
    }

    /// <inheritdoc />
    public IMajorRecordGetter Resolve(string editorId, params Type[] types)
    {
        return Resolve(editorId, (IEnumerable<Type>)types);
    }

    /// <inheritdoc />
    public IMajorRecordGetter Resolve(FormKey formKey, IEnumerable<Type> types, ResolveTarget target = ResolveTarget.Winner)
    {
        if (TryResolve(formKey, types, out var commonRec, target)) return commonRec;
        throw new MissingRecordException(formKey, types.ToArray());
    }

    /// <inheritdoc />
    public IMajorRecordGetter Resolve(string editorId, IEnumerable<Type> types)
    {
        if (TryResolve(editorId, types, out var commonRec)) return commonRec;
        throw new MissingRecordException(editorId, types.ToArray());
    }

    /// <inheritdoc />
    public bool TryResolveIdentifier(FormKey formKey, [MaybeNullWhen(false)] out string? editorId, ResolveTarget target = ResolveTarget.Winner)
    {
        if (TryResolve(formKey, out var rec, target))
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
        if (TryResolve(editorId, out var rec))
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
        if (TryResolve(formKey, type, out var rec, target))
        {
            editorId = rec.EditorID;
            return true;
        }
        editorId = default;
        return false;
    }

    /// <inheritdoc />
    public bool TryResolveIdentifier(IFormLinkIdentifier formLink, [MaybeNullWhen(false)] out string? editorId, ResolveTarget target = ResolveTarget.Winner)
    {
        return TryResolveIdentifier(formLink.FormKey, formLink.Type, out editorId, target);
    }

    /// <inheritdoc />
    public bool TryResolveIdentifier(string editorId, Type type, [MaybeNullWhen(false)] out FormKey formKey)
    {
        if (TryResolve(editorId, type, out var rec))
        {
            formKey = rec.FormKey;
            return true;
        }
        formKey = default;
        return false;
    }

    /// <inheritdoc />
    public bool TryResolveIdentifier<TMajor>(FormKey formKey, out string? editorId, ResolveTarget target = ResolveTarget.Winner)
        where TMajor : class, IMajorRecordQueryableGetter
    {
        if (TryResolve<TMajor>(formKey, out var rec, target)
            && rec is IMajorRecordGetter majRec)
        {
            editorId = majRec.EditorID;
            return true;
        }
        editorId = default;
        return false;
    }

    /// <inheritdoc />
    public bool TryResolveIdentifier<TMajor>(string editorId, out FormKey formKey)
        where TMajor : class, IMajorRecordQueryableGetter
    {
        if (TryResolve<TMajor>(editorId, out var rec) 
            && rec is IMajorRecordGetter majRec)
        {
            formKey = majRec.FormKey;
            return true;
        }
        formKey = default;
        return false;
    }

    /// <inheritdoc />
    public bool TryResolveIdentifier(FormKey formKey, [MaybeNullWhen(false)] out string? editorId, params Type[] types)
    {
        if (TryResolve(formKey, out var rec, types))
        {
            editorId = rec.EditorID;
            return true;
        }
        editorId = default;
        return false;
    }

    /// <inheritdoc />
    public bool TryResolveIdentifier(string editorId, [MaybeNullWhen(false)] out FormKey formKey, params Type[] types)
    {
        if (TryResolve(editorId, out var rec, types))
        {
            formKey = rec.FormKey;
            return true;
        }
        formKey = default;
        return false;
    }

    /// <inheritdoc />
    public bool TryResolveIdentifier(FormKey formKey, IEnumerable<Type> types, [MaybeNullWhen(false)] out string? editorId, ResolveTarget target = ResolveTarget.Winner)
    {
        if (TryResolve(formKey, types, out var rec, target))
        {
            editorId = rec.EditorID;
            return true;
        }
        editorId = default;
        return false;
    }

    public bool TryResolveIdentifier(FormKey formKey, IEnumerable<Type> types, out string? editorId, [MaybeNullWhen(false)] out Type matchedType,
        ResolveTarget target = ResolveTarget.Winner)
    {
        if (TryResolve(formKey, types, out var rec, out matchedType, target))
        {
            editorId = rec.EditorID;
            return true;
        }
        editorId = default;
        return false;
    }

    /// <inheritdoc />
    public bool TryResolveIdentifier(string editorId, IEnumerable<Type> types, [MaybeNullWhen(false)] out FormKey formKey)
    {
        if (TryResolve(editorId, types, out var rec))
        {
            formKey = rec.FormKey;
            return true;
        }
        formKey = default;
        return false;
    }

    public bool TryResolveIdentifier(string editorId, IEnumerable<Type> types, out FormKey formKey, out Type matchedType)
    {
        if (TryResolve(editorId, types, out var rec, out matchedType))
        {
            formKey = rec.FormKey;
            return true;
        }
        formKey = default;
        return false;
    }

    /// <inheritdoc />
    public string? ResolveIdentifier(FormKey formKey, ResolveTarget target = ResolveTarget.Winner)
    {
        if (TryResolveIdentifier(formKey, out var edid, target)) return edid;
        throw new MissingRecordException(formKey, typeof(IMajorRecordGetter));
    }

    public FormKey ResolveIdentifier(string editorId)
    {
        if (TryResolveIdentifier(editorId, out var formKey)) return formKey;
        throw new MissingRecordException(editorId, typeof(IMajorRecordGetter));
    }

    public string? ResolveIdentifier(FormKey formKey, Type type, ResolveTarget target = ResolveTarget.Winner)
    {
        if (TryResolveIdentifier(formKey, type, out var edid, target)) return edid;
        throw new MissingRecordException(formKey, typeof(IMajorRecordGetter));
    }

    public string? ResolveIdentifier(IFormLinkIdentifier formLink, ResolveTarget target = ResolveTarget.Winner)
    {
        if (TryResolveIdentifier(formLink.FormKey, out var edid, target)) return edid;
        throw new MissingRecordException(formLink);
    }

    public FormKey ResolveIdentifier(string editorId, Type type)
    {
        if (TryResolveIdentifier(editorId, type, out var formKey)) return formKey;
        throw new MissingRecordException(editorId, type);
    }

    public string? ResolveIdentifier<TMajor>(FormKey formKey, ResolveTarget target = ResolveTarget.Winner) where TMajor : class, IMajorRecordQueryableGetter
    {
        if (TryResolveIdentifier(formKey, out var edid, target)) return edid;
        throw new MissingRecordException(formKey, typeof(TMajor));
    }

    public FormKey ResolveIdentifier<TMajor>(string editorId) where TMajor : class, IMajorRecordQueryableGetter
    {
        if (TryResolveIdentifier(editorId, out var formKey)) return formKey;
        throw new MissingRecordException(editorId, typeof(TMajor));
    }

    public string? ResolveIdentifier(FormKey formKey, params Type[] types)
    {
        if (TryResolveIdentifier(formKey, types, out var editorId)) return editorId;
        throw new MissingRecordException(formKey, types);
    }

    public FormKey ResolveIdentifier(string editorId, params Type[] types)
    {
        if (TryResolveIdentifier(editorId, types, out var formKey)) return formKey;
        throw new MissingRecordException(editorId, types);
    }

    public string? ResolveIdentifier(FormKey formKey, IEnumerable<Type> types, ResolveTarget target = ResolveTarget.Winner)
    {
        if (TryResolveIdentifier(formKey, types, out var editorId)) return editorId;
        throw new MissingRecordException(formKey, types.ToArray());
    }

    public FormKey ResolveIdentifier(string editorId, IEnumerable<Type> types)
    {
        if (TryResolveIdentifier(editorId, types, out var formKey)) return formKey;
        throw new MissingRecordException(editorId, types.ToArray());
    }

    /// <inheritdoc />
    public IEnumerable<IMajorRecordIdentifierGetter> AllIdentifiers(Type type, CancellationToken? cancel = null)
    {
        return AllIdentifiersNoUniqueness(type, cancel)
            .Distinct(x => x.FormKey);
    }

    internal IEnumerable<IMajorRecordIdentifierGetter> AllIdentifiersNoUniqueness(Type type, CancellationToken? cancel)
    {
        CheckDisposal();
            
        return _sourceMod.EnumerateMajorRecords(type);
    }

    /// <inheritdoc />
    public IEnumerable<IMajorRecordIdentifierGetter> AllIdentifiers<TMajor>(CancellationToken? cancel = null)
        where TMajor : class, IMajorRecordQueryableGetter
    {
        return AllIdentifiers(typeof(TMajor), cancel);
    }

    /// <inheritdoc />
    public IEnumerable<IMajorRecordIdentifierGetter> AllIdentifiers(params Type[] types)
    {
        return AllIdentifiers((IEnumerable<Type>)types, CancellationToken.None);
    }

    /// <inheritdoc />
    public IEnumerable<IMajorRecordIdentifierGetter> AllIdentifiers(IEnumerable<Type> types, CancellationToken? cancel = null)
    {
        return types.SelectMany(type => AllIdentifiersNoUniqueness(type, cancel))
            .Distinct(x => x.FormKey);
    }

    /// <inheritdoc />
    public bool TryResolve<TMajor>(IFormLinkGetter<TMajor> formLink, [MaybeNullWhen(false)] out TMajor majorRec, ResolveTarget target = ResolveTarget.Winner)
        where TMajor : class, IMajorRecordGetter
    {
        return TryResolve<TMajor>(formLink.FormKey, out majorRec, target);
    }

    /// <inheritdoc />
    public TMajor Resolve<TMajor>(IFormLinkGetter<TMajor> formLink, ResolveTarget target = ResolveTarget.Winner)
        where TMajor : class, IMajorRecordGetter
    {
        return Resolve<TMajor>(formLink.FormKey, target);
    }

    /// <inheritdoc />
    public bool TryResolveSimpleContext<TMajor>(IFormLinkGetter<TMajor> formLink, [MaybeNullWhen(false)] out IModContext<TMajor> majorRec, ResolveTarget target = ResolveTarget.Winner)
        where TMajor : class, IMajorRecordGetter
    {
        return TryResolveSimpleContext<TMajor>(formLink.FormKey, out majorRec, target);
    }

    /// <inheritdoc />
    public IModContext<TMajor> ResolveSimpleContext<TMajor>(IFormLinkGetter<TMajor> formLink, ResolveTarget target = ResolveTarget.Winner)
        where TMajor : class, IMajorRecordGetter
    {
        return ResolveContext(formLink.FormKey, typeof(TMajor), target).AsType<IMajorRecordQueryableGetter, TMajor>();
    }

    /// <inheritdoc />
    public bool TryResolveContext<TMajor, TMajorGetter>(IFormLinkGetter<TMajorGetter> formLink, [MaybeNullWhen(false)] out IModContext<TMod, TModGetter, TMajor, TMajorGetter> majorRec, ResolveTarget target = ResolveTarget.Winner)
        where TMajor : class, IMajorRecord, TMajorGetter
        where TMajorGetter : class, IMajorRecordGetter
    {
        return TryResolveContext<TMajor, TMajorGetter>(formLink.FormKey, out majorRec, target);
    }

    /// <inheritdoc />
    public IModContext<TMod, TModGetter, TMajor, TMajorGetter> ResolveContext<TMajor, TMajorGetter>(IFormLinkGetter<TMajorGetter> formLink, ResolveTarget target = ResolveTarget.Winner)
        where TMajor : class, IMajorRecord, TMajorGetter
        where TMajorGetter : class, IMajorRecordGetter
    {
        return ResolveContext<TMajor, TMajorGetter>(formLink.FormKey, target);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _disposed = true;
    }

    public void Warmup(Type type)
    {
        CheckDisposal();
    }

    public void Warmup<TMajor>()
    {
        CheckDisposal();
    }

    public void Warmup(params Type[] types)
    {
        CheckDisposal();
    }

    public void Warmup(IEnumerable<Type> types)
    {
        CheckDisposal();
    }
}
