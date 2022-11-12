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
    private readonly IGameInstallProvider _gameInstallProvider;
    private readonly IGameDirectoryLookup _locator;

    public DirectoryPath? Path => _locator.TryGet(_release.Release, _gameInstallProvider.GetInstallMode(_release.Release));

    public GameDirectoryProvider(
        IGameReleaseContext release,
        IGameInstallProvider gameInstallProvider,
        IGameDirectoryLookup locator)
    {
        _release = release;
        _gameInstallProvider = gameInstallProvider;
        _locator = locator;
    }
}

public record GameDirectoryInjection(DirectoryPath? Path) : IGameDirectoryProvider;