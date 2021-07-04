using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using Noggog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using DynamicData;
using System.Reactive.Linq;
using Noggog.WPF;
using DynamicData.Binding;
using Mutagen.Bethesda.Plugins.Order;

namespace Mutagen.Bethesda.WPF.Plugins.Order.Implementations
{
    public class FileSyncedLoadOrderVM : ALoadOrderVM<FileSyncedLoadOrderListingVM>
    {
        public string LoadOrderFilePath { get; } = string.Empty;

        [Reactive]
        public string DataFolderPath { get; set; } = string.Empty;

        private readonly ObservableAsPropertyHelper<ErrorResponse> _State;
        public ErrorResponse State => _State.Value;

        [Reactive]
        public string CreationClubFilePath { get; set; } = string.Empty;

        [Reactive]
        public GameRelease GameRelease { get; set; }

        public override IObservableCollection<FileSyncedLoadOrderListingVM> LoadOrder { get; }

        public FileSyncedLoadOrderVM(FilePath loadOrderFilePath)
        {
            LoadOrderFilePath = loadOrderFilePath;
            
            var lo = Mutagen.Bethesda.Plugins.Order.LoadOrder.GetLiveLoadOrder(
                this.WhenAnyValue(x => x.GameRelease),
                Observable.Return(loadOrderFilePath),
                this.WhenAnyValue(x => x.DataFolderPath)
                    .Select(x => new DirectoryPath(x)),
                out var state,
                this.WhenAnyValue(x => x.CreationClubFilePath)
                    .Select(x => x.IsNullOrWhitespace() ? default(FilePath?) : new FilePath(x)));

            _State = state
                .ToGuiProperty(this, nameof(State), ErrorResponse.Fail("Uninitialized"));
                
            var loadOrder = lo
                .Transform(x => new FileSyncedLoadOrderListingVM(this, x))
                .PublishRefCount();

            LoadOrder = loadOrder
                .ToObservableCollection(this);

            // When listings change, resave to file
            Observable.Merge(
                    loadOrder
                        .AutoRefresh(x => x.Enabled)
                        .Transform(x => x.Enabled, transformOnRefresh: true)
                        .BufferInitialNoDeferred(TimeSpan.FromMilliseconds(500), RxApp.MainThreadScheduler)
                        .QueryWhenChanged(x => x)
                        .Unit(),
                    loadOrder
                        .AutoRefresh(x => x.GhostSuffix)
                        .Transform(x => x.GhostSuffix ?? string.Empty, transformOnRefresh: true)
                        .BufferInitialNoDeferred(TimeSpan.FromMilliseconds(500), RxApp.MainThreadScheduler)
                        .QueryWhenChanged(x => x)
                        .Unit())
                .Throttle(TimeSpan.FromMilliseconds(500), RxApp.MainThreadScheduler)
                .Select(x => LoadOrder.Select(x => new ModListing(x.ModKey, x.Enabled, x.GhostSuffix)).ToArray())
                .DistinctUntilChanged(new SequenceEqualityComparer())
                .Subscribe(x =>
                {
                    Mutagen.Bethesda.Plugins.Order.LoadOrder.Write(
                        LoadOrderFilePath,
                        GameRelease,
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
}
