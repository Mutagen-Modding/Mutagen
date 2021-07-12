using System;
using System.IO;
using System.IO.Abstractions;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Implicit.DI;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Order.DI;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Records.DI;
using Noggog;

namespace Mutagen.Bethesda.Environments
{
    public class GameEnvironment
    {
        public static readonly GameEnvironment Typical = new GameEnvironment();

        private GameEnvironment()
        {
        }
    }

    /// <summary>
    /// A class housing commonly used utilities when interacting with a game environment
    /// </summary>
    public class GameEnvironmentState<TModSetter, TModGetter> : IDisposable, IDataDirectoryProvider, IPluginListingsPathProvider, ICreationClubListingsPathProvider
        where TModSetter : class, IContextMod<TModSetter, TModGetter>, TModGetter
        where TModGetter : class, IContextGetterMod<TModSetter, TModGetter>
    {
        private readonly bool _dispose;
        private DirectoryPath _path;
        private FilePath _path1;
        private FilePath? _path2;

        public DirectoryPath DataFolderPath { get; }

        public FilePath LoadOrderFilePath { get; }

        public FilePath? CreationClubListingsFilePath { get; }

        /// <summary>
        /// Load Order object containing all the mods present in the environment.
        /// </summary>
        public ILoadOrder<IModListing<TModGetter>> LoadOrder { get; }

        /// <summary>
        /// Convenience Link Cache to use created from the provided Load Order object
        /// </summary>
        public ILinkCache<TModSetter, TModGetter> LinkCache { get; }

        public GameEnvironmentState(
            DirectoryPath dataFolderPath,
            FilePath loadOrderFilePath,
            FilePath? creationClubListingsFilePath,
            ILoadOrder<IModListing<TModGetter>> loadOrder,
            ILinkCache<TModSetter, TModGetter> linkCache,
            bool dispose = true)
        {
            LoadOrderFilePath = loadOrderFilePath;
            DataFolderPath = dataFolderPath;
            CreationClubListingsFilePath = creationClubListingsFilePath;
            LoadOrder = loadOrder;
            LinkCache = linkCache;
            _dispose = dispose;
        }

        public void Dispose()
        {
            if (!_dispose) return;
            LoadOrder.Dispose();
            LinkCache.Dispose();
        }

        public static GameEnvironmentState<TModSetter, TModGetter> Construct(
            GameRelease release,
            DirectoryPath gameFolder,
            LinkCachePreferences? linkCachePrefs = null)
        {
            var dataDirectory = new DataDirectoryInjection(Path.Combine(gameFolder, "Data"));
            var gameReleaseInjection = new GameReleaseInjection(release);
            var pluginRawListingsReader = new PluginRawListingsReader(
                IFileSystemExt.DefaultFilesystem,
                new PluginListingsParser(
                    new ModListingParser(
                        new HasEnabledMarkersProvider(
                            gameReleaseInjection))));
            var category = new GameCategoryContext(gameReleaseInjection);
            var pluginListingsPathProvider = new PluginListingsPathProvider(gameReleaseInjection);
            var creationClubListingsPathProvider = new CreationClubListingsPathProvider(
                category,
                new CreationClubEnabledProvider(
                    category),
                new GameDirectoryInjection(gameFolder));
            return new GameEnvironmentProvider<TModSetter, TModGetter>(
                    new LoadOrderImporter<TModGetter>(
                        IFileSystemExt.DefaultFilesystem,
                        dataDirectory,
                        new LoadOrderListingsProvider(
                            new OrderListings(),
                            new ImplicitListingsProvider(
                                IFileSystemExt.DefaultFilesystem,
                                dataDirectory,
                                new ImplicitListingModKeyProvider(
                                    gameReleaseInjection)),
                            new PluginListingsProvider(
                                gameReleaseInjection,
                                new TimestampedPluginListingsProvider(
                                    new TimestampAligner(IFileSystemExt.DefaultFilesystem),
                                    new TimestampedPluginListingsPreferences() {ThrowOnMissingMods = false},
                                    pluginRawListingsReader,
                                    dataDirectory,
                                    pluginListingsPathProvider),
                                new EnabledPluginListingsProvider(
                                    pluginRawListingsReader,
                                    pluginListingsPathProvider)),
                            new CreationClubListingsProvider(
                                IFileSystemExt.DefaultFilesystem,
                                dataDirectory,
                                creationClubListingsPathProvider,
                                new CreationClubRawListingsReader(
                                    IFileSystemExt.DefaultFilesystem,
                                    dataDirectory))),
                        new ModImporter<TModGetter>(
                            IFileSystemExt.DefaultFilesystem,
                            gameReleaseInjection)),
                    dataDirectory,
                    pluginListingsPathProvider,
                    creationClubListingsPathProvider)
                .Construct();
        }

        DirectoryPath IDataDirectoryProvider.Path => DataFolderPath;

        FilePath IPluginListingsPathProvider.Path => LoadOrderFilePath;

        FilePath? ICreationClubListingsPathProvider.Path => CreationClubListingsFilePath;
    }
}
