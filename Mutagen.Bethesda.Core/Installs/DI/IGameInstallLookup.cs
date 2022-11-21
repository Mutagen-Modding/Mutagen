namespace Mutagen.Bethesda.Installs.DI;

public interface IGameInstallLookup
{
    IEnumerable<GameInstallMode> GetInstallModes(GameRelease release);
    GameInstallMode GetInstallMode(GameRelease release);
    GameInstallMode? TryGetInstallMode(GameRelease release);
}
