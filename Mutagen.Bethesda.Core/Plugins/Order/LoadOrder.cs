using DynamicData;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Installs.DI;
using Mutagen.Bethesda.Plugins.Exceptions;
using Mutagen.Bethesda.Plugins.Implicit.DI;
using Mutagen.Bethesda.Plugins.Order.DI;
using Mutagen.Bethesda.Plugins.Records.DI;
using StrongInject;

namespace Mutagen.Bethesda.Plugins.Order;

/// <summary>
/// A static class with LoadOrder related utility functions
/// </summary>
public static partial class LoadOrder
{
    private static TimestampAligner Aligner = new(IFileSystemExt.DefaultFilesystem);
    private static OrderListings Orderer = new();
    
    [Register(typeof(DataDirectoryProvider), typeof(IDataDirectoryProvider))]
    [Register(typeof(ModImporter<>), typeof(IModImporter<>))]
    [Register(typeof(LoadOrderImporter<>), typeof(LoadOrderImporter<>), typeof(ILoadOrderImporter<>))]
    [Register(typeof(LiveLoadOrderTimings), typeof(ILiveLoadOrderTimings))]
    [Register(typeof(MasterFlagsLookupProvider), typeof(IMasterFlagsLookupProvider))]
    [Register(typeof(PluginLiveLoadOrderProvider), typeof(IPluginLiveLoadOrderProvider))]
    [Register(typeof(CreationClubLiveLoadOrderFolderWatcher), typeof(ICreationClubLiveLoadOrderFolderWatcher))]
    [Register(typeof(CreationClubLiveListingsFileReader), typeof(ICreationClubLiveListingsFileReader))]
    [Register(typeof(CreationClubLiveLoadOrderProvider), typeof(ICreationClubLiveLoadOrderProvider))]
    [Register(typeof(LiveLoadOrderProvider), typeof(LiveLoadOrderProvider), typeof(ILiveLoadOrderProvider))]
    [Register(typeof(CreationClubRawListingsReader), typeof(ICreationClubRawListingsReader))]
    [Register(typeof(GameDirectoryProvider), typeof(IGameDirectoryProvider))]
    [Register(typeof(GameCategoryContext), typeof(IGameCategoryContext))]
    [Register(typeof(CreationClubEnabledProvider), typeof(ICreationClubEnabledProvider))]
    [Register(typeof(CreationClubListingsPathProvider), typeof(ICreationClubListingsPathProvider))]
    [Register(typeof(PluginListingCommentTrimmer), typeof(IPluginListingCommentTrimmer))]
    [Register(typeof(EnabledPluginListingsProvider), typeof(IEnabledPluginListingsProvider))]
    [Register(typeof(CreationClubListingsProvider), typeof(ICreationClubListingsProvider))]
    [Register(typeof(HasEnabledMarkersProvider), typeof(IHasEnabledMarkersProvider))]
    [Register(typeof(LoadOrderListingParser), typeof(ILoadOrderListingParser))]
    [Register(typeof(PluginListingsParser), typeof(IPluginListingsParser))]
    [Register(typeof(PluginListingsPathProvider), typeof(IPluginListingsPathProvider))]
    [Register(typeof(PluginRawListingsReader), typeof(IPluginRawListingsReader))]
    [Register(typeof(PluginListingsPathContext), typeof(IPluginListingsPathContext))]
    [Register(typeof(TimestampAligner), typeof(ITimestampAligner))]
    [Register(typeof(TimestampedPluginListingsProvider), typeof(ITimestampedPluginListingsProvider))]
    [Register(typeof(ImplicitListingModKeyProvider), typeof(IImplicitListingModKeyProvider))]
    [Register(typeof(PluginListingsProvider), typeof(IPluginListingsProvider))]
    [Register(typeof(ImplicitListingsProvider), typeof(IImplicitListingsProvider))]
    [Register(typeof(OrderListings), typeof(IOrderListings))]
    [Register(typeof(TimestampedPluginListingsPreferences), typeof(ITimestampedPluginListingsPreferences))]
    [Register(typeof(LoadOrderListingsProvider), typeof(LoadOrderListingsProvider), typeof(ILoadOrderListingsProvider))]
    internal partial class CommonModule
    {
        [Instance(Options.AsImplementedInterfaces)] public static GameLocatorLookupCache LookupCache = GameLocatorLookupCache.Instance;
    }

    #region Timestamps

    /// <summary>
    /// Returns whether given game needs timestamp alignment for its load order
    /// </summary>
    /// <param name="game">Game to check</param>
    /// <returns>True if file located</returns>
    public static bool NeedsTimestampAlignment(GameCategory game) => Aligner.NeedsTimestampAlignment(game);

    /// <summary>
    /// Constructs a load order from a list of mods and a data folder.
    /// Load Order is sorted to the order the game will load the mod files: by file's date modified timestamp.
    /// </summary>
    /// <param name="incomingLoadOrder">Mods to include</param>
    /// <param name="dataPath">Path to data folder</param>
    /// <param name="throwOnMissingMods">Whether to throw and exception if mods are missing</param>
    /// <returns>Enumerable of modkeys in load order, excluding missing mods</returns>
    /// <exception cref="MissingModException">If throwOnMissingMods true and file is missing</exception>
    public static IEnumerable<ILoadOrderListingGetter> AlignToTimestamps(
        IEnumerable<ILoadOrderListingGetter> incomingLoadOrder,
        DirectoryPath dataPath,
        bool throwOnMissingMods = true)
    {
        return Aligner.AlignToTimestamps(incomingLoadOrder, dataPath, throwOnMissingMods: throwOnMissingMods);
    }

    /// <summary>
    /// Constructs a load order from a list of mods and a data folder.
    /// Load Order is sorted to the order the game will load the mod files: by file's date modified timestamp.
    /// </summary>
    /// <param name="incomingLoadOrder">Mods and their write timestamps</param>
    /// <returns>Enumerable of modkeys in load order, excluding missing mods</returns>
    public static IEnumerable<ModKey> AlignToTimestamps(IEnumerable<(ModKey ModKey, DateTime Write)> incomingLoadOrder)
    {
        return Aligner.AlignToTimestamps(incomingLoadOrder);
    }

