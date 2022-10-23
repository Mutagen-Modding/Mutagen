using Noggog;

namespace Mutagen.Bethesda.Installs.DI;

public interface IGameInstallLookup
{
    GameInstallMode GetInstallMode(GameRelease release);
    
    /// <summary>
    /// Given a release, will return all the located game directories it could find
    /// </summary>
    /// <param name="release">Release to query</param>
    /// <returns>The located game directories it could find</returns>
    IEnumerable<DirectoryPath> GetAll(GameRelease release);
}
