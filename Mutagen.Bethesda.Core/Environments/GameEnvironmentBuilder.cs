using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Records;
using System.Collections.Immutable;
using Mutagen.Bethesda.Installs.DI;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Implicit.DI;
using Mutagen.Bethesda.Plugins.Order.DI;
using Mutagen.Bethesda.Plugins.Records.DI;
using Noggog;

namespace Mutagen.Bethesda.Environments;

public record GameEnvironmentBuilder<TMod, TModGetter>
    where TMod : class, IContextMod<TMod, TModGetter>, TModGetter
    where TModGetter : class, IContextGetterMod<TMod, TModGetter>
{
    private IGameReleaseContext Release { get; }

    internal IDataDirectoryProvider? DataDirectoryProvider { get; init; }
    internal ILoadOrderListingsProvider? ListingsProvider { get; init; }
    internal IPluginListingsPathProvider? PluginListingsPathProvider { get; init; }
    internal ICreationClubListingsPathProvider? CccListingsPathProvider { get; init; }

    private ImmutableList<Func<IEnumerable<ILoadOrderListingGetter>, IEnumerable<ILoadOrderListingGetter>>> LoadOrderListingProcessors { get; init; }

    private ImmutableList<Func<IEnumerable<IModListingGetter<TModGetter>>, IEnumerable<IModListingGetter<TModGetter>>>> ModListingProcessors { get; init; }

    private ImmutableList<TMod> MutableMods { get; init; }

    private GameEnvironmentBuilder(GameRelease release)
    {
        Release = new GameReleaseInjection(release);
        LoadOrderListingProcessors = ImmutableList<Func<IEnumerable<ILoadOrderListingGetter>, IEnumerable<ILoadOrderListingGetter>>>.Empty;
        ModListingProcessors = ImmutableList<Func<IEnumerable<IModListingGetter<TModGetter>>, IEnumerable<IModListingGetter<TModGetter>>>>.Empty;
        MutableMods = ImmutableList<TMod>.Empty;
    }

    internal GameEnvironmentBuilder(
        IGameReleaseContext releaseProvider,
        IDataDirectoryProvider dataDirectoryProvider,
        ILoadOrderListingsProvider listingsProvider,
        IPluginListingsPathProvider pluginListingsPathProvider,
        ICreationClubListingsPathProvider cccListingsPathProvider)
    {
        Release = releaseProvider;
        DataDirectoryProvider = dataDirectoryProvider;
        ListingsProvider = listingsProvider;
        PluginListingsPathProvider = pluginListingsPathProvider;
        CccListingsPathProvider = cccListingsPathProvider;
        LoadOrderListingProcessors = ImmutableList<Func<IEnumerable<ILoadOrderListingGetter>, IEnumerable<ILoadOrderListingGetter>>>.Empty;
        ModListingProcessors = ImmutableList<Func<IEnumerable<IModListingGetter<TModGetter>>, IEnumerable<IModListingGetter<TModGetter>>>>.Empty;
        MutableMods = ImmutableList<TMod>.Empty;
    }

    public static GameEnvironmentBuilder<TMod, TModGetter> Create(GameRelease release)
    {
        return new(release);
    }

    /// <summary>
    /// Exposes the load order for transformation by the user before any mods are read from disk
    /// </summary>
    /// <param name="transformer">Transformation lambda to process the incoming enumerable and return a new desired one</param>
    /// <returns>New builder with the new rules</returns>
    public GameEnvironmentBuilder<TMod, TModGetter> TransformLoadOrderListings(Func<IEnumerable<ILoadOrderListingGetter>, IEnumerable<ILoadOrderListingGetter>> transformer)
    {
        return this with { LoadOrderListingProcessors = LoadOrderListingProcessors.Add(transformer) };
    }

    /// <summary>
    /// Sets the load order to the given modkeys, with all of them enabled
    /// </summary>
    /// <param name="modKeys">ModKeys to set the load order to</param>
    /// <returns>New builder with the new rules</returns>
    public GameEnvironmentBuilder<TMod, TModGetter> WithLoadOrder(params ModKey[] modKeys)
    {
        return WithLoadOrder(modKeys.Select(x => (ILoadOrderListingGetter)new LoadOrderListing(x, enabled: true)).ToArray());
    }

    /// <summary>
    /// Sets the load order to the given listings
    /// </summary>
    /// <param name="listings">Listings to set the load order to</param>
    /// <returns>New builder with the new rules</returns>
    public GameEnvironmentBuilder<TMod, TModGetter> WithLoadOrder(params ILoadOrderListingGetter[] listings)
    {
        return TransformLoadOrderListings(_ => listings);
    }

    /// <summary>
    /// Exposes the load order for transformation by the user with mod objects loaded and accessible
    /// </summary>
    /// <param name="transformer">Transformation lambda to process the incoming enumerable and return a new desired one</param>
    /// <returns>New builder with the new rules</returns>
    public GameEnvironmentBuilder<TMod, TModGetter> TransformModListings(Func<IEnumerable<IModListingGetter<TModGetter>>, IEnumerable<IModListingGetter<TModGetter>>> transformer)
    {
        return this with { ModListingProcessors = ModListingProcessors.Add(transformer) };
    }

    /// <summary>
    /// Adds an output mod to the end of the load order as a mod that is safe to mutate.
    /// </summary>
    /// <param name="mod">Mutable safe mod to add to the end of the load order</param>
    /// <param name="trimming">What load order trimming rules to follow</param>
    /// <returns>New builder with the new rules</returns>
    public GameEnvironmentBuilder<TMod, TModGetter> WithOutputMod(TMod mod, OutputModTrimming trimming = OutputModTrimming.SelfAndPast)
    {
        var ret = this;
        switch (trimming)
        {
            case OutputModTrimming.NoTrimming:
                break;
            case OutputModTrimming.Self:
                ret = ret.TransformLoadOrderListings(x => x.Where(x => x.ModKey != mod.ModKey));
                break;
            case OutputModTrimming.SelfAndPast:
                ret = ret.TransformLoadOrderListings(x => x.TakeWhile(x => x.ModKey != mod.ModKey));
                break;
            default:
                throw new NotImplementedException();
        }
        return ret with { MutableMods = MutableMods.Add(mod) };
    }

    public GameEnvironmentBuilder<TMod, TModGetter> WithTargetDataFolder(DirectoryPath path)
    {
        return this with { DataDirectoryProvider = new DataDirectoryInjection(path) };
    }

    /// <summary>
    /// Creates an environment with all the given rules added to the builder
    /// </summary>
    /// <returns>GameEnvironment with the rules applied</returns>
    public IGameEnvironment<TMod, TModGetter> Build()
    {
        Warmup.Init();
        var category = new GameCategoryContext(Release);
        var gameLocator = new GameLocator();
        var dataDirectory = DataDirectoryProvider ?? new DataDirectoryProvider(Release, gameLocator);
        var pluginPathProvider = PluginListingsPathProvider ?? new PluginListingsPathProvider(Release);
        var cccPath = CccListingsPathProvider ?? new CreationClubListingsPathProvider(
            category,
            new CreationClubEnabledProvider(category),
            new GameDirectoryProvider(
                Release,
                gameLocator));
        var pluginRawListingsReader = new PluginRawListingsReader(
            IFileSystemExt.DefaultFilesystem,
            new PluginListingsParser(
                new LoadOrderListingParser(
                    new HasEnabledMarkersProvider(
                        Release))));

        var listingsProv = ListingsProvider ?? new LoadOrderListingsProvider(
            new OrderListings(),
            new ImplicitListingsProvider(
                IFileSystemExt.DefaultFilesystem,
                dataDirectory,
                new ImplicitListingModKeyProvider(
                    Release)),
            new PluginListingsProvider(
                Release,
                new TimestampedPluginListingsProvider(
                    new TimestampAligner(IFileSystemExt.DefaultFilesystem),
                    new TimestampedPluginListingsPreferences() { ThrowOnMissingMods = false },
                    pluginRawListingsReader,
                    dataDirectory,
                    pluginPathProvider),
                new EnabledPluginListingsProvider(
                    pluginRawListingsReader,
                    pluginPathProvider)),
            new CreationClubListingsProvider(
                IFileSystemExt.DefaultFilesystem,
                dataDirectory,
                cccPath,
                new CreationClubRawListingsReader()));

        var filteredListings = listingsProv.Get();
        foreach (var filter in LoadOrderListingProcessors)
        {
            filteredListings = filter(filteredListings);
        }

        var loGetter = new LoadOrderImporter<TModGetter>(
            IFileSystemExt.DefaultFilesystem,
            dataDirectory,
            new LoadOrderListingsInjection(filteredListings),
            new ModImporter<TModGetter>(
                IFileSystemExt.DefaultFilesystem,
                Release));

        ILoadOrderGetter<IModListingGetter<TModGetter>> lo = loGetter.Import();
        foreach (var filter in ModListingProcessors)
        {
            lo = filter(lo.ListedOrder).ToLoadOrder();
        }

        var linkCache = lo.ToMutableLinkCache(MutableMods.ToArray());

        return new GameEnvironmentState<TMod, TModGetter>(
            Release.Release,
            dataFolderPath: dataDirectory.Path,
            loadOrderFilePath: pluginPathProvider.Path,
            creationClubListingsFilePath: cccPath.Path,
            loadOrder: loGetter.Import(),
            linkCache: linkCache);
    }
}

