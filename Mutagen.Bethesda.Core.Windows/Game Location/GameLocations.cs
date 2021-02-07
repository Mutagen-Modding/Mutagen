using Mutagen.Bethesda.GameLocation;
using Noggog;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda
{
    public static class GameLocations
    {
        private static readonly Lazy<GetResponse<SteamHandler>> _steamHandler = new Lazy<GetResponse<SteamHandler>>(() => SteamHandler.TryFactory());
        private static readonly Lazy<GetResponse<GOGHandler>> _gogHandler = new Lazy<GetResponse<GOGHandler>>(() => GOGHandler.TryFactory());

        public static bool TryGetGameFolder(GameRelease release, [MaybeNullWhen(false)] out string path)
        {
            var steamGames = _steamHandler.Value;
            if (steamGames.Succeeded && steamGames.Value.Games.TryGetValue(release, out var game))
            {
                path = game.Path;
                return true;
            }
            var gogGames = _gogHandler.Value;
            if (gogGames.Succeeded && gogGames.Value.Games.TryGetValue(release, out game))
            {
                path = game.Path;
                return true;
            }
            path = default;
            return false;
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

    class StoreGame
    {
        public string Name { get; internal set; } = string.Empty;
        public string Path { get; internal set; } = string.Empty;
        public int ID { get; internal set; }
    }

    abstract class AStoreHandler
    {
        public abstract IDictionary<GameRelease, StoreGame> Games { get; }
    }
}
