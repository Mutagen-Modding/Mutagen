using System.Collections.Generic;

namespace Mutagen.Bethesda.Installs;

record GameMetaData(
    GameRelease Game, 
    string NexusName,
    long NexusGameId,
    int? SteamId,
    int? GogId,
    string? RegistryPath,
    string? RegistryKey,
    IEnumerable<string> RequiredFiles);