using Mutagen.Bethesda.Installs.DI;
using Noggog;

namespace Mutagen.Bethesda.Environments.DI;

public interface IDataDirectoryProvider
{
    DirectoryPath Path { get; }
}

public sealed class DataDirectoryProvider : IDataDirectoryProvider
{
    private readonly IGameReleaseContext _release;
    private readonly IGameInstallModeContext _installModeContext;
    private readonly IDataDirectoryLookup _locator;

    public DirectoryPath Path => _locator.Get(_release.Release, _installModeContext.InstallMode);

    public DataDirectoryProvider(
        IGameReleaseContext release,
        IGameInstallModeContext installModeContext,
        IDataDirectoryLookup locator)
    {
        _release = release;
        _installModeContext = installModeContext;
        _locator = locator;
    }
}

public record DataDirectoryInjection(DirectoryPath Path) : IDataDirectoryProvider;