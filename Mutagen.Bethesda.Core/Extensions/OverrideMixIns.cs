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
                             RecordException.EnrichAndThrow(e, mod.ModKey);
                             throw e;
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
}