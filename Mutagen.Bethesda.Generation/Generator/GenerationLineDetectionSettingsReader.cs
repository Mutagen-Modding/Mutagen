using Noggog;
using Noggog.IO;
using System.IO.Abstractions;

namespace Mutagen.Bethesda.Generation.Generator;

public class GenerationLineDetectionSettingsReader
{
    private readonly IFileSystem fileSystem;
    private readonly ICurrentDirectoryProvider currentDirectoryProvider;

    public GenerationLineDetectionSettingsReader(
        IFileSystem fileSystem,
        ICurrentDirectoryProvider currentDirectoryProvider)
    {
        this.fileSystem = fileSystem;
        this.currentDirectoryProvider = currentDirectoryProvider;
    }

    public IEnumerable<string> ReadLinesToDetect()
    {
        var dir = currentDirectoryProvider.CurrentDirectory.Directory?.Directory?.Directory?.Directory!.Value;
        var path = Path.Combine(dir, "GenerationLines.txt");
        if (!fileSystem.File.Exists(path)) return Enumerable.Empty<string>();
        return fileSystem.File.ReadAllLines(path).Where(x => !x.IsNullOrWhitespace());
    }
}
