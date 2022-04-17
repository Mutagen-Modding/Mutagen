using System;
using System.IO;
using System.IO.Abstractions;
using System.Reactive.Linq;
using DynamicData;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Environments.DI;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Order.DI;

public interface ICreationClubLiveLoadOrderFolderWatcher
{
    IObservable<IChangeSet<ModKey, ModKey>> Get();
}

public class CreationClubLiveLoadOrderFolderWatcher : ICreationClubLiveLoadOrderFolderWatcher
{
    private readonly IFileSystem _fileSystem;
    public IDataDirectoryProvider DataDirectory { get; }

    public CreationClubLiveLoadOrderFolderWatcher(
        IFileSystem fileSystem,
        IDataDirectoryProvider dataDirectory)
    {
        _fileSystem = fileSystem;
        DataDirectory = dataDirectory;
    }
        
    public IObservable<IChangeSet<ModKey, ModKey>> Get()
    {
        if (!_fileSystem.Directory.Exists(DataDirectory.Path))
        {
            return Observable.Empty<IChangeSet<ModKey, ModKey>>();
        }
        return ObservableExt
            .WatchFolderContents(DataDirectory.Path, fileSystem: _fileSystem)
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