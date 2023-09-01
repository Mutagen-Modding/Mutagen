using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Strings;

namespace Mutagen.Bethesda.Tests;

public class StarfieldProcessor : Processor
{
    public override bool StrictStrings => true;
    
    public StarfieldProcessor(bool multithread) : base(multithread)
    {
    }

    public override GameRelease GameRelease => GameRelease.Starfield;

    protected override void AddDynamicProcessorInstructions()
    {
        base.AddDynamicProcessorInstructions();
    }

    protected override IEnumerable<Task> ExtraJobs(Func<IMutagenReadStream> streamGetter)
    {
        foreach (var job in base.ExtraJobs(streamGetter))
        {
            yield return job;
        }
    }
    
    protected override Dictionary<(ModKey ModKey, StringsSource Source), HashSet<uint>>? KnownDeadStringKeys()
    {
        return new Dictionary<(ModKey ModKey, StringsSource Source), HashSet<uint>>
        {
        };
    }

    protected override AStringsAlignment[] GetStringsFileAlignments(StringsSource source)
    {
        switch (source)
        {
            case StringsSource.Normal:
                return new AStringsAlignment[]
                {
                };
            case StringsSource.DL:
                return new AStringsAlignment[]
                {
                };
            case StringsSource.IL:
                return new AStringsAlignment[]
                {
                };
            default:
                throw new NotImplementedException();
        }
    }

}