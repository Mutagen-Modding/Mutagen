using Mutagen.Bethesda.Environments.DI;
using Noggog;

namespace Mutagen.Bethesda.Inis.DI;

public interface IIniPathProvider
{
    FilePath Path { get; }
}

public sealed class IniPathProvider : IIniPathProvider
{
    private readonly IGameReleaseContext _releaseContext;
    private readonly IIniPathLookup _lookup;
    
    public FilePath Path => _lookup.Get(_releaseContext.Release);

    public IniPathProvider(
        IGameReleaseContext releaseContext,
        IIniPathLookup lookup)
    {
        _releaseContext = releaseContext;
        _lookup = lookup;
    }
}

public record IniPathInjection(FilePath Path) : IIniPathProvider;