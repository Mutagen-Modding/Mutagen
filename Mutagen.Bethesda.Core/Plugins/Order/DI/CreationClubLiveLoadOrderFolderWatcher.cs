using System;
using System.IO;
using System.IO.Abstractions;
using System.Reactive.Linq;
using DynamicData;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Environments.DI;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Order.DI
{
    public interface ICreationClubLiveLoadOrderFolderWatcher
    {
        IObservable<IChangeSet<ModKey, ModKey>> Get();
    }

    public class CreationClubLiveLoadOrderFolderWatcher : ICreationClubLiveLoadOrderFolderWatcher
    {
        private readonly IFileSystem _fileSystem;
        private readonly IDataDirectoryContext _dataDirectory;

        public CreationClubLiveLoadOrderFolderWatcher(
            IFileSystem fileSystem,
            IDataDirectoryContext dataDirectory)
        {
            _fileSystem = fileSystem;
            _dataDirectory = dataDirectory;
        }
        
        public IObservable<IChangeSet<ModKey, ModKey>> Get()
        {
            if (!_fileSystem.Directory.Exists(_dataDirectory.Path))
            {
                return Observable.Empty<IChangeSet<ModKey, ModKey>>();
            }
            return ObservableExt
                .WatchFolderContents(_dataDirectory.Path, fileSystem: _fileSystem)
                .Select(x => x)
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
                .ChangeKey(x => x)
                .Catch<IChangeSet<ModKey, ModKey>, DirectoryNotFoundException>(_ => Observable.Empty<IChangeSet<ModKey, ModKey>>());
        }
    }
}