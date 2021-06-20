using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Noggog;

namespace Mutagen.Bethesda.Environments
{
    public interface IDataDirectoryLocationProvider
    {
        /// <summary>
        /// Given a release, will return all the located data folders it could find
        /// </summary>
        /// <param name="release">Release to query</param>
        /// <returns>The located data folders it could find</returns>
        IEnumerable<DirectoryPath> GetDataFolders(GameRelease release);

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
}