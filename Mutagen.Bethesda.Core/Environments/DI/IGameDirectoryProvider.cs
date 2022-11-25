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
    private readonly IGameDirectoryLookup _locator;

    public DirectoryPath? Path => Get();

    public GameDirectoryProvider(
        IGameReleaseContext release,
        IGameDirectoryLookup locator)
    {
        _release = release;
        _locator = locator;
    }

    private DirectoryPath? Get()
    {
        return _locator.TryGet(_release.Release);
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