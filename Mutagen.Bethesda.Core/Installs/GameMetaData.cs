namespace Mutagen.Bethesda.Installs;

sealed record GameMetaData(
    GameRelease Game, 
    string NexusName,
    long NexusGameId,
    IEnumerable<IGameSource> GameSources,
    IEnumerable<string> RequiredFiles);
    