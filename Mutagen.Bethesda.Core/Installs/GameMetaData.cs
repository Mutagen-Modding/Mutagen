namespace Mutagen.Bethesda.Installs;

sealed record GameMetaData(
    GameRelease Game, 
    string NexusName,
    long NexusGameId,
    int? SteamId,
    int? GogId,
    string? RegistryPath,
    string? RegistryKey,
    IEnumerable<string> RequiredFiles);