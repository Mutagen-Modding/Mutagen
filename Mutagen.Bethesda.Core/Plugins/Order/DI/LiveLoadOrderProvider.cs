using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using DynamicData;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Order.DI
{
    public interface ILiveLoadOrderProvider : ISomeLiveLoadOrderProvider
    {
    }
    
    public class LiveLoadOrderProvider : ILiveLoadOrderProvider
    {
        private readonly IPluginLiveLoadOrderProvider _pluginLive;
        private readonly ICreationClubLiveLoadOrderProvider _cccLive;
        private readonly ILoadOrderListingsProvider _loadOrderListingsProvider;
        private readonly ILiveLoadOrderTimings _timings;

        public LiveLoadOrderProvider(
            IPluginLiveLoadOrderProvider pluginLive,
            ICreationClubLiveLoadOrderProvider cccLive,
            ILoadOrderListingsProvider loadOrderListingsProvider,
            ILiveLoadOrderTimings timings)
        {
            _pluginLive = pluginLive;
            _cccLive = cccLive;
            _loadOrderListingsProvider = loadOrderListingsProvider;
            _timings = timings;
        }
    
        public IObservable<IChangeSet<IModListingGetter>> Get(out IObservable<ErrorResponse> state, IScheduler? scheduler = null)
        {
            var stateSubj = new BehaviorSubject<Exception?>(null);
            state = stateSubj
                .DistinctUntilChanged()
                .Select(x => x == null ? ErrorResponse.Success : ErrorResponse.Fail(x));
            return Observable.Create<IChangeSet<IModListingGetter>>((observer) =>
                {
                    CompositeDisposable disp = new();
                    SourceList<IModListingGetter> list = new();
                    var ret = _pluginLive.Changed
                        .Merge(_cccLive.Changed)
                        .StartWith(Unit.Default);
                    if (_timings.Throttle.Ticks > 0)
                    {
                        ret = ret.ThrottleWithOptionalScheduler(_timings.Throttle, scheduler);
                    }
                    disp.Add(
                        ret.Select(_ =>
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
                                .RetryWithRampingBackoff<Unit, Exception>(
                                    _timings.RetryInterval,
                                    _timings.RetryIntervalMax,
                                    scheduler);
                        })
                        .Switch()
                        .Subscribe());
                    list.Connect()
                        .Subscribe(observer);
                    return disp;
                })
                .ObserveOnIfApplicable(scheduler);
        }

        public IObservable<Unit> Changed => _pluginLive.Changed
                .Merge(_cccLive.Changed);

        public override string ToString()
        {
            return nameof(LiveLoadOrderProvider);
        }
    }
}