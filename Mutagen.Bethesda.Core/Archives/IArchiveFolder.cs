namespace Mutagen.Bethesda.Archives;

public interface IArchiveFolder
{
    string? Path { get; }
    IReadOnlyCollection<IArchiveFile> Files { get; }
}