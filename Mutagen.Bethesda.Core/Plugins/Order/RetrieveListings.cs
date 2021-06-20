using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Exceptions;
using Noggog;
using Path = System.IO.Path;
using FileNotFoundException = System.IO.FileNotFoundException;

namespace Mutagen.Bethesda.Plugins.Order
{
    public interface IRetrieveListings
    {
        /// <summary>
        /// Returns a load order listing from the usual sources
        /// </summary>
        /// <param name="game">Game type</param>
        /// <param name="dataPath">Path to game's data folder</param>
        /// <param name="throwOnMissingMods">Whether to throw and exception if mods are missing</param>
        /// <returns>Enumerable of modkeys representing a load order</returns>
        /// <exception cref="ArgumentException">Line in plugin file is unexpected</exception>
        /// <exception cref="FileNotFoundException">If plugin file not located</exception>
        /// <exception cref="MissingModException">If throwOnMissingMods true and file is missing</exception>
        IEnumerable<IModListingGetter> GetListings(
            GameRelease game,
            DirectoryPath dataPath,
            bool throwOnMissingMods = true);

        IEnumerable<IModListingGetter> GetListings(
            GameRelease game,
            FilePath pluginsFilePath,
            FilePath? creationClubFilePath,
            DirectoryPath dataPath,
            bool throwOnMissingMods = true);
    }

    public class RetrieveListings : IRetrieveListings
    {
        private readonly IFileSystem _fileSystem;
        private readonly IOrderListings _orderListings;
        private readonly IPluginListingsProvider _listingsRetriever;
        private readonly ICreationClubPathProvider _cccPathProvider;
        private readonly ICreationClubListingsProvider _cccListingsProvider;

        public RetrieveListings(
            IFileSystem fileSystem,
            IOrderListings orderListings,
            IPluginListingsProvider listingsRetriever, 
            ICreationClubPathProvider cccPathProvider,
            ICreationClubListingsProvider cccListingsProvider)
        {
            _fileSystem = fileSystem;
            _orderListings = orderListings;
            _listingsRetriever = listingsRetriever;
            _cccPathProvider = cccPathProvider;
            _cccListingsProvider = cccListingsProvider;
        }
        
        /// <inheritdoc />
        public IEnumerable<IModListingGetter> GetListings(
            GameRelease game,
            DirectoryPath dataPath,
            bool throwOnMissingMods = true)
        {
            if (!_listingsRetriever.TryGetListingsFile(game, out var path))
            {
                throw new FileNotFoundException("Could not locate plugins file");
            }
            return GetListings(
                game: game,
                pluginsFilePath: path,
                creationClubFilePath: _cccPathProvider.GetListingsPath(game.ToCategory(), dataPath),
                dataPath: dataPath,
                throwOnMissingMods: throwOnMissingMods);
        }

        /// <inheritdoc />
        public IEnumerable<IModListingGetter> GetListings(
            GameRelease game,
            FilePath pluginsFilePath,
            FilePath? creationClubFilePath,
            DirectoryPath dataPath,
            bool throwOnMissingMods = true)
        {
            var listings = Enumerable.Empty<IModListingGetter>();
            if (pluginsFilePath.Exists)
            {
                listings = _listingsRetriever.ListingsFromPath(pluginsFilePath, game, dataPath, throwOnMissingMods);
            }
            var implicitListings = Implicits.Get(game).Listings
                .Where(x => _fileSystem.File.Exists(Path.Combine(dataPath.Path, x.FileName.String)))
                .Select(x => new ModListing(x, enabled: true));
            var ccListings = Enumerable.Empty<IModListingGetter>();
            if (creationClubFilePath != null && _fileSystem.File.Exists(creationClubFilePath.Value))
            {
                ccListings = _cccListingsProvider.GetListingsFromPath(creationClubFilePath.Value, dataPath);
            }

            return _orderListings.Order(
                implicitListings: implicitListings,
                pluginsListings: listings.Except(implicitListings),
                creationClubListings: ccListings,
                selector: x => x.ModKey);
        }
    }
}