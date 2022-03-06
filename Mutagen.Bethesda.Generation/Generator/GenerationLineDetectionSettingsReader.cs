using Noggog;
using Noggog.IO;
using System.IO.Abstractions;
using System.Reactive;
using System.Reactive.Linq;
using DynamicData;
using Noggog.Reactive;

namespace Mutagen.Bethesda.Generation.Generator;

public class GenerationLineDetectionSettingsReader
{
    private readonly IObservable<IChangeSet<string>> _obs;

    public GenerationLineDetectionSettingsReader(
        IWatchFile watchFile,
        IFileSystem fileSystem,
        ICurrentDirectoryProvider currentDirectoryProvider)
    {
        _obs = Observable.Defer<IChangeSet<string>>(() =>
        {
            var dir = currentDirectoryProvider.CurrentDirectory.Directory?.Directory?.Directory?.Directory!.Value;
            var path = Path.Combine(dir, "GenerationLines.txt");
            return watchFile.Watch(path)
                .StartWith(Unit.Default)
                .Select(_ =>
                {
                    if (!fileSystem.File.Exists(path)) return Enumerable.Empty<string>();
                    return fileSystem.File.ReadAllLines(path).Where(x => !x.IsNullOrWhitespace());
                })
                .Select(x => x.AsObservableChangeSet())
                .Switch();
        });
    }

    public IObservable<IChangeSet<string>> Lines => _obs;
}
    