using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using Noggog;
using Path = System.IO.Path;

namespace Mutagen.Bethesda.Plugins.Order
{
    public interface ICreationClubListingsProvider
    {
        IEnumerable<IModListingGetter> GetListings(GameCategory category, DirectoryPath dataPath);

        IEnumerable<IModListingGetter> GetListingsFromPath(
            FilePath cccFilePath,
            DirectoryPath dataPath);
    }

    public class CreationClubListingsProvider : ICreationClubListingsProvider
    {
        private readonly IFileSystem _fileSystem;
        private readonly ICreationClubPathProvider _pathProvider;

        public CreationClubListingsProvider(
            IFileSystem fileSystem,
            ICreationClubPathProvider pathProvider)
        {
            _fileSystem = fileSystem;
            _pathProvider = pathProvider;
        }
        
        public IEnumerable<IModListingGetter> GetListings(GameCategory category, DirectoryPath dataPath)
        {
            var path = _pathProvider.GetListingsPath(category, dataPath);
            if (path == null || !path.Value.Exists) return Enumerable.Empty<IModListingGetter>();
            return GetListingsFromPath(
                path.Value,
                dataPath);
        }

        public IEnumerable<IModListingGetter> GetListingsFromPath(
            FilePath cccFilePath,
            DirectoryPath dataPath)
        {
            return _fileSystem.File.ReadAllLines(cccFilePath.Path)
                .Select(x => ModKey.FromNameAndExtension(x))
                .Where(x => _fileSystem.File.Exists(Path.Combine(dataPath.Path, x.FileName)))
                .Select(x => new ModListing(x, enabled: true))
                .ToList();
        }
    }
}