    /// <summary>
    /// Modifies time stamps of files to match the given ordering
    /// <param name="loadOrder">Order to conform files to</param>
    /// <param name="dataPath">Path to data folder</param>
    /// <param name="throwOnMissingMods">Whether to throw and exception if mods are missing</param>
    /// <param name="startDate">Date to give the first file</param>
    /// <param name="interval">Time interval to space between each file's date</param>
    /// <exception cref="MissingModException">If throwOnMissingMods true and file is missing</exception>
    /// </summary>
    public static void AlignTimestamps(
        IEnumerable<ModKey> loadOrder,
        DirectoryPath dataPath,
        bool throwOnMissingMods = true,
        DateTime? startDate = null,
        TimeSpan? interval = null)
    {
        Aligner.AlignTimestamps(
            loadOrder,
            dataPath,
            throwOnMissingMods: throwOnMissingMods,
            startDate: startDate,
            interval: interval);
    }

    #endregion

    [RegisterModule(typeof(CommonModule))]
    internal partial class GetLoadOrderListingsModule : IContainer<LoadOrderListingsProvider>
    {
        [Instance] private readonly IGameReleaseContext _release;
        [Instance] private readonly IFileSystem _fileSystem;
        [Instance] private readonly IDataDirectoryProvider _dataDirectory;
        [Instance] private readonly ITimestampedPluginListingsPreferences _timestampedPrefs;
        
        public GetLoadOrderListingsModule(
            GameRelease release,
            DirectoryPath dataPath,
            bool throwOnMissingMods,
            IFileSystem? fileSystem)
        {
            _release = new GameReleaseInjection(release);
            _fileSystem = fileSystem.GetOrDefault();
            _dataDirectory = new DataDirectoryInjection(dataPath);
            _timestampedPrefs = new TimestampedPluginListingsPreferences()
            {
                ThrowOnMissingMods = throwOnMissingMods
            };
        }
    }

    /// <summary>
    /// Returns a load order listing from the usual sources
    /// </summary>
    /// <param name="game">Game type</param>
    /// <param name="dataPath">Path to game's data folder</param>
    /// <param name="throwOnMissingMods">Whether to throw and exception if mods are missing</param>
    /// <param name="fileSystem">Filesystem to use</param>
    /// <returns>Enumerable of modkeys representing a load order</returns>
    /// <exception cref="ArgumentException">Line in plugin file is unexpected</exception>
    /// <exception cref="FileNotFoundException">If plugin file not located</exception>
    /// <exception cref="MissingModException">If throwOnMissingMods true and file is missing</exception>
    public static IEnumerable<ILoadOrderListingGetter> GetLoadOrderListings(
        GameRelease game,
        DirectoryPath dataPath,
        bool throwOnMissingMods = true,
        IFileSystem? fileSystem = null)
    {
        var prov = new GetLoadOrderListingsModule(game, dataPath, throwOnMissingMods, fileSystem)
            .Resolve().Value;
        return prov.Get();
    }

    [RegisterModule(typeof(CommonModule))]
    internal partial class GetLoadOrderListingsPluginsOverrideModule : IContainer<LoadOrderListingsProvider>
    {
        [Instance] private readonly IGameReleaseContext _release;
        [Instance] private readonly IFileSystem _fileSystem;
        [Instance] private readonly IDataDirectoryProvider _dataDirectory;
        [Instance] private readonly ITimestampedPluginListingsPreferences _timestampedPrefs;
        [Instance] private readonly ICreationClubListingsPathProvider _creationClubListingsPathProvider;
        [Instance] private readonly IPluginListingsPathContext _pluginListingsPathContext;
        
        public GetLoadOrderListingsPluginsOverrideModule(
            GameRelease release,
            FilePath pluginsFilePath,
            FilePath? creationClubFilePath,
            DirectoryPath dataPath,
            bool throwOnMissingMods,
            IFileSystem? fileSystem)
        {
            _release = new GameReleaseInjection(release);
            _fileSystem = fileSystem.GetOrDefault();
            _pluginListingsPathContext = new PluginListingsPathInjection(pluginsFilePath);
            _dataDirectory = new DataDirectoryInjection(dataPath);
            _creationClubListingsPathProvider = new CreationClubListingsPathInjection(creationClubFilePath);
            _timestampedPrefs = new TimestampedPluginListingsPreferences()
            {
                ThrowOnMissingMods = throwOnMissingMods
            };
        }
    }
    
    public static IEnumerable<ILoadOrderListingGetter> GetLoadOrderListings(
        GameRelease game,
        FilePath pluginsFilePath,
        FilePath? creationClubFilePath,
        DirectoryPath dataPath,
        bool throwOnMissingMods = true,
        IFileSystem? fileSystem = null)
    {
        var prov = new GetLoadOrderListingsPluginsOverrideModule(
                game, pluginsFilePath, creationClubFilePath, 
                dataPath, throwOnMissingMods, fileSystem)
            .Resolve().Value;
        return prov.Get();
    }

    public static IEnumerable<T> OrderListings<T>(IEnumerable<T> e, Func<T, ModKey> selector)
    {
        return Orderer.Order(e, selector);
    }

    public static IEnumerable<T> OrderListings<T>(
        IEnumerable<T> implicitListings,
        IEnumerable<T> pluginsListings,
        IEnumerable<T> creationClubListings,
        Func<T, ModKey> selector)
    {
        return Orderer.Order(implicitListings, pluginsListings, creationClubListings, selector);
    }

    [RegisterModule(typeof(CommonModule))]
    internal partial class LiveLoadOrderProviderModule : IContainer<LiveLoadOrderProvider>
    {
        [Instance] private readonly IGameReleaseContext _release;
        [Instance] private readonly IFileSystem _fileSystem;
        [Instance] private readonly IDataDirectoryProvider _dataDirectory;
        [Instance] private readonly ITimestampedPluginListingsPreferences _timestampedPrefs;

