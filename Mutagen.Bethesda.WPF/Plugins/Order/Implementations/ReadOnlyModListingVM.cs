using Mutagen.Bethesda.Plugins.Order;
using Noggog.WPF;
using ReactiveUI;
using System.IO;
using System.Reactive.Linq;
using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.WPF.Plugins.Order;

public class ReadOnlyModListingVM : ViewModel, IModListingGetter
{
    private readonly ILoadOrderListingGetter _listing;

    /// <inheritdoc />
    public ModKey ModKey => _listing.ModKey;

    /// <inheritdoc />
    public bool Enabled => _listing.Enabled;
        
    /// <inheritdoc />
    public bool Ghosted => _listing.Ghosted;

    /// <inheritdoc />
    public string GhostSuffix => _listing.GhostSuffix;

    /// <inheritdoc />
    public string FileName => _listing.FileName;

    private readonly ObservableAsPropertyHelper<bool> _existsOnDisk;
    public bool ExistsOnDisk => _existsOnDisk.Value;
        
    public ReadOnlyModListingVM(ILoadOrderListingGetter listing, string dataFolder)
    {
        _listing = listing;
        var path = Path.Combine(dataFolder, listing.FileName);
        var exists = File.Exists(path);
        _existsOnDisk = Observable.Defer(() =>
                Noggog.ObservableExt.WatchFile(path)
                    .Select(_ => File.Exists(path)))
            .ToGuiProperty(this, nameof(ExistsOnDisk), initialValue: exists);
    }

    public override string ToString()
    {
        return IModListing.ToString(this);
    }
}