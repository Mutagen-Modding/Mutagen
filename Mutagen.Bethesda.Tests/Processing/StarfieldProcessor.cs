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
        AddDynamicProcessing(RecordTypes.TRNS, ProcessTransforms);
        AddDynamicProcessing(RecordTypes.SCOL, ProcessStaticCollections);
        AddDynamicProcessing(RecordTypes.BNDS, ProcessBendableSplines);
    }

    protected override IEnumerable<Task> ExtraJobs(Func<IMutagenReadStream> streamGetter)
    {
        foreach (var job in base.ExtraJobs(streamGetter))
        {
            yield return job;
        }
    }

    private void ProcessStaticCollections(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        foreach (var frame in majorFrame.FindEnumerateSubrecords(RecordTypes.DATA))
        {
            int offset = 0;
            ProcessZeroFloats(frame, fileOffset, ref offset);
        }
    }

    private void ProcessBendableSplines(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        if (majorFrame.TryFindSubrecord(RecordTypes.DNAM, out var frame))
        {
            int offset = 8;
            ProcessColorFloat(frame, fileOffset, ref offset, alpha: false);
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
                    new RecordType[] { "KYWD", "FULL" },
                    new RecordType[] { "DMGT", "FULL" },
                    new RecordType[] { "CLAS", "FULL" },
                    new RecordType[] { "FACT", "FULL" },
                    new RecordType[] { "HDPT", "FULL" },
                    new RecordType[] { "RACE", "FULL" },
                    new RecordType[] { "PNDT", "FULL" },
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
        long loc,
        MajorRecordFrame major,
        List<StringEntry> processedStrings,
        IStringsLookup overlay)
    {
        if (!major.TryFindSubrecord("EDID", out var edidRec)) throw new ArgumentException();
        if (edidRec.Content[0] != (byte)'s') return;
        if (!major.TryFindSubrecord("DATA", out var dataRec)) throw new ArgumentException();
        AStringsAlignment.ProcessStringLink(loc, processedStrings, overlay, major, dataRec);
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

    private void ProcessTransforms(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        if (!majorFrame.TryFindSubrecord(RecordTypes.DATA, out var dataRec)) return;
        int offset = 0;
        ProcessZeroFloats(dataRec, fileOffset, ref offset, 9);
    }
}