public record GameEnvironmentBuilder
{
    private IGameReleaseContext Release { get; }

    internal IDataDirectoryProvider? DataDirectoryProvider { get; init; }
    internal ILoadOrderListingsProvider? ListingsProvider { get; init; }
    internal IPluginListingsPathProvider? PluginListingsPathProvider { get; init; }
    internal ICreationClubListingsPathProvider? CccListingsPathProvider { get; init; }

    private ImmutableList<Func<IEnumerable<ILoadOrderListingGetter>, IEnumerable<ILoadOrderListingGetter>>> LoadOrderListingProcessors { get; init; }

    private ImmutableList<Func<IEnumerable<IModListingGetter<IModGetter>>, IEnumerable<IModListingGetter<IModGetter>>>> ModListingProcessors { get; init; }

    private ImmutableList<IMod> MutableMods { get; init; }

    private GameEnvironmentBuilder(GameRelease release)
    {
        Release = new GameReleaseInjection(release);
        LoadOrderListingProcessors = ImmutableList<Func<IEnumerable<ILoadOrderListingGetter>, IEnumerable<ILoadOrderListingGetter>>>.Empty;
        ModListingProcessors = ImmutableList<Func<IEnumerable<IModListingGetter<IModGetter>>, IEnumerable<IModListingGetter<IModGetter>>>>.Empty;
        MutableMods = ImmutableList<IMod>.Empty;
    }

