using Noggog;

namespace Mutagen.Bethesda.Environments
{
    public interface IGameDirectoryContext
    {
        DirectoryPath Path { get; init; }
    }

    public record GameDirectoryInjection(DirectoryPath Path) : IGameDirectoryContext;
}