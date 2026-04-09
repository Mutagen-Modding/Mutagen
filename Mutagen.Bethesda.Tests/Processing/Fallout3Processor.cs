using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
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
        // this LAND record in FalloutNV.esm seems corrupted — bad zlib header or something
        new(new RecordType("LAND"), FormKey.Factory("150FC0:FalloutNV.esm")),
    ];

    private readonly bool _isFalloutNV;

    public Fallout3Processor(IWorkDropoff workDropoff, IReadOnlyCache<IModMasterStyledGetter, ModKey> masterFlagLookup,
        GameRelease release = GameRelease.Fallout3)
        : base(workDropoff, release, masterFlagLookup)
    {
        _isFalloutNV = release == GameRelease.FalloutNV;
    }

    private static readonly HashSet<string> _knownGroups =
    [
        "GMST", "TXST", "MICN", "GLOB", "CLAS", "FACT", "HDPT", "HAIR", "EYES",
        "RACE", "SOUN", "ASPC", "MGEF", "SCPT", "LTEX", "ENCH", "SPEL", "ACTI",
        "TACT", "TERM", "ARMO", "BOOK", "CONT", "DOOR", "INGR", "LIGH", "MISC",
        "STAT", "SCOL", "MSTT", "PWAT", "GRAS", "TREE", "FURN", "WEAP", "AMMO",
        "NPC_", "CREA", "LVLC", "LVLN", "KEYM", "ALCH", "IDLM", "NOTE", "COBJ",
        "PROJ", "LVLI", "WTHR", "CLMT", "REGN", "NAVI", "CELL", "WRLD", "DIAL",
        "QUST", "IDLE", "PACK", "CSTY", "LSCR", "ANIO", "WATR", "EFSH", "EXPL",
        "DEBR", "IMGS", "IMAD", "MESG", "PERK", "BPTD", "ADDN", "AVIF", "RADS",
        "CAMS", "CPTH", "VTYP", "IPCT", "IPDS", "ARMA", "ECZN", "RGDL", "DOBJ",
        "LGTM", "MUSC", "FLST"
    ];

    protected override async Task PreProcessorJobs(Func<IMutagenReadStream> streamGetter)
    {
        await base.PreProcessorJobs(streamGetter);

        if (!_isFalloutNV) return;

        // FNV ESM has extra top-level groups FO3 doesn't know about — strip them so the processor doesn't choke
        using var stream = streamGetter();

        stream.Position = 0;
        var tes4 = stream.ReadMajorRecord();
        long pos = stream.Position;

        while (pos < stream.Length - 24)
        {
            stream.Position = pos;
            if (!stream.TryReadGroupHeader(out var groupHeader))
                break;

            var label = groupHeader.ContainedRecordType;
            if (!_knownGroups.Contains(label.Type))
            {
                Instructions.SetRemove(RangeInt64.FromLength(pos, groupHeader.TotalLength));
            }

            pos += groupHeader.TotalLength;
        }
    }

    protected override void AddDynamicProcessorInstructions()
    {
        base.AddDynamicProcessorInstructions();
        AddDynamicProcessing(RecordTypes.GMST, ProcessGameSettings);
        AddDynamicProcessing(RecordTypes.FACT, ProcessFactions);
        AddDynamicProcessing(RecordTypes.REGN, ProcessRegions);
        AddDynamicProcessing(RecordTypes.REFR, ProcessPlacedObject);
        AddDynamicProcessing(RecordTypes.ACHR, ProcessPlacedNpc);
        AddDynamicProcessing(RecordTypes.ACRE, ProcessPlacedCreature);
        AddDynamicProcessing(RecordTypes.PGRE, ProcessPlacedGrenade);
        AddDynamicProcessing(
            ProcessPlacedDataOnly,
            RecordTypes.PMIS,
            RecordTypes.PBEA);
        AddDynamicProcessing(RecordTypes.CELL, ProcessCells);
        AddDynamicProcessing(RecordTypes.WEAP, ProcessWeapons);
        AddDynamicProcessing(RecordTypes.AMMO, ProcessAmmunition);
        AddDynamicProcessing(RecordTypes.SCOL, ProcessStaticCollections);
        AddDynamicProcessing(RecordTypes.TERM, ProcessTerminals);
        AddDynamicProcessing(RecordTypes.DIAL, ProcessDialogs);
        AddDynamicProcessing(RecordTypes.INFO, ProcessDialogResponses);
        AddDynamicProcessing(RecordTypes.WRLD, ProcessWorldspaces);
        AddDynamicProcessing(RecordTypes.WATR, ProcessWater);
        AddDynamicProcessing(RecordTypes.PACK, ProcessPackages);
        AddDynamicProcessing(RecordTypes.MESG, ProcessMessages);
        AddDynamicProcessing(RecordTypes.IMAD, ProcessImageSpaceAdapters);
        AddDynamicProcessing(RecordTypes.PERK, ProcessPerks);
        AddDynamicProcessing(RecordTypes.PROJ, ProcessProjectiles);
        AddDynamicProcessing(RecordTypes.ASPC, ProcessAcousticSpaces);
        AddDynamicProcessing(RecordTypes.ANIO, ProcessAnimatedObjects);
        AddDynamicProcessing(RecordTypes.WTHR, ProcessWeather);
        AddDynamicProcessing(
            ProcessDestructible,
            RecordTypes.ACTI, RecordTypes.TACT, RecordTypes.ARMO,
            RecordTypes.BOOK, RecordTypes.CONT, RecordTypes.DOOR,
            RecordTypes.LIGH, RecordTypes.MISC, RecordTypes.MSTT,
            RecordTypes.TERM, RecordTypes.PROJ, RecordTypes.WEAP,
            RecordTypes.AMMO, RecordTypes.FURN);
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

    private void ProcessTerminals(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        if (majorFrame.IsDeleted) return;
        // empty RNAM/ITXT stored as 4 null bytes but mutagen writes 1 — accumulate total and call ProcessLengths once, otherwise each call overwrites the last
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

        // VATS seems FNV only — 16-byte variant gets padded to 20, based off oblivion's tests
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

        // DAT2 sometimes shows up as 12 bytes instead of 20 — not sure why, just pad it. stole from oblivion's tests
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

    private void ProcessRegions(
        IMutagenReadStream stream,
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        int totalAmount = 0;

        foreach (var rdsd in majorFrame.FindEnumerateSubrecords(RecordTypes.RDSD))
        {
            int loc = 0;
            while (loc + 12 <= rdsd.ContentLength)
            {
                ProcessFormIDOverflow(rdsd, fileOffset, ref loc);
                loc += 8;
            }
        }
        foreach (var rdot in majorFrame.FindEnumerateSubrecords(RecordTypes.RDOT))
        {
            int loc = 0;
            while (loc + 52 <= rdot.ContentLength)
            {
                ProcessFormIDOverflow(rdot, fileOffset, ref loc);
                loc += 48;
            }
        }
        foreach (var rdwt in majorFrame.FindEnumerateSubrecords(RecordTypes.RDWT))
        {
            int loc = 0;
            while (loc + 12 <= rdwt.ContentLength)
            {
                ProcessFormIDOverflow(rdwt, fileOffset, ref loc);
                loc += 4;
                ProcessFormIDOverflow(rdwt, fileOffset, ref loc);
            }
        }
        ProcessFormIDOverflowsForRecords(
            majorFrame,
            fileOffset,
            RecordTypes.RDMO);
        foreach (var rdsb in majorFrame.FindEnumerateSubrecords(RecordTypes.RDSB))
        {
            ProcessFormIDOverflow(rdsb, fileOffset);
        }
        if (_isFalloutNV)
        {
            ProcessFormIDOverflowsForRecords(
                majorFrame,
                fileOffset,
                RecordTypes.RDSI);
        }

        if (!majorFrame.TryFindSubrecord(RecordTypes.RDAT, out var firstRdat)) return;

        var rdatBlocks = new SortedList<uint, RangeInt64>();
        var rawOrder = new List<uint>();
        var rdatList = majorFrame.FindEnumerateSubrecords(RecordTypes.RDAT).ToList();
        for (int i = 0; i < rdatList.Count; i++)
        {
            var rdat = rdatList[i];
            if (rdat.ContentLength < 4) continue;
            var regionType = (uint)rdat.Content.Int32();
            var blockStart = fileOffset + rdat.Location;
            long blockEnd;
            if (i + 1 < rdatList.Count)
                blockEnd = fileOffset + rdatList[i + 1].Location - 1;
            else
                blockEnd = fileOffset + majorFrame.TotalLength - 1;

            rdatBlocks[regionType] = new RangeInt64(blockStart, blockEnd);
            rawOrder.Add(regionType);
        }

        var removedTypes = new HashSet<uint>();

        // some REGN records have ICON/MICO floating outside their RDAT block — mutagen puts them right after EDID, move them back and remove the originals
        if (majorFrame.TryFindSubrecord(RecordTypes.EDID, out var edidFrame)
            && majorFrame.TryFindSubrecord(RecordTypes.ICON, out var iconFrame)
            && iconFrame.Location != edidFrame.Location + edidFrame.TotalLength)
        {
            var iconEnd = iconFrame.EndLocation;
            if (majorFrame.TryFindSubrecord(RecordTypes.MICO, offset: iconEnd, out var micoFrame)
                && micoFrame.Location == iconEnd)
            {
                iconEnd = micoFrame.EndLocation;
            }

            var iconAbsStart = fileOffset + iconFrame.Location;
            var iconAbsEnd = fileOffset + iconEnd;
            stream.Position = iconAbsStart;
            var iconBytes = stream.ReadMemory((int)(iconAbsEnd - iconAbsStart)).ToArray();

            var locToPlace = fileOffset + edidFrame.Location + edidFrame.TotalLength;
            Instructions.SetAddition(locToPlace, iconBytes);
            totalAmount += iconBytes.Length;

            SubrecordPinFrame? precedingRdat = null;
            foreach (var rdat in rdatList)
            {
                if (rdat.EndLocation == iconFrame.Location)
                {
                    precedingRdat = rdat;
                    break;
                }
            }

            if (precedingRdat != null)
            {
                var iconRdatType = (uint)precedingRdat.Value.Content.Int32();
                removedTypes.Add(iconRdatType);
            }
            else
            {
                Instructions.SetRemove(new RangeInt64(iconAbsStart, iconAbsEnd - 1));
                totalAmount -= iconBytes.Length;
            }
        }

        var sortedKeys = rdatBlocks.Keys.Where(k => !removedTypes.Contains(k)).ToList();
        var rawKept = rawOrder.Where(k => !removedTypes.Contains(k)).ToList();
        bool needsSort = !rawKept.SequenceEqual(sortedKeys);

        if (needsSort || removedTypes.Count > 0)
        {
            foreach (var item in rdatBlocks.Reverse())
            {
                if (removedTypes.Contains(item.Key))
                {
                    Instructions.SetRemove(item.Value);
                    totalAmount -= (int)item.Value.Width;
                }
                else if (needsSort)
                {
                    Instructions.SetMove(
                        section: item.Value,
                        loc: fileOffset + majorFrame.TotalLength);
                }
            }
        }

        if (totalAmount != 0)
            ProcessLengths(majorFrame, totalAmount, fileOffset);
    }

    private void ProcessPlacedDataOnly(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        if (majorFrame.TryFindSubrecord(RecordTypes.DATA, out var dataRec))
        {
            ProcessZeroFloats(dataRec, fileOffset, 6);
        }
    }

    private void ProcessPlacedGrenade(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        // mutagen writes XRGD before XOWN/XESP — move them back if they ended up before it
        if (majorFrame.TryFindSubrecord(RecordTypes.XRGD, out var xrgd))
        {
            var targetLoc = xrgd.EndLocation;
            if (majorFrame.TryFindSubrecord(RecordTypes.XRGB, offset: targetLoc, out var xrgb)
                && xrgb.Location == targetLoc)
            {
                targetLoc = xrgb.EndLocation;
            }

            foreach (var recType in new[] { RecordTypes.XOWN, RecordTypes.XESP })
            {
                if (majorFrame.TryFindSubrecord(recType, out var sub)
                    && sub.Location < xrgd.Location)
                {
                    Instructions.SetMove(
                        section: new RangeInt64(
                            fileOffset + sub.Location,
                            fileOffset + sub.EndLocation - 1),
                        loc: fileOffset + targetLoc);
                }
            }
        }

        if (majorFrame.TryFindSubrecord(RecordTypes.XESP, out var xesp)
            && majorFrame.TryFindSubrecord(RecordTypes.XOWN, out var xown)
            && xesp.Location < xown.Location)
        {
            Instructions.SetMove(
                section: new RangeInt64(
                    fileOffset + xesp.Location,
                    fileOffset + xesp.EndLocation - 1),
                loc: fileOffset + xown.EndLocation);
        }

        ProcessPlacedDataOnly(majorFrame, fileOffset);
    }

    private void ProcessPlacedObject(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        // normalize XRGB float zeros first — P3Float round-trip can turn -0.0 into +0.0
        foreach (var xrgbFrame in majorFrame.FindEnumerateSubrecords(RecordTypes.XRGB))
        {
            ProcessZeroFloats(xrgbFrame, fileOffset, 3);
        }

        int amount = 0;
        if (majorFrame.TryFindSubrecord(RecordTypes.XLOC, out var xloc)
            && xloc.ContentLength == 16)
        {
            ModifyLengthTracking(fileOffset, -4);
            var removeStart = fileOffset + xloc.Location + xloc.HeaderLength + 12;
            Instructions.SetSubstitution(
                loc: fileOffset + xloc.Location + 4,
                sub: new byte[] { 12, 0 });
            Instructions.SetRemove(new RangeInt64(removeStart, removeStart + 3));
            amount -= 4;
        }
        if (majorFrame.TryFindSubrecord(RecordTypes.XSED, out var xsed)
            && xsed.ContentLength == 4)
        {
            ModifyLengthTracking(fileOffset, -3);
            var removeStart = fileOffset + xsed.Location + xsed.HeaderLength + 1;
            Instructions.SetSubstitution(
                loc: fileOffset + xsed.Location + 4,
                sub: new byte[] { 1, 0 });
            Instructions.SetRemove(new RangeInt64(removeStart, removeStart + 2));
            amount -= 3;
        }

        if (majorFrame.TryFindSubrecord(RecordTypes.XRMR, out var xrmr)
            && xrmr.AsInt32() == 0
            && !majorFrame.TryFindSubrecord(RecordTypes.XLRM, out _))
        {
            Instructions.SetRemove(RangeInt64.FromLength(fileOffset + xrmr.Location, xrmr.TotalLength));
            amount -= xrmr.TotalLength;
        }

        // FNV: mutagen always emits MMRK right after the map marker block — move it if it's wrong, insert it if it's missing
        if (_isFalloutNV)
        {
            long mmrkCanonicalPos = fileOffset + majorFrame.HeaderLength;

            if (majorFrame.TryFindSubrecord(RecordTypes.XMRK, out var xmrkSub))
            {
                var mapMarkerEnd = xmrkSub.EndLocation;
                var mapMarkerTypes = new HashSet<string> { "FNAM", "FULL", "TNAM" };
                foreach (var sub in majorFrame.EnumerateSubrecords())
                {
                    if (sub.Location <= xmrkSub.Location) continue;
                    if (sub.RecordType == RecordTypes.MMRK) continue;
                    if (mapMarkerTypes.Contains(sub.RecordType.Type))
                    {
                        mapMarkerEnd = sub.EndLocation;
                    }
                    else
                    {
                        break;
                    }
                }
                mmrkCanonicalPos = fileOffset + mapMarkerEnd;
            }
            else
            {
                // no map marker — find the last predecessor field and insert after it
                var predecessors = new[] {
                    RecordTypes.XTEL, RecordTypes.XMBO, RecordTypes.XMBP,
                    RecordTypes.XTRI, RecordTypes.XPRM, RecordTypes.XRGD,
                    RecordTypes.XEZN, RecordTypes.FULL, RecordTypes.NAME
                };
                foreach (var predType in predecessors)
                {
                    if (majorFrame.TryFindSubrecord(predType, out var predSub))
                    {
                        mmrkCanonicalPos = fileOffset + predSub.EndLocation;
                        break;
                    }
                }
            }

            if (majorFrame.TryFindSubrecord(RecordTypes.MMRK, out var mmrkSub))
            {
                var mmrkCurrentPos = fileOffset + mmrkSub.Location;
                if (mmrkCurrentPos != mmrkCanonicalPos)
                {
                    Instructions.SetMove(
                        section: new RangeInt64(mmrkCurrentPos, mmrkCurrentPos + mmrkSub.TotalLength - 1),
                        loc: mmrkCanonicalPos);
                }
            }
            else
            {
                var mmrkBytes = new byte[] { (byte)'M', (byte)'M', (byte)'R', (byte)'K', 0, 0 };
                Instructions.SetAddition(mmrkCanonicalPos, mmrkBytes);
                amount += 6;
            }
        }

        ProcessPlacedDataOnly(majorFrame, fileOffset);

        if (majorFrame.TryFindSubrecord(RecordTypes.XRGB, out var xrgbSub))
        {
            int xrgbLoc = 0;
            ProcessZeroFloats(xrgbSub, fileOffset, ref xrgbLoc, 3);
        }

        ProcessFormIDOverflowsForRecords(
            majorFrame,
            fileOffset,
            RecordTypes.XTEL,
            RecordTypes.NAME,
            RecordTypes.XOWN);

        if (majorFrame.TryFindSubrecord(RecordTypes.XTEL, out var xtel))
        {
            var loc = 4;
            ProcessZeroFloats(xtel, fileOffset, ref loc, 6);
        }

        ProcessLengths(
            majorFrame,
            amount,
            fileOffset);
    }


    private void ProcessPlacedCreature(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        if (majorFrame.TryFindSubrecord(RecordTypes.XAPD, out var xapd))
        {
            ProcessBool(xapd, fileOffset, 0, 1, 1);
        }

        ProcessPlacedDataOnly(majorFrame, fileOffset);
        ProcessFormIDOverflowsForRecords(
            majorFrame,
            fileOffset,
            RecordTypes.NAME,
            RecordTypes.XESP,
            RecordTypes.XOWN);
    }

    private void ProcessPlacedNpc(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        // mutagen writes XLCM after TNAM — move it if it wound up before XPRD
        if (majorFrame.TryFindSubrecord(RecordTypes.XLCM, out var xlcm)
            && majorFrame.TryFindSubrecord(RecordTypes.XPRD, out var xprd)
            && xlcm.Location < xprd.Location
            && majorFrame.TryFindSubrecord(RecordTypes.TNAM, out var tnam))
        {
            Instructions.SetMove(
                section: new RangeInt64(
                    fileOffset + xlcm.Location,
                    fileOffset + xlcm.EndLocation - 1),
                loc: fileOffset + tnam.EndLocation);
        }

        ProcessPlacedDataOnly(majorFrame, fileOffset);

        if (majorFrame.TryFindSubrecord(RecordTypes.XRGB, out var xrgbSub))
        {
            int xrgbLoc = 0;
            ProcessZeroFloats(xrgbSub, fileOffset, ref xrgbLoc, 3);
        }

        ProcessFormIDOverflowsForRecords(
            majorFrame,
            fileOffset,
            RecordTypes.NAME,
            RecordTypes.XESP);
    }

    private void ProcessCells(
        IMutagenReadStream stream,
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        ProcessFormIDOverflowsForRecords(
            majorFrame,
            fileOffset,
            RecordTypes.XCLR,
            RecordTypes.XOWN);

        var formKey = FormKey.Factory(stream.MetaData.MasterReferences, majorFrame.FormID, reference: false);
        CleanEmptyCellGroups(
            stream,
            formKey,
            fileOffset,
            numSubGroups: 3);
    }

    private void ProcessWorldspaces(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        if (majorFrame.IsDeleted) return;

        // mutagen always writes DNAM even when source WRLD doesn't have one, probably can't make it nullable — insert after NAM4/NAM2
        if (!majorFrame.TryFindSubrecord(RecordTypes.DNAM, out _))
        {
            long insertPos;
            if (majorFrame.TryFindSubrecord(RecordTypes.NAM4, out var nam4))
            {
                insertPos = fileOffset + nam4.EndLocation;
            }
            else if (majorFrame.TryFindSubrecord(RecordTypes.NAM2, out var nam2))
            {
                insertPos = fileOffset + nam2.EndLocation;
            }
            else
            {
                insertPos = fileOffset + majorFrame.TotalLength;
            }

            var dnamBytes = new byte[14];
            dnamBytes[0] = (byte)'D';
            dnamBytes[1] = (byte)'N';
            dnamBytes[2] = (byte)'A';
            dnamBytes[3] = (byte)'M';
            dnamBytes[4] = 8;
            dnamBytes[5] = 0;

            Instructions.SetAddition(insertPos, dnamBytes);
            ProcessLengths(majorFrame, 14, fileOffset);
        }
    }

    private void ProcessDialogResponses(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        ProcessFormIDOverflowsForRecords(
            majorFrame,
            fileOffset,
            RecordTypes.QSTI);

        // mutagen always writes 2 SCHRs but source only has 1 — nullable="True" on EndScript doesn't fix it so just insert an empty second one
        var schrs = majorFrame.FindEnumerateSubrecords(RecordTypes.SCHR).ToList();
        if (schrs.Count == 1)
        {
            var schr = schrs[0];
            var insertPos = fileOffset + schr.EndLocation;
            var emptySchr = new byte[26];
            emptySchr[0] = (byte)'S';
            emptySchr[1] = (byte)'C';
            emptySchr[2] = (byte)'H';
            emptySchr[3] = (byte)'R';
            emptySchr[4] = 20;
            emptySchr[5] = 0;
            Instructions.SetAddition(insertPos, emptySchr);
            ProcessLengths(majorFrame, 26, fileOffset);
        }
    }

    private void ProcessDialogs(
        IMutagenReadStream stream,
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        // accumulate all changes and call ProcessLengths once at the end — multiple calls overwrite each other
        long totalAmount = 0;

        ProcessFormIDOverflowsForRecords(
            majorFrame,
            fileOffset,
            RecordTypes.QSTI);

        // FNV: SetTo() overwrites QSTI when INFC/INFX splits them mid-list — only the last contiguous block survives, strip the earlier ones
        if (_isFalloutNV)
        {
            var allQsti = majorFrame.FindEnumerateSubrecords(RecordTypes.QSTI).ToList();
            if (allQsti.Count > 1)
            {
                int lastBlockStart = allQsti.Count - 1;
                for (int i = allQsti.Count - 2; i >= 0; i--)
                {
                    if (allQsti[i].EndLocation == allQsti[i + 1].Location)
                    {
                        lastBlockStart = i;
                    }
                    else
                    {
                        break;
                    }
                }

                if (lastBlockStart > 0)
                {
                    for (int i = lastBlockStart - 1; i >= 0; i--)
                    {
                        Instructions.SetRemove(RangeInt64.FromLength(
                            fileOffset + allQsti[i].Location, allQsti[i].TotalLength));
                        totalAmount -= allQsti[i].TotalLength;
                    }
                }
            }
        }

        // FNV DIAL can have multiple INFC/INFX but mutagen only keeps one of each — remove dupes, keep the last
        if (_isFalloutNV)
        {
            var infcs = majorFrame.FindEnumerateSubrecords(RecordTypes.INFC).ToList();
            if (infcs.Count > 1)
            {
                for (int i = 0; i < infcs.Count - 1; i++)
                {
                    Instructions.SetRemove(RangeInt64.FromLength(
                        fileOffset + infcs[i].Location, infcs[i].TotalLength));
                    totalAmount -= infcs[i].TotalLength;
                }
            }

            var infxs = majorFrame.FindEnumerateSubrecords(RecordTypes.INFX).ToList();
            if (infxs.Count > 1)
            {
                for (int i = 0; i < infxs.Count - 1; i++)
                {
                    Instructions.SetRemove(RangeInt64.FromLength(
                        fileOffset + infxs[i].Location, infxs[i].TotalLength));
                    totalAmount -= infxs[i].TotalLength;
                }
            }

            // move INFC/INFX to after INOA to match mutagen's write order — remove+add instead of SetMove to avoid collisions
            {
                int targetLoc = -1;
                if (majorFrame.TryFindSubrecord(RecordTypes.INOA, out var inoa))
                    targetLoc = inoa.EndLocation;
                else if (majorFrame.TryFindSubrecord(RecordTypes.INOM, out var inom))
                    targetLoc = inom.EndLocation;
                else if (majorFrame.TryFindSubrecord(RecordTypes.DATA, out var dialData))
                    targetLoc = dialData.EndLocation;

                if (targetLoc >= 0)
                {
                    var blocksToMove = new List<byte[]>();
                    int totalMoveSize = 0;

                    var infcList = majorFrame.FindEnumerateSubrecords(RecordTypes.INFC).ToList();
                    if (infcList.Count > 0)
                    {
                        var lastInfc = infcList[^1];
                        if (lastInfc.Location < targetLoc)
                        {
                            var bytes = new byte[lastInfc.TotalLength];
                            lastInfc.HeaderAndContentData.Span.CopyTo(bytes);
                            blocksToMove.Add(bytes);
                            Instructions.SetRemove(RangeInt64.FromLength(
                                fileOffset + lastInfc.Location, lastInfc.TotalLength));
                            totalMoveSize += lastInfc.TotalLength;
                        }
                    }

                    var infxList = majorFrame.FindEnumerateSubrecords(RecordTypes.INFX).ToList();
                    if (infxList.Count > 0)
                    {
                        var lastInfx = infxList[^1];
                        if (lastInfx.Location < targetLoc)
                        {
                            var bytes = new byte[lastInfx.TotalLength];
                            lastInfx.HeaderAndContentData.Span.CopyTo(bytes);
                            blocksToMove.Add(bytes);
                            Instructions.SetRemove(RangeInt64.FromLength(
                                fileOffset + lastInfx.Location, lastInfx.TotalLength));
                            totalMoveSize += lastInfx.TotalLength;
                        }
                    }

                    if (blocksToMove.Count > 0)
                    {
                        var combined = new byte[totalMoveSize];
                        int offset = 0;
                        foreach (var block in blocksToMove)
                        {
                            block.CopyTo(combined, offset);
                            offset += block.Length;
                        }
                        Instructions.SetAddition(fileOffset + targetLoc, combined);
                    }
                }
            }
        }

        if (totalAmount != 0)
        {
            ProcessLengths(majorFrame, totalAmount, fileOffset);
        }

        var formKey = FormKey.Factory(stream.MetaData.MasterReferences, majorFrame.FormID, reference: false);
        CleanEmptyDialogGroups(
            stream,
            formKey,
            fileOffset);

        stream.Position = fileOffset + majorFrame.TotalLength;
        if (stream.TryReadGroupHeader(out var topicGroup)
            && topicGroup.GroupType == stream.MetaData.Constants.GroupConstants.Topic.TopGroupType)
        {
            var topicGroupLoc = fileOffset + majorFrame.TotalLength;
            var stampLoc = topicGroupLoc + 16;
            Instructions.SetSubstitution(stampLoc, new byte[8]);
        }
    }

    private void ProcessWater(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        if (majorFrame.IsDeleted) return;

        if (majorFrame.TryFindSubrecord(RecordTypes.DATA, out var data)
            && data.ContentLength > 2)
        {
            var excess = data.ContentLength - 2;
            var removeStart = fileOffset + data.Location + data.HeaderLength + 2;
            Instructions.SetRemove(new RangeInt64(removeStart, removeStart + excess - 1));
            Instructions.SetSubstitution(
                loc: fileOffset + data.Location + 4,
                sub: BitConverter.GetBytes((ushort)2));
            ProcessLengths(majorFrame, -excess, fileOffset);
        }
    }

    private void ProcessPackages(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        if (majorFrame.IsDeleted) return;

        if (majorFrame.TryFindSubrecord(RecordTypes.PKDT, out var pkdt)
            && pkdt.ContentLength == 8)
        {
            var padPos = fileOffset + pkdt.Location + Meta.SubConstants.HeaderLength + 8;
            Instructions.SetAddition(padPos, new byte[] { 0, 0, 0, 0 });
            Instructions.SetSubstitution(
                loc: fileOffset + pkdt.Location + 4,
                sub: BitConverter.GetBytes((ushort)12));
            ProcessLengths(majorFrame, 4, fileOffset);
        }
    }

    private void ProcessMessages(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        if (majorFrame.IsDeleted) return;

        if (!majorFrame.TryFindSubrecord(RecordTypes.DNAM, out _))
        {
            var insertPos = fileOffset + majorFrame.TotalLength;
            var dnamBytes = new byte[10];
            dnamBytes[0] = (byte)'D';
            dnamBytes[1] = (byte)'N';
            dnamBytes[2] = (byte)'A';
            dnamBytes[3] = (byte)'M';
            dnamBytes[4] = 4;
            dnamBytes[5] = 0;
            Instructions.SetAddition(insertPos, dnamBytes);
            ProcessLengths(majorFrame, 10, fileOffset);
        }
    }

    private void ProcessImageSpaceAdapters(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        if (majorFrame.IsDeleted) return;

        // mutagen writes DNAM as 244 bytes — pad smaller source records up to match
        if (majorFrame.TryFindSubrecord(RecordTypes.DNAM, out var dnam)
            && dnam.ContentLength < 244)
        {
            var padding = 244 - dnam.ContentLength;
            var padPos = fileOffset + dnam.Location + Meta.SubConstants.HeaderLength + dnam.ContentLength;
            Instructions.SetAddition(padPos, new byte[padding]);
            Instructions.SetSubstitution(
                loc: fileOffset + dnam.Location + 4,
                sub: BitConverter.GetBytes((ushort)244));
            ProcessLengths(majorFrame, padding, fileOffset);
        }
    }

    private void ProcessPerks(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        if (majorFrame.IsDeleted) return;

        // mutagen writes EPF3 as 4 bytes but source sometimes has 2 — pad it, should probably fix in XML instead
        foreach (var epf3 in majorFrame.FindEnumerateSubrecords(RecordTypes.EPF3))
        {
            if (epf3.ContentLength == 2)
            {
                var padPos = fileOffset + epf3.Location + Meta.SubConstants.HeaderLength + 2;
                Instructions.SetAddition(padPos, new byte[] { 0, 0 });
                Instructions.SetSubstitution(
                    loc: fileOffset + epf3.Location + 4,
                    sub: BitConverter.GetBytes((ushort)4));
                ProcessLengths(majorFrame, 2, fileOffset);
            }
        }

        // mutagen writes DATA for every perk effect but source sometimes skips it — insert an empty one sized for the effect type
        foreach (var prke in majorFrame.FindEnumerateSubrecords(RecordTypes.PRKE))
        {
            bool hasData = majorFrame.TryFindSubrecord(RecordTypes.DATA, offset: prke.EndLocation, out var dataRec);
            if (hasData)
            {
                if (majorFrame.TryFindSubrecord(RecordTypes.PRKF, offset: prke.EndLocation, out var prkf)
                    && dataRec.Location > prkf.Location)
                {
                    hasData = false; // belongs to a later effect
                }
            }

            if (!hasData)
            {
                var type = prke.Content[0];
                int dataContentLen = type switch
                {
                    0 => 8, // Quest: FormLink(4) + Stage(1) + Unknown(3)
                    1 => 4, // Ability: FormLink(4)
                    _ => 3, // EntryPoint: EntryType(1) + Func(1) + TabCount(1)
                };
                var dataBytes = new byte[6 + dataContentLen];
                dataBytes[0] = (byte)'D';
                dataBytes[1] = (byte)'A';
                dataBytes[2] = (byte)'T';
                dataBytes[3] = (byte)'A';
                dataBytes[4] = (byte)(dataContentLen & 0xFF);
                dataBytes[5] = (byte)((dataContentLen >> 8) & 0xFF);
                Instructions.SetAddition(fileOffset + prke.EndLocation, dataBytes);
                ProcessLengths(majorFrame, dataBytes.Length, fileOffset);
            }
        }
    }

    private void ProcessProjectiles(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        if (majorFrame.IsDeleted) return;

        if (majorFrame.TryFindSubrecord(RecordTypes.DATA, out var data))
        {
            foreach (var off in new[] { 16, 20, 36, 40, 56, 60, 64 })
            {
                if (off + 4 <= data.ContentLength)
                {
                    int loc = off;
                    ProcessFormIDOverflow(data, fileOffset, ref loc);
                }
            }

            int[] floatOffsets = { 4, 8, 12, 24, 28, 32, 44, 48, 52 };
            foreach (var fOff in floatOffsets)
            {
                if (fOff + 4 <= data.ContentLength)
                {
                    int fLoc = fOff;
                    ProcessZeroFloat(data, fileOffset, ref fLoc);
                }
            }
            if (data.ContentLength >= 84)
            {
                foreach (var fOff in new[] { 68, 72, 76, 80 })
                {
                    int fLoc = fOff;
                    ProcessZeroFloat(data, fileOffset, ref fLoc);
                }
            }

            // FNV DATA is 84 bytes, FO3 is 68 — pad FNV records that came in short
            if (_isFalloutNV && data.ContentLength == 68)
            {
                var padPos = fileOffset + data.Location + Meta.SubConstants.HeaderLength + 68;
                Instructions.SetAddition(padPos, new byte[16]); // 12 (P3Float) + 4 (Float)
                Instructions.SetSubstitution(
                    loc: fileOffset + data.Location + 4,
                    sub: BitConverter.GetBytes((ushort)84));
                ProcessLengths(majorFrame, 16, fileOffset);
            }
        }
    }

    private void ProcessAcousticSpaces(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        if (majorFrame.IsDeleted) return;

        int totalRemoved = 0;
        foreach (var snam in majorFrame.FindEnumerateSubrecords(RecordTypes.SNAM).Reverse())
        {
            if (snam.ContentLength == 4 && snam.AsInt32() == 0)
            {
                Instructions.SetRemove(RangeInt64.FromLength(
                    fileOffset + snam.Location, snam.TotalLength));
                totalRemoved += snam.TotalLength;
            }
        }
        if (totalRemoved > 0)
        {
            ProcessLengths(majorFrame, -totalRemoved, fileOffset);
        }
    }

    private void ProcessAnimatedObjects(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        if (majorFrame.IsDeleted) return;

        // FNV DATA has a FormID that can overflow
        ProcessFormIDOverflowsForRecords(
            majorFrame,
            fileOffset,
            RecordTypes.DATA);
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
}
