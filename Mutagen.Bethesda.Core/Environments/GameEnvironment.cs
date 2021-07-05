using System;
using System.IO;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Order.DI;
using Mutagen.Bethesda.Plugins.Records;
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
            var dataPath = Path.Combine(gameFolder.Path, "Data");

            var loadOrder = Mutagen.Bethesda.Plugins.Order.LoadOrder.Import<TModGetter>(
                dataPath,
                Mutagen.Bethesda.Plugins.Order.LoadOrder.GetListings(release, dataPath),
                release);

            if (!PluginListings.TryGetListingsFile(release, out var loadOrderFilePath))
            {
                throw new FileNotFoundException("Could not locate plugins file");
            }

            var ccPath = CreationClubListings.GetListingsPath(release.ToCategory(), dataPath);

            return new GameEnvironmentState<TModSetter, TModGetter>(
                dataFolderPath: dataPath,
                loadOrderFilePath: loadOrderFilePath.Path,
                creationClubListingsFilePath: ccPath,
                loadOrder: loadOrder,
                linkCache: loadOrder.ToImmutableLinkCache<TModSetter, TModGetter>(linkCachePrefs),
                dispose: true);
        }

        DirectoryPath IDataDirectoryProvider.Path => DataFolderPath;

        FilePath IPluginListingsPathProvider.Path => LoadOrderFilePath;

        FilePath? ICreationClubListingsPathProvider.Path => CreationClubListingsFilePath;
    }
}
