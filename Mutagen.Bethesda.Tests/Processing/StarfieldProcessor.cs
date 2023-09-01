using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Starfield.Internals;
using Mutagen.Bethesda.Strings;

namespace Mutagen.Bethesda.Tests;

public class StarfieldProcessor : Processor
{
    public override bool StrictStrings => false;
    
    public StarfieldProcessor(bool multithread) : base(multithread)
    {
    }

    public override GameRelease GameRelease => GameRelease.Starfield;

    protected override void AddDynamicProcessorInstructions()
    {
        base.AddDynamicProcessorInstructions();
        AddDynamicProcessing(RecordTypes.GMST, ProcessGameSettings);
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
                    new StringsAlignmentCustom("GMST", GameSettingStringHandler),
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
    
    public void GameSettingStringHandler(
        IMutagenReadStream stream,
        MajorRecordHeader major,
        List<StringEntry> processedStrings,
        IStringsLookup overlay)
    {
        stream.Position -= major.HeaderLength;
        var majorRec = stream.GetMajorRecord();
        if (!majorRec.TryFindSubrecord("EDID", out var edidRec)) throw new ArgumentException();
        if (edidRec.Content[0] != (byte)'s') return;
        if (!majorRec.TryFindSubrecord("DATA", out var dataRec)) throw new ArgumentException();
        stream.Position += dataRec.Location;
        AStringsAlignment.ProcessStringLink(stream, processedStrings, overlay, major);
    }

    private void ProcessGameSettings(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        if (!majorFrame.TryFindSubrecord("EDID", out var edidFrame)) return;
        if ((char)edidFrame.Content[0] != 'f') return;

        if (!majorFrame.TryFindSubrecord(RecordTypes.DATA, out var dataRec)) return;
        ProcessZeroFloat(dataRec, fileOffset);
    }
}