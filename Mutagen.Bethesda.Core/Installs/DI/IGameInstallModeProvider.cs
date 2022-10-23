using Microsoft.VisualBasic.CompilerServices;
using Mutagen.Bethesda.Environments.DI;

namespace Mutagen.Bethesda.Installs.DI;

public interface IGameInstallModeProvider
{
    GameInstallMode InstallMode { get; }
}

public sealed class GameInstallModeProvider : IGameInstallModeProvider
{
    private readonly IGameInstallLookup _gameInstallLookup;
    private readonly IGameReleaseContext _releaseContext;

    public GameInstallModeProvider(
        IGameInstallLookup gameInstallLookup,
        IGameReleaseContext releaseContext)
    {
        _gameInstallLookup = gameInstallLookup;
        _releaseContext = releaseContext;
    }

    public GameInstallMode InstallMode => _gameInstallLookup.GetInstallMode(_releaseContext.Release);
}

public sealed class GameInstallModePlaceholder : IGameInstallModeProvider
{
    public GameInstallMode InstallMode => throw new IncompleteInitialization();
}