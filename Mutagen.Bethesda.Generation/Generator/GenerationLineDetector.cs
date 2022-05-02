using System.Reactive.Linq;
using DynamicData;
using Noggog;
using Noggog.StructuredStrings;

namespace Mutagen.Bethesda.Generation.Generator;

public class GenerationLineDetector
{
    public IObservable<string> LineDetected { get; }

    public GenerationLineDetector(GenerationLineDetectionSettingsReader settings)
    {
        var lines = settings.Lines.QueryWhenChanged(q => q)
            .Replay(1)
            .RefCount();
        LineDetected = StructuredStringBuilder.LineAppended
            .WithLatestFrom(
                lines,
                (Line, Lines) => (Line, Lines))
            .Where(i =>
            {
                foreach (var line in i.Lines)
                {
                    if (i.Line.Contains(line)) return true;
                }

                return false;
            })
            .Select(x => x.Line)
            .FlowSwitch(lines.Select(x => x.Count > 0));
    }
}