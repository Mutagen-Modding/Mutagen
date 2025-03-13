using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins;
using Noggog;
using System.Diagnostics.CodeAnalysis;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Cache.Internals;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Exceptions;

namespace Mutagen.Bethesda;

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
        this ILoadOrderGetter<TMod> loadOrder,
        bool includeDeletedRecords = false)
        where TMod : class, IModGetter
        where TMajor : class, IMajorRecordQueryableGetter
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
    public static IEnumerable<IMajorRecordGetter> WinningOverrides<TMod>(
        this ILoadOrderGetter<TMod> loadOrder, 
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
        this IEnumerable<IModListingGetter<IModGetter>> modListings,
        bool includeDeletedRecords = false)
        where TMajor : class, IMajorRecordQueryableGetter
    {
        return modListings
            .Select(l => l.Mod)
            .WhereNotNull()
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
    public static IEnumerable<IMajorRecordGetter> WinningOverrides(
        this IEnumerable<IModListingGetter<IModGetter>> modListings, 
        Type type,
        bool includeDeletedRecords = false)
    {
        return modListings
            .Select(l => l.Mod)
            .WhereNotNull()
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
        where TMajor : class, IMajorRecordQueryableGetter
    {
        var passedRecords = new HashSet<FormKey>();
        foreach (var mod in mods)
        {
            foreach (var record in mod.EnumerateMajorRecords<TMajor>())
            {
                if (record is not IMajorRecordGetter maj) continue;
                if (!passedRecords.Add(maj.FormKey)) continue;
                if (!includeDeletedRecords && maj.IsDeleted) continue;
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
    public static IEnumerable<IMajorRecordGetter> WinningOverrides(
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
    public static IEnumerable<IModContext<TMod, TModGetter, TSetter, TGetter>> WinningContextOverrides<TMod, TModGetter, TSetter, TGetter>(
        this ILoadOrderGetter<TModGetter> loadOrder,
        ILinkCache linkCache,
        bool includeDeletedRecords = false)
        where TMod : class, IMod, TModGetter
        where TModGetter : class, IModGetter, IMajorRecordContextEnumerable<TMod, TModGetter>
        where TSetter : class, IMajorRecordQueryable, TGetter
        where TGetter : class, IMajorRecordQueryableGetter
    {
        return loadOrder.PriorityOrder.WinningContextOverrides<TMod, TModGetter, TSetter, TGetter>(linkCache, includeDeletedRecords: includeDeletedRecords);
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
    public static IEnumerable<IModContext<TMod, TModGetter, IMajorRecord, IMajorRecordGetter>> WinningContextOverrides<TMod, TModGetter>(
        this ILoadOrderGetter<TModGetter> loadOrder,
        ILinkCache linkCache,
        Type type,
        bool includeDeletedRecords = false)
        where TMod : class, IMod, TModGetter
        where TModGetter : class, IModGetter, IMajorRecordContextEnumerable<TMod, TModGetter>
    {
        return loadOrder.PriorityOrder.WinningContextOverrides<TMod, TModGetter>(linkCache, type, includeDeletedRecords: includeDeletedRecords);
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
    public static IEnumerable<IModContext<TMod, TModGetter, TSetter, TGetter>> WinningContextOverrides<TMod, TModGetter, TSetter, TGetter>(
        this IEnumerable<IModListingGetter<TModGetter>> modListings,
        ILinkCache linkCache,
        bool includeDeletedRecords = false)
        where TMod : class, IMod, TModGetter
        where TModGetter : class, IModGetter, IMajorRecordContextEnumerable<TMod, TModGetter>
        where TSetter : class, IMajorRecordQueryable, TGetter
        where TGetter : class, IMajorRecordQueryableGetter
    {
        return modListings
            .Select(l => l.Mod)
            .WhereNotNull()
            .WinningContextOverrides<TMod, TModGetter, TSetter, TGetter>(linkCache, includeDeletedRecords: includeDeletedRecords);
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
    public static IEnumerable<IModContext<TMod, TModGetter, IMajorRecord, IMajorRecordGetter>> WinningContextOverrides<TMod, TModGetter>(
        this IEnumerable<IModListingGetter<TModGetter>> modListings,
        ILinkCache linkCache,
        Type type,
        bool includeDeletedRecords = false)
        where TMod : class, IMod, TModGetter
        where TModGetter : class, IModGetter, IMajorRecordContextEnumerable<TMod, TModGetter>
    {
        return modListings
            .Select(l => l.Mod)
            .WhereNotNull()
            .WinningContextOverrides<TMod, TModGetter>(linkCache, type, includeDeletedRecords: includeDeletedRecords);
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
    public static IEnumerable<IModContext<TMod, TModGetter, TSetter, TGetter>> WinningContextOverrides<TMod, TModGetter, TSetter, TGetter>(
        this IEnumerable<TModGetter> mods,
        ILinkCache linkCache,
        bool includeDeletedRecords = false)
        where TMod : class, IMod, TModGetter
        where TModGetter : class, IModGetter, IMajorRecordContextEnumerable<TMod, TModGetter>
        where TSetter : class, IMajorRecordQueryable, TGetter
        where TGetter : class, IMajorRecordQueryableGetter
    {
        var passedRecords = new HashSet<FormKey>();
        foreach (var mod in mods)
        {
            foreach (var record in mod.EnumerateMajorRecordContexts<TSetter, TGetter>(linkCache))
            {
                if (record.Record is not IMajorRecordGetter maj) continue;
                if (!passedRecords.Add(maj.FormKey)) continue;
                if (!includeDeletedRecords && maj.IsDeleted) continue;
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
    public static IEnumerable<IModContext<TMod, TModGetter, IMajorRecord, IMajorRecordGetter>> WinningContextOverrides<TMod, TModGetter>(
        this IEnumerable<TModGetter> mods,
        ILinkCache linkCache,
        Type type,
        bool includeDeletedRecords = false)
        where TMod : class, IMod, TModGetter
        where TModGetter : class, IModGetter, IMajorRecordContextEnumerable<TMod, TModGetter>
    {
        var passedRecords = new HashSet<FormKey>();
        foreach (var mod in mods)
        {
            foreach (var record in mod.EnumerateMajorRecordContexts(linkCache, type)
                         .Catch((e) =>
                         {
                             throw RecordException.Enrich(e, mod.ModKey);
                         }))
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
    [Obsolete("Use WinningContextOverrides instead")]
    public static IEnumerable<IModContext<TMod, TModGetter, TSetter, TGetter>> WinningOverrideContexts<TMod, TModGetter, TSetter, TGetter>(
        this ILoadOrderGetter<TModGetter> loadOrder,
        ILinkCache linkCache,
        bool includeDeletedRecords = false)
        where TMod : class, IMod, TModGetter
        where TModGetter : class, IModGetter, IMajorRecordContextEnumerable<TMod, TModGetter>
        where TSetter : class, IMajorRecordQueryable, TGetter
        where TGetter : class, IMajorRecordQueryableGetter
    {
        return WinningContextOverrides<TMod, TModGetter, TSetter, TGetter>(loadOrder, linkCache, includeDeletedRecords);
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
    [Obsolete("Use WinningContextOverrides instead")]
    public static IEnumerable<IModContext<TMod, TModGetter, IMajorRecord, IMajorRecordGetter>> WinningOverrideContexts<TMod, TModGetter>(
        this ILoadOrderGetter<TModGetter> loadOrder,
        ILinkCache linkCache,
        Type type,
        bool includeDeletedRecords = false)
        where TMod : class, IMod, TModGetter
        where TModGetter : class, IModGetter, IMajorRecordContextEnumerable<TMod, TModGetter>
    {
        return WinningContextOverrides<TMod, TModGetter>(loadOrder, linkCache, type, includeDeletedRecords);
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
    [Obsolete("Use WinningContextOverrides instead")]
    public static IEnumerable<IModContext<TMod, TModGetter, TSetter, TGetter>> WinningOverrideContexts<TMod, TModGetter, TSetter, TGetter>(
        this IEnumerable<IModListingGetter<TModGetter>> modListings,
        ILinkCache linkCache,
        bool includeDeletedRecords = false)
        where TMod : class, IMod, TModGetter
        where TModGetter : class, IModGetter, IMajorRecordContextEnumerable<TMod, TModGetter>
        where TSetter : class, IMajorRecordQueryable, TGetter
        where TGetter : class, IMajorRecordQueryableGetter
    {
        return WinningContextOverrides<TMod, TModGetter, TSetter, TGetter>(modListings, linkCache, includeDeletedRecords);
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
    [Obsolete("Use WinningContextOverrides instead")]
    public static IEnumerable<IModContext<TMod, TModGetter, IMajorRecord, IMajorRecordGetter>> WinningOverrideContexts<TMod, TModGetter>(
        this IEnumerable<IModListingGetter<TModGetter>> modListings,
        ILinkCache linkCache,
        Type type,
        bool includeDeletedRecords = false)
        where TMod : class, IMod, TModGetter
        where TModGetter : class, IModGetter, IMajorRecordContextEnumerable<TMod, TModGetter>
    {
        return WinningContextOverrides<TMod, TModGetter>(modListings, linkCache, type, includeDeletedRecords);
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
    [Obsolete("Use WinningContextOverrides instead")]
    public static IEnumerable<IModContext<TMod, TModGetter, TSetter, TGetter>> WinningOverrideContexts<TMod, TModGetter, TSetter, TGetter>(
        this IEnumerable<TModGetter> mods,
        ILinkCache linkCache,
        bool includeDeletedRecords = false)
        where TMod : class, IMod, TModGetter
        where TModGetter : class, IModGetter, IMajorRecordContextEnumerable<TMod, TModGetter>
        where TSetter : class, IMajorRecordQueryable, TGetter
        where TGetter : class, IMajorRecordQueryableGetter
    {
        return WinningContextOverrides<TMod, TModGetter, TSetter, TGetter>(mods, linkCache, includeDeletedRecords);
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
    [Obsolete("Use WinningContextOverrides instead")]
    public static IEnumerable<IModContext<TMod, TModGetter, IMajorRecord, IMajorRecordGetter>> WinningOverrideContexts<TMod, TModGetter>(
        this IEnumerable<TModGetter> mods,
        ILinkCache linkCache,
        Type type,
        bool includeDeletedRecords = false)
        where TMod : class, IMod, TModGetter
        where TModGetter : class, IModGetter, IMajorRecordContextEnumerable<TMod, TModGetter>
    {
        return WinningContextOverrides<TMod, TModGetter>(mods, linkCache, type, includeDeletedRecords);
    }

    /// <summary>
    /// Takes in an existing record definition, and either returns the existing override definition
    /// from the Group, or copies the given record, inserts it, and then returns it as an override.
    /// </summary>
    /// <param name="group">Group to retrieve and/or insert from</param>
    /// <param name="major">Major record to query and potentially copy</param>
    /// <returns>Existing override record, or a copy of the given record that has already been inserted into the group</returns>
    public static TMajor GetOrAddAsOverride<TMajor, TMajorGetter>(this IGroup<TMajor> group, TMajorGetter major)
        where TMajor : IMajorRecordInternal, TMajorGetter
        where TMajorGetter : IMajorRecordGetter
    {
        try
        {
            if (group.RecordCache.TryGetValue(major.FormKey, out var existingMajor))
            {
                return existingMajor;
            }
            var mask = OverrideMaskRegistrations.Get<TMajor>();
            var copy = major.DeepCopy(mask as MajorRecord.TranslationMask);
            if (copy is not TMajor rhs)
            {
                throw new InvalidOperationException($"DeepCopy did not return a record of the expected type {typeof(TMajor).Name}");
            }
            existingMajor = rhs;
            group.RecordCache.Set(existingMajor);
            return existingMajor;
        }
        catch (Exception ex)
        {
            throw RecordException.Enrich<TMajor>(ex, major.FormKey, major.EditorID);
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
    public static bool TryGetOrAddAsOverride<TMajor, TMajorGetter>(this IGroup<TMajor> group, IFormLinkGetter<TMajorGetter> link, ILinkCache cache, [MaybeNullWhen(false)] out TMajor rec)
        where TMajor : class, IMajorRecordInternal, TMajorGetter
        where TMajorGetter : class, IMajorRecordGetter
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
            throw RecordException.Enrich<TMajor>(ex, link.FormKey, edid: null);
        }
    }

    /// <summary>
    /// Takes in a FormLink, and either returns the existing override definition
    /// from the Group, or attempts to link and copy the given record, inserting it, and then returning it as an override.
    /// </summary>
    /// <param name="group">Group to retrieve and/or insert from</param>
    /// <param name="link">Link to query and add</param>
    /// <param name="cache">Cache to query link against</param>
    /// <returns>Retrieved record if successful</returns>
    public static TMajor GetOrAddAsOverride<TMajor, TMajorGetter>(this IGroup<TMajor> group, IFormLinkGetter<TMajorGetter> link, ILinkCache cache)
        where TMajor : class, IMajorRecordInternal, TMajorGetter
        where TMajorGetter : class, IMajorRecordGetter
    {
        if (TryGetOrAddAsOverride(group, link, cache, out var rec))
        {
            return rec;
        }
        throw new MissingRecordException(link.FormKey, link.Type);
    }
}