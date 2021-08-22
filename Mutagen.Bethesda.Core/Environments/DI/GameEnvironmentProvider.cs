using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Order.DI;
using Mutagen.Bethesda.Plugins.Records;

namespace Mutagen.Bethesda.Environments.DI
{
    public interface IGameEnvironmentProvider<TModSetter, TModGetter> 
        where TModSetter : class, IContextMod<TModSetter, TModGetter>, TModGetter 
        where TModGetter : class, IContextGetterMod<TModSetter, TModGetter>
    {
        GameEnvironmentState<TModSetter, TModGetter> Construct(LinkCachePreferences? linkCachePrefs = null);
    }

    public class GameEnvironmentProvider<TModSetter, TModGetter> : IGameEnvironmentProvider<TModSetter, TModGetter>
        where TModSetter : class, IContextMod<TModSetter, TModGetter>, TModGetter
        where TModGetter : class, IContextGetterMod<TModSetter, TModGetter>
    {
        private readonly IGameReleaseContext _gameReleaseContext;
        private readonly ILoadOrderImporter<TModGetter> _loadOrderImporter;
        private readonly IDataDirectoryProvider _dataDirectoryProvider;
        private readonly IPluginListingsPathProvider _pluginListingsPathProvider;
        private readonly ICreationClubListingsPathProvider _cccPath;

        public GameEnvironmentProvider(
            IGameReleaseContext gameReleaseContext,
            ILoadOrderImporter<TModGetter> loadOrderImporter,
            IDataDirectoryProvider dataDirectoryProvider,
            IPluginListingsPathProvider pluginListingsPathProvider,
            ICreationClubListingsPathProvider cccPath)
        {
            _gameReleaseContext = gameReleaseContext;
            _loadOrderImporter = loadOrderImporter;
            _dataDirectoryProvider = dataDirectoryProvider;
            _pluginListingsPathProvider = pluginListingsPathProvider;
            _cccPath = cccPath;
        }

        public GameEnvironmentState<TModSetter, TModGetter> Construct(LinkCachePreferences? linkCachePrefs = null)        
        {
            var loadOrder = _loadOrderImporter.Import();

            return new GameEnvironmentState<TModSetter, TModGetter>(
                gameRelease: _gameReleaseContext.Release,
                dataFolderPath: _dataDirectoryProvider.Path,
                loadOrderFilePath: _pluginListingsPathProvider.Path,
                creationClubListingsFilePath: _cccPath.Path,
                loadOrder: loadOrder,
                linkCache: loadOrder.ToImmutableLinkCache<TModSetter, TModGetter>(linkCachePrefs),
                dispose: true);
        }
    }
}