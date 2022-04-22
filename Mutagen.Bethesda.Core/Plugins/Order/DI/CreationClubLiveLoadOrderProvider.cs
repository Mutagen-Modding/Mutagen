using System;
using System.Reactive;
using System.Reactive.Concurrency;
using DynamicData;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Order.DI;

public interface ICreationClubLiveLoadOrderProvider : ISomeLiveLoadOrderProvider
{
}

public class CreationClubLiveLoadOrderProvider : ICreationClubLiveLoadOrderProvider
{
    public ICreationClubLiveListingsFileReader FileReader { get; }
    public ICreationClubLiveLoadOrderFolderWatcher FolderWatcher { get; }

    public CreationClubLiveLoadOrderProvider(
        ICreationClubLiveListingsFileReader fileReader,
        ICreationClubLiveLoadOrderFolderWatcher folderWatcher)
    {
        FileReader = fileReader;
        FolderWatcher = folderWatcher;
    }
    
    public IObservable<IChangeSet<IModListingGetter>> Get(
        out IObservable<ErrorResponse> state,
        IScheduler? scheduler = null)
    {
        return InternalGet(out state)
            .ObserveOnIfApplicable(scheduler);
    }

    public IObservable<Unit> Changed => InternalGet(out _).Unit();

    private IObservable<IChangeSet<IModListingGetter>> InternalGet(out IObservable<ErrorResponse> state)
    {
        return ObservableCacheEx.And(
                FileReader.Get(out state)
                    .AddKey(x => x.ModKey),
                FolderWatcher.Get()
                    .Transform<IModListingGetter, ModKey, ModKey>(x => new ModListing(x, true)))
            .RemoveKey();
    }
}