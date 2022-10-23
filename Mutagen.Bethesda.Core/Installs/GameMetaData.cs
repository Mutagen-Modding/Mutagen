namespace Mutagen.Bethesda.Installs;

sealed record GameMetaData(
    GameRelease Game, 
    string NexusName,
    long NexusGameId,
    int? SteamId,
    long? GogId,
    string? RegistryPath,
    string? RegistryKey,
    IEnumerable<string> RequiredFiles);