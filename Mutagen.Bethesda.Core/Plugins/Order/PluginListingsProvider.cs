using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Noggog;

namespace Mutagen.Bethesda.Plugins.Order
{
    public interface IPluginListingsProvider
    {
        /// <summary>
        /// Parses the typical plugins file to retrieve all ModKeys in expected plugin file format,
        /// Will order mods by timestamps if applicable
        /// Will add implicit base mods if applicable
        /// </summary>
        /// <param name="game">Game type</param>
        /// <param name="dataPath">Path to game's data folder</param>
        /// <param name="throwOnMissingMods">Whether to throw and exception if mods are missing</param>
        /// <returns>Enumerable of ModKeys representing a load order</returns>
        /// <exception cref="InvalidDataException">Line in plugin file is unexpected</exception>
        IEnumerable<IModListingGetter> ListingsFromPath(
            GameRelease game,
            DirectoryPath dataPath,
            bool throwOnMissingMods = true);

        /// <summary>
        /// Parses a file to retrieve all ModKeys in expected plugin file format,
        /// Will order mods by timestamps if applicable
        /// Will add implicit base mods if applicable
        /// </summary>
        /// <param name="game">Game type</param>
        /// <param name="pluginTextPath">Path of plugin list</param>
        /// <param name="dataPath">Path to game's data folder</param>
        /// <param name="throwOnMissingMods">Whether to throw and exception if mods are missing</param>
        /// <returns>Enumerable of ModKeys representing a load order</returns>
        /// <exception cref="InvalidDataException">Line in plugin file is unexpected</exception>
        IEnumerable<IModListingGetter> ListingsFromPath(
            FilePath pluginTextPath,
            GameRelease game,
            DirectoryPath dataPath,
            bool throwOnMissingMods = true);

        IEnumerable<IModListingGetter> RawListingsFromPath(
            FilePath pluginTextPath,
            GameRelease game);
    }

    public class PluginListingsProvider : IPluginListingsProvider
    {
        private readonly IFileSystem _FileSystem;
        private readonly IPluginListingsParserFactory _parserFactory;
        private readonly IPluginPathProvider _pathProvider;
        private readonly ITimestampAligner _TimestampAligner;

        public PluginListingsProvider(
            IFileSystem fileSystem,
            IPluginListingsParserFactory parserFactory,
            IPluginPathProvider pathProvider,
            ITimestampAligner timestampAligner)
        {
            _FileSystem = fileSystem;
            _parserFactory = parserFactory;
            _pathProvider = pathProvider;
            _TimestampAligner = timestampAligner;
        }

        /// <inheritdoc />
        public IEnumerable<IModListingGetter> ListingsFromPath(
            GameRelease game,
            DirectoryPath dataPath,
            bool throwOnMissingMods = true)
        {
            return ListingsFromPath(
                pluginTextPath: _pathProvider.Get(game),
                game: game,
                dataPath: dataPath,
                throwOnMissingMods: throwOnMissingMods);
        }

        /// <inheritdoc />
        public IEnumerable<IModListingGetter> ListingsFromPath(
            FilePath pluginTextPath,
            GameRelease game,
            DirectoryPath dataPath,
            bool throwOnMissingMods = true)
        {
            var mods = RawListingsFromPath(pluginTextPath, game);
            if (_TimestampAligner.NeedsTimestampAlignment(game.ToCategory()))
            {
                return _TimestampAligner.AlignToTimestamps(mods, dataPath, throwOnMissingMods: throwOnMissingMods);
            }
            else
            {
                return mods;
            }
        }

        /// <inheritdoc />
        public IEnumerable<IModListingGetter> RawListingsFromPath(
            FilePath pluginTextPath,
            GameRelease game)
        {
            if (!_FileSystem.File.Exists(pluginTextPath))
            {
                throw new FileNotFoundException("Could not locate plugins file");
            }
            using var stream = _FileSystem.FileStream.Create(pluginTextPath.Path, FileMode.Open, FileAccess.Read, FileShare.Read);
            return _parserFactory.Create(game).Parse(stream).ToList();
        }
    }
}