using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Noggog;

namespace Mutagen.Bethesda.Installs
{
    /// <summary>
    /// Service that locates game installations
    /// </summary>
    public interface IGameLocator
    {
        /// <summary>
        /// Given a release, will return all the located game folders it could find
        /// </summary>
        /// <param name="release">Release to query</param>
        /// <returns>The located game folders it could find</returns>
        IEnumerable<DirectoryPath> GetGameFolders(GameRelease release);

        /// <summary>
        /// Given a release, tries to retrieve the game location from the windows directory
        /// </summary>
        /// <param name="release">Release to query</param>
        /// <param name="path">The game folder, if located</param>
        /// <returns>True if located</returns>
        bool TryGetGameFolderFromRegistry(GameRelease release,
            [MaybeNullWhen(false)] out DirectoryPath path);

        /// <summary>
        /// Given a release, tries to retrieve the preferred game directory (not the data folder within)
        /// </summary>
        /// <param name="release">Release to query</param>
        /// <param name="path">The game folder, if located</param>
        /// <returns>True if located</returns>
        bool TryGetGameFolder(GameRelease release, [MaybeNullWhen(false)] out DirectoryPath path);
        
        /// <summary>
        /// Given a release, tries to retrieve the preferred game directory (not the data folder within)
        /// </summary>
        /// <param name="release">Release to query</param>
        /// <exception cref="System.IO.DirectoryNotFoundException">Thrown if the game folder could not be located</exception>
        /// <returns>The game folder</returns>
        DirectoryPath GetGameFolder(GameRelease release);
        
        /// <summary>
        /// Given a release, tries to retrieve the preferred data directory
        /// </summary>
        /// <param name="release">Release to query</param>
        /// <param name="path">The data folder, if located</param>
        /// <returns>True if located</returns>
        bool TryGetDataFolder(GameRelease release, [MaybeNullWhen(false)] out DirectoryPath path);
        
        /// <summary>
        /// Given a release, tries to retrieve the preferred data directory
        /// </summary>
        /// <param name="release">Release to query</param>
        /// <exception cref="System.IO.DirectoryNotFoundException">Thrown if the data folder could not be located</exception>
        /// <returns>The data folder</returns>
        DirectoryPath GetDataFolder(GameRelease release);
    }

    public class GameLocator : IGameLocator
    {
        /// <inheritdoc />
        public IEnumerable<DirectoryPath> GetGameFolders(GameRelease release) => GameLocations.GetGameFolders(release);

        /// <inheritdoc />
        public bool TryGetGameFolderFromRegistry(GameRelease release,
            [MaybeNullWhen(false)] out DirectoryPath path)
        {
            return GameLocations.TryGetGameFolderFromRegistry(release, out path);
        }
        
        /// <inheritdoc />
        public bool TryGetGameFolder(GameRelease release, [MaybeNullWhen(false)] out DirectoryPath path)
        {
            return GameLocations.TryGetGameFolder(release, out path);
        }

        /// <inheritdoc />
        public DirectoryPath GetGameFolder(GameRelease release)
        {
            return GameLocations.GetGameFolder(release);
        }

        /// <inheritdoc />
        public bool TryGetDataFolder(GameRelease release, [MaybeNullWhen(false)] out DirectoryPath path)
        {
            return GameLocations.TryGetDataFolder(release, out path);
        }

        /// <inheritdoc />
        public DirectoryPath GetDataFolder(GameRelease release)
        {
            return GameLocations.GetDataFolder(release);
        }
    }
}