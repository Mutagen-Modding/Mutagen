using Mutagen.Bethesda.GameLocation;
using Noggog;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using GameFinder;
using GameFinder.StoreHandlers.Steam;
using GameFinder.StoreHandlers.GOG;

namespace Mutagen.Bethesda
{
    public static class GameLocations
    {
        private static readonly Lazy<GetResponse<SteamHandler>> _steamHandler = new(TryFactory<SteamHandler, SteamGame>);
        private static readonly Lazy<GetResponse<GOGHandler>> _gogHandler = new(TryFactory<GOGHandler, GOGGame>);

        private static GetResponse<TStoreHandler> TryFactory<TStoreHandler, TStoreGame>() 
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

        public static IEnumerable<DirectoryPath> GetGameFolders(GameRelease release)
        {
            return InternalGetGameFolders(release)
                .Distinct();
        }

        private static IEnumerable<DirectoryPath> InternalGetGameFolders(GameRelease release)
        {
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
        
        public static bool TryGetGameFolder(GameRelease release, [MaybeNullWhen(false)] out DirectoryPath path)
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

        public static DirectoryPath GetGameFolder(GameRelease release)
        {
            if (TryGetGameFolder(release, out var path))
            {
                return path;
            }
            throw new Exception($"Game folder for {release} cannot be found automatically");
        }

        public static bool TryGetDataFolder(GameRelease release, [MaybeNullWhen(false)] out DirectoryPath path)
        {
            if (TryGetGameFolder(release, out path))
            {
                path = Path.Combine(path, "Data");
                return true;
            }
            path = default;
            return false;
        }

        public static DirectoryPath GetDataFolder(GameRelease release)
        {
            if (TryGetDataFolder(release, out var path))
            {
                return path;
            }
            throw new Exception($"Data folder for {release} cannot be found automatically");
        }

        internal static IReadOnlyDictionary<GameRelease, GameMetaData> Games = new Dictionary<GameRelease, GameMetaData>
        {
            {
                GameRelease.Oblivion, new GameMetaData(
                    GameRelease.Oblivion,
                    NexusName: "oblivion",
                    NexusGameId: 101,
                    SteamId: 22330,
                    GogId: 1458058109,
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
                    RequiredFiles: new string[]
                    {
                        "SkyrimVR.exe"
                    })
            }
        };

    }
}
