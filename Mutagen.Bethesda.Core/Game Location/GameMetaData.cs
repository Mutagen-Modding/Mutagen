using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.GameLocation
{
    record GameMetaData(
        GameRelease Game, 
        string NexusName,
        long NexusGameId,
        int? SteamId,
        int? GogId,
        IEnumerable<string> RequiredFiles);
}