    internal GameEnvironmentBuilder(
        IGameReleaseContext releaseProvider,
        IDataDirectoryProvider dataDirectoryProvider,
        ILoadOrderListingsProvider listingsProvider,
        IPluginListingsPathProvider pluginListingsPathProvider,
        ICreationClubListingsPathProvider cccListingsPathProvider)
    {
        Release = releaseProvider;
        DataDirectoryProvider = dataDirectoryProvider;
        ListingsProvider = listingsProvider;
        PluginListingsPathProvider = pluginListingsPathProvider;
        CccListingsPathProvider = cccListingsPathProvider;
        LoadOrderListingProcessors = ImmutableList<Func<IEnumerable<ILoadOrderListingGetter>, IEnumerable<ILoadOrderListingGetter>>>.Empty;
        ModListingProcessors = ImmutableList<Func<IEnumerable<IModListingGetter<IModGetter>>, IEnumerable<IModListingGetter<IModGetter>>>>.Empty;
        MutableMods = ImmutableList<IMod>.Empty;
    }

    public static GameEnvironmentBuilder Create(GameRelease release)
    {
        return new(release);
    }

    /// <summary>
    /// Exposes the load order for transformation by the user before any mods are read from disk
    /// </summary>
    /// <param name="transformer">Transformation lambda to process the incoming enumerable and return a new desired one</param>
    /// <returns>New builder with the new rules</returns>
    public GameEnvironmentBuilder TransformLoadOrderListings(Func<IEnumerable<ILoadOrderListingGetter>, IEnumerable<ILoadOrderListingGetter>> transformer)
    {
        return this with { LoadOrderListingProcessors = LoadOrderListingProcessors.Add(transformer) };
    }

    /// <summary>
    /// Sets the load order to the given modkeys, with all of them enabled
    /// </summary>
    /// <param name="modKeys">ModKeys to set the load order to</param>
    /// <returns>New builder with the new rules</returns>
    public GameEnvironmentBuilder WithLoadOrder(params ModKey[] modKeys)
    {
        return WithLoadOrder(modKeys.Select(x => (ILoadOrderListingGetter)new LoadOrderListing(x, enabled: true)).ToArray());
    }

    /// <summary>
    /// Sets the load order to the given listings
    /// </summary>
    /// <param name="listings">Listings to set the load order to</param>
    /// <returns>New builder with the new rules</returns>
    public GameEnvironmentBuilder WithLoadOrder(params ILoadOrderListingGetter[] listings)
    {
        return TransformLoadOrderListings(_ => listings);
    }

    /// <summary>
    /// Exposes the load order for transformation by the user with mod objects loaded and accessible
    /// </summary>
    /// <param name="transformer">Transformation lambda to process the incoming enumerable and return a new desired one</param>
    /// <returns>New builder with the new rules</returns>
    public GameEnvironmentBuilder TransformModListings(Func<IEnumerable<IModListingGetter<IModGetter>>, IEnumerable<IModListingGetter<IModGetter>>> transformer)
    {
        return this with { ModListingProcessors = ModListingProcessors.Add(transformer) };
    }

