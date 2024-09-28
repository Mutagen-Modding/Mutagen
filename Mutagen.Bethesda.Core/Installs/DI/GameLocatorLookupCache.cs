using System.Diagnostics.CodeAnalysis;
using Mutagen.Bethesda.Environments.DI;
using Noggog;

namespace Mutagen.Bethesda.Installs.DI;

public class GameLocatorLookupCache : IGameDirectoryLookup, IDataDirectoryLookup
{
    internal static readonly GameLocatorLookupCache Instance = new();
    
    private readonly Dictionary<GameRelease, DirectoryPath?> _gameDirCache = new();
    private readonly Dictionary<GameRelease, IReadOnlyList<DirectoryPath>> _gameDirsCache = new();
    
    private IEnumerable<DirectoryPath> GetAllGameDirectories(GameRelease release)
    {
        lock (_gameDirsCache)
        {
            if (!_gameDirsCache.TryGetValue(release, out var dirs))
            {
                var ret = GameLocator.Instance.GetAllGameDirectories(release).ToArray();
                _gameDirsCache[release] = ret;
                dirs = ret;
            }
            return dirs;
        }
    }

    private bool TryGetGameDirectory(GameRelease release, [MaybeNullWhen(false)] out DirectoryPath path)
    {
        lock (_gameDirCache)
        {
            if (!_gameDirCache.TryGetValue(release, out var cachePath))
            {
                cachePath = GetAllGameDirectories(release).Select<DirectoryPath, DirectoryPath?>(x => x).FirstOrDefault();
                _gameDirCache[release] = cachePath;
            }

            if (cachePath != null)
            {
                path = cachePath.Value;
                return true;
            }

            path = default;
            return false;
        }
    }

    private DirectoryPath GetGameDirectory(GameRelease release)
    {
        if (TryGetGameDirectory(release, out var path))
        {
            return path;
        }
        throw new DirectoryNotFoundException($"Game folder for {release} cannot be found automatically");
    }

    private bool TryGetDataDirectory(GameRelease release, [MaybeNullWhen(false)] out DirectoryPath path)
    {
        if (TryGetGameDirectory(release, out path))
        {
            path = Path.Combine(path, "Data");
            return true;
        }
        path = default;
        return false;
    }

    public DirectoryPath GetDataDirectory(GameRelease release)
    {
        if (TryGetDataDirectory(release, out var path))
        {
            return path;
        }
        throw new DirectoryNotFoundException($"Data folder for {release} cannot be found automatically");
    }
    
    #region Interface Implementations

    IEnumerable<DirectoryPath> IDataDirectoryLookup.GetAll(GameRelease release)
    {
        return GetAllGameDirectories(release)
            .Select(path => new DirectoryPath(Path.Combine(path, "Data")));
    }

    bool IDataDirectoryLookup.TryGet(GameRelease release, out DirectoryPath path)
    {
        return TryGetDataDirectory(release, out path);
    }

    DirectoryPath IDataDirectoryLookup.Get(GameRelease release)
    {
        return GetDataDirectory(release);
    }

    IEnumerable<DirectoryPath> IGameDirectoryLookup.GetAll(GameRelease release)
    {
        return GetAllGameDirectories(release);
    }

    bool IGameDirectoryLookup.TryGet(GameRelease release, out DirectoryPath path)
    {
        return TryGetGameDirectory(release, out path);
    }

    DirectoryPath? IGameDirectoryLookup.TryGet(GameRelease release)
    {
        if (TryGetGameDirectory(release, out var path))
        {
            return path;
        }

        return null;
    }

    DirectoryPath IGameDirectoryLookup.Get(GameRelease release)
    {
        return GetGameDirectory(release);
    }

    #endregion
}