using Noggog;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Mutagen.Bethesda
{
    /// <summary>
    /// A link cache that allows a top set of mods on the load order to be modified without
    /// invalidating the cache.  This comes at a performance cost of needing to query those mods
    /// for every request.
    /// </summary>
    public class MutableLoadOrderLinkCache<TMod, TModGetter> : ILinkCache<TMod, TModGetter>
        where TMod : class, IContextMod<TMod>, TModGetter
        where TModGetter : class, IContextGetterMod<TMod>
    {
        public ImmutableLoadOrderLinkCache<TMod, TModGetter> WrappedImmutableCache { get; }
        private readonly List<MutableModLinkCache<TMod, TModGetter>> _mutableMods;

        /// <summary>
        /// Constructs a mutable load order link cache by combining an existing immutable load order cache,
        /// plus a set of mods to be put at the end of the load order and allow to be mutable.
        /// </summary>
        /// <param name="immutableBaseCache">LoadOrderCache to use as the immutable base</param>
        /// <param name="mutableMods">Set of mods to place at the end of the load order, which are allowed to be modified afterwards</param>
        public MutableLoadOrderLinkCache(ImmutableLoadOrderLinkCache<TMod, TModGetter> immutableBaseCache, LinkCachePreferences prefs, params TMod[] mutableMods)
        {
            WrappedImmutableCache = immutableBaseCache;
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
        public bool TryResolveContext(FormKey formKey, [MaybeNullWhen(false)] out IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter> majorRec)
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
        public bool TryResolveContext(string editorId, [MaybeNullWhen(false)] out IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter> majorRec)
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
        public bool TryResolveContext<TMajorSetter, TMajorGetter>(FormKey formKey, [MaybeNullWhen(false)] out IModContext<TMod, TMajorSetter, TMajorGetter> majorRec)
            where TMajorSetter : class, IMajorRecordCommon, TMajorGetter
            where TMajorGetter : class, IMajorRecordCommonGetter
        {
            if (formKey.IsNull)
            {
                majorRec = default;
                return false;
            }
            for (int i = _mutableMods.Count - 1; i >= 0; i--)
            {
                if (_mutableMods[i].TryResolveContext<TMajorSetter, TMajorGetter>(formKey, out majorRec)) return true;
            }
            return WrappedImmutableCache.TryResolveContext<TMajorSetter, TMajorGetter>(formKey, out majorRec);
        }

        /// <inheritdoc />
        public bool TryResolveContext<TMajorSetter, TMajorGetter>(string editorId, [MaybeNullWhen(false)] out IModContext<TMod, TMajorSetter, TMajorGetter> majorRec)
            where TMajorSetter : class, IMajorRecordCommon, TMajorGetter
            where TMajorGetter : class, IMajorRecordCommonGetter
        {
            if (editorId.IsNullOrWhitespace())
            {
                majorRec = default;
                return false;
            }
            for (int i = _mutableMods.Count - 1; i >= 0; i--)
            {
                if (_mutableMods[i].TryResolveContext<TMajorSetter, TMajorGetter>(editorId, out majorRec)) return true;
            }
            return WrappedImmutableCache.TryResolveContext<TMajorSetter, TMajorGetter>(editorId, out majorRec);
        }

        /// <inheritdoc />
        public bool TryResolveContext(FormKey formKey, Type type, [MaybeNullWhen(false)] out IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter> majorRec)
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
        public bool TryResolveContext(string editorId, Type type, [MaybeNullWhen(false)] out IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter> majorRec)
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
        public IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter> ResolveContext(FormKey formKey)
        {
            if (TryResolveContext(formKey, out var majorRec)) return majorRec;
            throw new KeyNotFoundException($"FormKey {formKey} could not be found.");
        }

        /// <inheritdoc />
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter> ResolveContext(string editorId)
        {
            if (TryResolveContext(editorId, out var majorRec)) return majorRec;
            throw new KeyNotFoundException($"EditorID {editorId} could not be found.");
        }

        /// <inheritdoc />
        public IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter> ResolveContext(FormKey formKey, Type type)
        {
            if (TryResolveContext(formKey, type, out var commonRec)) return commonRec;
            throw new KeyNotFoundException($"FormKey {formKey} could not be found.");
        }

        /// <inheritdoc />
        public IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter> ResolveContext(string editorId, Type type)
        {
            if (TryResolveContext(editorId, type, out var commonRec)) return commonRec;
            throw new KeyNotFoundException($"EditorID {editorId} could not be found.");
        }

        /// <inheritdoc />
        public IModContext<TMod, TMajorSetter, TMajorGetter> ResolveContext<TMajorSetter, TMajorGetter>(FormKey formKey)
            where TMajorSetter : class, IMajorRecordCommon, TMajorGetter
            where TMajorGetter : class, IMajorRecordCommonGetter
        {
            if (TryResolveContext<TMajorSetter, TMajorGetter>(formKey, out var commonRec)) return commonRec;
            throw new KeyNotFoundException($"FormKey {formKey} could not be found.");
        }

        /// <inheritdoc />
        public IModContext<TMod, TMajorSetter, TMajorGetter> ResolveContext<TMajorSetter, TMajorGetter>(string editorId)
            where TMajorSetter : class, IMajorRecordCommon, TMajorGetter
            where TMajorGetter : class, IMajorRecordCommonGetter
        {
            if (TryResolveContext<TMajorSetter, TMajorGetter>(editorId, out var commonRec)) return commonRec;
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
        public IEnumerable<IModContext<TMod, TMajorSetter, TMajorGetter>> ResolveAllContexts<TMajorSetter, TMajorGetter>(FormKey formKey)
            where TMajorSetter : class, IMajorRecordCommon, TMajorGetter
            where TMajorGetter : class, IMajorRecordCommonGetter
        {
            for (int i = _mutableMods.Count - 1; i >= 0; i--)
            {
                if (_mutableMods[i].TryResolveContext<TMajorSetter, TMajorGetter>(formKey, out var majorRec))
                {
                    yield return majorRec;
                }
            }
            foreach (var rec in WrappedImmutableCache.ResolveAllContexts<TMajorSetter, TMajorGetter>(formKey))
            {
                yield return rec;
            }
        }

        /// <inheritdoc />
        public IEnumerable<IModContext<TMod, TMajorSetter, TMajorGetter>> ResolveAllContexts<TMajorSetter, TMajorGetter>(string editorId)
            where TMajorSetter : class, IMajorRecordCommon, TMajorGetter
            where TMajorGetter : class, IMajorRecordCommonGetter
        {
            for (int i = _mutableMods.Count - 1; i >= 0; i--)
            {
                if (_mutableMods[i].TryResolveContext<TMajorSetter, TMajorGetter>(editorId, out var majorRec))
                {
                    yield return majorRec;
                }
            }
            foreach (var rec in WrappedImmutableCache.ResolveAllContexts<TMajorSetter, TMajorGetter>(editorId))
            {
                yield return rec;
            }
        }

        /// <inheritdoc />
        public IEnumerable<IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter>> ResolveAllContexts(FormKey formKey, Type type)
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
        public IEnumerable<IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter>> ResolveAllContexts(string editorId, Type type)
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
        public IEnumerable<IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter>> ResolveAllContexts(FormKey formKey)
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
        public IEnumerable<IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter>> ResolveAllContexts(string editorId)
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

        public void Dispose()
        {
        }
    }
}
