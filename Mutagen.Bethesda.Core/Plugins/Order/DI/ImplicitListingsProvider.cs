using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Plugins.Implicit.DI;

namespace Mutagen.Bethesda.Plugins.Order.DI
{
    public interface IImplicitListingsProvider : IListingsProvider
    {
    }

    public class ImplicitListingsProvider : IImplicitListingsProvider
    {
        private readonly IFileSystem _fileSystem;
        private readonly IDataDirectoryProvider _dataFolder;
        private readonly IImplicitListingModKeyProvider _listingModKeys;

        public ImplicitListingsProvider(
            IFileSystem fileSystem,
            IDataDirectoryProvider dataFolder,
            IImplicitListingModKeyProvider listingModKeys)
        {
            _fileSystem = fileSystem;
            _dataFolder = dataFolder;
            _listingModKeys = listingModKeys;
        }
        
        public IEnumerable<IModListingGetter> Get()
        {
            return _listingModKeys.Listings
                .Where(x => _fileSystem.File.Exists(Path.Combine(_dataFolder.Path, x.FileName.String)))
                .Select(x => new ModListing(x, enabled: true));
        }
    }
}