using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using DynamicData;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Order.DI;

public interface ILiveLoadOrderProvider : ISomeLiveLoadOrderProvider
{
}
    
public class LiveLoadOrderProvider : ILiveLoadOrderProvider
{
    public IPluginLiveLoadOrderProvider PluginLive { get; }
    public ICreationClubLiveLoadOrderProvider CccLive { get; }
    public ILoadOrderListingsProvider ListingsProvider { get; }
    public ILiveLoadOrderTimings Timings { get; }

    public LiveLoadOrderProvider(
        IPluginLiveLoadOrderProvider pluginLive,
        ICreationClubLiveLoadOrderProvider cccLive,
        ILoadOrderListingsProvider loadListingsProvider,
        ILiveLoadOrderTimings timings)
    {
        PluginLive = pluginLive;
        CccLive = cccLive;
        ListingsProvider = loadListingsProvider;
        Timings = timings;
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
                var ret = PluginLive.Changed
                    .Merge(CccLive.Changed)
                    .StartWith(Unit.Default);
                if (Timings.Throttle.Ticks > 0)
                {
                    ret = ret.ThrottleWithOptionalScheduler(Timings.Throttle, scheduler);
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

                                        var refreshedListings = ListingsProvider.Get().ToArray();
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
                                    Timings.RetryInterval,
                                    Timings.RetryIntervalMax,
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

    public IObservable<Unit> Changed => PluginLive.Changed
        .Merge(CccLive.Changed);

    public override string ToString()
    {
        return nameof(LiveLoadOrderProvider);
    }
}