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
    private readonly IGameInstallModeContext _installModeContext;
    private readonly IGameDirectoryLookup _locator;

    public DirectoryPath? Path => Get();

    public GameDirectoryProvider(
        IGameReleaseContext release,
        IGameInstallModeContext installModeContext,
        IGameDirectoryLookup locator)
    {
        _release = release;
        _installModeContext = installModeContext;
        _locator = locator;
    }

    private DirectoryPath? Get()
    {
        var install = _installModeContext.InstallModeOptional;
        if (install == null) return null;
        return _locator.TryGet(_release.Release, install.Value);
    }
}

public sealed class GameDirectoryRelativeProvider : IGameDirectoryProvider
{
    private readonly IDataDirectoryProvider _dataDirectoryProvider;

    public GameDirectoryRelativeProvider(
        IDataDirectoryProvider dataDirectoryProvider)
    {
        _dataDirectoryProvider = dataDirectoryProvider;
    }

    public DirectoryPath? Path => _dataDirectoryProvider.Path.Directory;
}

public record GameDirectoryInjection(DirectoryPath? Path) : IGameDirectoryProvider;