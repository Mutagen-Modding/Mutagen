namespace Mutagen.Bethesda.Installs;

sealed record GameMetaData(
    GameRelease Game, 
    IEnumerable<IGameSource> GameSources,
    IEnumerable<string> RequiredFiles);
    