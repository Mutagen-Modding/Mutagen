using Microsoft.Win32;
using Noggog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.GameLocation
{
    class SteamHandler : AStoreHandler
    {
        private const string SteamRegKey = @"Software\Valve\Steam";

        public string SteamPath { get; set; } = string.Empty;
        private string SteamConfig => Path.Combine(SteamPath, "config//config.vdf");

        private readonly Dictionary<GameRelease, StoreGame> _games = new Dictionary<GameRelease, StoreGame>();
        public override IDictionary<GameRelease, StoreGame> Games => _games;

        public static GetResponse<SteamHandler> TryFactory(Action<string>? logging = null)
        {
            var ret = new SteamHandler();
            var err = ret.Init(logging);
            if (err.Failed) return err.BubbleFailure<SteamHandler>();
            return ret;
        }

        private ErrorResponse Init(Action<string>? logging)
        {
            try
            {
                var steamKey = Registry.CurrentUser.OpenSubKey(SteamRegKey);

                var steamPathKey = steamKey?.GetValue("SteamPath");
                if (steamPathKey == null)
                {
                    return ErrorResponse.Fail("Could not open the SteamPath registry key!");
                }

                SteamPath = steamPathKey.ToString() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(SteamPath))
                {
                    return ErrorResponse.Fail("Path to the Steam Directory from registry is Null or Empty!");
                }

                if (!File.Exists(SteamPath))
                {
                    return ErrorResponse.Fail($"Path to the Steam Directory from registry does not exists: {SteamPath}");
                }

                if (!File.Exists(SteamConfig))
                {
                    return ErrorResponse.Fail($"The Steam config file could not be read: {SteamConfig}");
                }
            }
            catch (Exception e) when (e is SecurityException or UnauthorizedAccessException)
            {
                return ErrorResponse.Fail($"{nameof(SteamHandler)} could not read from registry!");
            }

            return LoadAllGames(logging);
        }

        private IReadOnlyList<string> LoadUniverses(Action<string>? logging)
        {
            var ret = File.ReadAllLines(SteamConfig)
                .Select(l =>
                {
                    if (!l.Contains("BaseInstallFolder_", StringComparison.OrdinalIgnoreCase)) return null;
                    var path = Path.Combine(GetVdfValue(l),"steamapps");

                    if (!File.Exists(path))
                    {
                        logging?.Invoke($"Directory {path} does not exist, skipping");
                        return null;
                    }

                    logging?.Invoke($"Steam Library found at {path}");
                    return path;
                })
                .NotNull()
                .ToList();

            logging?.Invoke($"Total number of Steam Libraries found: {ret.Count}");

            // Default path in the Steam folder isn't in the configs
            var defaultPath = Path.Combine(SteamPath, "steamapps");
            if (File.Exists(defaultPath))
                ret.Add(defaultPath);

            return ret;
        }

        private ErrorResponse LoadAllGames(Action<string>? logging)
        {
            var universes = LoadUniverses(logging);

            if (universes.Count == 0)
            {
                return ErrorResponse.Fail("Could not find any Steam Libraries");
            }

            universes.ForEach(u =>
            {
                logging?.Invoke($"Searching for Steam Games in {u}");

                Directory.EnumerateFiles(u, "*.acf")
                    .ForEach(f =>
                    {
                        var game = new StoreGame();
                        var gotID = false;

                        File.ReadAllLines(f).ForEach(l =>
                        {
                            if (l.ContainsInsensitive("\"appid\""))
                            {
                                if (!int.TryParse(GetVdfValue(l), out var id))
                                    return;
                                game.ID = id;
                                gotID = true;
                            }

                            if (l.ContainsInsensitive("\"name\""))
                                game.Name = GetVdfValue(l);

                            if (!l.ContainsInsensitive("\"installdir\""))
                                return;

                            var value = GetVdfValue(l);
                            string absPath;

                            if (Path.IsPathRooted(value))
                            {
                                absPath = value;
                            }
                            else
                            {
                                absPath = Path.Combine(u, value, "Common");
                            }

                            if (File.Exists(absPath))
                                game.Path = absPath;
                        });

                        if (!gotID || !Directory.Exists(game.Path)) return;

                        var gameMeta = GameLocations.Games.Values.FirstOrDefault(g => g.SteamId == game.ID);

                        if (gameMeta == null)
                        {
                            logging?.Invoke($"Steam Game \"{game.Name}\" ({game.ID}) is not supported, skipping");
                            return;
                        }

                        logging?.Invoke($"Found Steam Game: \"{game.Name}\" ({game.ID}) at {game.Path}");

                        _games[gameMeta.Game] = game;
                    });
            });

            logging?.Invoke($"Total number of Steam Games found: {Games.Count}");

            return ErrorResponse.Success;
        }

        private static string GetVdfValue(string line)
        {
            var trim = line.Trim('\t').Replace("\t", "");
            var split = trim.Split('\"');
            return split.Length >= 4 ? split[3].Replace("\\\\", "\\") : string.Empty;
        }

        private static string GetSingleVdfValue(string line)
        {
            var trim = line.Trim('\t').Replace("\t", "");
            var split = trim.Split('\"');
            return split.Length >= 2 ? split[1] : string.Empty;
        }
    }
}
