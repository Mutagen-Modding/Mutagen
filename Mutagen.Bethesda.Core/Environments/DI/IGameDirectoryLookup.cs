using System.Diagnostics.CodeAnalysis;
using Noggog;

namespace Mutagen.Bethesda.Environments.DI;

public interface IGameDirectoryLookup
{
    /// <summary>
    /// Given a release, will return all the located game directories it could find
    /// </summary>
    /// <param name="release">Release to query</param>
    /// <returns>The located game directories it could find</returns>
    IEnumerable<DirectoryPath> GetAll(GameRelease release);

    /// <summary>
    /// Given a release, tries to retrieve the preferred game directory (not the data directory within)
    /// </summary>
    /// <param name="release">Release to query</param>
    /// <param name="path">The game directory, if located</param>
    /// <returns>True if located</returns>
    bool TryGet(GameRelease release, [MaybeNullWhen(false)] out DirectoryPath path);
        
    /// <summary>
    /// Given a release, tries to retrieve the preferred game directory (not the data directory within)
    /// </summary>
    /// <param name="release">Release to query</param>
    /// <exception cref="System.IO.DirectoryNotFoundException">Thrown if the game directory could not be located</exception>
    /// <returns>The game directory</returns>
    DirectoryPath Get(GameRelease release);

    /// <summary>
    /// Given a release, tries to retrieve the preferred game directory (not the data directory within)
    /// </summary>
    /// <param name="release">Release to query</param>
    /// <returns>The game directory, if located</returns>
    DirectoryPath? TryGet(GameRelease release);
}

public class GameDirectoryLookupInjection : IGameDirectoryLookup
{
    private readonly GameRelease _release;
    private readonly DirectoryPath[] _path;
    
    public GameDirectoryLookupInjection(GameRelease release, DirectoryPath? path)
    {
        _release = release;
        if (path == null)
        {
            _path = [];
        }
        else
        {
            _path = [path.Value];
        }
    }

    private void CheckRelease(GameRelease release)
    {
        if (release != _release)
        {
            throw new ArgumentException($"Accessed a game release the inejction was not intended for: {release} != {_release}", nameof(release));
        }
    }

    public IEnumerable<DirectoryPath> GetAll(GameRelease release)
    {
        CheckRelease(release);
        return _path;
    }
    
    public bool TryGet(GameRelease release, out DirectoryPath path)
    {
        if (release != _release || _path.Length == 0)
        {
            path = default;
            return false;
        }
        
        path = _path[0];
        return true;
    }
    
    public DirectoryPath Get(GameRelease release)
    {
        CheckRelease(release);
        return _path[0];
    }
    
    public DirectoryPath? TryGet(GameRelease release)
    {
        CheckRelease(release);
        if (release != _release)
        {
            return default;
        }

        if (_path.Length == 0) return default;

        return _path[0];
    }
}