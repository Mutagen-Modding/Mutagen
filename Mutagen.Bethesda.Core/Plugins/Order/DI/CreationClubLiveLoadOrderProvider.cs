using System;
using System.Reactive;
using DynamicData;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Order.DI
{
    public interface ICreationClubLiveLoadOrderProvider : ISomeLiveLoadOrderProvider
    {
    }

    public class CreationClubLiveLoadOrderProvider : ICreationClubLiveLoadOrderProvider
    {
        private readonly ICreationClubLiveListingsFileReader _fileReader;
        private readonly ICreationClubLiveLoadOrderFolderWatcher _folderWatcher;

        public CreationClubLiveLoadOrderProvider(
            ICreationClubLiveListingsFileReader fileReader,
            ICreationClubLiveLoadOrderFolderWatcher folderWatcher)
        {
            _fileReader = fileReader;
            _folderWatcher = folderWatcher;
        }
    
        public IObservable<IChangeSet<IModListingGetter>> Get(
            out IObservable<ErrorResponse> state)
        {
            return ObservableCacheEx.And(
                _fileReader.Get(out state)
                    .AddKey(x => x.ModKey),
                _folderWatcher.Get()
                    .Transform<IModListingGetter, ModKey, ModKey>(x => new ModListing(x, true)))
                .RemoveKey();
        }

        public IObservable<Unit> Changed => Get(out _).Unit();
    }
}