using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using DynamicData;
using DynamicData.Kernel;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Order
{
    public interface IConstructLiveLoadOrder
    {
        IObservable<IChangeSet<IModListingGetter>> GetLiveLoadOrder(
            GameRelease game,
            DirectoryPath dataFolderPath,
            out IObservable<ErrorResponse> state,
            bool throwOnMissingMods = true);

        IObservable<IChangeSet<IModListingGetter>> GetLiveLoadOrder(
            IObservable<GameRelease> game,
            IObservable<DirectoryPath> dataFolderPath,
            out IObservable<ErrorResponse> state,
            bool throwOnMissingMods = true);

        IObservable<IChangeSet<IModListingGetter>> GetLiveLoadOrder(
            GameRelease game,
            FilePath loadOrderFilePath,
            DirectoryPath dataFolderPath,
            out IObservable<ErrorResponse> state,
            FilePath? cccLoadOrderFilePath = null,
            bool throwOnMissingMods = true);

        IObservable<IChangeSet<IModListingGetter>> GetLiveLoadOrder(
            IObservable<GameRelease> game,
            IObservable<FilePath> loadOrderFilePath,
            IObservable<DirectoryPath> dataFolderPath,
            out IObservable<ErrorResponse> state,
            IObservable<FilePath?>? cccLoadOrderFilePath = null,
            bool throwOnMissingMods = true);
    }

    public class ConstructLiveLoadOrder : IConstructLiveLoadOrder
    {
        private readonly IRetrieveListings _retrieveListings;

        public ConstructLiveLoadOrder(IRetrieveListings retrieveListings)
        {
            _retrieveListings = retrieveListings;
        }
        
        public IObservable<IChangeSet<IModListingGetter>> GetLiveLoadOrder(
            GameRelease game,
            DirectoryPath dataFolderPath,
            out IObservable<ErrorResponse> state,
            bool throwOnMissingMods = true)
        {
            return GetLiveLoadOrder(
                game: game,
                dataFolderPath: dataFolderPath,
                loadOrderFilePath: PluginListings.GetListingsPath(game),
                cccLoadOrderFilePath: CreationClubListings.GetListingsPath(game.ToCategory(), dataFolderPath),
                state: out state,
                throwOnMissingMods: throwOnMissingMods);
        }

        public IObservable<IChangeSet<IModListingGetter>> GetLiveLoadOrder(
            IObservable<GameRelease> game,
            IObservable<DirectoryPath> dataFolderPath,
            out IObservable<ErrorResponse> state,
            bool throwOnMissingMods = true)
        {
            var obs = Observable.CombineLatest(
                    game,
                    dataFolderPath,
                    (gameVal, dataFolderVal) =>
                    {
                        var lo = GetLiveLoadOrder(
                            game: gameVal,
                            dataFolderPath: dataFolderVal,
                            loadOrderFilePath: PluginListings.GetListingsPath(gameVal),
                            cccLoadOrderFilePath: CreationClubListings.GetListingsPath(gameVal.ToCategory(), dataFolderVal),
                            state: out var state,
                            throwOnMissingMods: throwOnMissingMods);
                        return (LoadOrder: lo, State: state);
                    })
                .Replay(1)
                .RefCount();
            state = obs.Select(x => x.State)
                .Switch();
            return obs.Select(x => x.LoadOrder)
                .Switch();
        }

        // ToDo
        // Add scheduler for throttle
        public IObservable<IChangeSet<IModListingGetter>> GetLiveLoadOrder(
            GameRelease game,
            FilePath loadOrderFilePath,
            DirectoryPath dataFolderPath,
            out IObservable<ErrorResponse> state,
            FilePath? cccLoadOrderFilePath = null,
            bool throwOnMissingMods = true)
        {
            var listings = Implicits.Get(game).Listings
                .Select(x => new ModListing(x, enabled: true))
                .ToArray();
            var listingsChanged = PluginListings.GetLoadOrderChanged(loadOrderFilePath);
            if (cccLoadOrderFilePath != null)
            {
                listingsChanged = listingsChanged.Merge(
                    CreationClubListings.GetLoadOrderChanged(cccFilePath: cccLoadOrderFilePath.Value, dataFolderPath));
            }
            listingsChanged = listingsChanged.PublishRefCount();
            var stateSubj = new BehaviorSubject<Exception?>(null);
            state = stateSubj
                .Distinct()
                .Select(x => x == null ? ErrorResponse.Success : ErrorResponse.Fail(x));
            return Observable.Create<IChangeSet<IModListingGetter>>((observer) =>
            {
                CompositeDisposable disp = new();
                SourceList<IModListingGetter> list = new();
                disp.Add(listingsChanged
                    .StartWith(Unit.Default)
                    .Throttle(TimeSpan.FromMilliseconds(150))
                    .Select(_ =>
                    {
                        return Observable.Return(Unit.Default)
                            .Do(_ =>
                            {
                                try
                                {
                                    // Short circuit if not subscribed anymore
                                    if (disp.IsDisposed) return;

                                    var refreshedListings = _retrieveListings.GetListings(
                                        game,
                                        loadOrderFilePath,
                                        cccLoadOrderFilePath,
                                        dataFolderPath,
                                        throwOnMissingMods).ToArray();
                                    // ToDo
                                    // Upgrade to SetTo mechanics.
                                    // SourceLists' EditDiff seems weird
                                    list.Clear();
                                    list.AddRange(refreshedListings);
                                    stateSubj.OnNext(null);
                                }
                                catch (Exception ex)
                                {
                                    // Short circuit if not subscribed anymore
                                    if (disp.IsDisposed) return;

                                    stateSubj.OnNext(ex);
                                    throw;
                                }
                            })
                            .RetryWithBackOff<Unit, Exception>((_, times) => TimeSpan.FromMilliseconds(Math.Min(times * 250, 5000)));
                    })
                    .Switch()
                    .Subscribe());
                list.Connect()
                    .Subscribe(observer);
                return disp;
            });
        }

        public IObservable<IChangeSet<IModListingGetter>> GetLiveLoadOrder(
            IObservable<GameRelease> game,
            IObservable<FilePath> loadOrderFilePath,
            IObservable<DirectoryPath> dataFolderPath,
            out IObservable<ErrorResponse> state,
            IObservable<FilePath?>? cccLoadOrderFilePath = null,
            bool throwOnMissingMods = true)
        {
            var obs = Observable.CombineLatest(
                    game,
                    dataFolderPath,
                    loadOrderFilePath,
                    cccLoadOrderFilePath ?? Observable.Return(default(FilePath?)),
                    (gameVal, dataFolderVal, loadOrderFilePathVal, cccVal) =>
                    {
                        var lo = GetLiveLoadOrder(
                            game: gameVal,
                            dataFolderPath: dataFolderVal,
                            loadOrderFilePath: loadOrderFilePathVal,
                            cccLoadOrderFilePath: cccVal,
                            state: out var state,
                            throwOnMissingMods: throwOnMissingMods);
                        return (LoadOrder: lo, State: state);
                    })
                .Replay(1)
                .RefCount();
            state = obs.Select(x => x.State)
                .Switch();
            return obs.Select(x => x.LoadOrder)
                .Switch();
        }
    }
}