using System;
using System.IO.Abstractions;
using System.Reactive;
using System.Reactive.Linq;
using DynamicData;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Order
{
    public interface IPluginLiveLoadOrderProvider
    {
        IObservable<IChangeSet<IModListingGetter>> GetLiveLoadOrder(
            GameRelease game,
            FilePath loadOrderFilePath,
            DirectoryPath dataFolderPath,
            out IObservable<ErrorResponse> state,
            bool throwOnMissingMods = true,
            bool orderListings = true);

        IObservable<Unit> GetLoadOrderChanged(FilePath loadOrderFilePath);
        IObservable<Unit> GetLoadOrderChanged(GameRelease game);
    }

    public class PluginLiveLoadOrderProvider : IPluginLiveLoadOrderProvider
    {
        private readonly IFileSystem _fileSystem;
        private readonly IPluginListingsProvider _listingsProvider;
        private readonly IPluginPathProvider _pathProvider;

        public PluginLiveLoadOrderProvider(
            IFileSystem fileSystem,
            IPluginListingsProvider listingsProvider,
            IPluginPathProvider pathProvider)
        {
            _fileSystem = fileSystem;
            _listingsProvider = listingsProvider;
            _pathProvider = pathProvider;
        }
        
        public IObservable<IChangeSet<IModListingGetter>> GetLiveLoadOrder(
            GameRelease game,
            FilePath loadOrderFilePath,
            DirectoryPath dataFolderPath,
            out IObservable<ErrorResponse> state,
            bool throwOnMissingMods = true,
            bool orderListings = true)
        {
            var results = ObservableExt.WatchFile(loadOrderFilePath.Path, fileWatcherFactory: _fileSystem.FileSystemWatcher)
                .StartWith(Unit.Default)
                .Select(_ =>
                {
                    try
                    {
                        var lo = _listingsProvider.ListingsFromPath(loadOrderFilePath, game, dataFolderPath, throwOnMissingMods: throwOnMissingMods);
                        if (orderListings)
                        {
                            lo = lo.OrderListings();
                        }
                        return GetResponse<IObservable<IChangeSet<IModListingGetter>>>.Succeed(lo.AsObservableChangeSet());
                    }
                    catch (Exception ex)
                    {
                        return GetResponse<IObservable<IChangeSet<IModListingGetter>>>.Fail(ex);
                    }
                })
                .Replay(1)
                .RefCount();
            state = results
                .Select(r => (ErrorResponse)r);
            return results
                .Select(r =>
                {
                    return r.Value ?? Observable.Empty<IChangeSet<IModListingGetter>>();
                })
                .Switch();
        }

        public IObservable<Unit> GetLoadOrderChanged(FilePath loadOrderFilePath)
        {
            return ObservableExt.WatchFile(loadOrderFilePath.Path);
        }

        public IObservable<Unit> GetLoadOrderChanged(GameRelease game) => GetLoadOrderChanged(_pathProvider.Get(game));
    }
}