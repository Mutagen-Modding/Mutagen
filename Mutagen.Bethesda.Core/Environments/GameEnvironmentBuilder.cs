using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Records;
using System.Collections.Immutable;
using System.IO.Abstractions;
using Mutagen.Bethesda.Installs.DI;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Implicit.DI;
using Mutagen.Bethesda.Plugins.Order.DI;
using Mutagen.Bethesda.Plugins.Records.DI;
using Noggog;

namespace Mutagen.Bethesda.Environments;

public sealed record GameEnvironmentBuilder<TMod, TModGetter>
    where TMod : class, IContextMod<TMod, TModGetter>, TModGetter
    where TModGetter : class, IContextGetterMod<TMod, TModGetter>
{
    private IGameReleaseContext Release { get; }
    internal IDataDirectoryProvider? DataDirectoryProvider { get; init; }
    internal ILoadOrderListingsProvider? ListingsProvider { get; init; }
    internal IPluginListingsPathContext? PluginListingsPathContext { get; init; }
    internal ICreationClubListingsPathProvider? CccListingsPathProvider { get; init; }
    internal IFileSystem? FileSystem { get; init; }
    internal Func<Type, object?>? Resolver { get; init; }

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
        IPluginListingsPathContext pluginListingsPathContext,
        ICreationClubListingsPathProvider cccListingsPathProvider)
    {
        Release = releaseProvider;
        DataDirectoryProvider = dataDirectoryProvider;
        ListingsProvider = listingsProvider;
        PluginListingsPathContext = pluginListingsPathContext;
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

    public GameEnvironmentBuilder<TMod, TModGetter> WithFileSystem(IFileSystem fileSystem)
    {
        return this with { FileSystem = fileSystem };
    }

    public GameEnvironmentBuilder<TMod, TModGetter> WithResolver(Func<Type, object?> resolver)
    {
        return this with { Resolver = resolver };
    }

    private TObject Resolve<TObject>(Func<TObject> fallback, TObject? value = default)
    {
        if (value != null)
        {
            return value;
        }
        
        if (Resolver != null)
        {
            var ret = Resolver(typeof(TObject));
            if (ret != null)
            {
                return (TObject)ret;
            }
        }

        return fallback();
    }

    /// <summary>
    /// Creates an environment with all the given rules added to the builder
    /// </summary>
    /// <returns>GameEnvironment with the rules applied</returns>
    public IGameEnvironment<TMod, TModGetter> Build()
    {
        Warmup.Init();
        var fs = Resolve<IFileSystem>(() => IFileSystemExt.DefaultFilesystem, FileSystem);

        var gameLocator = new Lazy<GameLocator>(() => GameLocator.Instance);
        var dataDirectory = Resolve<IDataDirectoryProvider>(
            () => new DataDirectoryProvider(
                Release,
                gameLocator.Value), 
            DataDirectoryProvider);

        var pluginPathProvider = Resolve<IPluginListingsPathContext>(
            () => new PluginListingsPathContext(
                new PluginListingsPathProvider(),
                Release),
            PluginListingsPathContext);

        var cccPath = Resolve<ICreationClubListingsPathProvider>(
            () =>
            {
                var category = new GameCategoryContext(Release);
                return new CreationClubListingsPathProvider(
                    category,
                    new CreationClubEnabledProvider(category),
                    new GameDirectoryProvider(
                        Release,
                        gameLocator.Value));
            },
            CccListingsPathProvider);

        var listingsProv = Resolve<ILoadOrderListingsProvider>(
            () =>
            {
                var pluginRawListingsReader = new PluginRawListingsReader(
                    fs,
                    new PluginListingsParser(
                        new PluginListingCommentTrimmer(),
                        new LoadOrderListingParser(
                            new HasEnabledMarkersProvider(
                                Release))));

                return new LoadOrderListingsProvider(
                    new OrderListings(),
                    new ImplicitListingsProvider(
                        fs,
                        dataDirectory,
                        new ImplicitListingModKeyProvider(
                            Release)),
                    new PluginListingsProvider(
                        Release,
                        new TimestampedPluginListingsProvider(
                            fs,
                            new TimestampAligner(fs),
                            new TimestampedPluginListingsPreferences() { ThrowOnMissingMods = false },
                            pluginRawListingsReader,
                            dataDirectory,
                            pluginPathProvider),
                        new EnabledPluginListingsProvider(
                            fs,
                            pluginRawListingsReader,
                            pluginPathProvider)),
                    new CreationClubListingsProvider(
                        fs,
                        dataDirectory,
                        cccPath,
                        new CreationClubRawListingsReader()));
            },
            ListingsProvider);

        var filteredListings = listingsProv.Get();
        foreach (var filter in LoadOrderListingProcessors)
        {
            filteredListings = filter(filteredListings);
        }

        var loGetter = new LoadOrderImporter<TModGetter>(
            fs,
            dataDirectory,
            new LoadOrderListingsInjection(filteredListings),
            new ModImporter<TModGetter>(
                fs,
                Release),
            new MasterFlagsLookupProvider(
                Release,
                fs,
                dataDirectory));

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
            loadOrder: lo,
            linkCache: linkCache);
    }
}

