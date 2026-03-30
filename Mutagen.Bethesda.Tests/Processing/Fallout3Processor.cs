using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Noggog;
using Mutagen.Bethesda.Fallout3.Internals;
using Mutagen.Bethesda.Plugins.Records;
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
        AddDynamicProcessing(RecordTypes.PROJ, ProcessProjectiles);
        AddDynamicProcessing(RecordTypes.WEAP, ProcessWeapons);
        AddDynamicProcessing(RecordTypes.AMMO, ProcessAmmunition);
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
        if (majorFrame.TryFindSubrecord(RecordTypes.DEST, out var dest))
        {
            // DEST layout: Int32 Health (4), UInt8 DESTCount (1), Bool VATSTargetable (1), ByteArray Unused (2)
            ProcessBool(dest, fileOffset, 5, 1, 1);
        }
    }

    private void ProcessProjectiles(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        ProcessDestructible(majorFrame, fileOffset);
        if (majorFrame.TryFindSubrecord(RecordTypes.DATA, out var data))
        {
            // DATA layout: Flags(2), Type(2), then float/FormID fields at 4-byte intervals
            // Scan all 4-byte positions starting at offset 4 for negative-zero floats
            int loc = 4;
            ProcessZeroFloats(data, fileOffset, ref loc, (data.ContentLength - 4) / 4);
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
        foreach (var sub in majorFrame.FindEnumerateSubrecords(RecordTypes.RNAM))
        {
            var trimmed = ProcessStringTermination(sub, fileOffset);
            if (trimmed > 0)
            {
                totalTrimmed += trimmed;
                ProcessLengths(sub, -trimmed, fileOffset);
            }
        }
        foreach (var sub in majorFrame.FindEnumerateSubrecords(RecordTypes.ITXT))
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

    private void ProcessWeapons(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        if (majorFrame.IsDeleted) return;

        ProcessDestructible(majorFrame, fileOffset);
        
        if (majorFrame.TryFindSubrecord(RecordTypes.DNAM, out var dnam))
        {
            int[] floatOffsets = [4, 8, 16, 20, 28, 44, 48, 60, 64, 68, 72, 76, 80, 84, 88, 92, 96, 100, 112, 116, 124, 128, 132];
            foreach (var off in floatOffsets)
            {
                if (off + 4 > dnam.ContentLength) break;
                int loc = off;
                ProcessZeroFloat(dnam, fileOffset, ref loc);
            }
            if (dnam.ContentLength > 136)
            {
                int[] fnvFloatOffsets = [136, 152, 156, 160, 176, 180, 184, 188, 192, 196];
                foreach (var off in fnvFloatOffsets)
                {
                    if (off + 4 > dnam.ContentLength) break;
                    int loc = off;
                    ProcessZeroFloat(dnam, fileOffset, ref loc);
                }
            }
        }

        if (majorFrame.TryFindSubrecord(RecordTypes.CRDT, out var crdt))
        {
            int loc = 8;
            ProcessBool(crdt, fileOffset, ref loc, 4, 1);
        }

        // VATS: FNV only — pad 16-byte variants to 20 so all fields are present.
        // FO3 VATS (if it existed) would stay at 16 bytes since the writer only emits 16 for FO3.
        if (majorFrame.TryFindSubrecord(RecordTypes.VATS, out var vats))
        {
            if (vats.ContentLength == 16)
            {
                var padPos = fileOffset + vats.Location + Meta.SubConstants.HeaderLength + 16;
                Instructions.SetAddition(padPos, new byte[] { 0, 0, 0, 0 });
                var vatsLenPos = fileOffset + vats.Location + 4;
                Instructions.SetSubstitution(vatsLenPos, BitConverter.GetBytes((ushort)20));
                ProcessLengths(majorFrame, 4, fileOffset);
            }
        }
    }

    private void ProcessAmmunition(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        if (majorFrame.IsDeleted) return;
        ProcessDestructible(majorFrame, fileOffset);

        // DAT2: FNV only — pad 12-byte variants to 20 so all 5 fields are present.
        // Mutagen always writes all 5 fields (20 bytes); processor normalizes short variants.
        if (majorFrame.TryFindSubrecord(RecordTypes.DAT2, out var dat2))
        {
            if (dat2.ContentLength == 12)
            {
                var padPos = fileOffset + dat2.Location + Meta.SubConstants.HeaderLength + 12;
                Instructions.SetAddition(padPos, new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 });
                var dat2LenPos = fileOffset + dat2.Location + 4;
                Instructions.SetSubstitution(dat2LenPos, BitConverter.GetBytes((ushort)20));
                ProcessLengths(majorFrame, 8, fileOffset);
            }
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