        public LiveLoadOrderProviderModule(
            GameRelease release,
            DirectoryPath dataFolderPath,
            bool throwOnMissingMods,
            IFileSystem? fileSystem)
        {
            _release = new GameReleaseInjection(release);
            _dataDirectory = new DataDirectoryInjection(dataFolderPath);
            _fileSystem = fileSystem.GetOrDefault();
            _timestampedPrefs = new TimestampedPluginListingsPreferences()
            {
                ThrowOnMissingMods = throwOnMissingMods
            };
        }
    }

    public static IObservable<IChangeSet<ILoadOrderListingGetter>> GetLiveLoadOrderListings(
        GameRelease game,
        DirectoryPath dataFolderPath,
        out IObservable<ErrorResponse> state,
        bool throwOnMissingMods = true,
        IScheduler? scheduler = null,
        IFileSystem? fileSystem = null)
    {
        var prov = new LiveLoadOrderProviderModule(game, dataFolderPath, throwOnMissingMods, fileSystem);
        return prov.Resolve().Value.Get(out state, scheduler);
    }

    public static IObservable<IChangeSet<ILoadOrderListingGetter>> GetLiveLoadOrderListings(
        IObservable<GameRelease> game,
        IObservable<DirectoryPath> dataFolderPath,
        out IObservable<ErrorResponse> state,
        bool throwOnMissingMods = true,
        IScheduler? scheduler = null)
    {
        var obs = Observable.CombineLatest(
                game,
                dataFolderPath,
                (gameVal, dataFolderVal) =>
                {
                    var lo = GetLiveLoadOrderListings(
                        game: gameVal,
                        dataFolderPath: dataFolderVal,
                        loadOrderFilePath: PluginListings.GetListingsPath(gameVal),
                        cccLoadOrderFilePath: CreationClubListings.GetListingsPath(gameVal.ToCategory(), dataFolderVal),
                        state: out var state,
                        throwOnMissingMods: throwOnMissingMods,
                        scheduler: scheduler);
                    return (LoadOrder: lo, State: state);
                })
            .Replay(1)
            .RefCount();
        state = obs.Select(x => x.State)
            .Switch();
        return obs.Select(x => x.LoadOrder)
            .Switch();
    }

    [RegisterModule(typeof(CommonModule))]
    internal partial class GetLiveLoadOrderListingsPluginsListingsOverrideModule : IContainer<LiveLoadOrderProvider>
    {
        [Instance] private readonly IGameReleaseContext _release;
        [Instance] private readonly IFileSystem _fileSystem;
        [Instance] private readonly IDataDirectoryProvider _dataDirectory;
        [Instance] private readonly ITimestampedPluginListingsPreferences _timestampedPrefs;
        [Instance] private readonly IPluginListingsPathContext _listingsPathContext;
        [Instance] private readonly ILiveLoadOrderTimings _liveLoadOrderTimings;
        private readonly FilePath? _cccLoadOrderFilePath;
        [Factory] private ICreationClubListingsPathProvider CreateCccPathProvider(
            IGameCategoryContext categoryContext,
            ICreationClubEnabledProvider isUsed,
            IGameDirectoryProvider gameDirectoryProvider) => _cccLoadOrderFilePath == null
            ? new CreationClubListingsPathProvider(categoryContext, isUsed, gameDirectoryProvider)
            : new CreationClubListingsPathInjection(_cccLoadOrderFilePath);

        public GetLiveLoadOrderListingsPluginsListingsOverrideModule(
            GameRelease release,
            FilePath loadOrderFilePath,
            DirectoryPath dataFolderPath,
            bool throwOnMissingMods,
            FilePath? cccLoadOrderFilePath,
            ILiveLoadOrderTimings? timings,
            IFileSystem? fileSystem)
        {
            _cccLoadOrderFilePath = cccLoadOrderFilePath;
            _liveLoadOrderTimings = timings ?? new LiveLoadOrderTimings();
            _listingsPathContext = new PluginListingsPathInjection(loadOrderFilePath);
            _release = new GameReleaseInjection(release);
            _dataDirectory = new DataDirectoryInjection(dataFolderPath);
            _fileSystem = fileSystem.GetOrDefault();
            _timestampedPrefs = new TimestampedPluginListingsPreferences()
            {
                ThrowOnMissingMods = throwOnMissingMods
            };
        }
    }
    
    public static IObservable<IChangeSet<ILoadOrderListingGetter>> GetLiveLoadOrderListings(
        GameRelease game,
        FilePath loadOrderFilePath,
        DirectoryPath dataFolderPath,
        out IObservable<ErrorResponse> state,
        FilePath? cccLoadOrderFilePath = null,
        bool throwOnMissingMods = true,
        IScheduler? scheduler = null,
        IFileSystem? fileSystem = null,
        ILiveLoadOrderTimings? timings = null)
    {
        var prov = new GetLiveLoadOrderListingsPluginsListingsOverrideModule(
            game, loadOrderFilePath, dataFolderPath, throwOnMissingMods, cccLoadOrderFilePath, timings, fileSystem);
        return prov.Resolve().Value.Get(out state, scheduler);
    }

    public static IObservable<IChangeSet<ILoadOrderListingGetter>> GetLiveLoadOrderListings(
        IObservable<GameRelease> game,
        IObservable<FilePath> loadOrderFilePath,
        IObservable<DirectoryPath> dataFolderPath,
        out IObservable<ErrorResponse> state,
        IObservable<FilePath?>? cccLoadOrderFilePath = null,
        bool throwOnMissingMods = true,
        IScheduler? scheduler = null)
    {
        var obs = Observable.CombineLatest(
                game,
                dataFolderPath,
                loadOrderFilePath,
                cccLoadOrderFilePath ?? Observable.Return(default(FilePath?)),
                (gameVal, dataFolderVal, loadOrderFilePathVal, cccVal) =>
                {
                    var lo = GetLiveLoadOrderListings(
                        game: gameVal,
                        dataFolderPath: dataFolderVal,
                        loadOrderFilePath: loadOrderFilePathVal,
                        cccLoadOrderFilePath: cccVal,
                        state: out var state,
                        throwOnMissingMods: throwOnMissingMods,
                        scheduler: scheduler);
                    return (LoadOrder: lo, State: state);
                })
            .Replay(1)
            .RefCount();
        state = obs.Select(x => x.State)
            .Switch();
        return obs.Select(x => x.LoadOrder)
            .Switch();
    }
    
