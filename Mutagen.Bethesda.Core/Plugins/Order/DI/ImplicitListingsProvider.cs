using System.IO.Abstractions;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins.Implicit.DI;

namespace Mutagen.Bethesda.Plugins.Order.DI;

public interface IImplicitListingsProvider : IListingsProvider
{
    new IEnumerable<IModListingGetter> Get();
}

public sealed class ImplicitListingsProvider : IImplicitListingsProvider
{
    private readonly IFileSystem _fileSystem;
    public IDataDirectoryProvider DataFolder { get; }
    public IImplicitListingModKeyProvider ListingModKeys { get; }

    public ImplicitListingsProvider(
        IFileSystem fileSystem,
        IDataDirectoryProvider dataFolder,
        IImplicitListingModKeyProvider listingModKeys)
    {
        _fileSystem = fileSystem;
        DataFolder = dataFolder;
        ListingModKeys = listingModKeys;
    }
        
    public IEnumerable<IModListingGetter> Get()
    {
        return ListingModKeys.Listings
            .Where(x => _fileSystem.File.Exists(Path.Combine(DataFolder.Path, x.FileName.String)))
            .Select(x => new ModListing(x, enabled: true, existsOnDisk: true));
    }

    IEnumerable<ILoadOrderListingGetter> IListingsProvider.Get() => Get();
}