using System.Diagnostics.CodeAnalysis;
using GameFinder.StoreHandlers.GOG;
using GameFinder.StoreHandlers.Steam;
using Microsoft.Win32;
using Mutagen.Bethesda.Environments.DI;
using Noggog;

namespace Mutagen.Bethesda.Installs.DI;

public sealed class GameLocator : IGameDirectoryLookup, IDataDirectoryLookup, IGameInstallLookup
{
    private readonly Lazy<SteamHandler> _steamHandler;
    private readonly Lazy<GOGHandler> _gogHandler;
        
    public GameLocator()
    {
        _steamHandler = new Lazy<SteamHandler>(() => new SteamHandler());
        _gogHandler = new(() => new GOGHandler());
    }
    
    private IEnumerable<DirectoryPath> GetAllGameDirectories(GameRelease release)
    {
        return GetSteamFolders(release)
            .And(GetGogFolders(release))
            .Distinct();
    }
    
    private bool TryGetSteam(GameRelease release, out DirectoryPath directoryPath)
    {
        directoryPath = GetSteamFolders(release).FirstOrDefault();
        return directoryPath != default;
    }

    private bool TryGetGog(GameRelease release, out DirectoryPath directoryPath)
    {
        directoryPath = GetGogFolders(release).FirstOrDefault();
        return directoryPath != default;
    }

    private IEnumerable<DirectoryPath> GetSteamFolders(GameRelease release)
    {
        if (TryGetGameDirectoryFromRegistry(release, out var regisPath)
            && regisPath.Exists)
        {
            yield return regisPath;
        }

        foreach (var game in _steamHandler.Value.FindAllGames()
                     .Where(x => x.Error.IsNullOrWhitespace())
                     .Select(x => x.Game)
                     .NotNull()
                     .Where(x => x.AppId == Games[release].SteamId))
        {
            yield return game.Path;
        }
    }

    private IEnumerable<DirectoryPath> GetGogFolders(GameRelease release)
    {
        foreach (var game in _gogHandler.Value.FindAllGames()
                     .Where(x => x.Error.IsNullOrWhitespace())
                     .Select(x => x.Game)
                     .NotNull()
                     .Where(x => x.Id == Games[release].GogId))
        {
            yield return game.Path;
        }
    }

    private IEnumerable<GameInstallMode> InternalGetInstallModes(GameRelease release)
    {
        if (TryGetSteam(release, out _))
        {
            yield return GameInstallMode.Steam;
        }
        if (TryGetGog(release, out _))
        {
            yield return GameInstallMode.Gog;
        }
    }
    
    [Obsolete("Will be made private at some point")]
    public bool TryGetGameDirectoryFromRegistry(GameRelease release,
        [MaybeNullWhen(false)] out DirectoryPath path)
    {
        try
        {
            var gameRegistration = Games[release];
                
            if (gameRegistration.RegistryPath == null || gameRegistration.RegistryKey == null)
            {
                path = default;
                return false;
            }
                
            using var regKey = Registry.LocalMachine.OpenSubKey(gameRegistration.RegistryPath);
            if (regKey == null)
            {
                path = default;
                return false;
            }

            var regRes = RegistryHelper.GetStringValueFromRegistry(regKey, gameRegistration.RegistryKey);
            if (regRes.Failed)
            {
                path = default;
                return false;
            }
            
            path = regRes.Value;
            return true;
        }
        catch (Exception)
        {
            path = default;
            return false;
        }
    }
    
    private bool TryGetGameDirectory(GameRelease release, GameInstallMode installMode, [MaybeNullWhen(false)] out DirectoryPath path)
    {
        switch (installMode)
        {
            case GameInstallMode.Steam:
                foreach (var folder in GetSteamFolders(release))
                {
                    path = folder;
                    return true;
                }
                break;
            case GameInstallMode.Gog:
                foreach (var folder in GetGogFolders(release))
                {
                    path = folder;
                    return true;
                }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(installMode), installMode, null);
        }
        
