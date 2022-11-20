using Microsoft.VisualBasic.CompilerServices;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Installs.Exceptions;

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

    public GameInstallMode InstallMode => Get();

    private GameInstallMode Get()
    {
        var install =  _gameInstallLookup
            .GetInstallModes(_releaseContext.Release)
            .Select(x => (GameInstallMode?)x)
            .FirstOrDefault();
        if (install == null)
        {
            throw new NoGameInstallationException();
        }

        return install.Value;
    }
}

public sealed class GameInstallModeInjection : IGameInstallModeContext
{
    public GameInstallMode InstallMode => throw new IncompleteInitialization();
}