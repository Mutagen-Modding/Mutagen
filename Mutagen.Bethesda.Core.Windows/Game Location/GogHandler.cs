using Microsoft.Win32;
using Noggog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.GameLocation
{
    class GOGHandler : AStoreHandler
    {
        private const string GOGRegKey = @"Software\GOG.com\Games";
        private const string GOG64RegKey = @"Software\WOW6432Node\GOG.com\Games";

        private readonly Dictionary<GameRelease, StoreGame> _games = new Dictionary<GameRelease, StoreGame>();
        public override IDictionary<GameRelease, StoreGame> Games => _games;

        public static GetResponse<GOGHandler> TryFactory(Action<string>? logging = null)
        {
            var ret = new GOGHandler();
            var err = ret.Init(logging);
            if (err.Failed) return err.BubbleFailure<GOGHandler>();
            return ret;
        }

        private ErrorResponse Init(Action<string>? logging)
        {
            try
            {
                var gogKey = Registry.LocalMachine.OpenSubKey(GOGRegKey) ??
                             Registry.LocalMachine.OpenSubKey(GOG64RegKey);

                if (gogKey == null)
                {
                    return ErrorResponse.Fail("Could not open the GOG registry key!");
                }

                string[] keys = gogKey.GetSubKeyNames();
                logging?.Invoke($"Found {keys.Length} SubKeys for GOG");

                keys.ForEach(key =>
                {
                    if (!int.TryParse(key, out var gameID))
                    {
                        logging?.Invoke($"Could not read gameID for key {key}");
                        return;
                    }

                    var subKey = gogKey.OpenSubKey(key);
                    if (subKey == null)
                    {
                        logging?.Invoke($"Could not open SubKey for {key}");
                        return;
                    }

                    var gameNameValue = subKey.GetValue("GAMENAME");
                    if (gameNameValue == null)
                    {
                        logging?.Invoke($"Could not get GAMENAME for {gameID} at {key}");
                        return;
                    }

                    var gameName = gameNameValue.ToString() ?? string.Empty;

                    var pathValue = subKey.GetValue("PATH");
                    if (pathValue == null)
                    {
                        logging?.Invoke($"Could not get PATH for {gameID} at {key}");
                        return;
                    }

                    var path = pathValue.ToString() ?? string.Empty;

                    var game = new StoreGame
                    {
                        ID = gameID,
                        Name = gameName,
                        Path = path,
                    };

                    var gameMeta = GameLocations.Games.Values.FirstOrDefault(g => g.GogId == gameID);

                    if (gameMeta == null)
                    {
                        logging?.Invoke($"GOG Game \"{gameName}\" ({gameID}) is not supported, skipping");
                        return;
                    }

                    logging?.Invoke($"Found GOG Game: \"{game.Name}\" ({game.ID}) at {game.Path}");

                    _games[gameMeta.Game] = game;
                });
            }
            catch (Exception e) 
            when (e is SecurityException or UnauthorizedAccessException)
            {
                return ErrorResponse.Fail($"{nameof(GOGHandler)} could not read from registry!");
            }
            catch (Exception e)
            {
                return ErrorResponse.Fail(e);
            }

            logging?.Invoke($"Total number of GOG Games found: {Games.Count}");

            return ErrorResponse.Success;
        }
    }
}
