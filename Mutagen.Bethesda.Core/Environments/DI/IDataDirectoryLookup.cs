using System.Diagnostics.CodeAnalysis;
using Noggog;

namespace Mutagen.Bethesda.Environments.DI;

public interface IDataDirectoryLookup
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
    /// <param name="installMode">Install mode to query</param>
    /// <param name="path">The data directory, if located</param>
    /// <returns>True if located</returns>
    bool TryGet(GameRelease release, GameInstallMode installMode, [MaybeNullWhen(false)] out DirectoryPath path);
        
    /// <summary>
    /// Given a release, tries to retrieve the preferred data directory
    /// </summary>
    /// <param name="release">Release to query</param>
    /// <param name="installMode">Install mode to query</param>
    /// <exception cref="System.IO.DirectoryNotFoundException">Thrown if the data directory could not be located</exception>
    /// <returns>The data directory</returns>
    DirectoryPath Get(GameRelease release, GameInstallMode installMode);

    /// <summary>
    /// Given a release, tries to retrieve the preferred data directory
    /// </summary>
    /// <param name="release">Release to query</param>
    /// <param name="path">The data directory, if located</param>
    /// <returns>True if located</returns>
    [Obsolete("Use variant with GameInstallMode instead")]
    bool TryGet(GameRelease release, [MaybeNullWhen(false)] out DirectoryPath path);
        
    /// <summary>
    /// Given a release, tries to retrieve the preferred data directory
    /// </summary>
    /// <param name="release">Release to query</param>
    /// <exception cref="System.IO.DirectoryNotFoundException">Thrown if the data directory could not be located</exception>
    /// <returns>The data directory</returns>
    [Obsolete("Use variant with GameInstallMode instead")]
    DirectoryPath Get(GameRelease release);
}