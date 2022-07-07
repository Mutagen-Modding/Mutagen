using Noggog;

namespace Mutagen.Bethesda.Environments.DI;

public interface IDataDirectoryProvider
{
    DirectoryPath Path { get; }
}

public sealed class DataDirectoryProvider : IDataDirectoryProvider
{
    private readonly IGameReleaseContext _release;
    private readonly IDataDirectoryLookup _locator;

    public DirectoryPath Path => _locator.Get(_release.Release);

    public DataDirectoryProvider(
        IGameReleaseContext release,
        IDataDirectoryLookup locator)
    {
        _release = release;
        _locator = locator;
    }
}

public record DataDirectoryInjection(DirectoryPath Path) : IDataDirectoryProvider;