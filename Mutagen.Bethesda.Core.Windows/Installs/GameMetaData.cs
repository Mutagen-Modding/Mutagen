using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Installs
{
    record GameMetaData(
        GameRelease Game, 
        string NexusName,
        long NexusGameId,
        int? SteamId,
        int? GogId,
        string? RegistryPath,
        string? RegistryKey,
        IEnumerable<string> RequiredFiles);
}
