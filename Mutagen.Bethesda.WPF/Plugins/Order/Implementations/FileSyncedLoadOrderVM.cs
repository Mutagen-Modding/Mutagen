using Noggog;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using DynamicData;
using System.Reactive.Linq;
using Noggog.WPF;
using DynamicData.Binding;

namespace Mutagen.Bethesda.WPF.Plugins.Order.Implementations
{
    public class FileSyncedLoadOrderVM : ALoadOrderVM<ModListingVM>
    {
        [Reactive]
        public string LoadOrderFilePath { get; set; } = string.Empty;

        [Reactive]
        public string DataFolderPath { get; set; } = string.Empty;

        private readonly ObservableAsPropertyHelper<ErrorResponse> _State;
        public ErrorResponse State => _State.Value;

        [Reactive]
        public string CreationClubFilePath { get; set; } = string.Empty;

        [Reactive]
        public GameRelease GameRelease { get; set; }

        public override IObservableCollection<ModListingVM> LoadOrder { get; }

        public FileSyncedLoadOrderVM()
        {
            var lo = Mutagen.Bethesda.Plugins.Order.LoadOrder.GetLiveLoadOrder(
                this.WhenAnyValue(x => x.GameRelease),
                this.WhenAnyValue(x => x.LoadOrderFilePath)
                    .Select(x => new FilePath(x)),
                this.WhenAnyValue(x => x.DataFolderPath)
                    .Select(x => new DirectoryPath(x)),
                out var state,
                this.WhenAnyValue(x => x.CreationClubFilePath)
                    .Select(x => x.IsNullOrWhitespace() ? default(FilePath?) : new FilePath(x)));

            _State = state
                .ToGuiProperty(this, nameof(State), ErrorResponse.Fail("Uninitialized"));

            LoadOrder = lo
                .Transform(x => new ModListingVM(x, this.DataFolderPath))
                .ToObservableCollection(this);
        }
    }
}
