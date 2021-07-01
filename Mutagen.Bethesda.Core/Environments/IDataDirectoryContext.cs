using Noggog;

namespace Mutagen.Bethesda.Environments
{
    public interface IDataDirectoryContext
    {
        DirectoryPath Path { get; init; }
    }

    public record DataDirectoryInjection(DirectoryPath Path) : IDataDirectoryContext;
}