using System.Reactive;
using System.Reactive.Concurrency;
using DynamicData;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Order.DI;

public interface ISomeLiveLoadOrderProvider
{
    IObservable<IChangeSet<ILoadOrderListingGetter>> Get(out IObservable<ErrorResponse> state, IScheduler? scheduler = null);
        
    IObservable<Unit> Changed { get; }
}