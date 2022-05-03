using System.IO.Abstractions;
using Mutagen.Bethesda.Plugins.Implicit.DI;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Order.DI;

public interface ILoadOrderWriter
{
    void Write(
        FilePath path,
        IEnumerable<ILoadOrderListingGetter> loadOrder,
        bool removeImplicitMods = true);
}

public class LoadOrderWriter : ILoadOrderWriter
{
    private readonly IFileSystem _fileSystem;
    private readonly IHasEnabledMarkersProvider _hasEnabledMarkersProvider;
    private readonly IImplicitListingModKeyProvider _implicitListingsProvider;

    public LoadOrderWriter(
        IFileSystem fileSystem,
        IHasEnabledMarkersProvider hasEnabledMarkersProvider,
        IImplicitListingModKeyProvider implicitListingsProvider)
    {
        _fileSystem = fileSystem;
        _hasEnabledMarkersProvider = hasEnabledMarkersProvider;
        _implicitListingsProvider = implicitListingsProvider;
    }
        
    /// <inheritdoc />
    public void Write(
        FilePath path,
        IEnumerable<ILoadOrderListingGetter> loadOrder,
        bool removeImplicitMods = true)
    {
        bool markers = _hasEnabledMarkersProvider.HasEnabledMarkers;
        var loadOrderList = loadOrder.ToList();
        if (removeImplicitMods)
        {
            foreach (var implicitMod in _implicitListingsProvider.Listings)
            {
                if (loadOrderList.Count > 0
                    && loadOrderList[0].ModKey == implicitMod
                    && loadOrderList[0].Enabled)
                {
                    loadOrderList.RemoveAt(0);
                }
            }
        }
        _fileSystem.File.WriteAllLines(path,
            loadOrderList.Where(x =>
                {
                    return (markers || x.Enabled);
                })
                .Select(x =>
                {
                    if (x.Enabled && markers)
                    {
                        return $"*{x.ModKey.FileName}";
                    }
                    else
                    {
                        return x.ModKey.FileName.String;
                    }
                }));
    }
}