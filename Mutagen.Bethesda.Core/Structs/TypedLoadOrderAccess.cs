using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Internals
{
    public struct TypedLoadOrderAccess<TModSetter, TMajorSetter, TMajorGetter>
        where TModSetter : IModGetter
        where TMajorSetter : IMajorRecordCommon, TMajorGetter
        where TMajorGetter : IMajorRecordCommonGetter
    {
        private Func<bool, IEnumerable<TMajorGetter>> _winningOverrides;
        private Func<ILinkCache, bool, IEnumerable<ModContext<TModSetter, TMajorSetter, TMajorGetter>>> _winningContextOverrides;

        public TypedLoadOrderAccess(
            Func<bool, IEnumerable<TMajorGetter>> winningOverrides,
            Func<ILinkCache, bool, IEnumerable<ModContext<TModSetter, TMajorSetter, TMajorGetter>>> winningContextOverrides)
        {
            _winningOverrides = winningOverrides;
            _winningContextOverrides = winningContextOverrides;
        }

        /// <summary>
        /// Will find and return the most overridden version of each record in the list of mods of the given type 
        /// </summary>
        /// <param name="includeDeletedRecords">Whether to include deleted records in the output</param>
        /// <returns>Enumerable of the most overridden version of each record of the given type, optionally including deleted ones</returns>
        public IEnumerable<TMajorGetter> WinningOverrides(bool includeDeletedRecords = false) => _winningOverrides(includeDeletedRecords);

        /// <summary>
        /// Will find and return the most overridden version of each record in the list of mods of the given type. <br/>
        /// <br />
        /// Additionally, it will come wrapped in a context object that has knowledge of where each record came from. <br/>
        /// This context helps when trying to override deep records such as Cells/PlacedObjects/etc, as the context is able to navigate
        /// and insert the record into the proper location for you. <br />
        /// <br />
        /// This system is overkill for simpler top-level records.
        /// </summary>
        /// <param name="linkCache">LinkCache to use when creating parent objects</param> 
        /// <param name="includeDeletedRecords">Whether to include deleted records in the output</param>
        /// <returns>Enumerable of the most overridden version of each record of the given type, optionally including deleted ones</returns>
        public IEnumerable<ModContext<TModSetter, TMajorSetter, TMajorGetter>> WinningContextOverrides(ILinkCache linkCache, bool includeDeletedRecords = false) => _winningContextOverrides(linkCache, includeDeletedRecords);
    }
}