    /// <summary>
    /// Adds an output mod to the end of the load order as a mod that is safe to mutate.
    /// </summary>
    /// <param name="mod">Mutable safe mod to add to the end of the load order</param>
    /// <param name="trimming">What load order trimming rules to follow</param>
    /// <returns>New builder with the new rules</returns>
    public GameEnvironmentBuilder WithOutputMod(IMod mod, OutputModTrimming trimming = OutputModTrimming.SelfAndPast)
    {
        var ret = this;
        switch (trimming)
        {
            case OutputModTrimming.NoTrimming:
                break;
            case OutputModTrimming.Self:
                ret = ret.TransformLoadOrderListings(x => x.Where(x => x.ModKey != mod.ModKey));
                break;
            case OutputModTrimming.SelfAndPast:
                ret = ret.TransformLoadOrderListings(x => x.TakeWhile(x => x.ModKey != mod.ModKey));
                break;
            default:
                throw new NotImplementedException();
        }
        return ret with { MutableMods = MutableMods.Add(mod) };
    }

    public GameEnvironmentBuilder WithTargetDataFolder(DirectoryPath path)
    {
        return this with { DataDirectoryProvider = new DataDirectoryInjection(path) };
    }

    /// <summary>
    /// Creates an environment with all the given rules added to the builder
    /// </summary>
    /// <returns>GameEnvironment with the rules applied</returns>
    public IGameEnvironment Build()
    {
        Warmup.Init();
        var category = new GameCategoryContext(Release);
        var gameLocator = new GameLocator();
        var dataDirectory = DataDirectoryProvider ?? new DataDirectoryProvider(Release, gameLocator);
        var pluginPathProvider = PluginListingsPathProvider ?? new PluginListingsPathProvider(Release);
        var cccPath = CccListingsPathProvider ?? new CreationClubListingsPathProvider(
            category,
            new CreationClubEnabledProvider(category),
            new GameDirectoryProvider(
                Release,
                gameLocator));
        var pluginRawListingsReader = new PluginRawListingsReader(
            IFileSystemExt.DefaultFilesystem,
            new PluginListingsParser(
                new LoadOrderListingParser(
                    new HasEnabledMarkersProvider(
                        Release))));

        var listingsProv = ListingsProvider ?? new LoadOrderListingsProvider(
            new OrderListings(),
            new ImplicitListingsProvider(
                IFileSystemExt.DefaultFilesystem,
                dataDirectory,
                new ImplicitListingModKeyProvider(
                    Release)),
            new PluginListingsProvider(
                Release,
                new TimestampedPluginListingsProvider(
                    new TimestampAligner(IFileSystemExt.DefaultFilesystem),
                    new TimestampedPluginListingsPreferences() { ThrowOnMissingMods = false },
                    pluginRawListingsReader,
                    dataDirectory,
                    pluginPathProvider),
                new EnabledPluginListingsProvider(
                    pluginRawListingsReader,
                    pluginPathProvider)),
            new CreationClubListingsProvider(
                IFileSystemExt.DefaultFilesystem,
                dataDirectory,
                cccPath,
                new CreationClubRawListingsReader()));

        var filteredListings = listingsProv.Get();
        foreach (var filter in LoadOrderListingProcessors)
        {
            filteredListings = filter(filteredListings);
        }

        var loGetter = new LoadOrderImporter(
            IFileSystemExt.DefaultFilesystem,
            dataDirectory,
            new LoadOrderListingsInjection(filteredListings),
            new ModImporter(
                IFileSystemExt.DefaultFilesystem,
                Release));

        ILoadOrderGetter<IModListingGetter<IModGetter>> lo = loGetter.Import();
        foreach (var filter in ModListingProcessors)
        {
            lo = filter(lo.ListedOrder).ToLoadOrder();
        }

        var linkCache = lo.ToUntypedMutableLinkCache(Release.Release.ToCategory(), MutableMods.ToArray());

        return new GameEnvironmentState(
            Release.Release,
            dataFolderPath: dataDirectory.Path,
            loadOrderFilePath: pluginPathProvider.Path,
            creationClubListingsFilePath: cccPath.Path,
            loadOrder: loGetter.Import(),
            linkCache: linkCache);
    }
}

public static class GameEnvironmentBuilderMixIns
{
    public static GameEnvironmentBuilder Builder(this GameEnvironment env, GameRelease release)
    {
        return GameEnvironmentBuilder.Create(release);
    }
    
    public static GameEnvironmentBuilder<TMod, TModGetter> Builder<TMod, TModGetter>(this GameEnvironment env, GameRelease release)
        where TMod : class, IContextMod<TMod, TModGetter>, TModGetter
        where TModGetter : class, IContextGetterMod<TMod, TModGetter>
    {
        return GameEnvironmentBuilder<TMod, TModGetter>.Create(release);
    }
}