    [RegisterModule(typeof(CommonModule))]
    internal partial class ImportDataFolderModule<TMod> : IContainer<LoadOrderImporter<TMod>>
        where TMod : class, IModGetter
    {
        [Instance] private readonly IGameReleaseContext _release;
        [Instance] private readonly IFileSystem _fileSystem;
        [Instance] private readonly IDataDirectoryProvider _dataDirectory;
        [Instance] private readonly ILoadOrderListingsProvider _loadOrder;

        public ImportDataFolderModule(
            GameRelease release,
            DirectoryPath dataPath,
            IEnumerable<ILoadOrderListingGetter> loadOrder,
            IFileSystem? fileSystem)
        {
            _release = new GameReleaseInjection(release);
            _dataDirectory = new DataDirectoryInjection(dataPath);
            _fileSystem = fileSystem.GetOrDefault();
            _loadOrder = new LoadOrderListingsInjection(loadOrder);
        }
    }

    /// <summary>
    /// Constructs a load order filled with mods constructed
    /// </summary>
    /// <param name="dataFolder">Path data folder containing mods</param>
    /// <param name="loadOrder">Unique list of listings to import</param>
    /// <param name="gameRelease">GameRelease associated with the mods to create<br/>
    /// This may be unapplicable to some games with only one release, but should still be passed in.
    /// </param>
    /// <param name="fileSystem">Filesystem to use</param>
    public static ILoadOrder<IModListing<TMod>> Import<TMod>(
        DirectoryPath dataFolder,
        IEnumerable<ILoadOrderListingGetter> loadOrder,
        GameRelease gameRelease,
        IFileSystem? fileSystem = null)
        where TMod : class, IModGetter
    {
        return new ImportDataFolderModule<TMod>(gameRelease, dataFolder, loadOrder, fileSystem)
            .Resolve().Value.Import();
    }

    /// <summary>
    /// Constructs a load order filled with mods constructed
    /// </summary>
    /// <param name="dataFolder">Path data folder containing mods</param>
    /// <param name="loadOrder">Unique list of mod keys to import</param>
    /// <param name="gameRelease">GameRelease associated with the mods to create<br/>
    /// This may be unapplicable to some games with only one release, but should still be passed in.
    /// </param>
    /// <param name="fileSystem">Filesystem to use</param>
    public static ILoadOrder<IModListing<TMod>> Import<TMod>(
        DirectoryPath dataFolder,
        IEnumerable<ModKey> loadOrder,
        GameRelease gameRelease,
        IFileSystem? fileSystem = null)
        where TMod : class, IModGetter
    {
        return Import<TMod>(
            dataFolder,
            loadOrder.Select(x => new LoadOrderListing(x, true)),
            gameRelease,
            fileSystem);
    }
    
    [RegisterModule(typeof(CommonModule))]
    internal partial class ImportDataFolderModFactoryModule<TMod> : IContainer<LoadOrderImporter<TMod>>
        where TMod : class, IModKeyed
    {
        [Instance] private readonly IGameReleaseContext _release;
        [Instance] private readonly IFileSystem _fileSystem;
        [Instance] private readonly IDataDirectoryProvider _dataDirectory;
        [Instance] private readonly IModImporter<TMod> _modImporter;
        [Instance] private readonly ILoadOrderListingsProvider _loadOrder;

        public ImportDataFolderModFactoryModule(
            GameRelease release,
            DirectoryPath dataPath,
            IEnumerable<ILoadOrderListingGetter> loadOrder,
            Func<ModPath, TMod> factory,
            IFileSystem? fileSystem)
        {
            _release = new GameReleaseInjection(release);
            _dataDirectory = new DataDirectoryInjection(dataPath);
            _fileSystem = fileSystem.GetOrDefault();
            _modImporter = new ModImporterWrapper<TMod>(factory);
            _loadOrder = new LoadOrderListingsInjection(loadOrder);
        }
    }

    /// <summary>
    /// Constructs a load order filled with mods constructed by given importer func
    /// </summary>
    /// <param name="dataFolder">Path data folder containing mods</param>
    /// <param name="loadOrder">Unique list of mod keys to import</param>
    /// <param name="gameRelease">GameRelease associated with the mods to create</param>
    /// <param name="factory">Func to use to create a new mod from a path</param>
    /// <param name="fileSystem">Filesystem to use</param>
    public static ILoadOrder<IModListing<TMod>> Import<TMod>(
        DirectoryPath dataFolder,
        IEnumerable<ModKey> loadOrder,
        GameRelease gameRelease,
        Func<ModPath, TMod> factory,
        IFileSystem? fileSystem = null)
        where TMod : class, IModKeyed
    {
        return new ImportDataFolderModFactoryModule<TMod>(gameRelease, dataFolder, loadOrder.Select(x => new LoadOrderListing(x, true)), factory, fileSystem)
            .Resolve().Value.Import();
    }

    /// <summary>
    /// Constructs a load order filled with mods constructed by given importer func
    /// </summary>
    /// <param name="dataFolder">Path data folder containing mods</param>
    /// <param name="loadOrder">Unique list of listings to import</param>
    /// <param name="gameRelease">GameRelease associated with the mods to create</param>
    /// <param name="factory">Func to use to create a new mod from a path</param>
    /// <param name="fileSystem">Filesystem to use</param>
    public static ILoadOrder<IModListing<TMod>> Import<TMod>(
        DirectoryPath dataFolder,
        IEnumerable<ILoadOrderListingGetter> loadOrder,
        GameRelease gameRelease,
        Func<ModPath, TMod> factory,
        IFileSystem? fileSystem = null)
        where TMod : class, IModKeyed
    {
        return new ImportDataFolderModFactoryModule<TMod>(gameRelease, dataFolder, loadOrder, factory, fileSystem)
            .Resolve().Value.Import();
    }
    
