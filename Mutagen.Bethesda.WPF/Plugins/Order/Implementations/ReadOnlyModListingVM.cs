using Mutagen.Bethesda.Plugins.Order;
using Noggog.WPF;
using ReactiveUI;
using System.IO;
using System.Reactive.Linq;
using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.WPF.Plugins.Order;

public class ReadOnlyModListingVM : ViewModel, IModListingGetter
{
    private readonly IModListingGetter _listing;

    public ModKey ModKey => _listing.ModKey;

    public bool Enabled => _listing.Enabled;
        
    public bool Ghosted => _listing.Ghosted;

    public string GhostSuffix => _listing.GhostSuffix;

    private readonly ObservableAsPropertyHelper<bool> _Exists;
    public bool Exists => _Exists.Value;
        
    public ReadOnlyModListingVM(IModListingGetter listing, string dataFolder)
    {
        _listing = listing;
        var path = Path.Combine(dataFolder, listing.ModKey.FileName);
        var exists = File.Exists(path);
        _Exists = Observable.Defer(() =>
                Noggog.ObservableExt.WatchFile(path)
                    .Select(_ =>
                    {
                        var ret = File.Exists(path);
                        return ret;
                    }))
            .ToGuiProperty(this, nameof(Exists), initialValue: exists);
    }

    public override string ToString()
    {
        return IModListing.ToString(this);
    }
}