using Loqui;
using System.Reactive.Linq;

namespace Mutagen.Bethesda.Generation.Generator;

public class GenerationLineDetector
{
    public IObservable<string> LineDetected { get; }

    public GenerationLineDetector(GenerationLineDetectionSettingsReader settings)
    {
        var lines = settings.ReadLinesToDetect().ToList();
        if (lines.Count == 0)
        {
            LineDetected = Observable.Empty<string>();
            return;
        }
        LineDetected = FileGeneration.LineAppended
            .Where(i =>
            {
                foreach (var line in lines)
                {
                    if (i.Contains(line)) return true;
                }
                return false;
            });
    }
}
