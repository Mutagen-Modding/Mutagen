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