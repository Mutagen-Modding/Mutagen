using Mutagen.Bethesda.Binary;
using Noggog;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Mutagen.Bethesda
{
    public static class OverrideMixIns
    {
        /// <summary>
        /// Will find and return the most overridden version of each record in the load order of the given type 
        /// USAGE NOTE: <br/>
        /// Typically you should only supply the Getter interfaces for the type. <br/>
        /// A setter interface being given can result in no records being found, as most LoadOrders are readonly.
        /// </summary>
        /// <typeparam name="TMod">Mod type that the load order contains</typeparam>
        /// <typeparam name="TMajor">
        /// Type of record to search for and iterate. <br/>
        /// USAGE NOTE: <br/>
        /// Typically you should only supply the Getter interfaces for the type. <br/>
        /// A setter interface being given can result in no records being found, as most LoadOrders are readonly.
        /// </typeparam>
        /// <param name="loadOrder">LoadOrder to iterate over</param>
        /// <param name="includeDeletedRecords">Whether to include deleted records in the output</param>
        /// <returns>Enumerable of the most overridden version of each record of the given type, optionally including deleted ones</returns>
        public static IEnumerable<TMajor> WinningOverrides<TMod, TMajor>(
            this LoadOrder<TMod> loadOrder,
            bool includeDeletedRecords = false)
            where TMod : class, IModGetter
            where TMajor : class, IMajorRecordCommonGetter
        {
            return loadOrder.PriorityOrder.WinningOverrides<TMajor>(includeDeletedRecords: includeDeletedRecords);
        }

        /// <summary>
        /// Will find and return the most overridden version of each record in the load order of the given type 
        /// USAGE NOTE: <br/>
        /// Typically you should only supply the Getter interfaces for the type. <br/>
        /// A setter interface being given can result in no records being found, as most LoadOrders are readonly.
        /// </summary>
        /// <typeparam name="TMod">Mod type that the load order contains</typeparam>
        /// <param name="loadOrder">LoadOrder to iterate over</param>
        /// <param name="type">
        /// Type of record to search for and iterate. <br/>
        /// USAGE NOTE: <br/>
        /// Typically you should only supply the Getter interfaces for the type. <br/>
        /// A setter interface being given can result in no records being found, as most LoadOrders are readonly.
        /// </param>
        /// <param name="includeDeletedRecords">Whether to include deleted records in the output</param>
        /// <returns>Enumerable of the most overridden version of each record of the given type, optionally including deleted ones</returns>
        public static IEnumerable<IMajorRecordCommonGetter> WinningOverrides<TMod>(
            this LoadOrder<TMod> loadOrder, 
            Type type,
            bool includeDeletedRecords = false)
            where TMod : class, IModGetter
        {
            return loadOrder.PriorityOrder.WinningOverrides(type, includeDeletedRecords: includeDeletedRecords);
        }

        /// <summary>
        /// Will find and return the most overridden version of each record in the list of mods of the given type 
        /// USAGE NOTE: <br/>
        /// Typically you should only supply the Getter interfaces for the type. <br/>
        /// A setter interface being given can result in no records being found, as most LoadOrders are readonly.
        /// </summary>
        /// <typeparam name="TMajor">
        /// Type of record to search for and iterate. <br/>
        /// USAGE NOTE: <br/>
        /// Typically you should only supply the Getter interfaces for the type. <br/>
        /// A setter interface being given can result in no records being found, as most LoadOrders are readonly.
        /// </typeparam>
        /// <param name="modListings">Mod listings to source from, in priority order</param>
        /// <param name="includeDeletedRecords">Whether to include deleted records in the output</param>
        /// <returns>Enumerable of the most overridden version of each record of the given type, optionally including deleted ones</returns>
        public static IEnumerable<TMajor> WinningOverrides<TMajor>(
            this IEnumerable<IModListing<IModGetter>> modListings,
            bool includeDeletedRecords = false)
            where TMajor : class, IMajorRecordCommonGetter
        {
            return modListings
                .Select(l => l.Mod)
                .NotNull()
                .WinningOverrides<TMajor>(includeDeletedRecords: includeDeletedRecords);
        }

        /// <summary>
        /// Will find and return the most overridden version of each record in the list of mods of the given type 
        /// USAGE NOTE: <br/>
        /// Typically you should only supply the Getter interfaces for the type. <br/>
        /// A setter interface being given can result in no records being found, as most LoadOrders are readonly.
        /// </summary>
        /// <param name="modListings">Mod listings to source from, in priority order</param>
        /// <param name="type">
        /// Type of record to search for and iterate. <br/>
        /// USAGE NOTE: <br/>
        /// Typically you should only supply the Getter interfaces for the type. <br/>
        /// A setter interface being given can result in no records being found, as most LoadOrders are readonly.
        /// </param>
        /// <param name="includeDeletedRecords">Whether to include deleted records in the output</param>
        /// <returns>Enumerable of the most overridden version of each record of the given type, optionally including deleted ones</returns>
        public static IEnumerable<IMajorRecordCommonGetter> WinningOverrides(
            this IEnumerable<IModListing<IModGetter>> modListings, 
            Type type,
            bool includeDeletedRecords = false)
        {
            return modListings
                .Select(l => l.Mod)
                .NotNull()
                .WinningOverrides(type, includeDeletedRecords: includeDeletedRecords);
        }

        /// <summary>
        /// Will find and return the most overridden version of each record in the list of mods of the given type 
        /// USAGE NOTE: <br/>
        /// Typically you should only supply the Getter interfaces for the type. <br/>
        /// A setter interface being given can result in no records being found, as most LoadOrders are readonly.
        /// </summary>
        /// <typeparam name="TMajor">
        /// Type of record to search for and iterate. <br/>
        /// USAGE NOTE: <br/>
        /// Typically you should only supply the Getter interfaces for the type. <br/>
        /// A setter interface being given can result in no records being found, as most LoadOrders are readonly.
        /// </typeparam>
        /// <param name="mods">Mods to source from, in priority order</param>
        /// <param name="includeDeletedRecords">Whether to include deleted records in the output</param>
        /// <returns>Enumerable of the most overridden version of each record of the given type, optionally including deleted ones</returns>
        public static IEnumerable<TMajor> WinningOverrides<TMajor>(
            this IEnumerable<IModGetter> mods,
            bool includeDeletedRecords = false)
            where TMajor : class, IMajorRecordCommonGetter
        {
            var passedRecords = new HashSet<FormKey>();
            foreach (var mod in mods)
            {
                foreach (var record in mod.EnumerateMajorRecords<TMajor>())
                {
                    if (!passedRecords.Add(record.FormKey)) continue;
                    if (!includeDeletedRecords && record.IsDeleted) continue;
                    yield return record;
                }
            }
        }

        /// <summary>
        /// Will find and return the most overridden version of each record in the list of mods of the given type 
        /// USAGE NOTE: <br/>
        /// Typically you should only supply the Getter interfaces for the type. <br/>
        /// A setter interface being given can result in no records being found, as most LoadOrders are readonly.
        /// </summary>
        /// <param name="mods">Mods to source from, in priority order</param>
        /// <param name="type">
        /// Type of record to search for and iterate. <br/>
        /// USAGE NOTE: <br/>
        /// Typically you should only supply the Getter interfaces for the type. <br/>
        /// A setter interface being given can result in no records being found, as most LoadOrders are readonly.
        /// </param>
        /// <param name="includeDeletedRecords">Whether to include deleted records in the output</param>
        /// <returns>Enumerable of the most overridden version of each record of the given type, optionally including deleted ones</returns>
        public static IEnumerable<IMajorRecordCommonGetter> WinningOverrides(
            this IEnumerable<IModGetter> mods,
            Type type,
            bool includeDeletedRecords = false)
        {
            var passedRecords = new HashSet<FormKey>();
            foreach (var mod in mods)
            {
                foreach (var record in mod.EnumerateMajorRecords(type))
                {
                    if (!passedRecords.Add(record.FormKey)) continue;
                    if (!includeDeletedRecords && record.IsDeleted) continue;
                    yield return record;
                }
            }
        }

        /// <summary>
        /// Will find and return the most overridden version of each record in the list of mods of the given type. <br/>
        /// <br />
        /// Additionally, it will come wrapped in a context object that has knowledge of where each record came from. <br/>
        /// This context helps when trying to override deep records such as Cells/PlacedObjects/etc, as the context is able to navigate
        /// and insert the record into the proper location for you. <br />
        /// <br />
        /// This system is overkill for simpler top-level records.
        /// </summary>
        /// <typeparam name="TMod">Setter Mod type to target</typeparam>
        /// <typeparam name="TModGetter">Getter Mod type to target</typeparam>
        /// <typeparam name="TSetter">
        /// Setter interface type of record to search for and iterate.
        /// </typeparam>
        /// <typeparam name="TGetter">
        /// Getter interface type of record to search for and iterate.
        /// </typeparam>
        /// <param name="loadOrder">LoadOrder to iterate over</param>
        /// <param name="linkCache">LinkCache to use when creating parent objects</param> 
        /// <param name="includeDeletedRecords">Whether to include deleted records in the output</param>
        /// <returns>Enumerable of the most overridden version of each record of the given type, optionally including deleted ones</returns>
        public static IEnumerable<IModContext<TMod, TSetter, TGetter>> WinningOverrideContexts<TMod, TModGetter, TSetter, TGetter>(
            this LoadOrder<TModGetter> loadOrder,
            ILinkCache linkCache,
            bool includeDeletedRecords = false)
            where TMod : class, IMod, TModGetter
            where TModGetter : class, IModGetter, IMajorRecordContextEnumerable<TMod>
            where TSetter : class, IMajorRecordCommon, TGetter
            where TGetter : class, IMajorRecordCommonGetter
        {
            return loadOrder.PriorityOrder.WinningOverrideContexts<TMod, TModGetter, TSetter, TGetter>(linkCache, includeDeletedRecords: includeDeletedRecords);
        }

        /// <summary>
        /// Will find and return the most overridden version of each record in the list of mods of the given type. <br/>
        /// <br />
        /// Additionally, it will come wrapped in a context object that has knowledge of where each record came from. <br/>
        /// This context helps when trying to override deep records such as Cells/PlacedObjects/etc, as the context is able to navigate
        /// and insert the record into the proper location for you. <br />
        /// <br />
        /// This system is overkill for simpler top-level records.
        /// </summary>
        /// <typeparam name="TMod">Setter Mod type to target</typeparam>
        /// <typeparam name="TModGetter">Getter Mod type to target</typeparam>
        /// <param name="loadOrder">LoadOrder to iterate over</param>
        /// <param name="linkCache">LinkCache to use when creating parent objects</param> 
        /// <param name="type">
        /// Type of record to search for and iterate. <br/>
        /// USAGE NOTE: <br/>
        /// Typically you should only supply the Getter interfaces for the type. <br/>
        /// A setter interface being given can result in no records being found, as most LoadOrders are readonly.
        /// </param>
        /// <param name="includeDeletedRecords">Whether to include deleted records in the output</param>
        /// <returns>Enumerable of the most overridden version of each record of the given type, optionally including deleted ones</returns>
        public static IEnumerable<IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter>> WinningOverrideContexts<TMod, TModGetter>(
            this LoadOrder<TModGetter> loadOrder,
            ILinkCache linkCache,
            Type type,
            bool includeDeletedRecords = false)
            where TMod : class, IMod, TModGetter
            where TModGetter : class, IModGetter, IMajorRecordContextEnumerable<TMod>
        {
            return loadOrder.PriorityOrder.WinningOverrideContexts<TMod, TModGetter>(linkCache, type, includeDeletedRecords: includeDeletedRecords);
        }

        /// <summary>
        /// Will find and return the most overridden version of each record in the list of mods of the given type. <br/>
        /// <br />
        /// Additionally, it will come wrapped in a context object that has knowledge of where each record came from. <br/>
        /// This context helps when trying to override deep records such as Cells/PlacedObjects/etc, as the context is able to navigate
        /// and insert the record into the proper location for you. <br />
        /// <br />
        /// This system is overkill for simpler top-level records.
        /// </summary>
        /// <typeparam name="TMod">Setter Mod type to target</typeparam>
        /// <typeparam name="TModGetter">Getter Mod type to target</typeparam>
        /// <typeparam name="TSetter">
        /// Setter interface type of record to search for and iterate.
        /// </typeparam>
        /// <typeparam name="TGetter">
        /// Getter interface type of record to search for and iterate.
        /// </typeparam>
        /// <param name="modListings">Mod listings to source from, in priority order</param>
        /// <param name="linkCache">LinkCache to use when creating parent objects</param> 
        /// <param name="includeDeletedRecords">Whether to include deleted records in the output</param>
        /// <returns>Enumerable of the most overridden version of each record of the given type, optionally including deleted ones</returns>
        public static IEnumerable<IModContext<TMod, TSetter, TGetter>> WinningOverrideContexts<TMod, TModGetter, TSetter, TGetter>(
            this IEnumerable<IModListing<TModGetter>> modListings,
            ILinkCache linkCache,
            bool includeDeletedRecords = false)
            where TMod : class, IMod, TModGetter
            where TModGetter : class, IModGetter, IMajorRecordContextEnumerable<TMod>
            where TSetter : class, IMajorRecordCommon, TGetter
            where TGetter : class, IMajorRecordCommonGetter
        {
            return modListings
                .Select(l => l.Mod)
                .NotNull()
                .WinningOverrideContexts<TMod, TModGetter, TSetter, TGetter>(linkCache, includeDeletedRecords: includeDeletedRecords);
        }

        /// <summary>
        /// Will find and return the most overridden version of each record in the list of mods of the given type. <br/>
        /// <br />
        /// Additionally, it will come wrapped in a context object that has knowledge of where each record came from. <br/>
        /// This context helps when trying to override deep records such as Cells/PlacedObjects/etc, as the context is able to navigate
        /// and insert the record into the proper location for you. <br />
        /// <br />
        /// This system is overkill for simpler top-level records.
        /// </summary>
        /// <typeparam name="TMod">Setter Mod type to target</typeparam>
        /// <typeparam name="TModGetter">Getter Mod type to target</typeparam>
        /// <param name="modListings">Mod listings to source from, in priority order</param>
        /// <param name="linkCache">LinkCache to use when creating parent objects</param> 
        /// <param name="type">
        /// Type of record to search for and iterate. <br/>
        /// USAGE NOTE: <br/>
        /// Typically you should only supply the Getter interfaces for the type. <br/>
        /// A setter interface being given can result in no records being found, as most LoadOrders are readonly.
        /// </param>
        /// <param name="includeDeletedRecords">Whether to include deleted records in the output</param>
        /// <returns>Enumerable of the most overridden version of each record of the given type, optionally including deleted ones</returns>
        public static IEnumerable<IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter>> WinningOverrideContexts<TMod, TModGetter>(
            this IEnumerable<IModListing<TModGetter>> modListings,
            ILinkCache linkCache,
            Type type,
            bool includeDeletedRecords = false)
            where TMod : class, IMod, TModGetter
            where TModGetter : class, IModGetter, IMajorRecordContextEnumerable<TMod>
        {
            return modListings
                .Select(l => l.Mod)
                .NotNull()
                .WinningOverrideContexts<TMod, TModGetter>(linkCache, type, includeDeletedRecords: includeDeletedRecords);
        }

        /// <summary>
        /// Will find and return the most overridden version of each record in the list of mods of the given type. <br/>
        /// <br />
        /// Additionally, it will come wrapped in a context object that has knowledge of where each record came from. <br/>
        /// This context helps when trying to override deep records such as Cells/PlacedObjects/etc, as the context is able to navigate
        /// and insert the record into the proper location for you. <br />
        /// <br />
        /// This system is overkill for simpler top-level records.
        /// </summary>
        /// <typeparam name="TMod">Setter Mod type to target</typeparam>
        /// <typeparam name="TModGetter">Getter Mod type to target</typeparam>
        /// <typeparam name="TSetter">
        /// Setter interface type of record to search for and iterate.
        /// </typeparam>
        /// <typeparam name="TGetter">
        /// Getter interface type of record to search for and iterate.
        /// </typeparam>
        /// <param name="mods">Mods to source from, in priority order</param>
        /// <param name="linkCache">LinkCache to use when creating parent objects</param> 
        /// <param name="includeDeletedRecords">Whether to include deleted records in the output</param>
        /// <returns>Enumerable of the most overridden version of each record of the given type, optionally including deleted ones</returns>
        public static IEnumerable<IModContext<TMod, TSetter, TGetter>> WinningOverrideContexts<TMod, TModGetter, TSetter, TGetter>(
            this IEnumerable<TModGetter> mods,
            ILinkCache linkCache,
            bool includeDeletedRecords = false)
            where TMod : class, IMod, TModGetter
            where TModGetter : class, IModGetter, IMajorRecordContextEnumerable<TMod>
            where TSetter : class, IMajorRecordCommon, TGetter
            where TGetter : class, IMajorRecordCommonGetter
        {
            var passedRecords = new HashSet<FormKey>();
            foreach (var mod in mods)
            {
                foreach (var record in mod.EnumerateMajorRecordContexts<TSetter, TGetter>(linkCache))
                {
                    if (!passedRecords.Add(record.Record.FormKey)) continue;
                    if (!includeDeletedRecords && record.Record.IsDeleted) continue;
                    yield return record;
                }
            }
        }

        /// <summary>
        /// Will find and return the most overridden version of each record in the list of mods of the given type. <br/>
        /// <br />
        /// Additionally, it will come wrapped in a context object that has knowledge of where each record came from. <br/>
        /// This context helps when trying to override deep records such as Cells/PlacedObjects/etc, as the context is able to navigate
        /// and insert the record into the proper location for you. <br />
        /// <br />
        /// This system is overkill for simpler top-level records.
        /// </summary>
        /// <typeparam name="TMod">Setter Mod type to target</typeparam>
        /// <typeparam name="TModGetter">Getter Mod type to target</typeparam>
        /// <param name="mods">Mods to source from, in priority order</param>
        /// <param name="linkCache">LinkCache to use when creating parent objects</param> 
        /// <param name="type">
        /// Type of record to search for and iterate. <br/>
        /// USAGE NOTE: <br/>
        /// Typically you should only supply the Getter interfaces for the type. <br/>
        /// A setter interface being given can result in no records being found, as most LoadOrders are readonly.
        /// </param>
        /// <param name="includeDeletedRecords">Whether to include deleted records in the output</param>
        /// <returns>Enumerable of the most overridden version of each record of the given type, optionally including deleted ones</returns>
        public static IEnumerable<IModContext<TMod, IMajorRecordCommon, IMajorRecordCommonGetter>> WinningOverrideContexts<TMod, TModGetter>(
            this IEnumerable<TModGetter> mods,
            ILinkCache linkCache,
            Type type,
            bool includeDeletedRecords = false)
            where TMod : class, IMod, TModGetter
            where TModGetter : class, IModGetter, IMajorRecordContextEnumerable<TMod>
        {
            var passedRecords = new HashSet<FormKey>();
            foreach (var mod in mods)
            {
                foreach (var record in mod.EnumerateMajorRecordContexts(linkCache, type))
                {
                    if (!passedRecords.Add(record.Record.FormKey)) continue;
                    if (!includeDeletedRecords && record.Record.IsDeleted) continue;
                    yield return record;
                }
            }
        }

        public readonly static Dictionary<Type, object> AddAsOverrideMasks = new Dictionary<Type, object>();

        /// <summary>
        /// Takes in an existing record definition, and either returns the existing override definition
        /// from the Group, or copies the given record, inserts it, and then returns it as an override.
        /// </summary>
        /// <param name="group">Group to retrieve and/or insert from</param>
        /// <param name="major">Major record to query and potentially copy</param>
        /// <returns>Existing override record, or a copy of the given record that has already been inserted into the group</returns>
        public static TMajor GetOrAddAsOverride<TMajor, TMajorGetter>(this IGroupCommon<TMajor> group, TMajorGetter major)
            where TMajor : class, IMajorRecordInternal, TMajorGetter
            where TMajorGetter : class, IMajorRecordGetter, IBinaryItem
        {
            try
            {
                if (group.RecordCache.TryGetValue(major.FormKey, out var existingMajor))
                {
                    return existingMajor;
                }
                var mask = AddAsOverrideMasks.GetValueOrDefault(typeof(TMajor));
                existingMajor = (major.DeepCopy(mask as MajorRecord.TranslationMask) as TMajor)!;
                group.RecordCache.Set(existingMajor);
                return existingMajor;
            }
            catch (Exception ex)
            {
                throw RecordException.Factory(ex, major.FormKey, major.EditorID);
            }
        }

        /// <summary>
        /// Takes in a FormLink, and either returns the existing override definition
        /// from the Group, or attempts to link and copy the given record, inserting it, and then returning it as an override.
        /// </summary>
        /// <param name="group">Group to retrieve and/or insert from</param>
        /// <param name="link">Link to query and add</param>
        /// <param name="cache">Cache to query link against</param>
        /// <param name="rec">Retrieved record if successful</param>
        /// <returns>True if a record was retrieved</returns>
        public static bool TryGetOrAddAsOverride<TMajor, TMajorGetter>(this IGroupCommon<TMajor> group, IFormLink<TMajorGetter> link, ILinkCache cache, [MaybeNullWhen(false)] out TMajor rec)
            where TMajor : class, IMajorRecordInternal, TMajorGetter
            where TMajorGetter : class, IMajorRecordGetter, IBinaryItem
        {
            try
            {
                if (group.RecordCache.TryGetValue(link.FormKey, out rec))
                {
                    return true;
                }
                if (!link.TryResolve<TMajorGetter>(cache, out var getter))
                {
                    rec = default;
                    return false;
                }
                rec = GetOrAddAsOverride(group, getter);
                return true;
            }
            catch (Exception ex)
            {
                throw RecordException.Factory(ex, link.FormKey, edid: null);
            }
        }
    }
}
