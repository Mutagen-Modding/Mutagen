using System;
using System.Reactive;
using DynamicData;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Order.DI
{
    public interface ISomeLiveLoadOrderProvider
    {
        IObservable<IChangeSet<IModListingGetter>> Get(out IObservable<ErrorResponse> state);
        
        IObservable<Unit> Changed { get; }
    }
}