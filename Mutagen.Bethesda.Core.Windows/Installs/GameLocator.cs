using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using GameFinder;
using GameFinder.StoreHandlers.GOG;
using GameFinder.StoreHandlers.Steam;
using Microsoft.Win32;
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
        private readonly Lazy<GetResponse<SteamHandler>> _steamHandler;
        private readonly Lazy<GetResponse<GOGHandler>> _gogHandler;
        
        public GameLocator()
        {
            _steamHandler = new(TryFactory<SteamHandler, SteamGame>);
            _gogHandler = new(TryFactory<GOGHandler, GOGGame>);
        }

        /// <inheritdoc />
        public IEnumerable<DirectoryPath> GetGameFolders(GameRelease release)
        {
            return InternalGetGameFolders(release)
                .Distinct();
        }

        private IEnumerable<DirectoryPath> InternalGetGameFolders(GameRelease release)
        {
            if (TryGetGameFolderFromRegistry(release, out var regisPath)
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
        
        /// <inheritdoc />
        public bool TryGetGameFolderFromRegistry(GameRelease release,
            [MaybeNullWhen(false)] out DirectoryPath path)
        {
            try
            {
                var key = Games[release].RegistryKey;
                using var regKey = Registry.LocalMachine.OpenSubKey(key);
                if (regKey == null)
                {
                    path = default;
                    return false;
                }

                var regRes = RegistryHelper.GetStringValueFromRegistry(regKey, "installed path");
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
        
        /// <inheritdoc />
        public bool TryGetGameFolder(GameRelease release, [MaybeNullWhen(false)] out DirectoryPath path)
        {
            var p = GetGameFolders(release).Select<DirectoryPath, DirectoryPath?>(x => x).FirstOrDefault();
            if (p == null)
            {
                path = default;
                return false;
            }

            path = p.Value;
            return true;
        }

        /// <inheritdoc />
        public DirectoryPath GetGameFolder(GameRelease release)
        {
            if (TryGetGameFolder(release, out var path))
            {
                return path;
            }
            throw new DirectoryNotFoundException($"Game folder for {release} cannot be found automatically");
        }

        /// <inheritdoc />
        public bool TryGetDataFolder(GameRelease release, [MaybeNullWhen(false)] out DirectoryPath path)
        {
            if (TryGetGameFolder(release, out path))
            {
                path = Path.Combine(path, "Data");
                return true;
            }
            path = default;
            return false;
        }

        /// <inheritdoc />
        public DirectoryPath GetDataFolder(GameRelease release)
        {
            if (TryGetDataFolder(release, out var path))
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
                        RegistryKey: @"SOFTWARE\WOW6432Node\Bethesda Softworks\Oblivion",
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
                        RegistryKey: @"SOFTWARE\WOW6432Node\Bethesda Softworks\Skyrim",
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
                        RegistryKey: @"SOFTWARE\WOW6432Node\Bethesda Softworks\Skyrim Special Edition",
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
                        RegistryKey: @"SOFTWARE\WOW6432Node\Bethesda Softworks\Fallout4",
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
                        RegistryKey: @"SOFTWARE\WOW6432Node\Bethesda Softworks\Skyrim VR",
                        RequiredFiles: new string[]
                        {
                            "SkyrimVR.exe"
                        })
                }
            };
            games[GameRelease.EnderalLE] = games[GameRelease.SkyrimLE];
            games[GameRelease.EnderalSE] = games[GameRelease.SkyrimSE];
            Games = games;
        }
    }
}