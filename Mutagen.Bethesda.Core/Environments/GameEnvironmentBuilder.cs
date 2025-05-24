using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Records;
using System.Collections.Immutable;
using System.IO.Abstractions;
using Mutagen.Bethesda.Archives.DI;
using Mutagen.Bethesda.Assets.DI;
using Mutagen.Bethesda.Inis.DI;
using Mutagen.Bethesda.Installs.DI;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Implicit.DI;
using Mutagen.Bethesda.Plugins.Masters.DI;
using Mutagen.Bethesda.Plugins.Order.DI;
using Mutagen.Bethesda.Plugins.Records.DI;
using Noggog;

namespace Mutagen.Bethesda.Environments;

internal record GameEnvironmentBuilderProcessorParameters();

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

    private Func<GameEnvironmentBuilderProcessorParameters, ILoadOrderListingGetter[]>? HardcodedListings { get; init; }

    private ImmutableList<Func<GameEnvironmentBuilderProcessorParameters, IEnumerable<ILoadOrderListingGetter>, IEnumerable<ILoadOrderListingGetter>>> LoadOrderListingProcessors { get; init; }

    private ImmutableList<Func<GameEnvironmentBuilderProcessorParameters, IEnumerable<IModListingGetter<TModGetter>>, IEnumerable<IModListingGetter<TModGetter>>>> ModListingProcessors { get; init; }

    private ImmutableList<TMod> MutableMods { get; init; }

    private GameEnvironmentBuilder(GameRelease release)
    {
        Release = new GameReleaseInjection(release);
        LoadOrderListingProcessors = [];
        ModListingProcessors = [];
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
        LoadOrderListingProcessors = [];
        ModListingProcessors = [];
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
        return this with { LoadOrderListingProcessors = LoadOrderListingProcessors.Add((_, l) => transformer(l)) };
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
    public GameEnvironmentBuilder<TMod, TModGetter> WithLoadOrder<T>(params T[] listings)
        where T : ILoadOrderListingGetter
    {
        return this with
        {
            HardcodedListings = _ => listings.Select(x => (ILoadOrderListingGetter)x).ToArray(),
            LoadOrderListingProcessors = []
        };
    }

    /// <summary>
    /// Sets the load order to the given listings
    /// </summary>
    /// <param name="loadOrder">Load Order to get listings from</param>
    /// <returns>New builder with the new rules</returns>
    public GameEnvironmentBuilder<TMod, TModGetter> WithLoadOrder(ILoadOrderGetter loadOrder)
    {
        return TransformLoadOrderListings(_ => loadOrder.ListedOrder.Select(x => (ILoadOrderListingGetter)new LoadOrderListing(x, enabled: true)).ToArray());
    }

    /// <summary>
    /// Exposes the load order for transformation by the user with mod objects loaded and accessible
    /// </summary>
    /// <param name="transformer">Transformation lambda to process the incoming enumerable and return a new desired one</param>
    /// <returns>New builder with the new rules</returns>
    public GameEnvironmentBuilder<TMod, TModGetter> TransformModListings(Func<IEnumerable<IModListingGetter<TModGetter>>, IEnumerable<IModListingGetter<TModGetter>>> transformer)
    {
        return this with { ModListingProcessors = ModListingProcessors.Add((_, l) => transformer(l)) };
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

        var dataDirectory = Resolve<IDataDirectoryProvider>(
            () => new DataDirectoryProvider(
                Release,
                GameLocatorLookupCache.Instance), 
            DataDirectoryProvider);
        
        var gameDirectoryLookup = Resolve<IGameDirectoryLookup>(
            () => new GameDirectoryLookupInjection(Release.Release, dataDirectory.Path.Directory));

        var pluginPathProvider = Resolve<IPluginListingsPathContext>(
            () => new PluginListingsPathContext(
                new PluginListingsPathProvider(dataDirectory),
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
                        GameLocatorLookupCache.Instance));
            },
            CccListingsPathProvider);

        var param = new GameEnvironmentBuilderProcessorParameters();

        ILoadOrderListingGetter[] listingsToUse;

        if (HardcodedListings != null)
        {
            listingsToUse = HardcodedListings(param);
        }
        else
        {
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
            
            listingsToUse = listingsProv.Get().ToArray();
        }

        IEnumerable<ILoadOrderListingGetter> filteredListings = listingsToUse;
        foreach (var filter in LoadOrderListingProcessors)
        {
            filteredListings = filter(param, filteredListings);
        }
            
        listingsToUse = filteredListings.ToArray();

        var loListings = new LoadOrderListingsInjection(listingsToUse);
        var loGetter = new LoadOrderImporter<TModGetter>(
            fs,
            dataDirectory,
            loListings,
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
            lo = filter(param, lo.ListedOrder).ToLoadOrder();
        }

        var linkCache = lo.ToMutableLinkCache(MutableMods.ToArray());

        var archiveExt = new ArchiveExtensionProvider(Release);
        var assetProvider = new GameAssetProvider(
            new DataDirectoryAssetProvider(
                fs,
                dataDirectory),
            new ArchiveAssetProvider(
                fs,
                new GetApplicableArchivePaths(
                    fs,
                    new CheckArchiveApplicability(
                        archiveExt),
                    dataDirectory,
                    archiveExt,
                    new CachedArchiveListingDetailsProvider(
                        loListings,
                        new GetArchiveIniListings(
                            fs,
                            new IniPathProvider(
                                Release,
                                new IniPathLookup(
                                    gameDirectoryLookup))),
                        new ArchiveNameFromModKeyProvider(Release))),
                Release));

        return new GameEnvironmentState<TMod, TModGetter>(
            Release.Release,
            dataFolderPath: dataDirectory.Path,
            pluginListingsPathContext: pluginPathProvider,
            creationClubListingsFilePathProvider: cccPath,
            loadOrder: lo,
            linkCache: linkCache,
            assetProvider: assetProvider);
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

    private ImmutableList<Func<GameEnvironmentBuilderProcessorParameters, IEnumerable<ILoadOrderListingGetter>, IEnumerable<ILoadOrderListingGetter>>> LoadOrderListingProcessors { get; init; }

    private ImmutableList<Func<GameEnvironmentBuilderProcessorParameters, IEnumerable<IModListingGetter<IModGetter>>, IEnumerable<IModListingGetter<IModGetter>>>> ModListingProcessors { get; init; }

    private Func<GameEnvironmentBuilderProcessorParameters, ILoadOrderListingGetter[]>? HardcodedListings { get; init; }

    private ImmutableList<IMod> MutableMods { get; init; }

    private GameEnvironmentBuilder(GameRelease release)
    {
        Release = new GameReleaseInjection(release);
        LoadOrderListingProcessors = [];
        ModListingProcessors = [];
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
        LoadOrderListingProcessors = [];
        ModListingProcessors = [];
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
        return this with { LoadOrderListingProcessors = LoadOrderListingProcessors.Add((_, l) => transformer(l)) };
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
    public GameEnvironmentBuilder WithLoadOrder<T>(params T[] listings)
        where T : ILoadOrderListingGetter
    {
        return this with
        {
            HardcodedListings = _ => listings.Select(x => (ILoadOrderListingGetter)x).ToArray(),
            LoadOrderListingProcessors = []
        };
    }

    /// <summary>
    /// Exposes the load order for transformation by the user with mod objects loaded and accessible
    /// </summary>
    /// <param name="transformer">Transformation lambda to process the incoming enumerable and return a new desired one</param>
    /// <returns>New builder with the new rules</returns>
    public GameEnvironmentBuilder TransformModListings(Func<IEnumerable<IModListingGetter<IModGetter>>, IEnumerable<IModListingGetter<IModGetter>>> transformer)
    {
        return this with { ModListingProcessors = ModListingProcessors.Add((_, l) => transformer(l)) };
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
        
        var dataDirectory = Resolve<IDataDirectoryProvider>(
            () => new DataDirectoryProvider(
                Release,
                GameLocatorLookupCache.Instance), 
            DataDirectoryProvider);
        
        var gameDirectoryLookup = Resolve<IGameDirectoryLookup>(
            () => new GameDirectoryLookupInjection(Release.Release, dataDirectory.Path.Directory));
        
        var pluginPathProvider = Resolve<IPluginListingsPathContext>(
            () => new PluginListingsPathContext(
                new PluginListingsPathProvider(dataDirectory),
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
                        GameLocatorLookupCache.Instance));
            },
            CccListingsPathProvider);
        
        var param = new GameEnvironmentBuilderProcessorParameters();

        ILoadOrderListingGetter[] listingsToUse;

        if (HardcodedListings != null)
        {
            listingsToUse = HardcodedListings(param);
        }
        else
        {
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
            
            listingsToUse = listingsProv.Get().ToArray();
        }

        IEnumerable<ILoadOrderListingGetter> filteredListings = listingsToUse;
        foreach (var filter in LoadOrderListingProcessors)
        {
            filteredListings = filter(param, filteredListings);
        }
            
        listingsToUse = filteredListings.ToArray();
        
        var loListings = new LoadOrderListingsInjection(listingsToUse);
        var loGetter = new LoadOrderImporter(
            fs,
            Release,
            dataDirectory,
            loListings,
            new KeyedMasterStyleReader(
                Release,
                fs),
            new ModImporter(
                fs,
                Release));

        ILoadOrderGetter<IModListingGetter<IModGetter>> lo = loGetter.Import(new BinaryReadParameters()
        {
            FileSystem = fs
        });
        foreach (var filter in ModListingProcessors)
        {
            lo = filter(param, lo.ListedOrder).ToLoadOrder();
        }

        var linkCache = lo.ToUntypedMutableLinkCache(Release.Release.ToCategory(), MutableMods.ToArray());

        var archiveExt = new ArchiveExtensionProvider(Release);
        var assetProvider = new GameAssetProvider(
            new DataDirectoryAssetProvider(
                fs,
                dataDirectory),
            new ArchiveAssetProvider(
                fs,
                new GetApplicableArchivePaths(
                    fs,
                    new CheckArchiveApplicability(
                        archiveExt),
                    dataDirectory,
                    archiveExt,
                    new CachedArchiveListingDetailsProvider(
                        loListings,
                        new GetArchiveIniListings(
                            fs,
                            new IniPathProvider(
                                Release,
                                new IniPathLookup(
                                    gameDirectoryLookup))),
                        new ArchiveNameFromModKeyProvider(Release))),
                Release));
        
        return new GameEnvironmentState(
            Release.Release,
            dataFolderPath: dataDirectory.Path,
            loadOrderFilePath: pluginPathProvider.Path,
            creationClubListingsFilePath: cccPath.Path,
            loadOrder: lo,
            linkCache: linkCache,
            assetProvider: assetProvider);
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
