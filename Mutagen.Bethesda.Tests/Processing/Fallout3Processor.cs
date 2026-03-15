using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Noggog;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Records.Internals;
using Mutagen.Bethesda.Strings;
using Noggog.WorkEngine;

namespace Mutagen.Bethesda.Tests;

public class Fallout3Processor : Processor
{
    public override bool StrictStrings => true;

    public override KeyValuePair<RecordType, FormKey>[] TrimmedRecords =>
    [
        // Corrupted compressed LAND record in FalloutNV.esm — bad zlib header
        new(new RecordType("LAND"), FormKey.Factory("150FC0:FalloutNV.esm")),
    ];

    public Fallout3Processor(IWorkDropoff workDropoff, IReadOnlyCache<IModMasterStyledGetter, ModKey> masterFlagLookup)
        : base(workDropoff, GameRelease.Fallout3, masterFlagLookup)
    {
    }

    protected override void AddDynamicProcessorInstructions()
    {
        base.AddDynamicProcessorInstructions();
        AddDynamicProcessing(RecordTypes.GMST, ProcessGameSettings);
        AddDynamicProcessing(RecordTypes.FACT, ProcessFactions);
        AddDynamicProcessing(RecordTypes.ACTI, ProcessDestructible);
        AddDynamicProcessing(RecordTypes.TACT, ProcessDestructible);
        AddDynamicProcessing(RecordTypes.ARMO, ProcessDestructible);
        AddDynamicProcessing(RecordTypes.BOOK, ProcessDestructible);
        AddDynamicProcessing(RecordTypes.CONT, ProcessDestructible);
        AddDynamicProcessing(RecordTypes.DOOR, ProcessDestructible);
        AddDynamicProcessing(RecordTypes.LIGH, ProcessDestructible);
        AddDynamicProcessing(RecordTypes.MISC, ProcessDestructible);
        AddDynamicProcessing(RecordTypes.MSTT, ProcessDestructible);
        AddDynamicProcessing(RecordTypes.TERM, ProcessDestructible);
        AddDynamicProcessing(RecordTypes.SCOL, ProcessStaticCollections);
        AddDynamicProcessing(RecordTypes.TERM, ProcessTerminals);
    }

    protected override AStringsAlignment[] GetStringsFileAlignments(StringsSource source)
    {
        return [];
    }

    private void ProcessGameSettings(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        var edidRec = majorFrame.FindSubrecord("EDID");
        if ((char)edidRec.Content[0] != 'f') return;

        if (majorFrame.TryFindSubrecordHeader(RecordTypes.DATA, out var dataRec))
        {
            var dataIndex = dataRec.EndLocation;
            ProcessZeroFloat(majorFrame, fileOffset, ref dataIndex);
        }
    }

    private void ProcessStaticCollections(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        if (majorFrame.IsDeleted) return;
        foreach (var frame in majorFrame.FindEnumerateSubrecords(RecordTypes.DATA))
        {
            int offset = 0;
            ProcessZeroFloats(frame, fileOffset, ref offset);
        }
    }

    private void ProcessDestructible(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        if (majorFrame.TryFindSubrecord(new RecordType("DEST"), out var dest))
        {
            // DEST layout: Int32 Health (4), UInt8 DESTCount (1), Bool VATSTargetable (1), ByteArray Unused (2)
            ProcessBool(dest, fileOffset, 5, 1, 1);
        }
    }

    private void ProcessTerminals(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        if (majorFrame.IsDeleted) return;
        // Trim all-null RNAM/ITXT subrecords to 1 byte (standard null terminator)
        // FO3/FNV terminal menu items with empty text store these as 00 00 00 00
        // Mutagen normalizes to a single 00 on write
        //
        // Must accumulate total and call record-level ProcessLengths once,
        // because the (MajorRecordFrame, SubrecordPinFrame, int, long) overload
        // writes original_size + amount each call, overwriting previous adjustments.
        int totalTrimmed = 0;
        foreach (var sub in majorFrame.FindEnumerateSubrecords(new RecordType("RNAM")))
        {
            var trimmed = ProcessStringTermination(sub, fileOffset);
            if (trimmed > 0)
            {
                totalTrimmed += trimmed;
                ProcessLengths(sub, -trimmed, fileOffset);
            }
        }
        foreach (var sub in majorFrame.FindEnumerateSubrecords(new RecordType("ITXT")))
        {
            var trimmed = ProcessStringTermination(sub, fileOffset);
            if (trimmed > 0)
            {
                totalTrimmed += trimmed;
                ProcessLengths(sub, -trimmed, fileOffset);
            }
        }
        if (totalTrimmed > 0)
        {
            ProcessLengths(majorFrame, -totalTrimmed, fileOffset);
        }
    }

    private void ProcessFactions(
        IMutagenReadStream stream,
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        if (majorFrame.TryFindSubrecord(RecordTypes.DATA, out var dat))
        {
            if (dat.ContentLength == 1)
            {
                Instructions.SetAddition(
                    fileOffset + dat.Location + stream.MetaData.Constants.SubConstants.HeaderLength + 1,
                    new byte[] { 0, 0, 0 });
                ProcessLengths(
                    majorFrame,
                    dat,
                    3,
                    fileOffset);
            }
        }
    }

}