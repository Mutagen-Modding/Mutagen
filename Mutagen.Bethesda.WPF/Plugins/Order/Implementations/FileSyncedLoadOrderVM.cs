using System;
using System.Collections.Generic;
using System.Linq;
using Noggog;
using ReactiveUI;
using DynamicData;
using System.Reactive.Linq;
using Noggog.WPF;
using DynamicData.Binding;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Order.DI;

namespace Mutagen.Bethesda.WPF.Plugins.Order.Implementations;

public class FileSyncedLoadOrderVM : ALoadOrderVM<FileSyncedLoadOrderListingVM>
{
    private readonly ObservableAsPropertyHelper<ErrorResponse> _state;
    public ErrorResponse State => _state.Value;

    public override IObservableCollection<FileSyncedLoadOrderListingVM> LoadOrder { get; }

    public FileSyncedLoadOrderVM(
        IPluginLiveLoadOrderProvider liveLoadOrderProvider,
        ILoadOrderWriter writer,
        IPluginListingsPathProvider pluginPathContext,
        IDataDirectoryProvider dataDirectoryContext)
    {
        var loadOrder = liveLoadOrderProvider.Get(out var state)
            .Transform(x => new FileSyncedLoadOrderListingVM(dataDirectoryContext, x))
            .RefCount();

        _state = state
            .ToGuiProperty(this, nameof(State), ErrorResponse.Fail("Uninitialized"));
            
        LoadOrder = loadOrder
            .ToObservableCollection(this);

        // When listings change, resave to file
        Observable.Merge(
                loadOrder
                    .AutoRefresh(x => x.Enabled)
                    .Transform(x => x.Enabled, transformOnRefresh: true)
                    .BufferInitial(TimeSpan.FromMilliseconds(500), RxApp.MainThreadScheduler)
                    .QueryWhenChanged(x => x)
                    .Unit(),
                loadOrder
                    .AutoRefresh(x => x.GhostSuffix)
                    .Transform(x => x.GhostSuffix ?? string.Empty, transformOnRefresh: true)
                    .BufferInitial(TimeSpan.FromMilliseconds(500), RxApp.MainThreadScheduler)
                    .QueryWhenChanged(x => x)
                    .Unit())
            .Throttle(TimeSpan.FromMilliseconds(500), RxApp.MainThreadScheduler)
            .Select(x => LoadOrder.Select(x => new ModListing(x.ModKey, x.Enabled, x.ExistsOnDisk, x.GhostSuffix)).ToArray())
            .DistinctUntilChanged(new SequenceEqualityComparer())
            .Subscribe(x =>
            {
                writer.Write(
                    pluginPathContext.Path,
                    LoadOrder);
            });
    }

    class SequenceEqualityComparer : IEqualityComparer<ModListing[]>
    {
        public bool Equals(ModListing[]? x, ModListing[]? y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x == null || y == null) return false;
            return x.SequenceEqual(y);
        }

        public int GetHashCode(ModListing[] obj)
        {
            throw new NotImplementedException();
        }
    }
}