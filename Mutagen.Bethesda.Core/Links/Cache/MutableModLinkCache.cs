using Noggog;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Mutagen.Bethesda
{
    /// <summary>
    /// A Link Cache using a single mod as its link target.  Mod is allowed to be modified afterwards, but
    /// this comes at a performance cost of not allowing much caching to be done.  If the mod is not expected to
    /// be modified afterwards, use ImmutableModLinkCache instead.<br/>
    /// <br/>
    /// If being used in a multithreaded scenario,<br/>
    /// this cache must be locked alongside any mutations to the mod the cache wraps
    /// </summary>
    public class MutableModLinkCache<TMod, TModGetter> : ILinkCache<TMod, TModGetter>
        where TMod : class, IContextMod<TMod, TModGetter>, TModGetter
        where TModGetter : class, IContextGetterMod<TMod, TModGetter>
    {
        private readonly TModGetter _sourceMod;

        /// <inheritdoc />
        public IReadOnlyList<IModGetter> ListedOrder { get; }

        /// <inheritdoc />
        public IReadOnlyList<IModGetter> PriorityOrder => ListedOrder;

        /// <summary>
        /// Constructs a link cache around a target mod
        /// </summary>
        /// <param name="sourceMod">Mod to resolve against when linking</param>
        public MutableModLinkCache(TModGetter sourceMod)
        {
            this._sourceMod = sourceMod;
            this.ListedOrder = new List<IModGetter>()
            {
                sourceMod
            };
        }

        /// <inheritdoc />
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public bool TryResolve(FormKey formKey, [MaybeNullWhen(false)] out IMajorRecordCommonGetter majorRec)
        {
            if (formKey.IsNull)
            {
                majorRec = default;
                return false;
            }
            // ToDo
            // Upgrade to call EnumerateGroups(), which will perform much better
            foreach (var item in this._sourceMod.EnumerateMajorRecords())
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
        public bool TryResolve(string editorId, [MaybeNullWhen(false)] out IMajorRecordCommonGetter majorRec)
        {
            if (editorId.IsNullOrWhitespace())
            {
                majorRec = default;
                return false;
            }
            // ToDo
            // Upgrade to call EnumerateGroups(), which will perform much better
            foreach (var item in this._sourceMod.EnumerateMajorRecords())
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
        public bool TryResolve<TMajor>(FormKey formKey, [MaybeNullWhen(false)] out TMajor majorRec)
            where TMajor : class, IMajorRecordCommonGetter
        {
            if (formKey.IsNull)
            {
                majorRec = default;
                return false;
            }
            // ToDo
            // Upgrade to EnumerateGroups<TMajor>()
            foreach (var major in this._sourceMod.EnumerateMajorRecords<TMajor>())
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
        public bool TryResolve<TMajor>(string editorId, [MaybeNullWhen(false)] out TMajor majorRec)
            where TMajor : class, IMajorRecordCommonGetter
        {
            if (editorId.IsNullOrWhitespace())
            {
                majorRec = default;
                return false;
            }
            // ToDo
            // Upgrade to EnumerateGroups<TMajor>()
            foreach (var major in this._sourceMod.EnumerateMajorRecords<TMajor>())
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
        public bool TryResolve(FormKey formKey, Type type, [MaybeNullWhen(false)] out IMajorRecordCommonGetter majorRec)
        {
            if (formKey.IsNull)
            {
                majorRec = default;
                return false;
            }
            // ToDo
            // Upgrade to EnumerateGroups<TMajor>()
            foreach (var major in this._sourceMod.EnumerateMajorRecords(type))
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
        public bool TryResolve(string editorId, Type type, [MaybeNullWhen(false)] out IMajorRecordCommonGetter majorRec)
        {
            if (editorId.IsNullOrWhitespace())
            {
                majorRec = default;
                return false;
            }
            // ToDo
            // Upgrade to EnumerateGroups<TMajor>()
            foreach (var major in this._sourceMod.EnumerateMajorRecords(type))
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

        /// <inheritdoc />
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public bool TryResolveContext(FormKey formKey, [MaybeNullWhen(false)] out IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter> majorRec)
        {
            if (formKey.IsNull)
            {
                majorRec = default;
                return false;
            }
            // ToDo
            // Upgrade to call EnumerateGroups(), which will perform much better
            foreach (var item in this._sourceMod.EnumerateMajorRecordContexts<IMajorRecordCommon, IMajorRecordCommonGetter>(this))
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
        public bool TryResolveContext(string editorId, [MaybeNullWhen(false)] out IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter> majorRec)
        {
            if (editorId.IsNullOrWhitespace())
            {
                majorRec = default;
                return false;
            }
            // ToDo
            // Upgrade to call EnumerateGroups(), which will perform much better
            foreach (var item in this._sourceMod.EnumerateMajorRecordContexts<IMajorRecordCommon, IMajorRecordCommonGetter>(this))
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
        public bool TryResolveContext<TMajor, TMajorGetter>(FormKey formKey, [MaybeNullWhen(false)] out IModContext<TMod, TModGetter, TMajor, TMajorGetter> majorRec)
            where TMajor : class, IMajorRecordCommon, TMajorGetter
            where TMajorGetter : class, IMajorRecordCommonGetter
        {
            if (formKey.IsNull)
            {
                majorRec = default;
                return false;
            }
            // ToDo
            // Upgrade to EnumerateGroups<TMajor>()
            foreach (var major in this._sourceMod.EnumerateMajorRecordContexts<TMajor, TMajorGetter>(this))
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
        public bool TryResolveContext<TMajor, TMajorGetter>(string editorId, [MaybeNullWhen(false)] out IModContext<TMod, TModGetter, TMajor, TMajorGetter> majorRec)
            where TMajor : class, IMajorRecordCommon, TMajorGetter
            where TMajorGetter : class, IMajorRecordCommonGetter
        {
            if (editorId.IsNullOrWhitespace())
            {
                majorRec = default;
                return false;
            }
            // ToDo
            // Upgrade to EnumerateGroups<TMajor>()
            foreach (var major in this._sourceMod.EnumerateMajorRecordContexts<TMajor, TMajorGetter>(this))
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
        public bool TryResolveContext(FormKey formKey, Type type, [MaybeNullWhen(false)] out IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter> majorRec)
        {
            if (formKey.IsNull)
            {
                majorRec = default;
                return false;
            }
            // ToDo
            // Upgrade to EnumerateGroups<TMajor>()
            foreach (var major in this._sourceMod.EnumerateMajorRecordContexts(this, type))
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
        public bool TryResolveContext(string editorId, Type type, [MaybeNullWhen(false)] out IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter> majorRec)
        {
            if (editorId.IsNullOrWhitespace())
            {
                majorRec = default;
                return false;
            }
            // ToDo
            // Upgrade to EnumerateGroups<TMajor>()
            foreach (var major in this._sourceMod.EnumerateMajorRecordContexts(this, type))
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
        public IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter> ResolveContext(FormKey formKey)
        {
            if (TryResolveContext<IMajorRecordCommon, IMajorRecordCommonGetter>(formKey, out var majorRec)) return majorRec;
            throw new KeyNotFoundException($"FormKey {formKey} could not be found.");
        }

        /// <inheritdoc />
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter> ResolveContext(string editorId)
        {
            if (TryResolveContext<IMajorRecordCommon, IMajorRecordCommonGetter>(editorId, out var majorRec)) return majorRec;
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
            if (TryResolve<TMajor>(formKey, out var rec))
            {
                yield return rec;
            }
        }

        /// <inheritdoc />
        public IEnumerable<TMajor> ResolveAll<TMajor>(string editorId)
            where TMajor : class, IMajorRecordCommonGetter
        {
            if (TryResolve<TMajor>(editorId, out var rec))
            {
                yield return rec;
            }
        }

        /// <inheritdoc />
        public IEnumerable<IMajorRecordCommonGetter> ResolveAll(FormKey formKey, Type type)
        {
            if (TryResolve(formKey, type, out var rec))
            {
                yield return rec;
            }
        }

        /// <inheritdoc />
        public IEnumerable<IMajorRecordCommonGetter> ResolveAll(string editorId, Type type)
        {
            if (TryResolve(editorId, type, out var rec))
            {
                yield return rec;
            }
        }

        /// <inheritdoc />
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public IEnumerable<IMajorRecordCommonGetter> ResolveAll(FormKey formKey)
        {
            if (TryResolve(formKey, out var rec))
            {
                yield return rec;
            }
        }

        /// <inheritdoc />
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public IEnumerable<IMajorRecordCommonGetter> ResolveAll(string editorId)
        {
            if (TryResolve(editorId, out var rec))
            {
                yield return rec;
            }
        }

        /// <inheritdoc />
        public IEnumerable<IModContext<TMod, TModGetter, TMajor, TMajorGetter>> ResolveAllContexts<TMajor, TMajorGetter>(FormKey formKey)
            where TMajor : class, IMajorRecordCommon, TMajorGetter
            where TMajorGetter : class, IMajorRecordCommonGetter
        {
            if (TryResolveContext<TMajor, TMajorGetter>(formKey, out var rec))
            {
                yield return rec;
            }
        }

        /// <inheritdoc />
        public IEnumerable<IModContext<TMod, TModGetter, TMajor, TMajorGetter>> ResolveAllContexts<TMajor, TMajorGetter>(string editorId)
            where TMajor : class, IMajorRecordCommon, TMajorGetter
            where TMajorGetter : class, IMajorRecordCommonGetter
        {
            if (TryResolveContext<TMajor, TMajorGetter>(editorId, out var rec))
            {
                yield return rec;
            }
        }

        /// <inheritdoc />
        public IEnumerable<IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter>> ResolveAllContexts(FormKey formKey, Type type)
        {
            if (TryResolveContext(formKey, type, out var rec))
            {
                yield return rec;
            }
        }

        /// <inheritdoc />
        public IEnumerable<IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter>> ResolveAllContexts(string editorId, Type type)
        {
            if (TryResolveContext(editorId, type, out var rec))
            {
                yield return rec;
            }
        }

        /// <inheritdoc />
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public IEnumerable<IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter>> ResolveAllContexts(FormKey formKey)
        {
            if (TryResolveContext(formKey, out var rec))
            {
                yield return rec;
            }
        }

        /// <inheritdoc />
        [Obsolete("This call is not as optimized as its generic typed counterpart.  Use as a last resort.")]
        public IEnumerable<IModContext<TMod, TModGetter, IMajorRecordCommon, IMajorRecordCommonGetter>> ResolveAllContexts(string editorId)
        {
            if (TryResolveContext(editorId, out var rec))
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
        public bool TryResolveSimple(FormKey formKey, [MaybeNullWhen(false)] out string? editorId)
        {
            if (TryResolve(formKey, out var rec))
            {
                editorId = rec.EditorID;
                return false;
            }
            editorId = default;
            return false;
        }

        /// <inheritdoc />
        public bool TryResolveSimple(string editorId, [MaybeNullWhen(false)] out FormKey formKey)
        {
            if (TryResolve(editorId, out var rec))
            {
                formKey = rec.FormKey;
                return false;
            }
            formKey = default;
            return false;
        }

        /// <inheritdoc />
        public bool TryResolveSimple(FormKey formKey, Type type, [MaybeNullWhen(false)] out string? editorId)
        {
            if (TryResolve(formKey, type, out var rec))
            {
                editorId = rec.EditorID;
                return false;
            }
            editorId = default;
            return false;
        }

        /// <inheritdoc />
        public bool TryResolveSimple(string editorId, Type type, [MaybeNullWhen(false)] out FormKey formKey)
        {
            if (TryResolve(editorId, type, out var rec))
            {
                formKey = rec.FormKey;
                return false;
            }
            formKey = default;
            return false;
        }

        /// <inheritdoc />
        public bool TryResolveSimple<TMajor>(FormKey formKey, out string? editorId)
            where TMajor : class, IMajorRecordCommonGetter
        {
            if (TryResolve<TMajor>(formKey, out var rec))
            {
                editorId = rec.EditorID;
                return false;
            }
            editorId = default;
            return false;
        }

        /// <inheritdoc />
        public bool TryResolveSimple<TMajor>(string editorId, out FormKey formKey)
            where TMajor : class, IMajorRecordCommonGetter
        {
            if (TryResolve<TMajor>(editorId, out var rec))
            {
                formKey = rec.FormKey;
                return false;
            }
            formKey = default;
            return false;
        }

        /// <inheritdoc />
        public bool TryResolveSimple(FormKey formKey, [MaybeNullWhen(false)] out string? editorId, params Type[] types)
        {
            if (TryResolve(formKey, out var rec, types))
            {
                editorId = rec.EditorID;
                return false;
            }
            editorId = default;
            return false;
        }

        /// <inheritdoc />
        public bool TryResolveSimple(string editorId, [MaybeNullWhen(false)] out FormKey formKey, params Type[] types)
        {
            if (TryResolve(editorId, out var rec, types))
            {
                formKey = rec.FormKey;
                return false;
            }
            formKey = default;
            return false;
        }

        /// <inheritdoc />
        public bool TryResolveSimple(FormKey formKey, IEnumerable<Type> types, [MaybeNullWhen(false)] out string? editorId)
        {
            if (TryResolve(formKey, types, out var rec))
            {
                editorId = rec.EditorID;
                return false;
            }
            editorId = default;
            return false;
        }

        /// <inheritdoc />
        public bool TryResolveSimple(string editorId, IEnumerable<Type> types, [MaybeNullWhen(false)] out FormKey formKey)
        {
            if (TryResolve(editorId, types, out var rec))
            {
                formKey = rec.FormKey;
                return false;
            }
            formKey = default;
            return false;
        }

        public void Dispose()
        {
        }
    }
}
