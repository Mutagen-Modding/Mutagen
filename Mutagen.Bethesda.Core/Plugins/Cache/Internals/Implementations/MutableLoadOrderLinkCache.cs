using System.Diagnostics.CodeAnalysis;
using DynamicData;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Cache.Internals.Implementations;

public sealed class MutableLoadOrderLinkCache : ILinkCache
{
    public ImmutableLoadOrderLinkCache? WrappedImmutableCache { get; }
    private readonly List<MutableModLinkCache> _mutableMods;
    private bool _disposed;

    /// <summary>
    /// Constructs a mutable load order link cache by combining an existing immutable load order cache,
    /// plus a set of mods to be put at the end of the load order and allow to be mutable.
    /// </summary>
    /// <param name="immutableBaseCache">LoadOrderCache to use as the immutable base</param>
    /// <param name="mutableMods">Set of mods to place at the end of the load order, which are allowed to be modified afterwards</param>
    public MutableLoadOrderLinkCache(ImmutableLoadOrderLinkCache immutableBaseCache, params IMod[] mutableMods)
    {
        WrappedImmutableCache = immutableBaseCache;
        _mutableMods = mutableMods.Select(m => m.ToUntypedMutableLinkCache()).ToList();
        ListedOrder = WrappedImmutableCache.ListedOrder.Concat(_mutableMods.Select(x => x.SourceMod)).ToArray();
        PriorityOrder = ListedOrder.Reverse().ToArray();
    }

    /// <summary>
    /// Constructs a mutable load order link cache by combining an existing immutable load order cache,
    /// plus a set of mods to be put at the end of the load order and allow to be mutable.
    /// </summary>
    /// <param name="mutableMods">Set of mods to place at the end of the load order, which are allowed to be modified afterwards</param>
    public MutableLoadOrderLinkCache(params IMod[] mutableMods)
    {
        _mutableMods = mutableMods.Select(m => m.ToUntypedMutableLinkCache()).ToList();
        ListedOrder = _mutableMods.Select(x => x.SourceMod).ToArray();
        PriorityOrder = ListedOrder.Reverse().ToArray();
    }

    /// <inheritdoc />
    public IReadOnlyList<IModGetter> ListedOrder { get; }

    /// <inheritdoc />
    public IReadOnlyList<IModGetter> PriorityOrder { get; }

