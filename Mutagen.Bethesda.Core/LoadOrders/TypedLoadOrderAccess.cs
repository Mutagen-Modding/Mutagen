using System;
using System.Collections.Generic;
using Mutagen.Bethesda.Records;

namespace Mutagen.Bethesda.LoadOrders.Internals
{
    public struct TypedLoadOrderAccess<TMod, TModGetter, TMajor, TMajorGetter>
        where TModGetter : IModGetter
        where TMod : IMod, TModGetter
        where TMajor : class, IMajorRecordCommon, TMajorGetter
        where TMajorGetter : class, IMajorRecordCommonGetter
    {
        private Func<bool, IEnumerable<TMajorGetter>> _winningOverrides;
        private Func<ILinkCache, bool, IEnumerable<IModContext<TMod, TModGetter, TMajor, TMajorGetter>>> _winningContextOverrides;

        public TypedLoadOrderAccess(
            Func<bool, IEnumerable<TMajorGetter>> winningOverrides,
            Func<ILinkCache, bool, IEnumerable<IModContext<TMod, TModGetter, TMajor, TMajorGetter>>> winningContextOverrides)
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
        /// and insert the record into the proper location for you.
        /// </summary>
        /// <param name="linkCache">LinkCache to use when creating parent objects</param> 
        /// <param name="includeDeletedRecords">Whether to include deleted records in the output</param>
        /// <returns>Enumerable of the most overridden version of each record of the given type, optionally including deleted ones</returns>
        public IEnumerable<IModContext<TMod, TModGetter, TMajor, TMajorGetter>> WinningContextOverrides(ILinkCache linkCache, bool includeDeletedRecords = false) => _winningContextOverrides(linkCache, includeDeletedRecords);
    }

    public struct TopLevelTypedLoadOrderAccess<TMod, TModGetter, TMajor, TMajorGetter>
        where TModGetter : IModGetter
        where TMod : IMod, TModGetter
        where TMajor : class, IMajorRecordCommon, TMajorGetter
        where TMajorGetter : class, IMajorRecordCommonGetter
    {
        private Func<bool, IEnumerable<TMajorGetter>> _winningOverrides;
        private Func<ILinkCache, bool, IEnumerable<IModContext<TMod, TModGetter, TMajor, TMajorGetter>>> _winningContextOverrides;

        public TopLevelTypedLoadOrderAccess(
            Func<bool, IEnumerable<TMajorGetter>> winningOverrides,
            Func<ILinkCache, bool, IEnumerable<IModContext<TMod, TModGetter, TMajor, TMajorGetter>>> winningContextOverrides)
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
        /// and insert the record into the proper location for you.
        /// </summary>
        /// <param name="includeDeletedRecords">Whether to include deleted records in the output</param>
        /// <returns>Enumerable of the most overridden version of each record of the given type, optionally including deleted ones</returns>
        public IEnumerable<IModContext<TMod, TModGetter, TMajor, TMajorGetter>> WinningContextOverrides(bool includeDeletedRecords = false) => _winningContextOverrides(default(ILinkCache?)!, includeDeletedRecords);
    }
}
