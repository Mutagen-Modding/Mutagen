using System.Diagnostics.CodeAnalysis;
using GameFinder;
using GameFinder.StoreHandlers.GOG;
using GameFinder.StoreHandlers.Steam;
using Microsoft.Win32;
using Mutagen.Bethesda.Environments.DI;
using Noggog;

namespace Mutagen.Bethesda.Installs.DI;

public sealed class GameLocator : IGameDirectoryLookup, IDataDirectoryLookup
{
    private readonly Lazy<GetResponse<SteamHandler>> _steamHandler;
    private readonly Lazy<GetResponse<GOGHandler>> _gogHandler;
        
    public GameLocator()
    {
        _steamHandler = new(TryFactory<SteamHandler, SteamGame>);
        _gogHandler = new(TryFactory<GOGHandler, GOGGame>);
    }

    /// <summary>
    /// Given a release, will return all the located game folders it could find
    /// </summary>
    /// <param name="release">Release to query</param>
    /// <returns>The located game folders it could find</returns>
    public IEnumerable<DirectoryPath> GetGameDirectories(GameRelease release)
    {
        return InternalGetGameFolders(release)
            .Distinct();
    }

    /// <summary>
    /// Given a release, will return all the located data folders it could find
    /// </summary>
    /// <param name="release">Release to query</param>
    /// <returns>The located data folders it could find</returns>
    public IEnumerable<DirectoryPath> GetDataDirectories(GameRelease release)
    {
        return GetGameDirectories(release)
            .Select(path => new DirectoryPath(Path.Combine(path, "Data")));
    }

    private IEnumerable<DirectoryPath> InternalGetGameFolders(GameRelease release)
    {
        if (TryGetGameDirectoryFromRegistry(release, out var regisPath)
            && regisPath.Exists)
        {
            yield return regisPath;
        }
            
        var steamHandler = _steamHandler.Value;
        if (steamHandler.Succeeded)
        {
            foreach (var game in steamHandler.Value.Games.Where(x => x.ID.Equals(Games[release].SteamId)))
            {
                yield return game.Path;
            }
        }
            
        var gogHandler = _gogHandler.Value;
        if (gogHandler.Succeeded)
        {
            foreach (var game in gogHandler.Value.Games.Where(x => x.GameID.Equals(Games[release].GogId)))
            {
                yield return game.Path;
            }
        }
    }
        
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
        
    /// <summary>
    /// Given a release, tries to retrieve the preferred game directory (not the data folder within)
    /// </summary>
    /// <param name="release">Release to query</param>
    /// <param name="path">The game folder, if located</param>
    /// <returns>True if located</returns>
    public bool TryGetGameDirectory(GameRelease release, [MaybeNullWhen(false)] out DirectoryPath path)
    {
        var p = GetGameDirectories(release).Select<DirectoryPath, DirectoryPath?>(x => x).FirstOrDefault();
        if (p == null)
        {
            path = default;
            return false;
        }

        path = p.Value;
        return true;
    }

    /// <summary>
    /// Given a release, tries to retrieve the preferred game directory (not the data folder within)
    /// </summary>
    /// <param name="release">Release to query</param>
    /// <exception cref="System.IO.DirectoryNotFoundException">Thrown if the game folder could not be located</exception>
    /// <returns>The game folder</returns>
    public DirectoryPath GetGameDirectory(GameRelease release)
    {
        if (TryGetGameDirectory(release, out var path))
        {
            return path;
        }
        throw new DirectoryNotFoundException($"Game folder for {release} cannot be found automatically");
    }

    /// <summary>
    /// Given a release, tries to retrieve the preferred data directory
    /// </summary>
    /// <param name="release">Release to query</param>
    /// <param name="path">The data folder, if located</param>
    /// <returns>True if located</returns>
    public bool TryGetDataDirectory(GameRelease release, [MaybeNullWhen(false)] out DirectoryPath path)
    {
        if (TryGetGameDirectory(release, out path))
        {
            path = Path.Combine(path, "Data");
            return true;
        }
        path = default;
        return false;
    }

    /// <summary>
    /// Given a release, tries to retrieve the preferred data directory
    /// </summary>
    /// <param name="release">Release to query</param>
    /// <exception cref="System.IO.DirectoryNotFoundException">Thrown if the data folder could not be located</exception>
    /// <returns>The data folder provider</returns>
    public DirectoryPath GetDataDirectory(GameRelease release)
    {
        if (TryGetDataDirectory(release, out var path))
        {
            return path;
        }
        throw new DirectoryNotFoundException($"Data folder for {release} cannot be found automatically");
    }
        
    private GetResponse<TStoreHandler> TryFactory<TStoreHandler, TStoreGame>() 
        where TStoreHandler : AStoreHandler<TStoreGame>, new()
        where TStoreGame : AStoreGame
    {
        try
        {
            var storeHandler = new TStoreHandler();
            storeHandler.FindAllGames();
            return storeHandler;
        }
        catch (Exception e)
        {
            return ErrorResponse.Fail(e).BubbleFailure<TStoreHandler>();
        }
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
                    GogId: null,
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
        return GetDataDirectories(release);
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
        return GetGameDirectories(release);
    }

    bool IGameDirectoryLookup.TryGet(GameRelease release, out DirectoryPath path)
    {
        return TryGetGameDirectory(release, out path);
    }

    DirectoryPath IGameDirectoryLookup.Get(GameRelease release)
    {
        return GetGameDirectory(release);
    }

    #endregion
}