        path = default;
        return false;
    }

    private DirectoryPath GetGameDirectory(GameRelease release, GameInstallMode installMode)
    {
        if (TryGetGameDirectory(release, installMode, out var path))
        {
            return path;
        }
        throw new DirectoryNotFoundException($"Game folder for {installMode} {release} cannot be found automatically");
    }

    private bool TryGetDataDirectory(GameRelease release, GameInstallMode installMode, [MaybeNullWhen(false)] out DirectoryPath path)
    {
        if (TryGetGameDirectory(release, installMode, out path))
        {
            path = Path.Combine(path, "Data");
            return true;
        }
        path = default;
        return false;
    }

    private DirectoryPath GetDataDirectory(GameRelease release, GameInstallMode installMode)
    {
        if (TryGetDataDirectory(release, installMode, out var path))
        {
            return path;
        }
        throw new DirectoryNotFoundException($"Data folder for {installMode} {release} cannot be found automatically");
    }

    public IEnumerable<GameInstallMode> GetInstallModes(GameRelease release)
    {
        return InternalGetInstallModes(release);
    }

    internal static IReadOnlyDictionary<GameRelease, GameMetaData> Games { get; }

    static GameLocator()
    {
        var games = new Dictionary<GameRelease, GameMetaData>
        {
            {
                GameRelease.Oblivion, new GameMetaData(
                    GameRelease.Oblivion,
                    NexusName: "oblivion",
                    NexusGameId: 101,
                    SteamId: 22330,
                    GogId: 1458058109,
                    RegistryPath: @"SOFTWARE\WOW6432Node\Bethesda Softworks\Oblivion",
                    RegistryKey: @"installed path",
                    RequiredFiles: new string[]
                    {
                        "oblivion.exe"
                    })
            },
            {
                GameRelease.SkyrimLE, new GameMetaData(
                    GameRelease.SkyrimLE,
                    NexusName: "skyrim",
                    NexusGameId: 110,
                    SteamId: 72850,
                    GogId: null,
                    RegistryPath: @"SOFTWARE\WOW6432Node\Bethesda Softworks\Skyrim",
                    RegistryKey: @"installed path",
                    RequiredFiles: new string[]
                    {
                        "tesv.exe"
                    })
            },
            {
                GameRelease.SkyrimSE, new GameMetaData(
                    GameRelease.SkyrimSE,
                    NexusName: "skyrimspecialedition",
                    NexusGameId: 1704,
                    SteamId: 489830,
                    GogId: 1711230643,
                    RegistryPath: @"SOFTWARE\WOW6432Node\Bethesda Softworks\Skyrim Special Edition",
                    RegistryKey: @"installed path",
                    RequiredFiles: new string[]
                    {
                        "SkyrimSE.exe"
                    })
            },
            {
                GameRelease.Fallout4, new GameMetaData(
                    GameRelease.Fallout4,
                    NexusName: "fallout4",
                    NexusGameId: 1151,
                    SteamId: 377160,
                    GogId: null,
                    RegistryPath: @"SOFTWARE\WOW6432Node\Bethesda Softworks\Fallout4",
                    RegistryKey: @"installed path",
                    RequiredFiles: new string[]
                    {
                        "Fallout4.exe"
                    })
            },
            {
                GameRelease.SkyrimVR, new GameMetaData(
                    GameRelease.SkyrimVR,
                    NexusName: "skyrimspecialedition",
                    NexusGameId: 1704,
                    SteamId: 611670,
                    GogId: null,
                    RegistryPath: @"SOFTWARE\WOW6432Node\Bethesda Softworks\Skyrim VR",
                    RegistryKey: @"installed path",
                    RequiredFiles: new string[]
                    {
                        "SkyrimVR.exe"
                    })
            }
        };
        games[GameRelease.EnderalLE] = games[GameRelease.SkyrimLE] with
        {
            RegistryPath = null,
            RegistryKey = null,
        };
        games[GameRelease.EnderalSE] = games[GameRelease.SkyrimSE] with
        {
            RegistryPath = @"SOFTWARE\WOW6432Node\SureAI\Enderal SE",
        };
        Games = games;
    }
        
    #region Interface Implementations

    IEnumerable<DirectoryPath> IDataDirectoryLookup.GetAll(GameRelease release)
    {
        return GetAllGameDirectories(release)
            .Select(path => new DirectoryPath(Path.Combine(path, "Data")));
    }

    bool IDataDirectoryLookup.TryGet(GameRelease release, out DirectoryPath path)
    {
        return TryGetDataDirectory(release, GameInstallMode.Steam, out path);
    }

    bool IDataDirectoryLookup.TryGet(GameRelease release, GameInstallMode installMode, out DirectoryPath path)
    {
        return TryGetDataDirectory(release, installMode, out path);
    }

    DirectoryPath? IGameDirectoryLookup.TryGet(GameRelease release, GameInstallMode installMode)
    {
        if (TryGetDataDirectory(release, installMode, out var dir))
        {
            return dir;
        }

        return null;
    }

    DirectoryPath IGameDirectoryLookup.Get(GameRelease release, GameInstallMode installMode)
    {
        return GetGameDirectory(release, installMode);
    }

    bool IGameDirectoryLookup.TryGet(GameRelease release, GameInstallMode installMode, out DirectoryPath path)
    {
        return TryGetGameDirectory(release, installMode, out path);
    }

    DirectoryPath IDataDirectoryLookup.Get(GameRelease release)
    {
        return GetDataDirectory(release, GameInstallMode.Steam);
    }

    DirectoryPath IDataDirectoryLookup.Get(GameRelease release, GameInstallMode installMode)
    {
        return GetDataDirectory(release, installMode);
    }

    IEnumerable<DirectoryPath> IGameDirectoryLookup.GetAll(GameRelease release)
    {
        return GetAllGameDirectories(release);
    }

    bool IGameDirectoryLookup.TryGet(GameRelease release, out DirectoryPath path)
    {
        return TryGetGameDirectory(release, GameInstallMode.Steam, out path);
    }

    DirectoryPath IGameDirectoryLookup.Get(GameRelease release)
    {
        return GetGameDirectory(release, GameInstallMode.Steam);
    }

    #endregion
}