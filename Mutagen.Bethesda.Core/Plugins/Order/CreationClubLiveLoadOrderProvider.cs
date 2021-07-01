using System;
using System.IO.Abstractions;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using DynamicData;
using Noggog;
using Path = System.IO.Path;

namespace Mutagen.Bethesda.Plugins.Order
{
    public class CreationClubLiveLoadOrderProvider
    {
        private readonly IFileSystem _fileSystem;

        public CreationClubLiveLoadOrderProvider(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }
    
        public IObservable<IChangeSet<IModListingGetter>> GetLiveLoadOrder(
            FilePath cccFilePath,
            DirectoryPath dataFolderPath,
            out IObservable<ErrorResponse> state,
            bool orderListings = true)
        {
            var raw = ObservableExt.WatchFile(cccFilePath.Path, fileWatcherFactory: _fileSystem.FileSystemWatcher)
                .StartWith(Unit.Default)
                .Select(_ =>
                {
                    try
                    {
                        return GetResponse<IObservable<IChangeSet<ModKey>>>.Succeed(
                            _fileSystem.File.ReadAllLines(cccFilePath.Path)
                                .Select(x => ModKey.FromNameAndExtension(x))
                                .AsObservableChangeSet());
                    }
                    catch (Exception ex)
                    {
                        return GetResponse<IObservable<IChangeSet<ModKey>>>.Fail(ex);
                    }
                })
                .Replay(1)
                .RefCount();
            state = raw
                .Select(r => (ErrorResponse)r);
            var ret = ObservableListEx.And(
                raw
                .Select(r =>
                {
                    return r.Value ?? Observable.Empty<IChangeSet<ModKey>>();
                })
                .Switch(),
                ObservableExt.WatchFolderContents(dataFolderPath.Path, fileSystem: _fileSystem)
                    .Transform(x =>
                    {
                        if (ModKey.TryFromNameAndExtension(Path.GetFileName(x), out var modKey))
                        {
                            return TryGet<ModKey>.Succeed(modKey);
                        }
                        return TryGet<ModKey>.Failure;
                    })
                    .Filter(x => x.Succeeded)
                    .Transform(x => x.Value)
                    .RemoveKey())
                .Transform<ModKey, IModListingGetter>(x => new ModListing(x, true));
            if (orderListings)
            {
                ret = ret.OrderListings();
            }
            return ret;
        }

        public IObservable<Unit> GetLoadOrderChanged(
            FilePath cccFilePath,
            DirectoryPath dataFolderPath)
        {
            return GetLiveLoadOrder(cccFilePath, dataFolderPath, out _, orderListings: false)
                .QueryWhenChanged(q => Unit.Default);
        }
    }
}