    [RegisterModule(typeof(CommonModule))]
    internal partial class ImportModule<TMod> : IContainer<LoadOrderImporter<TMod>>
        where TMod : class, IModGetter
    {
        [Instance] private readonly IGameReleaseContext _release;
        [Instance] private readonly IFileSystem _fileSystem;
        [Instance] private readonly ILoadOrderListingsProvider _loadOrder;

        public ImportModule(
            GameRelease release,
            IEnumerable<ILoadOrderListingGetter> loadOrder,
            IFileSystem? fileSystem)
        {
            _release = new GameReleaseInjection(release);
            _fileSystem = fileSystem.GetOrDefault();
            _loadOrder = new LoadOrderListingsInjection(loadOrder);
        }
    }
    
    /// <summary>
    /// Constructs a load order filled with mods constructed
    /// </summary>
    /// <param name="loadOrder">Unique list of listings to import</param>
    /// <param name="gameRelease">GameRelease associated with the mods to create<br/>
    /// This may be unapplicable to some games with only one release, but should still be passed in.
    /// </param>
    /// <param name="fileSystem">Filesystem to use</param>
    public static ILoadOrder<IModListing<TMod>> Import<TMod>(
        IEnumerable<ILoadOrderListingGetter> loadOrder,
        GameRelease gameRelease,
        IFileSystem? fileSystem = null)
        where TMod : class, IModGetter
    {
        return new ImportModule<TMod>(gameRelease, loadOrder, fileSystem)
            .Resolve().Value.Import();
    }

    /// <summary>
    /// Constructs a load order filled with mods constructed
    /// </summary>
    /// <param name="loadOrder">Unique list of mod keys to import</param>
    /// <param name="gameRelease">GameRelease associated with the mods to create<br/>
    /// This may be unapplicable to some games with only one release, but should still be passed in.
    /// </param>
    /// <param name="fileSystem">Filesystem to use</param>
    public static ILoadOrder<IModListing<TMod>> Import<TMod>(
        IEnumerable<ModKey> loadOrder,
        GameRelease gameRelease,
        IFileSystem? fileSystem = null)
        where TMod : class, IModGetter
    {
        return Import<TMod>(
            loadOrder.Select(x => new LoadOrderListing(x, true)),
            gameRelease,
            fileSystem);
    }
    
    [RegisterModule(typeof(CommonModule))]
    internal partial class ImportModFactoryModule<TMod> : IContainer<LoadOrderImporter<TMod>>
        where TMod : class, IModKeyed
    {
        [Instance] private readonly IGameReleaseContext _release;
        [Instance] private readonly IFileSystem _fileSystem;
        [Instance] private readonly IModImporter<TMod> _modImporter;
        [Instance] private readonly ILoadOrderListingsProvider _loadOrder;

        public ImportModFactoryModule(
            GameRelease release,
            IEnumerable<ILoadOrderListingGetter> loadOrder,
            Func<ModPath, TMod> factory,
            IFileSystem? fileSystem)
        {
            _release = new GameReleaseInjection(release);
            _fileSystem = fileSystem.GetOrDefault();
            _modImporter = new ModImporterWrapper<TMod>(factory);
            _loadOrder = new LoadOrderListingsInjection(loadOrder);
        }
    }
    
    /// <summary>
    /// Constructs a load order filled with mods constructed by given importer func
    /// </summary>
    /// <param name="loadOrder">Unique list of mod keys to import</param>
    /// <param name="gameRelease">GameRelease associated with the mods to create<br/>
    /// This may be unapplicable to some games with only one release, but should still be passed in.
    /// </param>
    /// <param name="factory">Func to use to create a new mod from a path</param>
    /// <param name="fileSystem">Filesystem to use</param>
    public static ILoadOrder<IModListing<TMod>> Import<TMod>(
        IEnumerable<ModKey> loadOrder,
        GameRelease gameRelease,
        Func<ModPath, TMod> factory,
        IFileSystem? fileSystem = null)
        where TMod : class, IModKeyed
    {
        return new ImportModFactoryModule<TMod>(gameRelease, 
                loadOrder.Select(x => new LoadOrderListing(x, true)),
                factory, fileSystem)
            .Resolve().Value.Import();
    }

    /// <summary>
    /// Constructs a load order filled with mods constructed by given importer func
    /// </summary>
    /// <param name="loadOrder">Unique list of listings to import</param>
    /// <param name="gameRelease">GameRelease associated with the mods to create<br/>
    /// This may be unapplicable to some games with only one release, but should still be passed in.
    /// </param>
    /// <param name="factory">Func to use to create a new mod from a path</param>
    /// <param name="fileSystem">Filesystem to use</param>
    public static ILoadOrder<IModListing<TMod>> Import<TMod>(
        IEnumerable<ILoadOrderListingGetter> loadOrder,
        GameRelease gameRelease,
        Func<ModPath, TMod> factory,
        IFileSystem? fileSystem = null)
        where TMod : class, IModKeyed
    {
        return new ImportModFactoryModule<TMod>(
                gameRelease, loadOrder,
                factory, fileSystem)
            .Resolve().Value.Import();
    }

    [RegisterModule(typeof(CommonModule))]
    internal partial class LoadOrderImporterModule<TMod> : IContainer<LoadOrderImporter<TMod>>
        where TMod : class, IModGetter
    {
        [Instance] private readonly IGameReleaseContext _release;
        [Instance] private readonly IFileSystem _fileSystem;

        public LoadOrderImporterModule(
            GameRelease release,
            IFileSystem? fileSystem)
        {
            _release = new GameReleaseInjection(release);
            _fileSystem = fileSystem.GetOrDefault();
        }
    }