public sealed record GameEnvironmentBuilder
{
    private IGameReleaseContext Release { get; }
    internal IDataDirectoryProvider? DataDirectoryProvider { get; init; }
    internal ILoadOrderListingsProvider? ListingsProvider { get; init; }
    internal IPluginListingsPathContext? PluginListingsPathContext { get; init; }
    internal ICreationClubListingsPathProvider? CccListingsPathProvider { get; init; }
    internal IFileSystem? FileSystem { get; init; }
    internal Func<Type, object?>? Resolver { get; init; }

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
        IPluginListingsPathContext pluginListingsPathContext,
        ICreationClubListingsPathProvider cccListingsPathProvider)
    {
        Release = releaseProvider;
        DataDirectoryProvider = dataDirectoryProvider;
        ListingsProvider = listingsProvider;
        PluginListingsPathContext = pluginListingsPathContext;
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
    
    public GameEnvironmentBuilder WithFileSystem(IFileSystem fileSystem)
    {
        return this with { FileSystem = fileSystem };
    }

    public GameEnvironmentBuilder WithResolver(Func<Type, object?> resolver)
    {
        return this with { Resolver = resolver };
    }

    private TObject Resolve<TObject>(Func<TObject> fallback, TObject? value = default)
    {
        if (value != null)
        {
            return value;
        }
        
        if (Resolver != null)
        {
            var ret = Resolver(typeof(TObject));
            if (ret != null)
            {
                return (TObject)ret;
            }
        }

        return fallback();
    }

    /// <summary>
    /// Creates an environment with all the given rules added to the builder
    /// </summary>
    /// <returns>GameEnvironment with the rules applied</returns>
    public IGameEnvironment Build()
    {
        Warmup.Init();
        var fs = Resolve<IFileSystem>(() => IFileSystemExt.DefaultFilesystem, FileSystem);
        
        var gameLocator = new Lazy<GameLocator>(() => GameLocator.Instance);
        var dataDirectory = Resolve<IDataDirectoryProvider>(
            () => new DataDirectoryProvider(
                Release,
                gameLocator.Value), 
            DataDirectoryProvider);
        
        var pluginPathProvider = Resolve<IPluginListingsPathContext>(
            () => new PluginListingsPathContext(
                new PluginListingsPathProvider(),
                Release),
            PluginListingsPathContext);
        
        var cccPath = Resolve<ICreationClubListingsPathProvider>(
            () =>
            {
                var category = new GameCategoryContext(Release);
                return new CreationClubListingsPathProvider(
                    category,
                    new CreationClubEnabledProvider(category),
                    new GameDirectoryProvider(
                        Release,
                        gameLocator.Value));
            },
            CccListingsPathProvider);

        var listingsProv = Resolve<ILoadOrderListingsProvider>(
            () =>
            {
                var pluginRawListingsReader = new PluginRawListingsReader(
                    fs,
                    new PluginListingsParser(
                        new PluginListingCommentTrimmer(),
                        new LoadOrderListingParser(
                            new HasEnabledMarkersProvider(
                                Release))));

                return new LoadOrderListingsProvider(
                    new OrderListings(),
                    new ImplicitListingsProvider(
                        fs,
                        dataDirectory,
                        new ImplicitListingModKeyProvider(
                            Release)),
                    new PluginListingsProvider(
                        Release,
                        new TimestampedPluginListingsProvider(
                            fs,
                            new TimestampAligner(fs),
                            new TimestampedPluginListingsPreferences() { ThrowOnMissingMods = false },
                            pluginRawListingsReader,
                            dataDirectory,
                            pluginPathProvider),
                        new EnabledPluginListingsProvider(
                            fs,
                            pluginRawListingsReader,
                            pluginPathProvider)),
                    new CreationClubListingsProvider(
                        fs,
                        dataDirectory,
                        cccPath,
                        new CreationClubRawListingsReader()));
            },
            ListingsProvider);

        var filteredListings = listingsProv.Get();
        foreach (var filter in LoadOrderListingProcessors)
        {
            filteredListings = filter(filteredListings);
        }

        var loGetter = new LoadOrderImporter(
            fs,
            Release,
            dataDirectory,
            new LoadOrderListingsInjection(filteredListings),
            new ModImporter(
                fs,
                Release));

        ILoadOrderGetter<IModListingGetter<IModGetter>> lo = loGetter.Import(new BinaryReadParameters()
        {
            FileSystem = fs
        });
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
            loadOrder: lo,
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
