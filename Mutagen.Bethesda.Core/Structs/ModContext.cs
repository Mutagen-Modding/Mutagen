using Mutagen.Bethesda.Binary;
using Mutagen.Bethesda.Internals;
using Noggog;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Mutagen.Bethesda
{
    public interface IModContext
    {
        ModKey ModKey { get; }
        IModContext? Parent { get; }
        object? Record { get; }
    }

    public interface IModContext<out T> : IModContext
    {
        new T Record { get; }
    }

    public interface IModContext<TMod, TModGetter, out TMajor, out TMajorGetter> : IModContext<TMajorGetter>
        where TModGetter : IModGetter
        where TMod : TModGetter, IMod
        where TMajor : class, IMajorRecordCommon, TMajorGetter
        where TMajorGetter : class, IMajorRecordCommonGetter
    {
        TMajor GetOrAddAsOverride(TMod mod);
        TMajor DuplicateIntoAsNewRecord(TMod mod, string? editorID = null);
        bool TryGetParentContext<TTargetMajorSetter, TTargetMajorGetter>([MaybeNullWhen(false)] out IModContext<TMod, TModGetter, TTargetMajorSetter, TTargetMajorGetter> parent)
            where TTargetMajorSetter : class, IMajorRecordCommon, TTargetMajorGetter
            where TTargetMajorGetter : class, IMajorRecordCommonGetter;
    }

    public class ModContext<T> : IModContext<T>
    {
        public ModKey ModKey { get; set; }

        public IModContext? Parent { get; set; }

        public T Record { get; set; }
        object? IModContext.Record => Record;

        public ModContext(ModKey modKey, IModContext? parent, T record)
        {
            ModKey = modKey;
            Parent = parent;
            Record = record;
        }
    }

    /// <summary>
    /// A pairing of a record as well as the logic and knowledge of where it came from in its parent mod.
    /// This allows a context to insert the record into a new mod, using the knowledge to properly insert and find the appropriate
    /// location within the new mod. <br />
    /// <br />
    /// This is typically only useful for deeper nested records such as Cell/PlacedObjects/Navmeshes/etc
    /// </summary>
    /// <typeparam name="TMod">The setter interface of the mod type to target</typeparam>
    /// <typeparam name="TModGetter">The getter interface of the mod type to target</typeparam>
    /// <typeparam name="TMajor">The setter interface of the contained record</typeparam>
    /// <typeparam name="TMajorGetter">The getter interface of the contained record</typeparam>
    public class ModContext<TMod, TModGetter, TMajor, TMajorGetter> : IModContext<TMod, TModGetter, TMajor, TMajorGetter>
        where TModGetter : IModGetter
        where TMod : TModGetter, IMod
        where TMajor : class, IMajorRecordCommon, TMajorGetter
        where TMajorGetter : class, IMajorRecordCommonGetter
    {
        private readonly Func<TMod, TMajorGetter, TMajor> _getOrAddAsOverride;
        private readonly Func<TMod, TMajorGetter, string?, TMajor> _duplicateInto;

        /// <summary>
        /// The contained record
        /// </summary>
        public TMajorGetter Record { get; }
        object IModContext.Record => Record;

        /// <summary>
        /// The source ModKey the record originated from
        /// </summary>
        public ModKey ModKey { get; }

        /// <summary>
        /// Parent context, if any
        /// </summary>
        public IModContext? Parent { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="modKey">ModKey the record is originating from</param>
        /// <param name="record">The record to wrap</param>
        /// <param name="getOrAddAsOverride">Logic for how to navigate a mod and insert a copy of the wrapped record</param>
        /// <param name="duplicateInto">Logic for how to navigate a mod and insert a duplicate of the wrapped record</param>
        /// <param name="parent">Optional parent context</param>
        public ModContext(
            ModKey modKey,
            TMajorGetter record,
            Func<TMod, TMajorGetter, TMajor> getOrAddAsOverride,
            Func<TMod, TMajorGetter, string?, TMajor> duplicateInto,
            IModContext? parent = null)
        {
            ModKey = modKey;
            Record = record;
            _getOrAddAsOverride = getOrAddAsOverride;
            _duplicateInto = duplicateInto;
            Parent = parent;
        }

        public static implicit operator TMajorGetter(ModContext<TMod, TModGetter, TMajor, TMajorGetter> context)
        {
            return context.Record;
        }

        /// <summary>
        /// Searches a mod for an existing override of the record wrapped by this context. <br/>
        /// If one is found, it is returned. <br/>
        /// Otherwise, this contexts knowledge is used to insert a copy into the target mod, which is then returned.
        /// </summary>
        /// <param name="mod">Mod to search/insert into</param>
        /// <returns>An override of the wrapped record, which is sourced from the target mod</returns>
        public TMajor GetOrAddAsOverride(TMod mod)
        {
            try
            {
                return _getOrAddAsOverride(mod, Record);
            }
            catch (Exception ex)
            {
                throw RecordException.Factory(ex, ModKey, Record);
            }
        }

        public bool TryGetParentContext<TTargetMajorSetter, TTargetMajorGetter>([MaybeNullWhen(false)] out IModContext<TMod, TModGetter, TTargetMajorSetter, TTargetMajorGetter> parent)
            where TTargetMajorSetter : class, IMajorRecordCommon, TTargetMajorGetter
            where TTargetMajorGetter : class, IMajorRecordCommonGetter
        {
            var targetContext = this.Parent;
            while (targetContext != null)
            {
                if (targetContext.Record is TTargetMajorGetter)
                {
                    parent = (ModContext<TMod, TModGetter, TTargetMajorSetter, TTargetMajorGetter>)targetContext;
                    return true;
                }
                targetContext = targetContext.Parent;
            }
            parent = default;
            return false;
        }

        public TMajor DuplicateIntoAsNewRecord(TMod mod, string? editorID = null)
        {
            return _duplicateInto(mod, this.Record, editorID);
        }
    }

    public static class ModContextExt
    {
        public static bool IsUnderneath<T>(this IModContext context)
        {
            return TryGetParent<T>(context, out _);
        }

        public static bool TryGetParent<T>(this IModContext context, [MaybeNullWhen(false)] out T item)
        {
            var targetContext = context.Parent;
            while (targetContext != null)
            {
                if (targetContext.Record is T t)
                {
                    item = t;
                    return true;
                }
                targetContext = targetContext.Parent;
            }
            item = default;
            return false;
        }

        public static IModContext<TMod, TModGetter, RMajorSetter, RMajorGetter> AsType<TMod, TModGetter, TMajor, TMajorGetter, RMajorSetter, RMajorGetter>(this IModContext<TMod, TModGetter, TMajor, TMajorGetter> context)
            where TModGetter : IModGetter
            where TMod : TModGetter, IMod
            where TMajor : class, IMajorRecordCommon, TMajorGetter
            where TMajorGetter : class, IMajorRecordCommonGetter
            where RMajorSetter : class, TMajor, RMajorGetter
            where RMajorGetter : class, TMajorGetter
        {
            return new ModContextCaster<TMod, TModGetter, TMajor, TMajorGetter, RMajorSetter, RMajorGetter>(context);
        }
    }
    
    namespace Internals
    {
        class ModContextCaster<TMod, TModGetter, TMajor, TMajorGetter, RMajorSetter, RMajorGetter> : IModContext<TMod, TModGetter, RMajorSetter, RMajorGetter>
            where TModGetter : IModGetter
            where TMod : TModGetter, IMod
            where TMajor : class, IMajorRecordCommon, TMajorGetter
            where TMajorGetter : class, IMajorRecordCommonGetter
            where RMajorSetter : class, TMajor, RMajorGetter
            where RMajorGetter : class, TMajorGetter
        {
            private readonly IModContext<TMod, TModGetter, TMajor, TMajorGetter> _context;

            public ModKey ModKey => _context.ModKey;

            public IModContext? Parent => _context.Parent;

            object? IModContext.Record => _context.Record;

            public RMajorGetter Record => (RMajorGetter)_context.Record;

            public ModContextCaster(IModContext<TMod, TModGetter, TMajor, TMajorGetter> source)
            {
                _context = source;
            }

            public RMajorSetter GetOrAddAsOverride(TMod mod)
            {
                return (RMajorSetter)_context.GetOrAddAsOverride(mod);
            }

            public RMajorSetter DuplicateIntoAsNewRecord(TMod mod, string? editorID = null)
            {
                return (RMajorSetter)_context.DuplicateIntoAsNewRecord(mod, editorID);
            }

            public bool TryGetParentContext<TTargetMajorSetter, TTargetMajorGetter>([MaybeNullWhen(false)] out IModContext<TMod, TModGetter, TTargetMajorSetter, TTargetMajorGetter> parent)
                where TTargetMajorSetter : class, IMajorRecordCommon, TTargetMajorGetter
                where TTargetMajorGetter : class, IMajorRecordCommonGetter
            {
                return _context.TryGetParentContext(out parent);
            }
        }
    }
}
