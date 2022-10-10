using System.IO;
using System.Reactive.Linq;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Order;
using Noggog;
using Noggog.WPF;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Mutagen.Bethesda.WPF.Plugins.Order.Implementations;

public class FileSyncedLoadOrderListingVM : ViewModel, IModListing
{
    public ModKey ModKey { get; }
        
    [Reactive]
    public bool Enabled { get; set; }
        
    [Reactive]
    public string GhostSuffix { get; set; }

    private readonly ObservableAsPropertyHelper<bool> _existsOnDisk;
    public bool ExistsOnDisk => _existsOnDisk.Value;

    private readonly ObservableAsPropertyHelper<bool> _ghosted;
    public bool Ghosted => _ghosted.Value;

    private readonly ObservableAsPropertyHelper<string> _fileName;
    public string FileName => _fileName.Value;

    public FileSyncedLoadOrderListingVM(
        IDataDirectoryProvider dataDirectoryContext,
        ILoadOrderListingGetter listing)
    {
        ModKey = listing.ModKey;
        Enabled = listing.Enabled;
        GhostSuffix = listing.GhostSuffix;
            
        var path = Path.Combine(dataDirectoryContext.Path, listing.ModKey.FileName);
        var exists = File.Exists(path);
        _existsOnDisk = Observable.Defer(() =>
                Noggog.ObservableExt.WatchFile(path)
                    .Select(_ =>
                    {
                        var ret = File.Exists(path);
                        return ret;
                    }))
            .ToGuiProperty(this, nameof(ExistsOnDisk), initialValue: exists);
        _ghosted = this.WhenAnyValue(x => x.GhostSuffix)
            .Select(x => !x.IsNullOrWhitespace())
            .ToGuiProperty(this, nameof(Ghosted));
        _fileName = this.WhenAnyValue( x => x.GhostSuffix)
            .Skip(1)
            .Select(x => OrderUtility.GetListingFilename(ModKey, x))
            .ToGuiProperty(this, nameof(FileName), OrderUtility.GetListingFilename(ModKey, GhostSuffix));
    }

    public override string ToString()
    {
        return IModListingGetter.ToString(this);
    }
}