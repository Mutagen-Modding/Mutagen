using Microsoft.VisualBasic.CompilerServices;
using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Installs.Exceptions;

namespace Mutagen.Bethesda.Installs.DI;

public interface IGameInstallModeContext
{
    GameInstallMode InstallMode { get; }
    GameInstallMode? InstallModeOptional { get; }
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

    public GameInstallMode InstallMode => Get() ?? throw new NoGameInstallationException();

    public GameInstallMode? InstallModeOptional => Get();

    private GameInstallMode? Get()
    {
        return _gameInstallLookup
            .GetInstallModes(_releaseContext.Release)
            .Select(x => (GameInstallMode?)x)
            .FirstOrDefault();
    }
}

public sealed class GameInstallModeInjection : IGameInstallModeContext
{
    public GameInstallMode InstallMode => InstallModeOptional ?? throw new NoGameInstallationException();
    public GameInstallMode? InstallModeOptional { get; }

    public GameInstallModeInjection(GameInstallMode? installMode)
    {
        InstallModeOptional = installMode;
    }
}