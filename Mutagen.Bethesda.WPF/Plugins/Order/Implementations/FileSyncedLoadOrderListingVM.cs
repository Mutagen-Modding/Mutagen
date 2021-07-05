using System.IO;
using System.Reactive.Linq;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Order;
using Noggog;
using Noggog.WPF;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Mutagen.Bethesda.WPF.Plugins.Order.Implementations
{
    public class FileSyncedLoadOrderListingVM : ViewModel, IModListing
    {
        public ModKey ModKey { get; }
        
        [Reactive]
        public bool Enabled { get; set; }
        
        [Reactive]
        public string GhostSuffix { get; set; } = string.Empty;

        private readonly ObservableAsPropertyHelper<bool> _Exists;
        public bool Exists => _Exists.Value;

        private readonly ObservableAsPropertyHelper<bool> _Ghosted;
        public bool Ghosted => _Ghosted.Value;

        public FileSyncedLoadOrderListingVM(
            IDataDirectoryProvider dataDirectoryContext,
            IModListingGetter listing)
        {
            ModKey = listing.ModKey;
            Enabled = listing.Enabled;
            GhostSuffix = listing.GhostSuffix;
            
            var path = Path.Combine(dataDirectoryContext.Path, listing.ModKey.FileName);
            var exists = File.Exists(path);
            _Exists = Observable.Defer(() =>
                    Noggog.ObservableExt.WatchFile(path)
                        .Select(_ =>
                        {
                            var ret = File.Exists(path);
                            return ret;
                        }))
                .ToGuiProperty(this, nameof(Exists), initialValue: exists);
            _Ghosted = this.WhenAnyValue(x => x.GhostSuffix)
                .Select(x => !x.IsNullOrWhitespace())
                .ToGuiProperty(this, nameof(Ghosted));
        }

        public override string ToString()
        {
            return IModListingExt.ToString(this);
        }
    }
}