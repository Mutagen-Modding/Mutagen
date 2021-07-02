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
    public interface ILiveLoadOrderProvider : ISomeLiveLoadOrderProvider
    {
    }
    
    public class LiveLoadOrderProvider : ILiveLoadOrderProvider
    {
        private readonly IPluginLiveLoadOrderProvider _pluginLive;
        private readonly ICreationClubLiveLoadOrderProvider _cccLive;
        private readonly ILoadOrderListingsProvider _loadOrderListingsProvider;

        public LiveLoadOrderProvider(
            IPluginLiveLoadOrderProvider pluginLive,
            ICreationClubLiveLoadOrderProvider cccLive,
            ILoadOrderListingsProvider loadOrderListingsProvider)
        {
            _pluginLive = pluginLive;
            _cccLive = cccLive;
            _loadOrderListingsProvider = loadOrderListingsProvider;
        }
    
        // ToDo
        // Add scheduler for throttle
        public IObservable<IChangeSet<IModListingGetter>> Get(out IObservable<ErrorResponse> state, bool orderListings = true)
        {
            var stateSubj = new BehaviorSubject<Exception?>(null);
            state = stateSubj
                .Distinct()
                .Select(x => x == null ? ErrorResponse.Success : ErrorResponse.Fail(x));
            return Observable.Create<IChangeSet<IModListingGetter>>((observer) =>
            {
                CompositeDisposable disp = new();
                SourceList<IModListingGetter> list = new();
                disp.Add(_pluginLive.Changed
                    .Merge(_cccLive.Changed)
                    .PublishRefCount()
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
    
                                    var refreshedListings = _loadOrderListingsProvider.Get().ToArray();
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

        public IObservable<Unit> Changed => _pluginLive.Changed
                .Merge(_cccLive.Changed);
    }
}