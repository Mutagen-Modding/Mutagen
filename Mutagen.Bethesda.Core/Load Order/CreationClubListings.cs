using Noggog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Mutagen.Bethesda
{
    public static class CreationClubListings
    {
        public static FilePath GetListingsPath(GameCategory category, DirectoryPath dataPath)
        {
            switch (category)
            {
                case GameCategory.Oblivion:
                    throw new ArgumentException();
                case GameCategory.Skyrim:
                    return Path.Combine(dataPath.Path, $"{category}.ccc");
                default:
                    throw new NotImplementedException();
            }
        }

        public static IEnumerable<LoadOrderListing> GetListings(GameCategory category, DirectoryPath dataPath)
        {
            var path = GetListingsPath(category, dataPath);
            if (!path.Exists) return Enumerable.Empty<LoadOrderListing>();
            return ListingsFromPath(
                path,
                dataPath);
        }

        public static IEnumerable<LoadOrderListing> ListingsFromPath(
            FilePath cccFilePath,
            DirectoryPath dataPath)
        {
            return File.ReadAllLines(cccFilePath.Path)
                .Select(x => ModKey.FromNameAndExtension(x))
                .Where(x => File.Exists(Path.Combine(dataPath.Path, x.FileName)))
                .Select(x => new LoadOrderListing(x, enabled: true))
                .ToList();
        }
    }
}
