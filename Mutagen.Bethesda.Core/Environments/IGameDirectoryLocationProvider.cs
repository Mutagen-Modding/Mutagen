using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Noggog;

namespace Mutagen.Bethesda.Environments
{
    public interface IGameDirectoryLocationProvider
    {
        /// <summary>
        /// Given a release, will return all the located game folders it could find
        /// </summary>
        /// <param name="release">Release to query</param>
        /// <returns>The located game folders it could find</returns>
        IEnumerable<DirectoryPath> GetGameFolders(GameRelease release);

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
    }
}