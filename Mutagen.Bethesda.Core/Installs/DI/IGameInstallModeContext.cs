using Microsoft.VisualBasic.CompilerServices;
using Mutagen.Bethesda.Environments.DI;

namespace Mutagen.Bethesda.Installs.DI;

public interface IGameInstallModeContext
{
    GameInstallMode InstallMode { get; }
}

public sealed class GameInstallModeContext : IGameInstallModeContext
{
    private readonly IGameInstallLookup _gameInstallLookup;
    private readonly IGameReleaseContext _releaseContext;

    public GameInstallModeContext(
        IGameInstallLookup gameInstallLookup,
        IGameReleaseContext releaseContext)
    {
        _gameInstallLookup = gameInstallLookup;
        _releaseContext = releaseContext;
    }

    public GameInstallMode InstallMode => _gameInstallLookup.GetInstallMode(_releaseContext.Release);
}

public sealed class GameInstallModePlaceholder : IGameInstallModeContext
{
    public GameInstallMode InstallMode => throw new IncompleteInitialization();
}