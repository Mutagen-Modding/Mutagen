using System;
using System.Reactive;
using System.Reactive.Concurrency;
using DynamicData;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Order
{
    public interface ISomeLiveLoadOrderProvider
    {
        IObservable<IChangeSet<IModListingGetter>> Get(
            out IObservable<ErrorResponse> state,
            bool orderListings = true);
        
        IObservable<Unit> Changed { get; }
    }
}