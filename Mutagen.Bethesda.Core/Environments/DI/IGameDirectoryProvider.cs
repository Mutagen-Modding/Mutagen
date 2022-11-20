using Mutagen.Bethesda.Installs.DI;
using Noggog;

namespace Mutagen.Bethesda.Environments.DI;

public interface IGameDirectoryProvider
{
    DirectoryPath? Path { get; }
}

public sealed class GameDirectoryProvider : IGameDirectoryProvider
{
    private readonly IGameReleaseContext _release;
    private readonly IGameInstallLookup _gameInstallLookup;
    private readonly IGameDirectoryLookup _locator;

    public DirectoryPath? Path => Get();

    public GameDirectoryProvider(
        IGameReleaseContext release,
        IGameInstallLookup gameInstallLookup,
        IGameDirectoryLookup locator)
    {
        _release = release;
        _gameInstallLookup = gameInstallLookup;
        _locator = locator;
    }

    private DirectoryPath? Get()
    {
        var install = _gameInstallLookup
            .GetInstallModes(_release.Release)
            .Select(x => (GameInstallMode?)x)
            .FirstOrDefault();
        if (install == null) return null;
        return _locator.TryGet(_release.Release, install.Value);
    }
}

public record GameDirectoryInjection(DirectoryPath? Path) : IGameDirectoryProvider;