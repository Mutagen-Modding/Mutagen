using Mutagen.Bethesda.Environments.DI;
using Mutagen.Bethesda.Installs.DI;
using Noggog;

namespace Mutagen.Bethesda.Inis.DI;

public interface IIniPathProvider
{
    FilePath Path { get; }
}

public sealed class IniPathProvider : IIniPathProvider
{
    private readonly IGameInstallModeContext _gameInstallModeContext;
    private readonly IGameReleaseContext _releaseContext;
    private readonly IIniPathLookup _lookup;
    
    public FilePath Path => _lookup.Get(_releaseContext.Release, _gameInstallModeContext.InstallMode);

    public IniPathProvider(
        IGameInstallModeContext gameInstallModeContext,
        IGameReleaseContext releaseContext,
        IIniPathLookup lookup)
    {
        _gameInstallModeContext = gameInstallModeContext;
        _releaseContext = releaseContext;
        _lookup = lookup;
    }
}

public record IniPathInjection(FilePath Path) : IIniPathProvider;