using Noggog;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;

namespace Mutagen.Bethesda
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
        }

        /// <inheritdoc />
        public IReadOnlyList<IModGetter> ListedOrder => throw new NotImplementedException();

        /// <inheritdoc />
        public IReadOnlyList<IModGetter> PriorityOrder => throw new NotImplementedException();

        /// <inheritdoc />
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public bool TryResolve(FormKey formKey, [MaybeNullWhen(false)] out IMajorRecordCommonGetter majorRec)
        {
            if (formKey.IsNull)
            {
                majorRec = default;
                return false;
            }
            for (int i = _mutableMods.Count - 1; i >= 0; i--)
            {
                if (_mutableMods[i].TryResolve(formKey, out majorRec)) return true;
            }
            return WrappedImmutableCache.TryResolve(formKey, out majorRec);
        }

        /// <inheritdoc />
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public bool TryResolve(string editorId, [MaybeNullWhen(false)] out IMajorRecordCommonGetter majorRec)
        {
            if (editorId.IsNullOrWhitespace())
            {
                majorRec = default;
                return false;
            }
            for (int i = _mutableMods.Count - 1; i >= 0; i--)
            {
                if (_mutableMods[i].TryResolve(editorId, out majorRec)) return true;
            }
            return WrappedImmutableCache.TryResolve(editorId, out majorRec);
        }

        /// <inheritdoc />
        public bool TryResolve<TMajor>(FormKey formKey, [MaybeNullWhen(false)] out TMajor majorRec)
            where TMajor : class, IMajorRecordCommonGetter
        {
            if (formKey.IsNull)
            {
                majorRec = default;
                return false;
            }
            for (int i = _mutableMods.Count - 1; i >= 0; i--)
            {
                if (_mutableMods[i].TryResolve<TMajor>(formKey, out majorRec)) return true;
            }
            return WrappedImmutableCache.TryResolve<TMajor>(formKey, out majorRec);
        }

        /// <inheritdoc />
        public bool TryResolve<TMajor>(string editorId, [MaybeNullWhen(false)] out TMajor majorRec)
            where TMajor : class, IMajorRecordCommonGetter
        {
            if (editorId.IsNullOrWhitespace())
            {
                majorRec = default;
                return false;
            }
            for (int i = _mutableMods.Count - 1; i >= 0; i--)
            {
                if (_mutableMods[i].TryResolve<TMajor>(editorId, out majorRec)) return true;
            }
            return WrappedImmutableCache.TryResolve<TMajor>(editorId, out majorRec);
        }

        /// <inheritdoc />
        public bool TryResolve(FormKey formKey, Type type, [MaybeNullWhen(false)] out IMajorRecordCommonGetter majorRec)
        {
            if (formKey.IsNull)
            {
                majorRec = default;
                return false;
            }
            for (int i = _mutableMods.Count - 1; i >= 0; i--)
            {
                if (_mutableMods[i].TryResolve(formKey, type, out majorRec)) return true;
            }
            return WrappedImmutableCache.TryResolve(formKey, type, out majorRec);
        }

        /// <inheritdoc />
        public bool TryResolve(string editorId, Type type, [MaybeNullWhen(false)] out IMajorRecordCommonGetter majorRec)
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
        public IMajorRecordCommonGetter Resolve(FormKey formKey)
        {
            if (TryResolve<IMajorRecordCommonGetter>(formKey, out var majorRec)) return majorRec;
            throw new KeyNotFoundException($"FormKey {formKey} could not be found.");
        }

        /// <inheritdoc />
        public IMajorRecordCommonGetter Resolve(string editorId)
        {
            if (TryResolve<IMajorRecordCommonGetter>(editorId, out var majorRec)) return majorRec;
            throw new KeyNotFoundException($"EditorID {editorId} could not be found.");
        }

        /// <inheritdoc />
        public IMajorRecordCommonGetter Resolve(FormKey formKey, Type type)
        {
            if (TryResolve(formKey, type, out var commonRec)) return commonRec;
            throw new KeyNotFoundException($"FormKey {formKey} could not be found.");
        }

        /// <inheritdoc />
        public IMajorRecordCommonGetter Resolve(string editorId, Type type)
        {
            if (TryResolve(editorId, type, out var commonRec)) return commonRec;
            throw new KeyNotFoundException($"EditorID {editorId} could not be found.");
        }

        /// <inheritdoc />
        public TMajor Resolve<TMajor>(FormKey formKey)
            where TMajor : class, IMajorRecordCommonGetter
        {
            if (TryResolve<TMajor>(formKey, out var commonRec)) return commonRec;
            throw new KeyNotFoundException($"FormKey {formKey} could not be found.");
        }

        /// <inheritdoc />
        public TMajor Resolve<TMajor>(string editorId)
            where TMajor : class, IMajorRecordCommonGetter
        {
            if (TryResolve<TMajor>(editorId, out var commonRec)) return commonRec;
            throw new KeyNotFoundException($"EditorID {editorId} could not be found.");
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
        public bool TryResolveContext(FormKey formKey, [MaybeNullWhen(false)] out IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter> majorRec)
        {
            if (formKey.IsNull)
            {
                majorRec = default;
                return false;
            }
            for (int i = _mutableMods.Count - 1; i >= 0; i--)
            {
                if (_mutableMods[i].TryResolveContext(formKey, out majorRec)) return true;
            }
            return WrappedImmutableCache.TryResolveContext(formKey, out majorRec);
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
            for (int i = _mutableMods.Count - 1; i >= 0; i--)
            {
                if (_mutableMods[i].TryResolveContext(editorId, out majorRec)) return true;
            }
            return WrappedImmutableCache.TryResolveContext(editorId, out majorRec);
        }

        /// <inheritdoc />
        public bool TryResolveContext<TMajor, TMajorGetter>(FormKey formKey, [MaybeNullWhen(false)] out IModContext<TMod, TModGetter, TMajor, TMajorGetter> majorRec)
            where TMajor : class, IMajorRecordCommon, TMajorGetter
            where TMajorGetter : class, IMajorRecordCommonGetter
        {
            if (formKey.IsNull)
            {
                majorRec = default;
                return false;
            }
            for (int i = _mutableMods.Count - 1; i >= 0; i--)
            {
                if (_mutableMods[i].TryResolveContext<TMajor, TMajorGetter>(formKey, out majorRec)) return true;
            }
            return WrappedImmutableCache.TryResolveContext<TMajor, TMajorGetter>(formKey, out majorRec);
        }

        /// <inheritdoc />
        public bool TryResolveContext<TMajor, TMajorGetter>(string editorId, [MaybeNullWhen(false)] out IModContext<TMod, TModGetter, TMajor, TMajorGetter> majorRec)
            where TMajor : class, IMajorRecordCommon, TMajorGetter
            where TMajorGetter : class, IMajorRecordCommonGetter
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
        public bool TryResolveContext(FormKey formKey, Type type, [MaybeNullWhen(false)] out IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter> majorRec)
        {
            if (formKey.IsNull)
            {
                majorRec = default;
                return false;
            }
            for (int i = _mutableMods.Count - 1; i >= 0; i--)
            {
                if (_mutableMods[i].TryResolveContext(formKey, type, out majorRec)) return true;
            }
            return WrappedImmutableCache.TryResolveContext(formKey, type, out majorRec);
        }

        /// <inheritdoc />
        public bool TryResolveContext(string editorId, Type type, [MaybeNullWhen(false)] out IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter> majorRec)
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
        public IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter> ResolveContext(FormKey formKey)
        {
            if (TryResolveContext(formKey, out var majorRec)) return majorRec;
            throw new KeyNotFoundException($"FormKey {formKey} could not be found.");
        }

        /// <inheritdoc />
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter> ResolveContext(string editorId)
        {
            if (TryResolveContext(editorId, out var majorRec)) return majorRec;
            throw new KeyNotFoundException($"EditorID {editorId} could not be found.");
        }

        /// <inheritdoc />
        public IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter> ResolveContext(FormKey formKey, Type type)
        {
            if (TryResolveContext(formKey, type, out var commonRec)) return commonRec;
            throw new KeyNotFoundException($"FormKey {formKey} could not be found.");
        }

        /// <inheritdoc />
        public IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter> ResolveContext(string editorId, Type type)
        {
            if (TryResolveContext(editorId, type, out var commonRec)) return commonRec;
            throw new KeyNotFoundException($"EditorID {editorId} could not be found.");
        }

        /// <inheritdoc />
        public IModContext<TMod, TModGetter, TMajor, TMajorGetter> ResolveContext<TMajor, TMajorGetter>(FormKey formKey)
            where TMajor : class, IMajorRecordCommon, TMajorGetter
            where TMajorGetter : class, IMajorRecordCommonGetter
        {
            if (TryResolveContext<TMajor, TMajorGetter>(formKey, out var commonRec)) return commonRec;
            throw new KeyNotFoundException($"FormKey {formKey} could not be found.");
        }

        /// <inheritdoc />
        public IModContext<TMod, TModGetter, TMajor, TMajorGetter> ResolveContext<TMajor, TMajorGetter>(string editorId)
            where TMajor : class, IMajorRecordCommon, TMajorGetter
            where TMajorGetter : class, IMajorRecordCommonGetter
        {
            if (TryResolveContext<TMajor, TMajorGetter>(editorId, out var commonRec)) return commonRec;
            throw new KeyNotFoundException($"EditorID {editorId} could not be found.");
        }

        /// <inheritdoc />
        public IEnumerable<TMajor> ResolveAll<TMajor>(FormKey formKey)
            where TMajor : class, IMajorRecordCommonGetter
        {
            for (int i = _mutableMods.Count - 1; i >= 0; i--)
            {
                if (_mutableMods[i].TryResolve<TMajor>(formKey, out var majorRec))
                {
                    yield return majorRec;
                }
            }
            foreach (var rec in WrappedImmutableCache.ResolveAll<TMajor>(formKey))
            {
                yield return rec;
            }
        }

        /// <inheritdoc />
        public IEnumerable<TMajor> ResolveAll<TMajor>(string editorId)
            where TMajor : class, IMajorRecordCommonGetter
        {
            for (int i = _mutableMods.Count - 1; i >= 0; i--)
            {
                if (_mutableMods[i].TryResolve<TMajor>(editorId, out var majorRec))
                {
                    yield return majorRec;
                }
            }
            foreach (var rec in WrappedImmutableCache.ResolveAll<TMajor>(editorId))
            {
                yield return rec;
            }
        }

        /// <inheritdoc />
        public IEnumerable<IMajorRecordCommonGetter> ResolveAll(FormKey formKey, Type type)
        {
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
        }

        /// <inheritdoc />
        public IEnumerable<IMajorRecordCommonGetter> ResolveAll(string editorId, Type type)
        {
            for (int i = _mutableMods.Count - 1; i >= 0; i--)
            {
                if (_mutableMods[i].TryResolve(editorId, type, out var majorRec))
                {
                    yield return majorRec;
                }
            }
            foreach (var rec in WrappedImmutableCache.ResolveAll(editorId, type))
            {
                yield return rec;
            }
        }

        /// <inheritdoc />
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public IEnumerable<IMajorRecordCommonGetter> ResolveAll(FormKey formKey)
        {
            for (int i = _mutableMods.Count - 1; i >= 0; i--)
            {
                if (_mutableMods[i].TryResolve(formKey, out var majorRec))
                {
                    yield return majorRec;
                }
            }
            foreach (var rec in WrappedImmutableCache.ResolveAll(formKey))
            {
                yield return rec;
            }
        }

        /// <inheritdoc />
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public IEnumerable<IMajorRecordCommonGetter> ResolveAll(string editorId)
        {
            for (int i = _mutableMods.Count - 1; i >= 0; i--)
            {
                if (_mutableMods[i].TryResolve(editorId, out var majorRec))
                {
                    yield return majorRec;
                }
            }
            foreach (var rec in WrappedImmutableCache.ResolveAll(editorId))
            {
                yield return rec;
            }
        }

        /// <inheritdoc />
        public IEnumerable<IModContext<TMod, TModGetter, TMajor, TMajorGetter>> ResolveAllContexts<TMajor, TMajorGetter>(FormKey formKey)
            where TMajor : class, IMajorRecordCommon, TMajorGetter
            where TMajorGetter : class, IMajorRecordCommonGetter
        {
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
        }

        /// <inheritdoc />
        public IEnumerable<IModContext<TMod, TModGetter, TMajor, TMajorGetter>> ResolveAllContexts<TMajor, TMajorGetter>(string editorId)
            where TMajor : class, IMajorRecordCommon, TMajorGetter
            where TMajorGetter : class, IMajorRecordCommonGetter
        {
            for (int i = _mutableMods.Count - 1; i >= 0; i--)
            {
                if (_mutableMods[i].TryResolveContext<TMajor, TMajorGetter>(editorId, out var majorRec))
                {
                    yield return majorRec;
                }
            }
            foreach (var rec in WrappedImmutableCache.ResolveAllContexts<TMajor, TMajorGetter>(editorId))
            {
                yield return rec;
            }
        }

        /// <inheritdoc />
        public IEnumerable<IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter>> ResolveAllContexts(FormKey formKey, Type type)
        {
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
        }

        /// <inheritdoc />
        public IEnumerable<IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter>> ResolveAllContexts(string editorId, Type type)
        {
            for (int i = _mutableMods.Count - 1; i >= 0; i--)
            {
                if (_mutableMods[i].TryResolveContext(editorId, type, out var majorRec))
                {
                    yield return majorRec;
                }
            }
            foreach (var rec in WrappedImmutableCache.ResolveAllContexts(editorId, type))
            {
                yield return rec;
            }
        }

        /// <inheritdoc />
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public IEnumerable<IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter>> ResolveAllContexts(FormKey formKey)
        {
            for (int i = _mutableMods.Count - 1; i >= 0; i--)
            {
                if (_mutableMods[i].TryResolveContext(formKey, out var majorRec))
                {
                    yield return majorRec;
                }
            }
            foreach (var rec in WrappedImmutableCache.ResolveAllContexts(formKey))
            {
                yield return rec;
            }
        }

        /// <inheritdoc />
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public IEnumerable<IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter>> ResolveAllContexts(string editorId)
        {
            for (int i = _mutableMods.Count - 1; i >= 0; i--)
            {
                if (_mutableMods[i].TryResolveContext(editorId, out var majorRec))
                {
                    yield return majorRec;
                }
            }
            foreach (var rec in WrappedImmutableCache.ResolveAllContexts(editorId))
            {
                yield return rec;
            }
        }

        /// <inheritdoc />
        public bool TryResolve(FormKey formKey, [MaybeNullWhen(false)] out IMajorRecordCommonGetter majorRec, params Type[] types)
        {
            return TryResolve(formKey, (IEnumerable<Type>)types, out majorRec);
        }

        /// <inheritdoc />
        public bool TryResolve(string editorId, [MaybeNullWhen(false)] out IMajorRecordCommonGetter majorRec, params Type[] types)
        {
            return TryResolve(editorId, (IEnumerable<Type>)types, out majorRec);
        }

        /// <inheritdoc />
        public bool TryResolve(FormKey formKey, IEnumerable<Type> types, [MaybeNullWhen(false)] out IMajorRecordCommonGetter majorRec)
        {
            foreach (var type in types)
            {
                if (TryResolve(formKey, type, out majorRec))
                {
                    return true;
                }
            }
            majorRec = default;
            return false;
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public IMajorRecordCommonGetter Resolve(FormKey formKey, params Type[] types)
        {
            return Resolve(formKey, (IEnumerable<Type>)types);
        }

        /// <inheritdoc />
        public IMajorRecordCommonGetter Resolve(string editorId, params Type[] types)
        {
            return Resolve(editorId, (IEnumerable<Type>)types);
        }

        /// <inheritdoc />
        public IMajorRecordCommonGetter Resolve(FormKey formKey, IEnumerable<Type> types)
        {
            if (TryResolve(formKey, types, out var commonRec)) return commonRec;
            throw new KeyNotFoundException($"FormKey {formKey} could not be found.");
        }

        /// <inheritdoc />
        public IMajorRecordCommonGetter Resolve(string editorId, IEnumerable<Type> types)
        {
            if (TryResolve(editorId, types, out var commonRec)) return commonRec;
            throw new KeyNotFoundException($"EditorID {editorId} could not be found.");
        }

        /// <inheritdoc />
        public bool TryResolveIdentifier(FormKey formKey, [MaybeNullWhen(false)] out string? editorId)
        {
            if (formKey.IsNull)
            {
                editorId = default;
                return false;
            }
            for (int i = _mutableMods.Count - 1; i >= 0; i--)
            {
                if (_mutableMods[i].TryResolveIdentifier(formKey, out editorId)) return true;
            }
            return WrappedImmutableCache.TryResolveIdentifier(formKey, out editorId);
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
        public bool TryResolveIdentifier(FormKey formKey, Type type, [MaybeNullWhen(false)] out string? editorId)
        {
            if (formKey.IsNull)
            {
                editorId = default;
                return false;
            }
            for (int i = _mutableMods.Count - 1; i >= 0; i--)
            {
                if (_mutableMods[i].TryResolveIdentifier(formKey, type, out editorId)) return true;
            }
            return WrappedImmutableCache.TryResolveIdentifier(formKey, type, out editorId);
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
        public bool TryResolveIdentifier<TMajor>(FormKey formKey, out string? editorId)
            where TMajor : class, IMajorRecordCommonGetter
        {
            return TryResolveIdentifier(formKey, typeof(TMajor), out editorId);
        }

        /// <inheritdoc />
        public bool TryResolveIdentifier<TMajor>(string editorId, out FormKey formKey)
            where TMajor : class, IMajorRecordCommonGetter
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
        public bool TryResolveIdentifier(FormKey formKey, IEnumerable<Type> types, [MaybeNullWhen(false)] out string? editorId)
        {
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
            where TMajor : class, IMajorRecordCommonGetter
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
    }
}
