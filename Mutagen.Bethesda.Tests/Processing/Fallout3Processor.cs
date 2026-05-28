using System.Buffers.Binary;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Noggog;
using Mutagen.Bethesda.Fallout3;
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

    private readonly bool _isFalloutNV;

    public Fallout3Processor(IWorkDropoff workDropoff, IReadOnlyCache<IModMasterStyledGetter, ModKey> masterFlagLookup,
        GameRelease release = GameRelease.Fallout3)
        : base(workDropoff, release, masterFlagLookup)
    {
        _isFalloutNV = release == GameRelease.FalloutNV;
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
        AddDynamicProcessing(RecordTypes.WTHR, ProcessWeather);
        AddDynamicProcessing(RecordTypes.REGN, ProcessRegions);
        AddDynamicProcessing(RecordTypes.CELL, ProcessCells);
        AddDynamicProcessing(
            ProcessPlaced,
            PlacedObject_Registration.TriggeringRecordType,
            PlacedNpc_Registration.TriggeringRecordType,
            PlacedCreature_Registration.TriggeringRecordType,
            PlacedGrenade_Registration.TriggeringRecordType,
            PlacedMissile_Registration.TriggeringRecordType,
            PlacedBeam_Registration.TriggeringRecordType);
    }

    private void ProcessCells(
        IMutagenReadStream stream,
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        var formKey = FormKey.Factory(stream.MetaData.MasterReferences, majorFrame.FormID, reference: false);
        CleanEmptyCellGroups(
            stream,
            formKey,
            fileOffset,
            numSubGroups: 2);

        if (majorFrame.TryFindSubrecord(RecordTypes.XCLR, out var xclrSub))
        {
            int xclrLoc = 0;
            ProcessFormIDOverflows(xclrSub, fileOffset, ref xclrLoc);
        }
    }

    private void ProcessPlaced(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        var sizeChange = 0;

        if (majorFrame.TryFindSubrecord(RecordTypes.DATA, out var dataRec))
        {
            ProcessZeroFloats(dataRec, fileOffset, 6);
        }

        if (majorFrame.TryFindSubrecord(RecordTypes.XTEL, out var xtelRec))
        {
            var offset = 4;
            ProcessZeroFloats(xtelRec, fileOffset, ref offset, 6);
        }

        if (majorFrame.TryFindSubrecord(RecordTypes.XPRM, out var xprmRec))
        {
            int offset = 0;
            ProcessZeroFloats(xprmRec, fileOffset, ref offset, 3);
            ProcessColorFloat(xprmRec, fileOffset, ref offset, alpha: false);
            ProcessZeroFloat(xprmRec, fileOffset, ref offset);
        }

        if (majorFrame.TryFindSubrecord(RecordTypes.XMBO, out var xmboRec))
        {
            ProcessZeroFloats(xmboRec, fileOffset, 3);
        }

        if (majorFrame.TryFindSubrecord(RecordTypes.XRGB, out var xrgbRec))
        {
            ProcessZeroFloats(xrgbRec, fileOffset, 3);
        }

        if (majorFrame.TryFindSubrecord(RecordTypes.XRGD, out var xrgdRec))
        {
            int loc = 0;
            while (loc < xrgdRec.ContentLength)
            {
                loc += 4;
                ProcessZeroFloats(xrgdRec, fileOffset, ref loc, 6);
            }
        }

        if (majorFrame.TryFindSubrecord(RecordTypes.XSCL, out var xsclRec))
        {
            ProcessZeroFloat(xsclRec, fileOffset);
        }

        if (majorFrame.TryFindSubrecord(RecordTypes.XPRD, out var xprdRec))
        {
            ProcessZeroFloat(xprdRec, fileOffset);
        }

        if (majorFrame.TryFindSubrecord(RecordTypes.XRDS, out var xrdsRec))
        {
            ProcessZeroFloat(xrdsRec, fileOffset);
        }

        if (majorFrame.TryFindSubrecord(RecordTypes.XHLP, out var xhlpRec))
        {
            ProcessZeroFloat(xhlpRec, fileOffset);
        }

        if (majorFrame.TryFindSubrecord(RecordTypes.XRAD, out var xradRec))
        {
            ProcessZeroFloat(xradRec, fileOffset);
        }

        if (majorFrame.TryFindSubrecord(RecordTypes.XCHG, out var xchgRec))
        {
            ProcessZeroFloat(xchgRec, fileOffset);
        }

        if (majorFrame.TryFindSubrecord(RecordTypes.XCLW, out var xclwRec))
        {
            ProcessZeroFloat(xclwRec, fileOffset);
        }

        if (majorFrame.TryFindSubrecord(RecordTypes.XAPD, out var xapdRec))
        {
            ProcessBool(xapdRec, fileOffset, loc: 0, length: 1, importantBytes: 1);
        }

        // Normalize FormIDs whose master index is out of range (e.g. GRA has stray master 2 refs
        // in a file that declares only one master; Mutagen clamps them to _numMasters on write).
        foreach (var formIdSub in new[]
                 {
                     RecordTypes.NAME, RecordTypes.XEZN, RecordTypes.XTRG, RecordTypes.XOWN,
                     RecordTypes.XAMT, RecordTypes.XLKR, RecordTypes.XEMI, RecordTypes.XMBR,
                     RecordTypes.INAM, RecordTypes.TNAM, RecordTypes.LTMP, RecordTypes.XCCM,
                     RecordTypes.XCIM, RecordTypes.XCWT, RecordTypes.CNAM, RecordTypes.WMI1,
                     RecordTypes.XCAS, RecordTypes.XCMO, RecordTypes.XMRC, RecordTypes.SCRO,
                 })
        {
            if (majorFrame.TryFindSubrecord(formIdSub, out var sub)
                && sub.ContentLength == 4)
            {
                ProcessFormIDOverflow(sub, fileOffset);
            }
        }

        // Mutagen skips XRMR (BoundData) entirely when LinkedRoomsCount and Unknown are both zero.
        // Mirror that by removing a fully-zero XRMR subrecord from the reference.
        if (majorFrame.TryFindSubrecord(RecordTypes.XRMR, out var xrmrRec)
            && xrmrRec.ContentLength == 4
            && xrmrRec.AsInt32() == 0)
        {
            Instructions.SetRemove(
                RangeInt64.FromLength(
                    fileOffset + xrmrRec.Location,
                    xrmrRec.TotalLength));
            sizeChange -= (int)xrmrRec.TotalLength;
        }

        ProcessLengths(
            majorFrame,
            sizeChange,
            fileOffset);
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

    private void ProcessWeather(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        if (majorFrame.IsDeleted) return;
        // FNV records can store PNAM/NAM0 in the older FO3-shape (4 colors per layer/type, 16
        // bytes each) instead of the FNV-shape (6 colors, 24 bytes each — adds HighNoon and
        // Midnight). Mutagen always emits the FNV-shape on FNV writes (with zeros for the
        // missing trailing slots), so we transform the reference's FO3-shape into FNV-shape.
        // The transformation isn't a trailing append — the 8 missing bytes (HighNoon+Midnight)
        // must be inserted AFTER each slot's existing 16 bytes, interleaved.
        // FO3 records intrinsically use the 4-color form and need no padding.
        if (!_isFalloutNV) return;

        long totalAdded = 0;

        if (majorFrame.TryFindSubrecord(RecordTypes.PNAM, out var pnam))
        {
            if (pnam.ContentLength == 64)
            {
                // 4 cloud layers, each 16 bytes; insert 8 zeros after each layer.
                var contentStart = fileOffset + pnam.Location + Meta.SubConstants.HeaderLength;
                for (int layer = 0; layer < 4; layer++)
                {
                    Instructions.SetAddition(contentStart + (layer + 1) * 16, new byte[8]);
                }
                ProcessLengths(pnam, 32, fileOffset);
                totalAdded += 32;
            }
        }

        if (majorFrame.TryFindSubrecord(RecordTypes.NAM0, out var nam0))
        {
            if (nam0.ContentLength == 160)
            {
                // 10 typed color slots, each 16 bytes; insert 8 zeros after each slot.
                var contentStart = fileOffset + nam0.Location + Meta.SubConstants.HeaderLength;
                for (int slot = 0; slot < 10; slot++)
                {
                    Instructions.SetAddition(contentStart + (slot + 1) * 16, new byte[8]);
                }
                ProcessLengths(nam0, 80, fileOffset);
                totalAdded += 80;
            }
        }

        if (totalAdded > 0)
        {
            ProcessLengths(majorFrame, totalAdded, fileOffset);
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

    private void ProcessRegions(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        if (majorFrame.IsDeleted) return;

        foreach (var pin in majorFrame.FindEnumerateSubrecords(RecordTypes.RDSI))
        {
            ProcessFormIDOverflow(pin, fileOffset);
        }
        foreach (var pin in majorFrame.FindEnumerateSubrecords(RecordTypes.RDSD))
        {
            int loc = 0;
            while (loc + 12 <= pin.ContentLength)
            {
                ProcessFormIDOverflow(pin, fileOffset, ref loc);
                loc += 8; // skip Flags + Chance
            }
        }

        var rdat = RecordSpanExtensions.TryFindSubrecord(majorFrame.Content, majorFrame.Meta, RecordTypes.RDAT);
        if (rdat == null) return;

        SortedList<uint, RangeInt64> rdats = new();
        List<uint> raw = new();
        while (rdat != null)
        {
            var index = BinaryPrimitives.ReadUInt32LittleEndian(rdat.Value.Content);
            var nextRdat = RecordSpanExtensions.TryFindSubrecord(
                majorFrame.Content,
                majorFrame.Meta,
                RecordTypes.RDAT,
                offset: rdat.Value.EndLocation);
            rdats[index] =
                new RangeInt64(
                    fileOffset + majorFrame.HeaderLength + rdat.Value.Location,
                    nextRdat == null
                        ? fileOffset + majorFrame.TotalLength - 1
                        : nextRdat.Value.Location - 1 + fileOffset + majorFrame.HeaderLength);
            raw.Add(index);
            rdat = nextRdat;
        }

        if (raw.SequenceEqual(rdats.Keys)) return;
        foreach (var item in rdats.Reverse())
        {
            Instructions.SetMove(
                loc: fileOffset + majorFrame.TotalLength,
                section: item.Value);
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
