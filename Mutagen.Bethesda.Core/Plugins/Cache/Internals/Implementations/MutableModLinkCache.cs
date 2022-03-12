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
            this._sourceMod = sourceMod;
            this.ListedOrder = new List<IModGetter>()
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
            foreach (var item in this._sourceMod.EnumerateMajorRecords()
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
            foreach (var item in this._sourceMod.EnumerateMajorRecords()
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
            where TMajor : class, IMajorRecordGetter
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
            foreach (var major in this._sourceMod.EnumerateMajorRecords<TMajor>()
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
        public bool TryResolve<TMajor>(string editorId, [MaybeNullWhen(false)] out TMajor majorRec)
            where TMajor : class, IMajorRecordGetter
        {
            CheckDisposal();
            
            if (editorId.IsNullOrWhitespace())
            {
                majorRec = default;
                return false;
            }
            
            // ToDo
            // Upgrade to EnumerateGroups<TMajor>()
            foreach (var major in this._sourceMod.EnumerateMajorRecords<TMajor>()
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
            foreach (var major in this._sourceMod.EnumerateMajorRecords(type)
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
            foreach (var major in this._sourceMod.EnumerateMajorRecords(type)
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
            foreach (var item in this._sourceMod.EnumerateMajorRecordContexts<IMajorRecord, IMajorRecordGetter>(this)
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
            foreach (var item in this._sourceMod.EnumerateMajorRecordContexts<IMajorRecord, IMajorRecordGetter>(this)
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
            where TMajor : class, IMajorRecord, TMajorGetter
            where TMajorGetter : class, IMajorRecordGetter
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
            foreach (var major in this._sourceMod.EnumerateMajorRecordContexts<TMajor, TMajorGetter>(this)
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
        public bool TryResolveContext<TMajor, TMajorGetter>(string editorId, [MaybeNullWhen(false)] out IModContext<TMod, TModGetter, TMajor, TMajorGetter> majorRec)
            where TMajor : class, IMajorRecord, TMajorGetter
            where TMajorGetter : class, IMajorRecordGetter
        {
            CheckDisposal();
            
            if (editorId.IsNullOrWhitespace())
            {
                majorRec = default;
                return false;
            }
            
            // ToDo
            // Upgrade to EnumerateGroups<TMajor>()
            foreach (var major in this._sourceMod.EnumerateMajorRecordContexts<TMajor, TMajorGetter>(this)
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
            foreach (var major in this._sourceMod.EnumerateMajorRecordContexts(this, type)
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
            foreach (var major in this._sourceMod.EnumerateMajorRecordContexts(this, type)
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
            where TMajor : class, IMajorRecordGetter
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
            foreach (var major in _sourceMod.EnumerateMajorRecordSimpleContexts<TMajor>(this)
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
        public bool TryResolveSimpleContext<TMajor>(string editorId, [MaybeNullWhen(false)] out IModContext<TMajor> majorRec) where TMajor : class, IMajorRecordGetter
        {
            CheckDisposal();
            
            if (editorId.IsNullOrWhitespace())
            {
                majorRec = default;
                return false;
            }
            
            // ToDo
            // Upgrade to EnumerateGroups<TMajor>()
            foreach (var major in _sourceMod.EnumerateMajorRecordSimpleContexts<TMajor>(this)
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
        public IModContext<IMajorRecordGetter> ResolveSimpleContext(string editorId, Type type)
        {
            return ResolveContext(editorId, type);
        }

        /// <inheritdoc />
        public IModContext<TMajor> ResolveSimpleContext<TMajor>(FormKey formKey, ResolveTarget target = ResolveTarget.Winner) where TMajor : class, IMajorRecordGetter
        {
            if (TryResolveSimpleContext<TMajor>(formKey, out var commonRec, target)) return commonRec;
            throw new MissingRecordException(formKey, typeof(TMajor));
        }

        /// <inheritdoc />
        public IModContext<TMajor> ResolveSimpleContext<TMajor>(string editorId) where TMajor : class, IMajorRecordGetter
        {
            if (TryResolveSimpleContext<TMajor>(editorId, out var commonRec)) return commonRec;
            throw new MissingRecordException(editorId, typeof(TMajor));
        }

        /// <inheritdoc />
        public IEnumerable<IModContext<TMajor>> ResolveAllSimpleContexts<TMajor>(FormKey formKey, ResolveTarget target = ResolveTarget.Winner) where TMajor : class, IMajorRecordGetter
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
            where TMajor : class, IMajorRecord, TMajorGetter
            where TMajorGetter : class, IMajorRecordGetter
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
            where TMajor : class, IMajorRecordGetter
        {
            if (TryResolve<TMajor>(formKey, out var rec, target))
            {
                editorId = rec.EditorID;
                return true;
            }
            editorId = default;
            return false;
        }

        /// <inheritdoc />
        public bool TryResolveIdentifier<TMajor>(string editorId, out FormKey formKey)
            where TMajor : class, IMajorRecordGetter
        {
            if (TryResolve<TMajor>(editorId, out var rec))
            {
                formKey = rec.FormKey;
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
        public IEnumerable<IMajorRecordIdentifier> AllIdentifiers(Type type, CancellationToken? cancel = null)
        {
            return AllIdentifiersNoUniqueness(type, cancel)
                .Distinct(x => x.FormKey);
        }

        internal IEnumerable<IMajorRecordIdentifier> AllIdentifiersNoUniqueness(Type type, CancellationToken? cancel)
        {
            CheckDisposal();
            
            return _sourceMod.EnumerateMajorRecords(type);
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
}