    /// <summary>
    /// Constructs a load order filled with mods constructed
    /// </summary>
    /// <param name="gameRelease">GameRelease associated with the mods to create<br/>
    /// This may be unapplicable to some games with only one release, but should still be passed in.
    /// </param>
    /// <param name="fileSystem">Filesystem to use</param>
    public static ILoadOrder<IModListing<TMod>> Import<TMod>(
        GameRelease gameRelease,
        IFileSystem? fileSystem = null)
        where TMod : class, IModGetter
    {
        return new LoadOrderImporterModule<TMod>(gameRelease, fileSystem).Resolve().Value.Import();
    }

    [RegisterModule(typeof(CommonModule))]
    internal partial class LoadOrderImporterFactoryModule<TMod> : IContainer<LoadOrderImporter<TMod>>
        where TMod : class, IModKeyed
    {
        [Instance] private readonly IGameReleaseContext _release;
        [Instance] private readonly IFileSystem _fileSystem;
        [Instance] private readonly IModImporter<TMod> _modImporter;

        public LoadOrderImporterFactoryModule(
            GameRelease release,
            IFileSystem? fileSystem,
            Func<ModPath, TMod> factory)
        {
            _release = new GameReleaseInjection(release);
            _fileSystem = fileSystem.GetOrDefault();
            _modImporter = new ModImporterWrapper<TMod>(factory);
        }
    }

    /// <summary>
    /// Constructs a load order filled with mods constructed by given importer func
    /// </summary>
    /// <param name="gameRelease">GameRelease associated with the mods to create<br/>
    /// This may be unapplicable to some games with only one release, but should still be passed in.
    /// </param>
    /// <param name="factory">Func to use to create a new mod from a path</param>
    /// <param name="fileSystem">Filesystem to use</param>
    public static ILoadOrder<IModListing<TMod>> Import<TMod>(
        GameRelease gameRelease,
        Func<ModPath, TMod> factory,
        IFileSystem? fileSystem = null)
        where TMod : class, IModKeyed
    {
        return new LoadOrderImporterFactoryModule<TMod>(gameRelease, fileSystem, factory).Resolve().Value.Import();
    }

    [RegisterModule(typeof(CommonModule))]
    internal partial class LoadOrderImporterDataFolderModule<TMod> : IContainer<LoadOrderImporter<TMod>>
        where TMod : class, IModGetter
    {
        [Instance] private readonly IGameReleaseContext _release;
        [Instance] private readonly IFileSystem _fileSystem;
        [Instance] private readonly IDataDirectoryProvider _dataDirectory;

        public LoadOrderImporterDataFolderModule(
            GameRelease release,
            IFileSystem? fileSystem,
            DirectoryPath dataFolder)
        {
            _release = new GameReleaseInjection(release);
            _fileSystem = fileSystem.GetOrDefault();
            _dataDirectory = new DataDirectoryInjection(dataFolder);
        }
    }

    /// <summary>
    /// Constructs a load order filled with mods constructed
    /// </summary>
    /// <param name="dataFolder">Path data folder containing mods</param>
    /// <param name="gameRelease">GameRelease associated with the mods to create<br/>
    /// This may be unapplicable to some games with only one release, but should still be passed in.
    /// </param>
    /// <param name="fileSystem">Filesystem to use</param>
    public static ILoadOrder<IModListing<TMod>> Import<TMod>(
        DirectoryPath dataFolder,
        GameRelease gameRelease,
        IFileSystem? fileSystem = null)
        where TMod : class, IModGetter
    {
        return new LoadOrderImporterDataFolderModule<TMod>(gameRelease, fileSystem, dataFolder).Resolve().Value.Import();
    }

    [RegisterModule(typeof(CommonModule))]
    internal partial class LoadOrderImporterDataFolderFactoryModule<TMod> : IContainer<LoadOrderImporter<TMod>>
        where TMod : class, IModKeyed
    {
        [Instance] private readonly IGameReleaseContext _release;
        [Instance] private readonly IFileSystem _fileSystem;
        [Instance] private readonly IDataDirectoryProvider _dataDirectory;
        [Instance] private readonly IModImporter<TMod> _modImporter;

        public LoadOrderImporterDataFolderFactoryModule(
            GameRelease release,
            IFileSystem? fileSystem,
            Func<ModPath, TMod> factory,
            DirectoryPath dataFolder)
        {
            _release = new GameReleaseInjection(release);
            _fileSystem = fileSystem.GetOrDefault();
            _dataDirectory = new DataDirectoryInjection(dataFolder);
            _modImporter = new ModImporterWrapper<TMod>(factory);
        }
    }
    
    /// <summary>
    /// Constructs a load order filled with mods constructed by given importer func
    /// </summary>
    /// <param name="dataFolder">Path data folder containing mods</param>
    /// <param name="gameRelease">GameRelease associated with the mods to create<br/>
    /// This may be unapplicable to some games with only one release, but should still be passed in.
    /// </param>
    /// <param name="factory">Func to use to create a new mod from a path</param>
    /// <param name="fileSystem">Filesystem to use</param>
    public static ILoadOrder<IModListing<TMod>> Import<TMod>(
        DirectoryPath dataFolder,
        GameRelease gameRelease,
        Func<ModPath, TMod> factory,
        IFileSystem? fileSystem = null)
        where TMod : class, IModKeyed
    {
        return new LoadOrderImporterDataFolderFactoryModule<TMod>(gameRelease, fileSystem, factory, dataFolder).Resolve().Value.Import();
    }

    public static void Write(
        FilePath path,
        GameRelease release,
        IEnumerable<ILoadOrderListingGetter> loadOrder,
        bool removeImplicitMods = true,
        IFileSystem? fileSystem = null)
    {
        fileSystem ??= IFileSystemExt.DefaultFilesystem;
        var rel = new GameReleaseInjection(release);
        new LoadOrderWriter(
                fileSystem,
                new HasEnabledMarkersProvider(rel),
                new ImplicitListingModKeyProvider(rel))
            .Write(path, loadOrder, removeImplicitMods);
    }

    public static void Write(
        FilePath path,
        GameRelease release,
        ILoadOrderGetter<IModListingGetter> loadOrder,
        bool removeImplicitMods = true,
        IFileSystem? fileSystem = null)
    {
        Write(
            path: path,
            release: release,
            loadOrder: loadOrder.ListedOrder,
            removeImplicitMods: removeImplicitMods,
            fileSystem: fileSystem);
    }

