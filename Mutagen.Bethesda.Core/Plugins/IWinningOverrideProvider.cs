using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Plugins;

public interface IWinningOverrideProvider
{
    /// <summary>
    /// Will find and return the most overridden version of each record in the load order of the given type
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
    /// <param name="includeDeletedRecords">Whether to include deleted records in the output</param>
    /// <returns>Enumerable of the most overridden version of each record of the given type, optionally including deleted ones</returns>
    IEnumerable<TMajor> WinningOverrides<TMajor>(bool includeDeletedRecords = false)
        where TMajor : class, IMajorRecordQueryableGetter;

    /// <summary>
    /// Will find and return the most overridden version of each record in the load order of the given type
    /// USAGE NOTE: <br/>
    /// Typically you should only supply the Getter interfaces for the type. <br/>
    /// A setter interface being given can result in no records being found, as most LoadOrders are readonly.
    /// </summary>
    /// <param name="type">
    /// Type of record to search for and iterate. <br/>
    /// USAGE NOTE: <br/>
    /// Typically you should only supply the Getter interfaces for the type. <br/>
    /// A setter interface being given can result in no records being found, as most LoadOrders are readonly.
    /// </param>
    /// <param name="includeDeletedRecords">Whether to include deleted records in the output</param>
    /// <returns>Enumerable of the most overridden version of each record of the given type, optionally including deleted ones</returns>
    IEnumerable<IMajorRecordGetter> WinningOverrides(Type type, bool includeDeletedRecords = false);
}

public interface IWinningOverrideProvider<TMod, TModGetter> : IWinningOverrideProvider
    where TModGetter : class, IModGetter
    where TMod : class, TModGetter, IMod
{
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
    /// <param name="linkCache">LinkCache to use when creating parent objects</param>
    /// <param name="includeDeletedRecords">Whether to include deleted records in the output</param>
    /// <returns>Enumerable of the most overridden version of each record of the given type, optionally including deleted ones</returns>
    IEnumerable<IModContext<TMod, TModGetter, TSetter, TGetter>> WinningContextOverrides<TSetter, TGetter>(
        ILinkCache linkCache,
        bool includeDeletedRecords = false)
        where TSetter : class, IMajorRecordQueryable, TGetter
        where TGetter : class, IMajorRecordQueryableGetter;

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
    /// <param name="linkCache">LinkCache to use when creating parent objects</param>
    /// <param name="type">
    /// Type of record to search for and iterate. <br/>
    /// USAGE NOTE: <br/>
    /// Typically you should only supply the Getter interfaces for the type. <br/>
    /// A setter interface being given can result in no records being found, as most LoadOrders are readonly.
    /// </param>
    /// <param name="includeDeletedRecords">Whether to include deleted records in the output</param>
    /// <returns>Enumerable of the most overridden version of each record of the given type, optionally including deleted ones</returns>
    IEnumerable<IModContext<TMod, TModGetter, IMajorRecord, IMajorRecordGetter>> WinningContextOverrides(
        ILinkCache linkCache,
        Type type,
        bool includeDeletedRecords = false);
}