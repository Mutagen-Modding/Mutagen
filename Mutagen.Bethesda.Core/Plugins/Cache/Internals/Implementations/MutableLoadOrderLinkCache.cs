using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Cache.Internals.Implementations
{
    /// <summary>
    /// A link cache that allows a top set of mods on the load order to be modified without
    /// invalidating the cache.  This comes at a performance cost of needing to query those mods
    /// for every request.
    /// </summary>
    public class MutableLoadOrderLinkCache<TMod, TModGetter> : ILinkCache<TMod, TModGetter>
        where TMod : class, IContextMod<TMod, TModGetter>, TModGetter
        where TModGetter : class, IContextGetterMod<TMod, TModGetter>
    {
        public ImmutableLoadOrderLinkCache<TMod, TModGetter> WrappedImmutableCache { get; }
        private readonly List<MutableModLinkCache<TMod, TModGetter>> _mutableMods;

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
            where TMajor : class, IMajorRecordGetter
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
        public bool TryResolve<TMajor>(string editorId, [MaybeNullWhen(false)] out TMajor majorRec)
            where TMajor : class, IMajorRecordGetter
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
        public bool TryResolve(string editorId, Type type, [MaybeNullWhen(false)] out IMajorRecordGetter majorRec)
        {
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
        public IMajorRecordGetter Resolve(string editorId, Type type)
        {
            if (TryResolve(editorId, type, out var commonRec)) return commonRec;
            throw new MissingRecordException(editorId, type);
        }

        /// <inheritdoc />
        public TMajor Resolve<TMajor>(FormKey formKey, ResolveTarget target = ResolveTarget.Winner)
            where TMajor : class, IMajorRecordGetter
        {
            if (TryResolve<TMajor>(formKey, out var commonRec, target)) return commonRec;
            throw new MissingRecordException(formKey, typeof(TMajor));
        }

        /// <inheritdoc />
        public TMajor Resolve<TMajor>(string editorId)
            where TMajor : class, IMajorRecordGetter
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
            where TMajor : class, IMajorRecord, TMajorGetter
            where TMajorGetter : class, IMajorRecordGetter
        {
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
            where TMajor : class, IMajorRecord, TMajorGetter
            where TMajorGetter : class, IMajorRecordGetter
        {
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
        public bool TryResolveContext(string editorId, Type type, [MaybeNullWhen(false)] out IModContext<TMod, TModGetter, IMajorRecord, IMajorRecordGetter> majorRec)
        {
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
        public IModContext<TMod, TModGetter, IMajorRecord, IMajorRecordGetter> ResolveContext(string editorId, Type type)
        {
            if (TryResolveContext(editorId, type, out var commonRec)) return commonRec;
            throw new MissingRecordException(editorId, type);
        }

        /// <inheritdoc />
        public IModContext<TMod, TModGetter, TMajor, TMajorGetter> ResolveContext<TMajor, TMajorGetter>(FormKey formKey, ResolveTarget target = ResolveTarget.Winner)
            where TMajor : class, IMajorRecord, TMajorGetter
            where TMajorGetter : class, IMajorRecordGetter
        {
            if (TryResolveContext<TMajor, TMajorGetter>(formKey, out var commonRec, target)) return commonRec;
            throw new MissingRecordException(formKey, typeof(TMajorGetter));
        }

        /// <inheritdoc />
        public IModContext<TMod, TModGetter, TMajor, TMajorGetter> ResolveContext<TMajor, TMajorGetter>(string editorId)
            where TMajor : class, IMajorRecord, TMajorGetter
            where TMajorGetter : class, IMajorRecordGetter
        {
            if (TryResolveContext<TMajor, TMajorGetter>(editorId, out var commonRec)) return commonRec;
            throw new MissingRecordException(editorId, typeof(TMajorGetter));
        }

        /// <inheritdoc />
        public IEnumerable<TMajor> ResolveAll<TMajor>(FormKey formKey, ResolveTarget target = ResolveTarget.Winner)
            where TMajor : class, IMajorRecordGetter
        {
            return ResolveAll(formKey, typeof(TMajor), target).Cast<TMajor>();
        }

        /// <inheritdoc />
        public IEnumerable<IMajorRecordGetter> ResolveAll(FormKey formKey, Type type, ResolveTarget target = ResolveTarget.Winner)
        {
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
        public bool TryResolveSimpleContext<TMajor>(FormKey formKey, [MaybeNullWhen(false)] out IModContext<TMajor> majorRec,
            ResolveTarget target = ResolveTarget.Winner) where TMajor : class, IMajorRecordGetter
        {
            if (TryResolveContext(formKey, typeof(TMajor), out var resolve, target))
            {
                majorRec = resolve.AsType<IMajorRecordGetter, TMajor>();
                return true;
            }

            majorRec = default;
            return false;
        }

        /// <inheritdoc />
        public bool TryResolveSimpleContext<TMajor>(string editorId, [MaybeNullWhen(false)] out IModContext<TMajor> majorRec) where TMajor : class, IMajorRecordGetter
        {
            if (TryResolveContext(editorId, typeof(TMajor), out var resolve))
            {
                majorRec = resolve.AsType<IMajorRecordGetter, TMajor>();
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
        public IModContext<IMajorRecordGetter> ResolveSimpleContext(string editorId, Type type)
        {
            if (TryResolveSimpleContext(editorId, type, out var rec)) return rec;
            throw new MissingRecordException(editorId, typeof(IMajorRecordGetter));
        }

        /// <inheritdoc />
        public IModContext<TMajor> ResolveSimpleContext<TMajor>(FormKey formKey, ResolveTarget target = ResolveTarget.Winner) where TMajor : class, IMajorRecordGetter
        {
            if (TryResolveSimpleContext<TMajor>(formKey, out var rec, target)) return rec;
            throw new MissingRecordException(formKey, typeof(IMajorRecordGetter));
        }

        /// <inheritdoc />
        public IModContext<TMajor> ResolveSimpleContext<TMajor>(string editorId) where TMajor : class, IMajorRecordGetter
        {
            if (TryResolveSimpleContext<TMajor>(editorId, out var rec)) return rec;
            throw new MissingRecordException(editorId, typeof(IMajorRecordGetter));
        }

        /// <inheritdoc />
        public IEnumerable<IModContext<TMajor>> ResolveAllSimpleContexts<TMajor>(FormKey formKey, ResolveTarget target = ResolveTarget.Winner) where TMajor : class, IMajorRecordGetter
        {
            return ResolveAllContexts(formKey, typeof(TMajor), target).Select(x => x.AsType<IMajorRecordGetter, TMajor>());
        }

        /// <inheritdoc />
        public IEnumerable<IModContext<IMajorRecordGetter>> ResolveAllSimpleContexts(FormKey formKey, Type type, ResolveTarget target = ResolveTarget.Winner)
        {
            return ResolveAllContexts(formKey, type, target);
        }

        /// <inheritdoc />
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public IEnumerable<IModContext<IMajorRecordGetter>> ResolveAllSimpleContexts(FormKey formKey, ResolveTarget target = ResolveTarget.Winner)
        {
            return ResolveAllContexts(formKey, target);
        }

        /// <inheritdoc />
        public IEnumerable<IModContext<TMod, TModGetter, TMajor, TMajorGetter>> ResolveAllContexts<TMajor, TMajorGetter>(FormKey formKey, ResolveTarget target = ResolveTarget.Winner)
            where TMajor : class, IMajorRecord, TMajorGetter
            where TMajorGetter : class, IMajorRecordGetter
        {
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
        public bool TryResolveIdentifier(string editorId, Type type, [MaybeNullWhen(false)] out FormKey formKey)
        {
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
            where TMajor : class, IMajorRecordGetter
        {
            return TryResolveIdentifier(formKey, typeof(TMajor), out editorId);
        }

        /// <inheritdoc />
        public bool TryResolveIdentifier<TMajor>(string editorId, out FormKey formKey)
            where TMajor : class, IMajorRecordGetter
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

        private IEnumerable<IMajorRecordIdentifier> AllIdentifiersNoUniqueness(Type type, CancellationToken? cancel)
        {
            return _mutableMods.SelectMany(x => x.AllIdentifiersNoUniqueness(type, cancel))
                .Concat(WrappedImmutableCache.AllIdentifiers(type, cancel));
        }

        /// <inheritdoc />
        public IEnumerable<IMajorRecordIdentifier> AllIdentifiers(Type type, CancellationToken? cancel = null)
        {
            return AllIdentifiersNoUniqueness(type, cancel)
                .Distinct(x => x.FormKey);
        }

        /// <inheritdoc />
        public IEnumerable<IMajorRecordIdentifier> AllIdentifiers<TMajor>(CancellationToken? cancel = null)
            where TMajor : class, IMajorRecordGetter
        {
            return AllIdentifiers(typeof(TMajor), cancel);
        }

        /// <inheritdoc />
        public IEnumerable<IMajorRecordIdentifier> AllIdentifiers(params Type[] types)
        {
            return AllIdentifiers((IEnumerable<Type>)types, CancellationToken.None);
        }

        /// <inheritdoc />
        public IEnumerable<IMajorRecordIdentifier> AllIdentifiers(IEnumerable<Type> types, CancellationToken? cancel = null)
        {
            return types.SelectMany(type => AllIdentifiersNoUniqueness(type, cancel))
                .Distinct(x => x.FormKey);
        }

        /// <inheritdoc />
        public void Dispose()
        {
        }

        public void Warmup(Type type)
        {
            WrappedImmutableCache.Warmup(type);
        }

        public void Warmup<TMajor>()
        {
            WrappedImmutableCache.Warmup<TMajor>();
        }

        public void Warmup(params Type[] types)
        {
            WrappedImmutableCache.Warmup(types);
        }

        public void Warmup(IEnumerable<Type> types)
        {
            WrappedImmutableCache.Warmup(types);
        }
    }
}