    public static ILoadOrderGetter CreateReadonly(IEnumerable<ModKey> modKeys)
    {
        return new LoadOrderGetter(modKeys);
    }
}

public interface ILoadOrderGetter : IDisposable
{
    /// <summary>
    /// Number of listings on the Load Order
    /// </summary>
    int Count { get; }
    
    /// <summary>
    /// Whether the contained items will be disposed with the load order
    /// </summary>
    bool DisposingItems { get; }

    /// <summary>
    /// Listings in the order they were listed
    /// </summary>
    IEnumerable<ModKey> ListedOrder { get; }

    /// <summary>
    /// Listings in priority order, where the mod with the highest priority comes first.  (Reverse of ListedOrder)
    /// </summary>
    IEnumerable<ModKey> PriorityOrder { get; }

    /// <summary>
    /// Whether the load order contains a listing with the given key
    /// </summary>
    bool ContainsKey(ModKey key);
}

public interface ILoadOrderGetter<out TListing> : 
    ILoadOrderGetter,
    IReadOnlyList<Noggog.IKeyValue<ModKey, TListing>>, 
    IReadOnlyCache<TListing, ModKey>
    where TListing : IModKeyed
{
    new TListing this[int index] { get; }

    TListing? TryGetAtIndex(int index);

    /// <summary>
    /// Listings in the order they were listed
    /// </summary>
    new IEnumerable<TListing> ListedOrder { get; }

    /// <summary>
    /// Listings in priority order, where the mod with the highest priority comes first.  (Reverse of ListedOrder)
    /// </summary>
    new IEnumerable<TListing> PriorityOrder { get; }

    /// <summary>
    /// Number of listings on the Load Order
    /// </summary>
    new int Count { get; }

    /// <summary>
    /// Whether the load order contains a listing with the given key
    /// </summary>
    new bool ContainsKey(ModKey key);

    /// <summary>
    /// Locates index of an item with given key
    /// </summary>
    /// <param name="key">Key to query</param>
    /// <returns>Index of item on list with key. -1 if not located</returns>
    int IndexOf(ModKey key);
}

public interface ILoadOrder<TListing> : ILoadOrderGetter<TListing>
    where TListing : IModKeyed
{
    /// <summary>
    /// Adds an item to the end of the load order
    /// </summary>
    /// <param name="item">Item to put at end of load order</param>
    /// <exception cref="ArgumentException">If an item with same ModKey exists already</exception>
    void Add(TListing item);

    /// <summary>
    /// Adds items to the end of the load order
    /// </summary>
    /// <param name="items">Items to put at end of load order</param>
    /// <exception cref="ArgumentException">If an item with same ModKey exists already</exception>
    void Add(IEnumerable<TListing> items);

    /// <summary>
    /// Adds an item at the given index in load order, with the given ModKey
    /// </summary>
    /// <param name="item">Item to put at end of load order</param>
    /// <param name="index">Index to insert at</param>
    /// <exception cref="ArgumentException">If an item with same ModKey exists already</exception>
    void Add(TListing item, int index);

    /// <summary>
    /// Clears load order of all items
    /// </summary>
    void Clear();

    bool RemoveKey(ModKey modKey);

    void RemoveAt(int index);

    void Set(TListing listing);

    void Set(IEnumerable<TListing> items);
}