    private void CheckDisposal()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException($"MutableLoadOrderLinkCache");
        }
    }

    /// <inheritdoc />
    [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
    public bool TryResolve(FormKey formKey, [MaybeNullWhen(false)] out IMajorRecordGetter majorRec, ResolveTarget target = ResolveTarget.Winner)
    {
        return TryResolve(formKey, typeof(IMajorRecordGetter), out majorRec, target);
    }

    /// <inheritdoc />
    [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
    public bool TryResolve(string editorId, [MaybeNullWhen(false)] out IMajorRecordGetter majorRec)
    {
        return TryResolve(editorId, typeof(IMajorRecordGetter), out majorRec);
    }

    /// <inheritdoc />
    public bool TryResolve<TMajor>(FormKey formKey, [MaybeNullWhen(false)] out TMajor majorRec, ResolveTarget target = ResolveTarget.Winner)
        where TMajor : class, IMajorRecordQueryableGetter
    {
        if (TryResolve(formKey, typeof(TMajor), out var majorRecInner, target))
        {
            majorRec = majorRecInner as TMajor;
            return majorRec != null;
        }

        majorRec = default;
        return false;
    }

    /// <inheritdoc />
    public bool TryResolve<TMajor>(TMajor record, [MaybeNullWhen(false)] out TMajor majorRec, ResolveTarget target = ResolveTarget.Winner) 
        where TMajor : class, IMajorRecordGetter
    {
        return TryResolve<TMajor>(record.FormKey, out majorRec, target);
    }
    
    /// <inheritdoc />
    public bool TryResolve<TMajor>(string editorId, [MaybeNullWhen(false)] out TMajor majorRec)
        where TMajor : class, IMajorRecordQueryableGetter
    {
        if (TryResolve(editorId, typeof(TMajor), out var majorRecInner))
        {
            majorRec = majorRecInner as TMajor;
            return majorRec != null;
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

        switch (target)
        {
            case ResolveTarget.Origin:
                if (WrappedImmutableCache?.TryResolve(formKey, type, out majorRec, target) ?? false) return true;
                foreach (var mod in _mutableMods)
                {
                    if (mod.TryResolve(formKey, type, out majorRec, target)) return true;
                }

                majorRec = default;
                return false;
            case ResolveTarget.Winner:
                for (int i = _mutableMods.Count - 1; i >= 0; i--)
                {
                    if (_mutableMods[i].TryResolve(formKey, type, out majorRec, target)) return true;
                }

                if (WrappedImmutableCache?.TryResolve(formKey, type, out majorRec, target) ?? false) return true;
                majorRec = default;
                return false;
            default:
                throw new NotImplementedException();
        }
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
        for (int i = _mutableMods.Count - 1; i >= 0; i--)
        {
            if (_mutableMods[i].TryResolve(editorId, type, out majorRec)) return true;
        }
        if (WrappedImmutableCache?.TryResolve(editorId, type, out majorRec) ?? false) return true;
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
    public TMajor Resolve<TMajor>(TMajor record, ResolveTarget target = ResolveTarget.Winner) 
        where TMajor : class, IMajorRecordGetter
    {
        return Resolve<TMajor>(record.FormKey, target);
    }
    
    /// <inheritdoc />
    public TMajor Resolve<TMajor>(string editorId)
        where TMajor : class, IMajorRecordQueryableGetter
    {
        if (TryResolve<TMajor>(editorId, out var commonRec)) return commonRec;
        throw new MissingRecordException(editorId, typeof(TMajor));
    }

    /// <summary>
    /// Adds a mutable mod to the end of the load order
    /// </summary>
    /// <param name="mod">Mod that is safe to mutate to add to end of load order</param>
    public void Add(IMod mod)
    {
        CheckDisposal();
        _mutableMods.Add(mod.ToUntypedMutableLinkCache());
    }
    
    /// <inheritdoc />
    public IEnumerable<TMajor> ResolveAll<TMajor>(FormKey formKey, ResolveTarget target = ResolveTarget.Winner)
        where TMajor : class, IMajorRecordQueryableGetter
    {
        return ResolveAll(formKey, typeof(TMajor), target).Cast<TMajor>();
    }
    
    /// <inheritdoc />
    public IEnumerable<TMajor> ResolveAll<TMajor>(TMajor record, ResolveTarget target = ResolveTarget.Winner)
        where TMajor : class, IMajorRecordGetter
    {
        return ResolveAll<TMajor>(record.FormKey, target);
    }

    /// <inheritdoc />
    public IEnumerable<IMajorRecordGetter> ResolveAll(FormKey formKey, Type type, ResolveTarget target = ResolveTarget.Winner)
    {
        CheckDisposal();
            
        switch (target)
        {
            case ResolveTarget.Origin:
                if (WrappedImmutableCache != null)
                {
                    foreach (var rec in WrappedImmutableCache.ResolveAll(formKey, type, target))
                    {
                        yield return rec;
                    }
                }

                foreach (var mod in _mutableMods)
                {
                    if (mod.TryResolve(formKey, type, out var majorRec, target))
                    {
                        yield return majorRec;
                    }
                }

                break;
            case ResolveTarget.Winner:
                for (int i = _mutableMods.Count - 1; i >= 0; i--)
                {
                    if (_mutableMods[i].TryResolve(formKey, type, out var majorRec))
                    {
                        yield return majorRec;
                    }
                }

                if (WrappedImmutableCache != null)
                {
                    foreach (var rec in WrappedImmutableCache.ResolveAll(formKey, type))
                    {
                        yield return rec;
                    }
                }

                break;
            default:
                throw new NotImplementedException();
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
        return ResolveAll(formKey, typeof(IMajorRecordGetter), target);
    }

    /// <inheritdoc />
    [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
    public bool TryResolveSimpleContext(FormKey formKey, [MaybeNullWhen(false)] out IModContext<IMajorRecordGetter> majorRec, ResolveTarget target = ResolveTarget.Winner)
    {
        return TryResolveSimpleContext(formKey, typeof(IMajorRecordGetter), out majorRec, target);
    }

    /// <inheritdoc />
    [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
    public bool TryResolveSimpleContext(string editorId, [MaybeNullWhen(false)] out IModContext<IMajorRecordGetter> majorRec)
    {
        return TryResolveSimpleContext(editorId, typeof(IMajorRecordGetter), out majorRec);
    }

    /// <inheritdoc />
    public bool TryResolveSimpleContext<TMajor>(
        FormKey formKey, 
        [MaybeNullWhen(false)] out IModContext<TMajor> majorRec,
        ResolveTarget target = ResolveTarget.Winner) 
        where TMajor : class, IMajorRecordQueryableGetter
    {
        CheckDisposal();
            
        if (formKey.IsNull)
        {
            majorRec = default;
            return false;
        }
        
        switch (target)
        {
            case ResolveTarget.Origin:
                if (WrappedImmutableCache?.TryResolveSimpleContext<TMajor>(formKey, out majorRec, target) ?? false) return true;
                foreach (var mod in _mutableMods)
                {
                    if (mod.TryResolveSimpleContext<TMajor>(formKey, out majorRec, target)) return true;
                }

                majorRec = default;
                return false;
            case ResolveTarget.Winner:
                for (int i = _mutableMods.Count - 1; i >= 0; i--)
                {
                    if (_mutableMods[i].TryResolveSimpleContext<TMajor>(formKey, out majorRec, target)) return true;
                }
                if (WrappedImmutableCache?.TryResolveSimpleContext<TMajor>(formKey, out majorRec, target) ?? false) return true;
                
                majorRec = default;
                return false;
            default:
                throw new NotImplementedException();
        }
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
        for (int i = _mutableMods.Count - 1; i >= 0; i--)
        {
            if (_mutableMods[i].TryResolveSimpleContext<TMajor>(editorId, out majorRec)) return true;
        }
        if (WrappedImmutableCache?.TryResolveSimpleContext<TMajor>(editorId, out majorRec) ?? false) return true;

        majorRec = default;
        return false;
    }

    /// <inheritdoc />
    public bool TryResolveSimpleContext(FormKey formKey, Type type, [MaybeNullWhen(false)] out IModContext<IMajorRecordGetter> majorRec,
        ResolveTarget target = ResolveTarget.Winner)
    {
        CheckDisposal();
            
        if (formKey.IsNull)
        {
            majorRec = default;
            return false;
        }
        
        switch (target)
        {
            case ResolveTarget.Origin:
                if (WrappedImmutableCache?.TryResolveSimpleContext(formKey, type, out majorRec, target) ?? false) return true;
                foreach (var mod in _mutableMods)
                {
                    if (mod.TryResolveSimpleContext(formKey, type, out majorRec, target)) return true;
                }

                majorRec = null;
                return false;
            case ResolveTarget.Winner:
                for (int i = _mutableMods.Count - 1; i >= 0; i--)
                {
                    if (_mutableMods[i].TryResolveSimpleContext(formKey, type, out majorRec, target)) return true;
                }
                if (WrappedImmutableCache?.TryResolveSimpleContext(formKey, type, out majorRec, target) ?? false) return true;
                
                majorRec = null;
                return false;
            default:
                throw new NotImplementedException();
        }
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
        CheckDisposal();
            
        if (editorId.IsNullOrWhitespace())
        {
            majorRec = default;
            return false;
        }
        for (int i = _mutableMods.Count - 1; i >= 0; i--)
        {
            if (_mutableMods[i].TryResolveSimpleContext(editorId, type, out majorRec)) return true;
        }
        if (WrappedImmutableCache?.TryResolveSimpleContext(editorId, type, out majorRec) ?? false) return true;

        majorRec = null;
        return false;
    }

    /// <inheritdoc />
    [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
    public IModContext<IMajorRecordGetter> ResolveSimpleContext(FormKey formKey, ResolveTarget target = ResolveTarget.Winner)
    {
        if (TryResolveSimpleContext(formKey, out var rec, target)) return rec;
        throw new MissingRecordException(formKey, typeof(IMajorRecordGetter));
    }

    /// <inheritdoc />
    [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
    public IModContext<IMajorRecordGetter> ResolveSimpleContext(string editorId)
    {
        if (TryResolveSimpleContext(editorId, out var rec)) return rec;
        throw new MissingRecordException(editorId, typeof(IMajorRecordGetter));
    }

    /// <inheritdoc />
    public IModContext<IMajorRecordGetter> ResolveSimpleContext(FormKey formKey, Type type, ResolveTarget target = ResolveTarget.Winner)
    {
        if (TryResolveSimpleContext(formKey, type, out var rec, target)) return rec;
        throw new MissingRecordException(formKey, typeof(IMajorRecordGetter));
    }

    /// <inheritdoc />
    public IModContext<IMajorRecordGetter> ResolveSimpleContext(IFormLinkIdentifier formLink, ResolveTarget target = ResolveTarget.Winner)
    {
        return ResolveSimpleContext(formLink.FormKey, formLink.Type, target);
    }
    
    /// <inheritdoc />
    public IModContext<TMajor> ResolveSimpleContext<TMajor>(TMajor record, ResolveTarget target = ResolveTarget.Winner) 
        where TMajor : class, IMajorRecordGetter
    {
        return ResolveSimpleContext<TMajor>(record.FormKey, target);
    }

    /// <inheritdoc />
    public IModContext<IMajorRecordGetter> ResolveSimpleContext(string editorId, Type type)
    {
        if (TryResolveSimpleContext(editorId, type, out var rec)) return rec;
        throw new MissingRecordException(editorId, typeof(IMajorRecordGetter));
    }

    /// <inheritdoc />
    public IModContext<TMajor> ResolveSimpleContext<TMajor>(FormKey formKey, ResolveTarget target = ResolveTarget.Winner) 
        where TMajor : class, IMajorRecordQueryableGetter
    {
        if (TryResolveSimpleContext<TMajor>(formKey, out var rec, target)) return rec;
        throw new MissingRecordException(formKey, typeof(IMajorRecordGetter));
    }

    /// <inheritdoc />
    public IModContext<TMajor> ResolveSimpleContext<TMajor>(string editorId) 
        where TMajor : class, IMajorRecordQueryableGetter
    {
        if (TryResolveSimpleContext<TMajor>(editorId, out var rec)) return rec;
        throw new MissingRecordException(editorId, typeof(IMajorRecordGetter));
    }

    /// <inheritdoc />
    public IEnumerable<IModContext<TMajor>> ResolveAllSimpleContexts<TMajor>(FormKey formKey, ResolveTarget target = ResolveTarget.Winner) 
        where TMajor : class, IMajorRecordQueryableGetter
    {
        CheckDisposal();
            
        switch (target)
        {
            case ResolveTarget.Origin:
                if (WrappedImmutableCache != null)
                {
                    foreach (var rec in WrappedImmutableCache.ResolveAllSimpleContexts<TMajor>(formKey, target))
                    {
                        yield return rec;
                    }
                }

                foreach (var mod in _mutableMods)
                {
                    if (mod.TryResolveSimpleContext<TMajor>(formKey, out var majorRec, target))
                    {
                        yield return majorRec;
                    }
                }

                break;
            case ResolveTarget.Winner:
                    
                for (int i = _mutableMods.Count - 1; i >= 0; i--)
                {
                    if (_mutableMods[i].TryResolveSimpleContext<TMajor>(formKey, out var majorRec))
                    {
                        yield return majorRec;
                    }
                }

                if (WrappedImmutableCache != null)
                {
                    foreach (var rec in WrappedImmutableCache.ResolveAllSimpleContexts<TMajor>(formKey))
                    {
                        yield return rec;
                    }
                }

                break;
            default:
                throw new NotImplementedException();
        }
    }

    /// <inheritdoc />
    public IEnumerable<IModContext<IMajorRecordGetter>> ResolveAllSimpleContexts(FormKey formKey, Type type, ResolveTarget target = ResolveTarget.Winner)
    {
        CheckDisposal();
            
        switch (target)
        {
            case ResolveTarget.Origin:
                if (WrappedImmutableCache != null)
                {
                    foreach (var rec in WrappedImmutableCache.ResolveAllSimpleContexts(formKey, type, target))
                    {
                        yield return rec;
                    }
                }

                foreach (var mod in _mutableMods)
                {
                    if (mod.TryResolveSimpleContext(formKey, type, out var majorRec, target))
                    {
                        yield return majorRec;
                    }
                }

                break;
            case ResolveTarget.Winner:
                    
                for (int i = _mutableMods.Count - 1; i >= 0; i--)
                {
                    if (_mutableMods[i].TryResolveSimpleContext(formKey, type, out var majorRec))
                    {
                        yield return majorRec;
                    }
                }

                if (WrappedImmutableCache != null)
                {
                    foreach (var rec in WrappedImmutableCache.ResolveAllSimpleContexts(formKey, type))
                    {
                        yield return rec;
                    }
                }

                break;
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
    public IEnumerable<IModContext<TMajor>> ResolveAllSimpleContexts<TMajor>(TMajor record, ResolveTarget target = ResolveTarget.Winner) 
        where TMajor : class, IMajorRecordGetter
    {
        return ResolveAllSimpleContexts<TMajor>(record.FormKey, target);
    }

    /// <inheritdoc />
    [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
    public IEnumerable<IModContext<IMajorRecordGetter>> ResolveAllSimpleContexts(FormKey formKey, ResolveTarget target = ResolveTarget.Winner)
    {
        return ResolveAllSimpleContexts(formKey, typeof(IMajorRecordGetter), target);
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
        return TryResolveIdentifier(formKey, typeof(IMajorRecordGetter), out editorId, target);
    }

    /// <inheritdoc />
    public bool TryResolveIdentifier(string editorId, [MaybeNullWhen(false)] out FormKey formKey)
    {
        CheckDisposal();
            
        if (editorId.IsNullOrWhitespace())
        {
            formKey = default;
            return false;
        }
        for (int i = _mutableMods.Count - 1; i >= 0; i--)
        {
            if (_mutableMods[i].TryResolveIdentifier(editorId, out formKey)) return true;
        }

        if (WrappedImmutableCache?.TryResolveIdentifier(editorId, out formKey) ?? false) return true;

        formKey = default;
        return false;
    }

    /// <inheritdoc />
    public bool TryResolveIdentifier(FormKey formKey, Type type, [MaybeNullWhen(false)] out string? editorId, ResolveTarget target = ResolveTarget.Winner)
    {
        CheckDisposal();
            
        if (formKey.IsNull)
        {
            editorId = default;
            return false;
        }
        switch (target)
        {
            case ResolveTarget.Origin:
                if (WrappedImmutableCache?.TryResolveIdentifier(formKey, type, out editorId, target) ?? false) return true;
                foreach (var mod in _mutableMods)
                {
                    if (mod.TryResolveIdentifier(formKey, type, out editorId)) return true;
                }

                editorId = default;
                return false;
            case ResolveTarget.Winner:
                for (int i = _mutableMods.Count - 1; i >= 0; i--)
                {
                    if (_mutableMods[i].TryResolveIdentifier(formKey, type, out editorId, target)) return true;
                }
                if (WrappedImmutableCache?.TryResolveIdentifier(formKey, type, out editorId, target) ?? false) return true;
                editorId = default;
                return false;
            default:
                throw new NotImplementedException();
        }
    }

    /// <inheritdoc />
    public bool TryResolveIdentifier(IFormLinkIdentifier formLink, [MaybeNullWhen(false)] out string? editorId, ResolveTarget target = ResolveTarget.Winner)
    {
        return TryResolveIdentifier(formLink.FormKey, formLink.Type, out editorId, target);
    }

    /// <inheritdoc />
    public bool TryResolveIdentifier(string editorId, Type type, [MaybeNullWhen(false)] out FormKey formKey)
    {
        CheckDisposal();
            
        if (editorId.IsNullOrWhitespace())
        {
            formKey = default;
            return false;
        }
        for (int i = _mutableMods.Count - 1; i >= 0; i--)
        {
            if (_mutableMods[i].TryResolveIdentifier(editorId, type, out formKey)) return true;
        }
        if (WrappedImmutableCache?.TryResolveIdentifier(editorId, type, out formKey) ?? false) return true;

        formKey = default;
        return false;
    }

    /// <inheritdoc />
    public bool TryResolveIdentifier<TMajor>(FormKey formKey, out string? editorId, ResolveTarget target = ResolveTarget.Winner)
        where TMajor : class, IMajorRecordQueryableGetter
    {
        return TryResolveIdentifier(formKey, typeof(TMajor), out editorId);
    }

    /// <inheritdoc />
    public bool TryResolveIdentifier<TMajor>(string editorId, out FormKey formKey)
        where TMajor : class, IMajorRecordQueryableGetter
    {
        return TryResolveIdentifier(editorId, typeof(TMajor), out formKey);
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
        foreach (var type in types)
        {
            if (TryResolveIdentifier(formKey, type, out editorId, target))
            {
                return true;
            }
        }
        editorId = default;
        return false;
    }

    public bool TryResolveIdentifier(FormKey formKey, IEnumerable<Type> types, out string? editorId, [MaybeNullWhen(false)] out Type matchedType,
        ResolveTarget target = ResolveTarget.Winner)
    {
        foreach (var type in types)
        {
            if (TryResolveIdentifier(formKey, type, out editorId, target))
            {
                matchedType = type;
                return true;
            }
        }
        editorId = default;
        matchedType = default;
        return false;
    }

    /// <inheritdoc />
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

    public bool TryResolveIdentifier(string editorId, IEnumerable<Type> types, out FormKey formKey, [MaybeNullWhen(false)] out Type matchedType)
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

    private IEnumerable<IMajorRecordIdentifierGetter> AllIdentifiersNoUniqueness(Type type, CancellationToken? cancel)
    {
        CheckDisposal();

        var ret = _mutableMods.SelectMany(x => x.AllIdentifiersNoUniqueness(type, cancel));
        if (WrappedImmutableCache != null)
        {
            ret = ret.Concat(WrappedImmutableCache.AllIdentifiers(type, cancel));
        }
        return ret;
    }

    /// <inheritdoc />
    public IEnumerable<IMajorRecordIdentifierGetter> AllIdentifiers(Type type, CancellationToken? cancel = null)
    {
        return AllIdentifiersNoUniqueness(type, cancel)
            .Distinct(x => x.FormKey);
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
        CheckDisposal();
        return TryResolve<TMajor>(formLink.FormKey, out majorRec, target);
    }

    /// <inheritdoc />
    public TMajor Resolve<TMajor>(IFormLinkGetter<TMajor> formLink, ResolveTarget target = ResolveTarget.Winner)
        where TMajor : class, IMajorRecordGetter
    {
        CheckDisposal();
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
        WrappedImmutableCache?.Warmup(type);
    }

    public void Warmup<TMajor>()
    {
        CheckDisposal();
        WrappedImmutableCache?.Warmup<TMajor>();
    }

    public void Warmup(params Type[] types)
    {
        CheckDisposal();
        WrappedImmutableCache?.Warmup(types);
    }

    public void Warmup(IEnumerable<Type> types)
    {
        CheckDisposal();
        WrappedImmutableCache?.Warmup(types);
    }
}

/// <summary>
/// A link cache that allows a top set of mods on the load order to be modified without
/// invalidating the cache.  This comes at a performance cost of needing to query those mods
/// for every request.
/// </summary>
public sealed class MutableLoadOrderLinkCache<TMod, TModGetter> : ILinkCache<TMod, TModGetter>
    where TMod : class, IContextMod<TMod, TModGetter>, TModGetter
    where TModGetter : class, IContextGetterMod<TMod, TModGetter>
{
    public ImmutableLoadOrderLinkCache<TMod, TModGetter> WrappedImmutableCache { get; }
    private readonly List<MutableModLinkCache<TMod, TModGetter>> _mutableMods;
    private bool _disposed;

    /// <summary>
    /// Constructs a mutable load order link cache by combining an existing immutable load order cache,
    /// plus a set of mods to be put at the end of the load order and allow to be mutable.
    /// </summary>
    /// <param name="immutableBaseCache">LoadOrderCache to use as the immutable base</param>
    /// <param name="mutableMods">Set of mods to place at the end of the load order, which are allowed to be modified afterwards</param>
    public MutableLoadOrderLinkCache(ImmutableLoadOrderLinkCache<TMod, TModGetter> immutableBaseCache, params TMod[] mutableMods)
    {
        WrappedImmutableCache = immutableBaseCache;
        _mutableMods = mutableMods.Select(m => m.ToMutableLinkCache<TMod, TModGetter>()).ToList();
        ListedOrder = WrappedImmutableCache.ListedOrder.Concat(_mutableMods.Select(x => x.SourceMod)).ToArray();
        PriorityOrder = ListedOrder.Reverse().ToArray();
    }

    /// <summary>
    /// Constructs a mutable load order link cache by combining an existing immutable load order cache,
    /// plus a set of mods to be put at the end of the load order and allow to be mutable.
    /// </summary>
    /// <param name="mutableMods">Set of mods to place at the end of the load order, which are allowed to be modified afterwards</param>
    public MutableLoadOrderLinkCache(params TMod[] mutableMods)
    {
        WrappedImmutableCache = ImmutableLoadOrderLinkCache<TMod, TModGetter>.Empty;
        _mutableMods = mutableMods.Select(m => m.ToMutableLinkCache<TMod, TModGetter>()).ToList();
        ListedOrder = _mutableMods.Select(x => x.SourceMod).ToArray();
        PriorityOrder = ListedOrder.Reverse().ToArray();
    }

    /// <inheritdoc />
    public IReadOnlyList<IModGetter> ListedOrder { get; }

    /// <inheritdoc />
    public IReadOnlyList<IModGetter> PriorityOrder { get; }

    private void CheckDisposal()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException($"MutableLoadOrderLinkCache<{typeof(TMod)}, {typeof(TModGetter)}>");
        }
    }

    /// <inheritdoc />
    [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
    public bool TryResolve(FormKey formKey, [MaybeNullWhen(false)] out IMajorRecordGetter majorRec, ResolveTarget target = ResolveTarget.Winner)
    {
        return TryResolve(formKey, typeof(IMajorRecordGetter), out majorRec, target);
    }

    /// <inheritdoc />
    [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
    public bool TryResolve(string editorId, [MaybeNullWhen(false)] out IMajorRecordGetter majorRec)
    {
        return TryResolve(editorId, typeof(IMajorRecordGetter), out majorRec);
    }

    /// <inheritdoc />
    public bool TryResolve<TMajor>(FormKey formKey, [MaybeNullWhen(false)] out TMajor majorRec, ResolveTarget target = ResolveTarget.Winner)
        where TMajor : class, IMajorRecordQueryableGetter
    {
        if (TryResolve(formKey, typeof(TMajor), out var majorRecInner, target))
        {
            majorRec = majorRecInner as TMajor;
            return majorRec != null;
        }

        majorRec = default;
        return false;
    }

    /// <inheritdoc />
    public bool TryResolve<TMajor>(TMajor record, [MaybeNullWhen(false)] out TMajor majorRec, ResolveTarget target = ResolveTarget.Winner) 
        where TMajor : class, IMajorRecordGetter
    {
        return TryResolve<TMajor>(record.FormKey, out majorRec, target);
    }
    
    /// <inheritdoc />
    public bool TryResolve<TMajor>(string editorId, [MaybeNullWhen(false)] out TMajor majorRec)
        where TMajor : class, IMajorRecordQueryableGetter
    {
        if (TryResolve(editorId, typeof(TMajor), out var majorRecInner))
        {
            majorRec = majorRecInner as TMajor;
            return majorRec != null;
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

        switch (target)
        {
            case ResolveTarget.Origin:
                if (WrappedImmutableCache.TryResolve(formKey, type, out majorRec, target)) return true;
                foreach (var mod in _mutableMods)
                {
                    if (mod.TryResolve(formKey, type, out majorRec, target)) return true;
                }

                return false;
            case ResolveTarget.Winner:
                for (int i = _mutableMods.Count - 1; i >= 0; i--)
                {
                    if (_mutableMods[i].TryResolve(formKey, type, out majorRec, target)) return true;
                }
                return WrappedImmutableCache.TryResolve(formKey, type, out majorRec, target);
            default:
                throw new NotImplementedException();
        }
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
        for (int i = _mutableMods.Count - 1; i >= 0; i--)
        {
            if (_mutableMods[i].TryResolve(editorId, type, out majorRec)) return true;
        }
        return WrappedImmutableCache.TryResolve(editorId, type, out majorRec);
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
    public TMajor Resolve<TMajor>(TMajor record, ResolveTarget target = ResolveTarget.Winner)
        where TMajor : class, IMajorRecordGetter
    {
        return Resolve<TMajor>(record.FormKey, target);
    }
    
    /// <inheritdoc />
    public TMajor Resolve<TMajor>(string editorId)
        where TMajor : class, IMajorRecordQueryableGetter
    {
        if (TryResolve<TMajor>(editorId, out var commonRec)) return commonRec;
        throw new MissingRecordException(editorId, typeof(TMajor));
    }

    /// <summary>
    /// Adds a mutable mod to the end of the load order
    /// </summary>
    /// <param name="mod">Mod that is safe to mutate to add to end of load order</param>
    public void Add(TMod mod)
    {
        CheckDisposal();
        _mutableMods.Add(mod.ToMutableLinkCache<TMod, TModGetter>());
    }

    /// <inheritdoc />
    [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
    public bool TryResolveContext(FormKey formKey, [MaybeNullWhen(false)] out IModContext<TMod, TModGetter, IMajorRecord, IMajorRecordGetter> majorRec, ResolveTarget target = ResolveTarget.Winner)
    {
        return TryResolveContext(formKey, typeof(IMajorRecordGetter), out majorRec, target);
    }

    /// <inheritdoc />
    [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
    public bool TryResolveContext(string editorId, [MaybeNullWhen(false)] out IModContext<TMod, TModGetter, IMajorRecord, IMajorRecordGetter> majorRec)
    {
        return TryResolveContext(editorId, typeof(IMajorRecordGetter), out majorRec);
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

        switch (target)
        {
            case ResolveTarget.Origin:
                if (WrappedImmutableCache.TryResolveContext<TMajor, TMajorGetter>(formKey, out majorRec, target)) return true;
                foreach (var mod in _mutableMods)
                {
                    if (mod.TryResolveContext<TMajor, TMajorGetter>(formKey, out majorRec, target)) return true;
                }

                return false;
            case ResolveTarget.Winner:
                for (int i = _mutableMods.Count - 1; i >= 0; i--)
                {
                    if (_mutableMods[i].TryResolveContext<TMajor, TMajorGetter>(formKey, out majorRec, target)) return true;
                }
                return WrappedImmutableCache.TryResolveContext<TMajor, TMajorGetter>(formKey, out majorRec, target);
            default:
                throw new NotImplementedException();
        }
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
        for (int i = _mutableMods.Count - 1; i >= 0; i--)
        {
            if (_mutableMods[i].TryResolveContext<TMajor, TMajorGetter>(editorId, out majorRec)) return true;
        }
        return WrappedImmutableCache.TryResolveContext<TMajor, TMajorGetter>(editorId, out majorRec);
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

        switch (target)
        {
            case ResolveTarget.Origin:
                if (WrappedImmutableCache.TryResolveContext(formKey, type, out majorRec, target)) return true;
                foreach (var mod in _mutableMods)
                {
                    if (mod.TryResolveContext(formKey, type, out majorRec, target)) return true;
                }

                return false;
            case ResolveTarget.Winner:
                for (int i = _mutableMods.Count - 1; i >= 0; i--)
                {
                    if (_mutableMods[i].TryResolveContext(formKey, type, out majorRec, target)) return true;
                }
                return WrappedImmutableCache.TryResolveContext(formKey, type, out majorRec, target);
            default:
                throw new NotImplementedException();
        }
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
        for (int i = _mutableMods.Count - 1; i >= 0; i--)
        {
            if (_mutableMods[i].TryResolveContext(editorId, type, out majorRec)) return true;
        }
        return WrappedImmutableCache.TryResolveContext(editorId, type, out majorRec);
    }

    /// <inheritdoc />
    [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
    public IModContext<TMod, TModGetter, IMajorRecord, IMajorRecordGetter> ResolveContext(FormKey formKey, ResolveTarget target = ResolveTarget.Winner)
    {
        if (TryResolveContext(formKey, out var majorRec, target)) return majorRec;
        throw new MissingRecordException(formKey, typeof(IMajorRecordGetter));
    }

    /// <inheritdoc />
    [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
    public IModContext<TMod, TModGetter, IMajorRecord, IMajorRecordGetter> ResolveContext(string editorId)
    {
        if (TryResolveContext(editorId, out var majorRec)) return majorRec;
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
    public IModContext<TMod, TModGetter, TMajor, TMajorGetter> ResolveContext<TMajor, TMajorGetter>(TMajor record, ResolveTarget target = ResolveTarget.Winner) 
        where TMajor : class, IMajorRecord, TMajorGetter 
        where TMajorGetter : class, IMajorRecordGetter
    {
        return ResolveContext<TMajor, TMajorGetter>(record.FormKey, target);
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
        return ResolveAll(formKey, typeof(TMajor), target).Cast<TMajor>();
    }
    
    /// <inheritdoc />
    public IEnumerable<TMajor> ResolveAll<TMajor>(TMajor record, ResolveTarget target = ResolveTarget.Winner)
        where TMajor : class, IMajorRecordGetter
    {
        return ResolveAll<TMajor>(record.FormKey, target);
    }

    /// <inheritdoc />
    public IEnumerable<IMajorRecordGetter> ResolveAll(FormKey formKey, Type type, ResolveTarget target = ResolveTarget.Winner)
    {
        CheckDisposal();
            
        switch (target)
        {
            case ResolveTarget.Origin:
                foreach (var rec in WrappedImmutableCache.ResolveAll(formKey, type, target))
                {
                    yield return rec;
                }

                foreach (var mod in _mutableMods)
                {
                    if (mod.TryResolve(formKey, type, out var majorRec, target))
                    {
                        yield return majorRec;
                    }
                }

                break;
            case ResolveTarget.Winner:
                for (int i = _mutableMods.Count - 1; i >= 0; i--)
                {
                    if (_mutableMods[i].TryResolve(formKey, type, out var majorRec))
                    {
                        yield return majorRec;
                    }
                }
                foreach (var rec in WrappedImmutableCache.ResolveAll(formKey, type))
                {
                    yield return rec;
                }

                break;
            default:
                throw new NotImplementedException();
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
        return ResolveAll(formKey, typeof(IMajorRecordGetter), target);
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
    [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
    public IModContext<IMajorRecordGetter> ResolveSimpleContext(FormKey formKey, ResolveTarget target = ResolveTarget.Winner)
    {
        if (TryResolveSimpleContext(formKey, out var rec, target)) return rec;
        throw new MissingRecordException(formKey, typeof(IMajorRecordGetter));
    }

    /// <inheritdoc />
    [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
    public IModContext<IMajorRecordGetter> ResolveSimpleContext(string editorId)
    {
        if (TryResolveSimpleContext(editorId, out var rec)) return rec;
        throw new MissingRecordException(editorId, typeof(IMajorRecordGetter));
    }

    /// <inheritdoc />
    public IModContext<IMajorRecordGetter> ResolveSimpleContext(FormKey formKey, Type type, ResolveTarget target = ResolveTarget.Winner)
    {
        if (TryResolveSimpleContext(formKey, type, out var rec, target)) return rec;
        throw new MissingRecordException(formKey, typeof(IMajorRecordGetter));
    }

    /// <inheritdoc />
    public IModContext<IMajorRecordGetter> ResolveSimpleContext(IFormLinkIdentifier formLink, ResolveTarget target = ResolveTarget.Winner)
    {
        return ResolveSimpleContext(formLink.FormKey, formLink.Type, target);
    }
    
    /// <inheritdoc />
    public IModContext<TMajor> ResolveSimpleContext<TMajor>(TMajor record, ResolveTarget target = ResolveTarget.Winner)
        where TMajor : class, IMajorRecordGetter
    {
        return ResolveSimpleContext<TMajor>(record.FormKey, target);
    }

    /// <inheritdoc />
    public IModContext<IMajorRecordGetter> ResolveSimpleContext(string editorId, Type type)
    {
        if (TryResolveSimpleContext(editorId, type, out var rec)) return rec;
        throw new MissingRecordException(editorId, typeof(IMajorRecordGetter));
    }

    /// <inheritdoc />
    public IModContext<TMajor> ResolveSimpleContext<TMajor>(FormKey formKey, ResolveTarget target = ResolveTarget.Winner) 
        where TMajor : class, IMajorRecordQueryableGetter
    {
        if (TryResolveSimpleContext<TMajor>(formKey, out var rec, target)) return rec;
        throw new MissingRecordException(formKey, typeof(IMajorRecordGetter));
    }

    /// <inheritdoc />
    public IModContext<TMajor> ResolveSimpleContext<TMajor>(string editorId) 
        where TMajor : class, IMajorRecordQueryableGetter
    {
        if (TryResolveSimpleContext<TMajor>(editorId, out var rec)) return rec;
        throw new MissingRecordException(editorId, typeof(IMajorRecordGetter));
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
        return ResolveAllContexts(formLink.FormKey, formLink.Type, target);
    }
    
    /// <inheritdoc />
    public IEnumerable<IModContext<TMajor>> ResolveAllSimpleContexts<TMajor>(TMajor record, ResolveTarget target = ResolveTarget.Winner)
        where TMajor : class, IMajorRecordGetter
    {
        return ResolveAllSimpleContexts<TMajor>(record.FormKey, target);
    }

    /// <inheritdoc />
    [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
    public IEnumerable<IModContext<IMajorRecordGetter>> ResolveAllSimpleContexts(FormKey formKey, ResolveTarget target = ResolveTarget.Winner)
    {
        return ResolveAllContexts(formKey, target);
    }

    /// <inheritdoc />
    public IEnumerable<IModContext<TMod, TModGetter, TMajor, TMajorGetter>> ResolveAllContexts<TMajor, TMajorGetter>(FormKey formKey, ResolveTarget target = ResolveTarget.Winner)
        where TMajor : class, IMajorRecordQueryable, TMajorGetter
        where TMajorGetter : class, IMajorRecordQueryableGetter
    {
        CheckDisposal();
            
        switch (target)
        {
            case ResolveTarget.Origin:
                foreach (var rec in WrappedImmutableCache.ResolveAllContexts<TMajor, TMajorGetter>(formKey, target))
                {
                    yield return rec;
                }

                foreach (var mod in _mutableMods)
                {
                    if (mod.TryResolveContext<TMajor, TMajorGetter>(formKey, out var majorRec, target))
                    {
                        yield return majorRec;
                    }
                }

                break;
            case ResolveTarget.Winner:
                    
                for (int i = _mutableMods.Count - 1; i >= 0; i--)
                {
                    if (_mutableMods[i].TryResolveContext<TMajor, TMajorGetter>(formKey, out var majorRec))
                    {
                        yield return majorRec;
                    }
                }
                foreach (var rec in WrappedImmutableCache.ResolveAllContexts<TMajor, TMajorGetter>(formKey))
                {
                    yield return rec;
                }

                break;
            default:
                throw new NotImplementedException();
        }
    }

    /// <inheritdoc />
    public IEnumerable<IModContext<TMod, TModGetter, IMajorRecord, IMajorRecordGetter>> ResolveAllContexts(FormKey formKey, Type type, ResolveTarget target = ResolveTarget.Winner)
    {
        CheckDisposal();
            
        switch (target)
        {
            case ResolveTarget.Origin:
                foreach (var rec in WrappedImmutableCache.ResolveAllContexts(formKey, type, target))
                {
                    yield return rec;
                }

                foreach (var mod in _mutableMods)
                {
                    if (mod.TryResolveContext(formKey, type, out var majorRec, target))
                    {
                        yield return majorRec;
                    }
                }

                break;
            case ResolveTarget.Winner:
                    
                for (int i = _mutableMods.Count - 1; i >= 0; i--)
                {
                    if (_mutableMods[i].TryResolveContext(formKey, type, out var majorRec))
                    {
                        yield return majorRec;
                    }
                }
                foreach (var rec in WrappedImmutableCache.ResolveAllContexts(formKey, type))
                {
                    yield return rec;
                }

                break;
            default:
                throw new NotImplementedException();
        }
    }

    /// <inheritdoc />
    public IEnumerable<IModContext<TMod, TModGetter, IMajorRecord, IMajorRecordGetter>> ResolveAllContexts(IFormLinkIdentifier formLink, ResolveTarget target = ResolveTarget.Winner)
    {
        return ResolveAllContexts(formLink.FormKey, formLink.Type, target);
    }
    
    /// <inheritdoc />
    public IEnumerable<IModContext<TMod, TModGetter, TMajor, TMajorGetter>> ResolveAllContexts<TMajor, TMajorGetter>(TMajor record, ResolveTarget target = ResolveTarget.Winner)
        where TMajor : class, IMajorRecord, TMajorGetter 
        where TMajorGetter : class, IMajorRecordGetter
    {
        return ResolveAllContexts<TMajor, TMajorGetter>(record.FormKey, target);
    }
    
    /// <inheritdoc />
    public IEnumerable<IModContext<TMod, TModGetter, TMajor, TMajorGetter>> ResolveAllContexts<TMajor, TMajorGetter>(TMajorGetter record, ResolveTarget target = ResolveTarget.Winner)
        where TMajor : class, IMajorRecord, TMajorGetter 
        where TMajorGetter : class, IMajorRecordGetter
    {
        return ResolveAllContexts<TMajor, TMajorGetter>(record.FormKey, target);
    }

    /// <inheritdoc />
    [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
    public IEnumerable<IModContext<TMod, TModGetter, IMajorRecord, IMajorRecordGetter>> ResolveAllContexts(FormKey formKey, ResolveTarget target = ResolveTarget.Winner)
    {
        return ResolveAllContexts(formKey, typeof(IMajorRecordGetter), target);
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
        return TryResolveIdentifier(formKey, typeof(IMajorRecordGetter), out editorId, target);
    }

    /// <inheritdoc />
    public bool TryResolveIdentifier(string editorId, [MaybeNullWhen(false)] out FormKey formKey)
    {
        CheckDisposal();
            
        if (editorId.IsNullOrWhitespace())
        {
            formKey = default;
            return false;
        }
        for (int i = _mutableMods.Count - 1; i >= 0; i--)
        {
            if (_mutableMods[i].TryResolveIdentifier(editorId, out formKey)) return true;
        }
        return WrappedImmutableCache.TryResolveIdentifier(editorId, out formKey);
    }

    /// <inheritdoc />
    public bool TryResolveIdentifier(FormKey formKey, Type type, [MaybeNullWhen(false)] out string? editorId, ResolveTarget target = ResolveTarget.Winner)
    {
        CheckDisposal();
            
        if (formKey.IsNull)
        {
            editorId = default;
            return false;
        }
        switch (target)
        {
            case ResolveTarget.Origin:
                if (WrappedImmutableCache.TryResolveIdentifier(formKey, type, out editorId, target)) return true;
                foreach (var mod in _mutableMods)
                {
                    if (mod.TryResolveIdentifier(formKey, type, out editorId)) return true;
                }

                return false;
            case ResolveTarget.Winner:
                for (int i = _mutableMods.Count - 1; i >= 0; i--)
                {
                    if (_mutableMods[i].TryResolveIdentifier(formKey, type, out editorId, target)) return true;
                }
                return WrappedImmutableCache.TryResolveIdentifier(formKey, type, out editorId, target);
            default:
                throw new NotImplementedException();
        }
    }

    /// <inheritdoc />
    public bool TryResolveIdentifier(IFormLinkIdentifier formLink, [MaybeNullWhen(false)] out string? editorId, ResolveTarget target = ResolveTarget.Winner)
    {
        return TryResolveIdentifier(formLink.FormKey, formLink.Type, out editorId, target);
    }

    /// <inheritdoc />
    public bool TryResolveIdentifier(string editorId, Type type, [MaybeNullWhen(false)] out FormKey formKey)
    {
        CheckDisposal();
            
        if (editorId.IsNullOrWhitespace())
        {
            formKey = default;
            return false;
        }
        for (int i = _mutableMods.Count - 1; i >= 0; i--)
        {
            if (_mutableMods[i].TryResolveIdentifier(editorId, type, out formKey)) return true;
        }
        return WrappedImmutableCache.TryResolveIdentifier(editorId, type, out formKey);
    }

    /// <inheritdoc />
    public bool TryResolveIdentifier<TMajor>(FormKey formKey, out string? editorId, ResolveTarget target = ResolveTarget.Winner)
        where TMajor : class, IMajorRecordQueryableGetter
    {
        return TryResolveIdentifier(formKey, typeof(TMajor), out editorId);
    }

    /// <inheritdoc />
    public bool TryResolveIdentifier<TMajor>(string editorId, out FormKey formKey)
        where TMajor : class, IMajorRecordQueryableGetter
    {
        return TryResolveIdentifier(editorId, typeof(TMajor), out formKey);
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
        foreach (var type in types)
        {
            if (TryResolveIdentifier(formKey, type, out editorId, target))
            {
                return true;
            }
        }
        editorId = default;
        return false;
    }

    public bool TryResolveIdentifier(FormKey formKey, IEnumerable<Type> types, out string? editorId, [MaybeNullWhen(false)] out Type matchedType,
        ResolveTarget target = ResolveTarget.Winner)
    {
        foreach (var type in types)
        {
            if (TryResolveIdentifier(formKey, type, out editorId, target))
            {
                matchedType = type;
                return true;
            }
        }
        matchedType = default;
        editorId = default;
        return false;
    }

    /// <inheritdoc />
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

    public bool TryResolveIdentifier(string editorId, IEnumerable<Type> types, out FormKey formKey, [MaybeNullWhen(false)] out Type matchedType)
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

    private IEnumerable<IMajorRecordIdentifierGetter> AllIdentifiersNoUniqueness(Type type, CancellationToken? cancel)
    {
        CheckDisposal();
            
        return _mutableMods.SelectMany(x => x.AllIdentifiersNoUniqueness(type, cancel))
            .Concat(WrappedImmutableCache.AllIdentifiers(type, cancel));
    }

    /// <inheritdoc />
    public IEnumerable<IMajorRecordIdentifierGetter> AllIdentifiers(Type type, CancellationToken? cancel = null)
    {
        return AllIdentifiersNoUniqueness(type, cancel)
            .Distinct(x => x.FormKey);
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
    public IModContext<TMod, TModGetter, TMajor, TMajorGetter> ResolveContext<TMajor, TMajorGetter>(TMajorGetter record, ResolveTarget target = ResolveTarget.Winner)
        where TMajor : class, IMajorRecord, TMajorGetter
        where TMajorGetter : class, IMajorRecordGetter
    {
        return ResolveContext<TMajor, TMajorGetter>(record.FormKey, target);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _disposed = true;
    }

    public void Warmup(Type type)
    {
        CheckDisposal();
        WrappedImmutableCache.Warmup(type);
    }

    public void Warmup<TMajor>()
    {
        CheckDisposal();
        WrappedImmutableCache.Warmup<TMajor>();
    }

    public void Warmup(params Type[] types)
    {
        CheckDisposal();
        WrappedImmutableCache.Warmup(types);
    }

    public void Warmup(IEnumerable<Type> types)
    {
        CheckDisposal();
        WrappedImmutableCache.Warmup(types);
    }
}