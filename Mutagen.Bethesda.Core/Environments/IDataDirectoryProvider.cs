using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Noggog;

namespace Mutagen.Bethesda.Environments
{
    public interface IDataDirectoryProvider
    {
        /// <summary>
        /// Given a release, will return all the located data directories it could find
        /// </summary>
        /// <param name="release">Release to query</param>
        /// <returns>The located data directories it could find</returns>
        IEnumerable<DirectoryPath> GetAll(GameRelease release);

        /// <summary>
        /// Given a release, tries to retrieve the preferred data directory
        /// </summary>
        /// <param name="release">Release to query</param>
        /// <param name="path">The data directory, if located</param>
        /// <returns>True if located</returns>
        bool TryGet(GameRelease release, [MaybeNullWhen(false)] out DirectoryPath path);
        
        /// <summary>
        /// Given a release, tries to retrieve the preferred data directory
        /// </summary>
        /// <param name="release">Release to query</param>
        /// <exception cref="System.IO.DirectoryNotFoundException">Thrown if the data directory could not be located</exception>
        /// <returns>The data directory</returns>
        DirectoryPath Get(GameRelease release);
    }
}