/// <summary>
/// A container for objects with in a specific load order, that are associated with ModKeys.
/// LoadOrder does not need to be disposed for proper use, but rather can optionally be disposed of which will dispose any contained items that implement IDisposable
/// </summary>
public sealed class LoadOrder<TListing> : ILoadOrder<TListing>
    where TListing : IModKeyed
{
    private readonly List<ItemContainer> _byLoadOrder = new();
    private readonly Dictionary<ModKey, ItemContainer> _byModKey = new();

    /// <inheritdoc />
    public int Count => _byLoadOrder.Count;

    public bool DisposingItems { get; }

    /// <inheritdoc />
    public TListing this[int index] => _byLoadOrder[index].Item;

    IEnumerable<TListing> IReadOnlyCache<TListing, ModKey>.Items => ListedOrder;

    /// <inheritdoc />
    public IEnumerable<ModKey> Keys => _byModKey.Keys;

    /// <inheritdoc />
    public IEnumerable<TListing> ListedOrder => _byLoadOrder.Select(i => i.Item);

    /// <inheritdoc />
    public IEnumerable<TListing> PriorityOrder =>
        ((IEnumerable<ItemContainer>)_byLoadOrder).Reverse().Select(i => i.Item);

    IEnumerable<ModKey> ILoadOrderGetter.ListedOrder => _byLoadOrder.Select(x => x.Item.ModKey);

    IEnumerable<ModKey> ILoadOrderGetter.PriorityOrder => _byLoadOrder.Select(x => x.Item.ModKey).Reverse();

    Noggog.IKeyValue<ModKey, TListing> IReadOnlyList<Noggog.IKeyValue<ModKey, TListing>>.this[int index]
    {
        get
        {
            var cont = _byLoadOrder[index];
            return new KeyValue<ModKey, TListing>(cont.Item.ModKey, cont.Item);
        }
    }

    /// <inheritdoc />
    public TListing this[ModKey key]
    {
        get
        {
            try
            {
                return _byModKey[key].Item;
            }
            catch (KeyNotFoundException e)
            {
                throw new MissingModException(key, "Tried to retrieve a mod from the load order that did not exist", e);
            }
        }
    }

    public LoadOrder(bool disposeItems = true)
    {
        DisposingItems = disposeItems;
    }

    public LoadOrder(IEnumerable<TListing> items, bool disposeItems = true)
    {
        DisposingItems = disposeItems;
        int index = 0;
        _byLoadOrder.AddRange(items.Select(i => new ItemContainer(i, index++)));
        foreach (var item in _byLoadOrder)
        {
            try
            {
                _byModKey.Add(item.Item.ModKey, item);
            }
            catch (ArgumentException)
            {
                throw new ArgumentException($"ModKey was already present: {item.Item.ModKey}");
            }
        }
    }

    /// <summary>
    /// Attempts to retrieve an item given a ModKey
    /// </summary>
    /// <param name="key">ModKey to query for</param>
    /// <param name="value">Result reference to the item</param>
    /// <returns>True if matching key located</returns>
    public bool TryGetValue(ModKey key, [MaybeNullWhen(false)] out TListing value)
    {
        if (_byModKey.TryGetValue(key, out var container))
        {
            value = container.Item;
            return true;
        }

        value = default;
        return false;
    }

    /// <summary>
    /// Attempts to retrieve an item given a ModKey
    /// </summary>
    /// <param name="key">ModKey to query for</param>
    /// <returns>Result reference to the item, or null if not found</returns>
    public TListing? TryGetValue(ModKey key)
    {
        if (_byModKey.TryGetValue(key, out var container))
        {
            return container.Item;
        }

        return default;
    }

    /// <summary>
    /// Attempts to retrieve an item given an index
    /// </summary>
    /// <param name="index">Index to retrieve</param>
    /// <returns>TryGet result of the item</returns>
    public TListing? TryGetAtIndex(int index)
    {
        if (!_byLoadOrder.InRange(index))
        {
            return default;
        }

        return _byLoadOrder[index].Item;
    }

    /// <inheritdoc />
    public void Add(TListing item)
    {
        var index = _byLoadOrder.Count;
        var container = new ItemContainer(item, index);
        try
        {
            _byModKey.Add(item.ModKey, container);
        }
        catch (ArgumentException)
        {
            throw new ArgumentException($"ModKey was already present: {item.ModKey}");
        }

        _byLoadOrder.Add(container);
    }

    /// <inheritdoc />
    public void Add(IEnumerable<TListing> items)
    {
        foreach (var item in items)
        {
            Add(item);
        }
    }

    /// <inheritdoc />
    public void Add(TListing item, int index)
    {
        if (!_byLoadOrder.InRange(index))
        {
            throw new ArgumentException("Tried to insert at an out of range index.");
        }

        var container = new ItemContainer(item, index);
        try
        {
            _byModKey.Add(item.ModKey, container);
        }
        catch (ArgumentException)
        {
            throw new ArgumentException($"ModKey was already present: {item.ModKey}");
        }

        _byLoadOrder.Add(container);
        for (int i = index + 1; i < _byLoadOrder.Count; i++)
        {
            _byLoadOrder[i].Index += 1;
        }
    }

    /// <inheritdoc />
    public bool ContainsKey(ModKey key)
    {
        return IndexOf(key) != -1;
    }

    /// <inheritdoc />
    public int IndexOf(ModKey key)
    {
        if (!_byModKey.TryGetValue(key, out var container))
        {
            return -1;
        }

        return container.Index;
    }

    /// <inheritdoc />
    public void Clear()
    {
        Dispose();
        _byLoadOrder.Clear();
        _byModKey.Clear();
    }

    public bool RemoveKey(ModKey key)
    {
        if (!_byModKey.TryGetValue(key, out var registry)) return false;
        _byLoadOrder.RemoveAt(registry.Index);
        for (int i = registry.Index; i < _byLoadOrder.Count; i++)
        {
            _byLoadOrder[i].Index--;
        }

        _byModKey.Remove(key);
        return true;
    }

    public void RemoveAt(int index)
    {
        var item = _byLoadOrder[index];
        _byLoadOrder.RemoveAt(index);
        _byModKey.Remove(item.Item.ModKey);
        for (int i = index; i < _byLoadOrder.Count; i++)
        {
            _byLoadOrder[i].Index--;
        }

        if (DisposingItems && item.Item is IDisposable disp)
        {
            disp.Dispose();
        }
    }

    public void Set(TListing listing)
    {
        if (!_byModKey.TryGetValue(listing.ModKey, out var existing))
        {
            Add(listing);
            return;
        }

        var old = existing.Item;
        existing.Item = listing;

        if (DisposingItems && old is IDisposable disp)
        {
            disp.Dispose();
        }
    }

    public void Set(IEnumerable<TListing> items)
    {
        foreach (var item in items)
        {
            Set(item);
        }
    }

    IEnumerator<Noggog.IKeyValue<ModKey, TListing>> IEnumerable<Noggog.IKeyValue<ModKey, TListing>>.GetEnumerator()
    {
        return ListedOrder.Select(x => (Noggog.IKeyValue<ModKey, TListing>)new KeyValue<ModKey, TListing>(x.ModKey, x))
            .GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// Disposes all contained items that implement IDisposable
    /// </summary>
    public void Dispose()
    {
        if (!DisposingItems) return;
        foreach (var item in _byLoadOrder)
        {
            if (item.Item is IDisposable disp)
            {
                disp.Dispose();
            }
        }
    }

    public IEnumerator<TListing> GetEnumerator()
    {
        foreach (var item in _byLoadOrder)
        {
            yield return item.Item;
        }
    }

    private class ItemContainer
    {
        public TListing Item;
        public int Index;

        public ItemContainer(TListing item, int index)
        {
            Item = item;
            Index = index;
        }
    }
}

internal class LoadOrderGetter : ILoadOrderGetter
{
    private readonly IReadOnlyList<ModKey> _byLoadOrder;
    private readonly HashSet<ModKey> _byModKey;

    public int Count => _byLoadOrder.Count;
    public bool DisposingItems => false;

    public IEnumerable<ModKey> ListedOrder => _byLoadOrder;

    public IEnumerable<ModKey> PriorityOrder => _byLoadOrder.Reverse();

    public LoadOrderGetter(IEnumerable<ModKey> listedOrder)
    {
        _byLoadOrder = listedOrder.ToArray();
        _byModKey = listedOrder.ToHashSet();
    }

    public bool ContainsKey(ModKey key)
    {
        return _byModKey.Contains(key);
    }

    public void Dispose()
    {
    }
}