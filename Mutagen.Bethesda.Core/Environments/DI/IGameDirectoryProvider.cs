using Noggog;

namespace Mutagen.Bethesda.Environments.DI
{
    public interface IGameDirectoryProvider
    {
        DirectoryPath Path { get; }
    }

    public class GameDirectoryProvider : IGameDirectoryProvider
    {
        private readonly IGameReleaseContext _release;
        private readonly IDataDirectoryLookup _locator;

        public DirectoryPath Path => _locator.Get(_release.Release);

        public GameDirectoryProvider(
            IGameReleaseContext release,
            IDataDirectoryLookup locator)
        {
            _release = release;
            _locator = locator;
        }
    }

    public record GameDirectoryInjection(DirectoryPath Path) : IGameDirectoryProvider;
}