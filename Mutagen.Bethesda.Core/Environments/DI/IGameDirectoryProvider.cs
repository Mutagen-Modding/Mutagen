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

    public DirectoryPath? Path => _locator.TryGet(_release.Release, _gameInstallLookup.GetInstallMode(_release.Release));

    public GameDirectoryProvider(
        IGameReleaseContext release,
        IGameInstallLookup gameInstallLookup,
        IGameDirectoryLookup locator)
    {
        _release = release;
        _gameInstallLookup = gameInstallLookup;
        _locator = locator;
    }
}

public record GameDirectoryInjection(DirectoryPath? Path) : IGameDirectoryProvider;