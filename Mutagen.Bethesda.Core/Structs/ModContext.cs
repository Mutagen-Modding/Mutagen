using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda
{
    public interface IModContext
    {
        ModKey ModKey { get; }
        public IModContext? Parent { get; }
        public object Record { get; }
    }

    public interface IModContext<TModSetter, TMajorSetter, TMajorGetter> : IModContext
        where TModSetter : IModGetter
        where TMajorSetter : IMajorRecordCommon, TMajorGetter
        where TMajorGetter : IMajorRecordCommonGetter
    {
        new TMajorGetter Record { get; }
    }

    public class ModContext : IModContext
    {
        public ModKey ModKey { get; set; }

        public IModContext? Parent { get; set; }

        public object Record { get; set; }

        public ModContext(ModKey modKey, IModContext? parent, object record)
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
    /// <typeparam name="TModSetter">The setter interface of the mod type to target</typeparam>
    /// <typeparam name="TMajorSetter">The setter interface of the contained record</typeparam>
    /// <typeparam name="TMajorGetter">The getter interface of the contained record</typeparam>
    public class ModContext<TModSetter, TMajorSetter, TMajorGetter> : IModContext
        where TModSetter : IModGetter
        where TMajorSetter : IMajorRecordCommon, TMajorGetter
        where TMajorGetter : IMajorRecordCommonGetter
    {
        private readonly Func<TModSetter, TMajorGetter, TMajorSetter> _getOrAddAsOverride;

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
        /// <param name="getter">Logic for how to navigate a mod and insert a copy of the wrapped record</param>
        /// <param name="parent">Optional parent context</param>
        public ModContext(
            ModKey modKey, 
            TMajorGetter record, 
            Func<TModSetter, TMajorGetter, TMajorSetter> getter,
            IModContext? parent = null)
        {
            ModKey = modKey;
            Record = record;
            _getOrAddAsOverride = getter;
            Parent = parent;
        }

        public static implicit operator TMajorGetter(ModContext<TModSetter, TMajorSetter, TMajorGetter> context)
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
        public TMajorSetter GetOrAddAsOverride(TModSetter mod)
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

        public ModContext<TModSetter, RMajorSetter, RMajorGetter> AsType<RMajorSetter, RMajorGetter>()
            where RMajorSetter : TMajorSetter, RMajorGetter
            where RMajorGetter : TMajorGetter
        {
            return new ModContext<TModSetter, RMajorSetter, RMajorGetter>(
                ModKey,
                (RMajorGetter)Record,
                (mod, rec) => (RMajorSetter)this.GetOrAddAsOverride(mod),
                Parent);
        }
    }
}
