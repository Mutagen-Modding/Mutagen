using Microsoft.VisualBasic.CompilerServices;
using Mutagen.Bethesda.Environments.DI;

namespace Mutagen.Bethesda.Installs.DI;

public interface IGameInstallModeContext
{
    GameInstallMode InstallMode { get; }
}

public sealed class GameInstallModeContext : IGameInstallModeContext
{
    private readonly IGameInstallProvider _gameInstallProvider;
    private readonly IGameReleaseContext _releaseContext;

    public GameInstallModeContext(
        IGameInstallProvider gameInstallProvider,
        IGameReleaseContext releaseContext)
    {
        _gameInstallProvider = gameInstallProvider;
        _releaseContext = releaseContext;
    }

    public GameInstallMode InstallMode => _gameInstallProvider.GetInstallMode(_releaseContext.Release);
}

public sealed class GameInstallModePlaceholder : IGameInstallModeContext
{
    public GameInstallMode InstallMode => throw new IncompleteInitialization();
}