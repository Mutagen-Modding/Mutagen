using System;
using System.IO.Abstractions;
using System.Reactive;
using System.Reactive.Linq;
using DynamicData;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Order.DI
{
    public interface IPluginLiveLoadOrderProvider : ISomeLiveLoadOrderProvider
    {
    }

    public class PluginLiveLoadOrderProvider : IPluginLiveLoadOrderProvider
    {
        private readonly IFileSystem _fileSystem;
        private readonly IPluginListingsProvider _listingsProvider;
        private readonly IPluginPathContext _pluginFilePath;

        public PluginLiveLoadOrderProvider(
            IFileSystem fileSystem,
            IPluginListingsProvider listingsProvider,
            IPluginPathContext pluginFilePath)
        {
            _fileSystem = fileSystem;
            _listingsProvider = listingsProvider;
            _pluginFilePath = pluginFilePath;
        }
        
        public IObservable<IChangeSet<IModListingGetter>> Get(out IObservable<ErrorResponse> state)
        {
            var results = ObservableExt.WatchFile(_pluginFilePath.Path, fileWatcherFactory: _fileSystem.FileSystemWatcher)
                .StartWith(Unit.Default)
                .Select(_ =>
                {
                    try
                    {
                        return GetResponse<IObservable<IChangeSet<IModListingGetter>>>.Succeed(
                            _listingsProvider.Get()
                                .AsObservableChangeSet());
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

        public IObservable<Unit> Changed => ObservableExt
            .WatchFile(_pluginFilePath.Path, fileWatcherFactory: _fileSystem.FileSystemWatcher)
            .StartWith(Unit.Default);
    }
}