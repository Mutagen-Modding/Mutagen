using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Fallout4.Internals;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Internals;
using Mutagen.Bethesda.Strings;
using Mutagen.Bethesda.Strings.DI;
using Noggog;
using System.Buffers.Binary;

namespace Mutagen.Bethesda.Tests;

public class Fallout4Processor : Processor
{
    public Fallout4Processor(bool multithread) : base(multithread)
    {
    }

    public override GameRelease GameRelease => GameRelease.Fallout4;

    protected override void AddDynamicProcessorInstructions()
    {
        base.AddDynamicProcessorInstructions();
        AddDynamicProcessing(RecordTypes.GMST, ProcessGameSettings);
        AddDynamicProcessing(RecordTypes.TRNS, ProcessTransforms);
        AddDynamicProcessing(RecordTypes.RACE, ProcessRaces);
        AddDynamicProcessing(RecordTypes.SCOL, ProcessStaticCollections);
        AddDynamicProcessing(RecordTypes.FURN, ProcessFurniture);
        AddDynamicProcessing(RecordTypes.WEAP, ProcessWeapons);
        AddDynamicProcessing(RecordTypes.NPC_, ProcessNpcs);
        AddDynamicProcessing(RecordTypes.BNDS, ProcessBendableSplines);
        AddDynamicProcessing(RecordTypes.REGN, ProcessRegions);
        AddDynamicProcessing(
            ProcessPlaced,
            PlacedObject_Registration.TriggeringRecordType,
            PlacedNpc_Registration.TriggeringRecordType,
            PlacedArrow_Registration.TriggeringRecordType,
            PlacedBarrier_Registration.TriggeringRecordType,
            PlacedBeam_Registration.TriggeringRecordType,
            PlacedCone_Registration.TriggeringRecordType,
            PlacedFlame_Registration.TriggeringRecordType,
            PlacedHazard_Registration.TriggeringRecordType,
            PlacedMissile_Registration.TriggeringRecordType,
            PlacedTrap_Registration.TriggeringRecordType);
        AddDynamicProcessing(RecordTypes.ACHR, ProcessPlacedNpc);
        AddDynamicProcessing(RecordTypes.REFR, ProcessPlacedObject);
        AddDynamicProcessing(RecordTypes.NAVM, ProcessNavmeshes);
        AddDynamicProcessing(RecordTypes.CELL, ProcessCells);
        AddDynamicProcessing(RecordTypes.WRLD, ProcessWorldspaces);
        AddDynamicProcessing(RecordTypes.DIAL, ProcessDialogs);
        AddDynamicProcessing(RecordTypes.INFO, ProcessDialogResponses);
        AddDynamicProcessing(RecordTypes.QUST, ProcessQuests);
        AddDynamicProcessing(RecordTypes.PACK, ProcessPackages);
        AddDynamicProcessing(RecordTypes.WATR, ProcessWater);
        AddDynamicProcessing(RecordTypes.IMGS, ProcessImageSpace);
        AddDynamicProcessing(RecordTypes.IMAD, ProcessImageSpaceAdapters);
        AddDynamicProcessing(RecordTypes.FLST, ProcessFormLists);
        AddDynamicProcessing(RecordTypes.MATT, ProcessMaterialTypes);
        AddDynamicProcessing(RecordTypes.DFOB, ProcessDefaultObjects);
        AddDynamicProcessing(RecordTypes.SMQN, ProcessStoryManagerQuestNodes);
        AddDynamicProcessing(RecordTypes.EQUP, ProcessEquipTypes);
        AddDynamicProcessing(RecordTypes.LENS, ProcessLenses);
        AddDynamicProcessing(RecordTypes.GDRY, ProcessGodRays);
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

    private void ProcessRaces(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        if (!majorFrame.TryFindSubrecord(RecordTypes.MLSI, out var mlsi)) return;

        if (majorFrame.TryFindSubrecordHeader(RecordTypes.MSID, out _))
        {
            var max = majorFrame.FindEnumerateSubrecords(RecordTypes.MSID)
                .Select(x => x.AsInt32())
                .Max(0);

            var existing = mlsi.AsInt32();
            if (existing == max) return;

            byte[] sub = new byte[4];
            BinaryPrimitives.WriteInt32LittleEndian(sub, max);
            _instructions.SetSubstitution(
                fileOffset + mlsi.Location + mlsi.HeaderLength,
                sub);
        }
        else
        {
            _instructions.SetRemove(RangeInt64.FromLength(fileOffset + mlsi.Location, mlsi.TotalLength));
            ProcessLengths(
                majorFrame,
                -mlsi.TotalLength,
                fileOffset);
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

    private void ProcessFurniture(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        if (majorFrame.TryFindSubrecord(RecordTypes.SNAM, out var frame))
        {
            int offset = 0;
            int i = 0;
            while (i * 24 < frame.ContentLength)
            {
                ProcessZeroFloats(frame, fileOffset, ref offset, 4);
                i++;
                offset = i * 24;
            }
        }
    }

    private void ProcessWeapons(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        if (majorFrame.TryFindSubrecord(RecordTypes.DNAM, out var frame))
        {
            int offset = 4;
            ProcessZeroFloats(frame, fileOffset, ref offset, 8);
            offset += 19;
            ProcessZeroFloats(frame, fileOffset, ref offset, 2);
            offset += 19;
        }
    }

    private void ProcessNpcs(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        if (majorFrame.TryFindSubrecord(RecordTypes.QNAM, out var frame))
        {
            int offset = 0;
            ProcessColorFloat(frame, fileOffset, ref offset, alpha: true);
        }
        if (majorFrame.TryFindSubrecord(RecordTypes.AIDT, out frame))
        {
            int offset = 6;
            ProcessBool(frame, fileOffset, offset, 2, 1);
            offset = 20;
            ProcessBool(frame, fileOffset, offset, 4, 1);
        }
        if (majorFrame.TryFindSubrecord(RecordTypes.TPLT, out frame))
        {
            ProcessFormIDOverflow(frame, fileOffset);
        }
        if (majorFrame.TryFindSubrecord(RecordTypes.TPTA, out frame))
        {
            ProcessFormIDOverflows(frame, fileOffset);
        }
        if (majorFrame.FormID.ID == 0x3D62A
            && majorFrame.TryFindSubrecord(RecordTypes.COCT, out frame))
        {
            var bytes = new byte[4];
            BinaryPrimitives.WriteInt32LittleEndian(bytes, 1);
            _instructions.SetSubstitution(
                fileOffset + frame.Location + frame.HeaderLength,
                bytes);
        }
    }

    private void ProcessBendableSplines(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        if (majorFrame.TryFindSubrecord(RecordTypes.DNAM, out var frame))
        {
            int offset = 8;
            ProcessColorFloat(frame, fileOffset, ref offset, alpha: true);
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
        AStringsAlignment.ProcessStringLink(stream, processedStrings, overlay);
    }

    private void ProcessRegions(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        var rdatHeader = RecordSpanExtensions.TryFindSubrecord(majorFrame.Content, majorFrame.Meta, RecordTypes.RDAT);
        if (rdatHeader == null) return;

        // Order RDATs by index
        SortedList<uint, RangeInt64> rdats = new SortedList<uint, RangeInt64>();
        List<uint> raw = new List<uint>();
        while (rdatHeader != null)
        {
            var index = BinaryPrimitives.ReadUInt32LittleEndian(rdatHeader.Value.Content);
            var nextRdat = RecordSpanExtensions.TryFindSubrecord(
                majorFrame.Content,
                majorFrame.Meta,
                RecordTypes.RDAT,
                offset: rdatHeader.Value.Location + rdatHeader.Value.TotalLength);
            rdats[index] =
                new RangeInt64(
                    fileOffset + majorFrame.HeaderLength + rdatHeader.Value.Location,
                    nextRdat == null
                        ? fileOffset + majorFrame.TotalLength - 1
                        : nextRdat.Value.Location - 1 + fileOffset + majorFrame.HeaderLength);
            raw.Add(index);
            rdatHeader = nextRdat;
        }

        if (raw.SequenceEqual(rdats.Keys)) return;
        foreach (var item in rdats.Reverse())
        {
            _instructions.SetMove(
                loc: fileOffset + majorFrame.TotalLength,
                section: item.Value);
        }
    }

    private void ProcessPlacedObject(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        if (majorFrame.TryFindSubrecord(RecordTypes.XTEL, out var xtel))
        {
            int loc = 0;
            ProcessZeroFloats(xtel, fileOffset, ref loc, 6);
        }
        if (majorFrame.TryFindSubrecord(RecordTypes.XPRM, out var xprm))
        {
            int loc = 0;
            ProcessZeroFloats(xprm, fileOffset, ref loc, 3);
            ProcessColorFloat(xprm, fileOffset, ref loc, alpha: false);
            ProcessZeroFloats(xprm, fileOffset, ref loc, 1);
        }
        if (majorFrame.TryFindSubrecord(RecordTypes.XBSD, out var xbsd)
            && xbsd.ContentLength > 20)
        {
            int loc = 20;
            ProcessBool(xbsd, fileOffset, loc, 1, 1);
        }
        if (majorFrame.TryFindSubrecord(RecordTypes.XRMR, out var xrmr)
            && xrmr.AsInt32() == 0
            && !majorFrame.TryFindSubrecord(RecordTypes.LNAM, out _)
            && !majorFrame.TryFindSubrecord(RecordTypes.INAM, out _)
            && !majorFrame.TryFindSubrecord(RecordTypes.XLRM, out _))
        {
            _instructions.SetRemove(RangeInt64.FromLength(fileOffset + xrmr.Location, xrmr.TotalLength));
            ProcessLengths(
                majorFrame,
                -xrmr.TotalLength,
                fileOffset);
        } 
        if (majorFrame.TryFindSubrecord(RecordTypes.XOWN, out var xown)
            && xown.ContentLength == 12)
        {
            int offset = 8;
            ProcessBool(xown, fileOffset, offset, 4, 1);
        }

        var removed = 0;
        var xwpgs = majorFrame.FindEnumerateSubrecords(RecordTypes.XWPG).ToArray();
        if (xwpgs.Length > 1)
        {
            var first = xwpgs[0];
            var firstAmount = first.AsUInt32();
            for (int i = 1; i < xwpgs.Length; i++)
            {
                var following = xwpgs[i];
                var amount = following.AsUInt32();
                firstAmount += amount;
                _instructions.SetRemove(RangeInt64.FromLength(fileOffset + following.Location, following.TotalLength));
                removed += following.TotalLength;
            }
            byte[] b = new byte[4];
            BinaryPrimitives.WriteUInt32LittleEndian(b.AsSpan(), firstAmount);
            _instructions.SetSubstitution(fileOffset + first.Location + first.HeaderLength, b);
        }
        ProcessLengths(
            majorFrame,
            -removed,
            fileOffset);
    }

    private void ProcessNavmeshes(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        if (majorFrame.TryFindSubrecord(RecordTypes.NVNM, out var nvnm))
        {
            var vertexCount = checked((int)BinaryPrimitives.ReadUInt32LittleEndian(nvnm.Content.Slice(16)));
            var loc = 20;
            ProcessZeroFloats(nvnm, fileOffset, ref loc, vertexCount * 3);
        }
        if (majorFrame.TryFindSubrecord(RecordTypes.MNAM, out var mnam))
        {
            int loc = 0;
            while (loc < mnam.ContentLength)
            {
                long offset = fileOffset + mnam.Location + mnam.HeaderLength + loc;
                ProcessFormIDOverflow(mnam.Content.Slice(loc, 4), ref offset);
                var count = mnam.Content.Slice(loc + 4, 2).UInt16();
                loc += 4 + 2 + count * 2;
            }
        }
    }

    private void ProcessPlacedNpc(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        if (majorFrame.TryFindSubrecord(RecordTypes.XOWN, out var xown)
            && xown.ContentLength == 12)
        {
            int offset = 8;
            ProcessBool(xown, fileOffset, offset, 4, 1);
        }
    }

    private void ProcessCells(
        IMutagenReadStream stream,
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        var formKey = FormKey.Factory(stream.MetaData.MasterReferences!, majorFrame.FormID.Raw);
        CleanEmptyCellGroups(
            stream,
            formKey,
            fileOffset,
            numSubGroups: 2);

        if (majorFrame.TryFindSubrecord(RecordTypes.XOWN, out var xown)
            && xown.ContentLength == 12)
        {
            int offset = 8;
            ProcessBool(xown, fileOffset, offset, 4, 1);
        }
    }

    private void ProcessWorldspaces(
        IMutagenReadStream stream,
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        if (majorFrame.TryFindSubrecord(RecordTypes.ONAM, out var onam))
        {
            ProcessZeroFloats(onam, fileOffset);
        }
    }

    private void ProcessDialogs(
        IMutagenReadStream stream,
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        var formKey = FormKey.Factory(stream.MetaData.MasterReferences!, majorFrame.FormID.Raw);
        CleanEmptyDialogGroups(
            stream,
            formKey,
            fileOffset);

        // Reset misnumbered counter
        if (majorFrame.TryFindSubrecord(RecordTypes.TIFC, out var tifcRec))
        {
            var count = tifcRec.AsUInt32();

            uint actualCount = 0;
            stream.Position = fileOffset + majorFrame.TotalLength;
            if (stream.TryReadGroup(out var groupFrame))
            {
                int groupPos = 0;
                while (groupPos < groupFrame.Content.Length)
                {
                    var majorMeta = stream.MetaData.Constants.MajorRecordHeader(groupFrame.Content.Slice(groupPos));
                    actualCount++;
                    groupPos += checked((int)majorMeta.TotalLength);
                }
            }

            if (actualCount != count)
            {
                byte[] b = new byte[4];
                BinaryPrimitives.WriteUInt32LittleEndian(b, actualCount);
                _instructions.SetSubstitution(
                    fileOffset + tifcRec.Location + stream.MetaData.Constants.SubConstants.HeaderLength,
                    b);
            }
        }
    }

    private void ProcessDialogResponses(
        IMutagenReadStream stream,
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        foreach (var trda in majorFrame.FindEnumerateSubrecords(RecordTypes.TRDA))
        {
            ProcessFormIDOverflow(trda, fileOffset);
        }
        foreach (var ctda in majorFrame.FindEnumerateSubrecords(RecordTypes.CTDA))
        {
            int i = BinaryPrimitives.ReadInt32LittleEndian(ctda.Content.Slice(12));
            if (i == 0x0100087C)
            {
                _instructions.SetSubstitution(fileOffset + ctda.Location + ctda.HeaderLength + 15, 0);
            }
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

        if (majorFrame.TryFindSubrecord(RecordTypes.XOWN, out var xown)
            && xown.ContentLength == 12)
        {
            int offset = 8;
            ProcessBool(xown, fileOffset, offset, 4, 1);
        }

        ProcessLengths(
            majorFrame,
            sizeChange,
            fileOffset);
    }

    private void ProcessWater(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        if (majorFrame.TryFindSubrecord(RecordTypes.DNAM, out var dnam))
        {
            int loc = 0;
            ProcessZeroFloat(dnam, fileOffset, ref loc);
            loc += 8;
            ProcessZeroFloats(dnam, fileOffset, ref loc, 6);
            loc += 4;
            ProcessZeroFloats(dnam, fileOffset, ref loc, 14);
        }
    }

    private void ProcessImageSpace(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        if (majorFrame.TryFindSubrecord(RecordTypes.TNAM, out var tnam))
        {
            int loc = 0;
            ProcessZeroFloat(tnam, fileOffset, ref loc);
            ProcessColorFloat(tnam, fileOffset, ref loc, alpha: false);
        }
        if (majorFrame.TryFindSubrecord(RecordTypes.HNAM, out var hnam))
        {
            ProcessZeroFloats(tnam, fileOffset, 9);
        }
    }

    private void ProcessImageSpaceAdapters(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        foreach (var subrecord in majorFrame)
        {
            switch (subrecord.RecordTypeInt)
            {
                case RecordTypeInts.QIAD:
                case RecordTypeInts.RIAD:
                    ProcessZeroFloats(subrecord, fileOffset, subrecord.ContentLength / 4);
                    break;
                default:
                    break;
            }
        }

        void ProcessColorFrames(SubrecordPinFrame subrecord, ref int loc)
        {
            while(loc < subrecord.ContentLength)
            {
                ProcessZeroFloat(subrecord, fileOffset, ref loc);
                ProcessColorFloat(subrecord, fileOffset, ref loc, alpha: true);
            }
        }

        foreach (var rec in majorFrame.FindEnumerateSubrecords(RecordCollection.Factory(RecordTypes.TNAM, RecordTypes.NAM3)))
        {
            int loc = 0;
            ProcessColorFrames(rec, ref loc);
        }

        if (majorFrame.TryFindSubrecord(RecordTypes.NAM3, out var nam3))
        {
            int loc = 0;
            ProcessColorFrames(nam3, ref loc);
        }

        if (majorFrame.TryFindSubrecord(RecordTypes.BIAD, out var biad))
        {
            ProcessZeroFloats(biad, fileOffset);
        }

        if (majorFrame.TryFindSubrecord(RecordTypes.DATA, out var dataRec))
        {
            int offset = 0;
            ProcessFormIDOverflows(dataRec, fileOffset, ref offset, 6);
            ProcessZeroFloats(dataRec, fileOffset, ref offset, 5);
        }
    }
    private void ProcessFormLists(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        foreach (var rec in majorFrame.FindEnumerateSubrecords(RecordTypes.LNAM))
        {
            ProcessFormIDOverflow(rec, fileOffset);
        }
    }
    private void ProcessMaterialTypes(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {

        if (majorFrame.TryFindSubrecord(RecordTypes.CNAM, out var cnam))
        {
            ProcessColorFloat(cnam, fileOffset, alpha: false);
        }
    }
    private void ProcessDefaultObjects(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        if (majorFrame.TryFindSubrecord(RecordTypes.DATA, out var data))
        {
            ProcessFormIDOverflow(data, fileOffset);
        }
    }

    private void ProcessStoryManagerQuestNodes(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        if (majorFrame.TryFindSubrecord(RecordTypes.NNAM, out var data))
        {
            ProcessFormIDOverflow(data, fileOffset);
        }
        foreach (var ctda in majorFrame.FindEnumerateSubrecords(RecordTypes.CTDA))
        {
            int i = BinaryPrimitives.ReadInt32LittleEndian(ctda.Content.Slice(12));
            if (i == 0x0100080E)
            {
                _instructions.SetSubstitution(fileOffset + ctda.Location + ctda.HeaderLength + 15, 0);
            }
        }
    }

    private void ProcessEquipTypes(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        foreach (var subRec in majorFrame.FindEnumerateSubrecords(RecordTypes.ANAM))
        {
            int i = BinaryPrimitives.ReadInt32LittleEndian(subRec.Content);
            if (i == -1)
            {
                _instructions.SetSubstitution(fileOffset + subRec.Location + subRec.HeaderLength, new byte[4]);
            }
        }
    }

    private void ProcessLenses(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        foreach (var subRec in majorFrame.FindEnumerateSubrecords(RecordTypes.LFSD))
        {
            int loc = 0;
            ProcessColorFloat(subRec, fileOffset, ref loc, alpha: false);
            ProcessZeroFloats(subRec, fileOffset, ref loc, 5);
        }
    }

    private void ProcessGodRays(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        foreach (var subRec in majorFrame.FindEnumerateSubrecords(RecordTypes.DATA))
        {
            int loc = 0;
            ProcessColorFloats(subRec, fileOffset, ref loc, alpha: false, amount: 2);
            ProcessZeroFloats(subRec, fileOffset, ref loc, 5);
            ProcessColorFloats(subRec, fileOffset, ref loc, alpha: false, amount: 1);
            ProcessZeroFloats(subRec, fileOffset, ref loc, 1);
        }
    }

    private void ProcessPackages(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        // Reorder data values
        var xnamPos =
            RecordSpanExtensions.TryFindSubrecord(majorFrame.Content, majorFrame.Meta, RecordTypes.XNAM)?.Location;
        if (xnamPos == null)
        {
            throw new ArgumentException();
        }

        if (!majorFrame.TryFindSubrecord(RecordTypes.PKCU, out var pkcuRec))
        {
            throw new ArgumentException();
        }

        var count = pkcuRec.Content.Int32();

        if (count == 0) return;

        var anamPos =
            RecordSpanExtensions.TryFindSubrecord(majorFrame.Content, majorFrame.Meta, RecordTypes.ANAM)?.Location;
        RecordType pldt = new RecordType("PLDT");
        RecordType ptda = new RecordType("PTDA");
        RecordType pdto = new RecordType("PDTO");
        RecordType tpic = new RecordType("TPIC");
        RecordType unam = new RecordType("UNAM");
        RecordType bnam = new RecordType("BNAM");
        RecordType pnam = new RecordType("PNAM");
        // Reorder data values to be in index ordering
        if (anamPos.HasValue && anamPos.Value < xnamPos.Value)
        {
            var startLoc = anamPos.Value;
            var dataValues = new List<(sbyte Index, ReadOnlyMemorySlice<byte> Data)>();
            var curLoc = startLoc;
            while (anamPos.HasValue && anamPos.Value < xnamPos.Value)
            {
                var anamRecord = majorFrame.Meta.Subrecord(majorFrame.Content.Slice(anamPos.Value));
                var recs = RecordSpanExtensions.TryFindNextSubrecords(
                    majorFrame.Content.Slice(anamPos.Value + anamRecord.TotalLength),
                    majorFrame.Meta,
                    out var _,
                    RecordTypes.ANAM,
                    RecordTypes.CNAM,
                    pldt,
                    ptda,
                    pdto,
                    tpic);
                int finalLoc;
                if (recs[0] == null)
                {
                    finalLoc = recs.NotNull().Select(x => x.Location).Max();
                }
                else if (recs[0]!.Value.Location == 0)
                {
                    dataValues.Add(
                        (-1, majorFrame.Content.Slice(anamPos.Value, anamRecord.TotalLength)));
                    curLoc = anamPos.Value + anamRecord.TotalLength;
                    anamPos = anamPos.Value + anamRecord.TotalLength;
                    continue;
                }
                else
                {
                    finalLoc = recs
                        .NotNull()
                        .Select(x => x.Location)
                        .Where(i => i < recs[0]!.Value.Location)
                        .Max();
                }

                var finalRec =
                    majorFrame.Meta.SubrecordHeader(
                        majorFrame.Content.Slice(anamPos.Value + anamRecord.TotalLength + finalLoc));
                var dataSlice = majorFrame.Content.Slice(anamPos.Value,
                    anamRecord.TotalLength + finalLoc + finalRec.TotalLength);
                if (BinaryStringUtility.ProcessWholeToZString(anamRecord.Content, MutagenEncodingProvider._1252) ==
                    "Bool"
                    && recs[1] != null)
                {
                    // Ensure bool value is 1 or 0
                    var cnam = majorFrame.Meta.Subrecord(
                        majorFrame.Content.Slice(anamPos.Value + anamRecord.TotalLength + recs[1].Value.Location));
                    if (cnam.Content.Length != 1)
                    {
                        throw new ArgumentException();
                    }

                    if (cnam.Content[0] > 1)
                    {
                        var bytes = dataSlice.ToArray();
                        int boolIndex = anamRecord.TotalLength + recs[1].Value.Location + cnam.HeaderLength;
                        bytes[boolIndex] = (byte)(bytes[boolIndex] > 0 ? 1 : 0);
                        dataSlice = bytes;
                    }
                }

                dataValues.Add((-1, dataSlice));

                curLoc = anamPos.Value + anamRecord.TotalLength + finalLoc + finalRec.TotalLength;
                anamPos = anamPos.Value + anamRecord.TotalLength + recs[0]?.Location;
            }

            var unamLocs = RecordSpanExtensions.ParseRepeatingSubrecord(
                majorFrame.Content.Slice(curLoc),
                majorFrame.Meta,
                unam,
                out var _);
            if (unamLocs == null
                || unamLocs.Count != dataValues.Count
                || unamLocs.Count != count)
            {
                throw new ArgumentException();
            }

            for (sbyte i = 0; i < unamLocs.Count; i++)
            {
                var unamRec = majorFrame.Meta.Subrecord(majorFrame.Content.Slice(curLoc + unamLocs[i].Location));
                dataValues[i] = (
                    (sbyte)unamRec.Content[0],
                    dataValues[i].Data);
            }

            var subLoc = startLoc;
            foreach (var item in dataValues.OrderBy(i => i.Index))
            {
                _instructions.SetSubstitution(
                    fileOffset + majorFrame.HeaderLength + subLoc,
                    item.Data.ToArray());
                subLoc += item.Data.Length;
            }

            foreach (var item in dataValues.OrderBy(i => i.Index))
            {
                byte[] bytes = new byte[] { 0x55, 0x4E, 0x41, 0x4D, 0x01, 0x00, 0x00 };
                bytes[6] = (byte)item.Index;
                _instructions.SetSubstitution(
                    fileOffset + majorFrame.HeaderLength + subLoc,
                    bytes.ToArray());
                subLoc += bytes.Length;
            }
        }

        // Reorder inputs
        var unamPos =
            RecordSpanExtensions.TryFindSubrecord(majorFrame.Content.Slice(xnamPos.Value), majorFrame.Meta, unam)?.Location;
        if (!unamPos.HasValue) return;
        unamPos += xnamPos.Value;
        var writeLoc = fileOffset + majorFrame.HeaderLength + unamPos.Value;
        var inputValues = new List<(sbyte Index, ReadOnlyMemorySlice<byte> Data)>();
        while (unamPos.HasValue)
        {
            var unamRecord = majorFrame.Meta.Subrecord(majorFrame.Content.Slice(unamPos.Value));
            var recs = RecordSpanExtensions.TryFindNextSubrecords(
                majorFrame.Content.Slice(unamPos.Value + unamRecord.TotalLength),
                majorFrame.Meta,
                out var _,
                unam,
                bnam,
                pnam);
            int finalLoc;
            if (recs[0] == null)
            {
                finalLoc = recs.NotNull().Select(x => x.Location).Max();
            }
            else if (recs[0].Value.Location == 0)
            {
                inputValues.Add(
                    ((sbyte)unamRecord.Content[0], majorFrame.Content.Slice(unamPos.Value, unamRecord.TotalLength)));
                unamPos = unamPos.Value + unamRecord.TotalLength;
                continue;
            }
            else
            {
                finalLoc = recs
                    .NotNull()
                    .Select(x => x.Location)
                    .Where(i => i < recs[0]!.Value.Location)
                    .Max();
            }

            var finalRec =
                majorFrame.Meta.SubrecordHeader(majorFrame.Content.Slice(unamPos.Value + unamRecord.TotalLength + finalLoc));
            inputValues.Add(
                ((sbyte)unamRecord.Content[0],
                    majorFrame.Content.Slice(unamPos.Value, unamRecord.TotalLength + finalLoc + finalRec.TotalLength)));

            unamPos = unamPos.Value + unamRecord.TotalLength + recs[0]?.Location;
        }

        foreach (var item in inputValues.OrderBy(i => i.Index))
        {
            _instructions.SetSubstitution(
                writeLoc,
                item.Data.ToArray());
            writeLoc += item.Data.Length;
        }
    }

    private void ProcessQuests(
        IMutagenReadStream stream,
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        var formKey = FormKey.Factory(stream.MetaData.MasterReferences!, majorFrame.FormID.Raw);

        if (majorFrame.TryFindSubrecord(RecordTypes.ANAM, out var anamRec))
        {
            var next = anamRec.AsUInt32();
            var targets = new RecordType[]
            {
                RecordTypes.ALST,
                RecordTypes.ALLS,
                RecordTypes.ALCS,
            };
            var locs = RecordSpanExtensions.FindAllOfSubrecords(
                majorFrame.Content,
                majorFrame.Meta,
                targets);
            uint actualNext = 0;
            if (locs.Count > 0)
            {
                actualNext = locs
                    .Select(l => l.AsUInt32())
                    .Max();
                actualNext++;
            }

            if (actualNext != next)
            {
                byte[] sub = new byte[4];
                BinaryPrimitives.WriteUInt32LittleEndian(sub, actualNext);
                _instructions.SetSubstitution(
                    fileOffset + anamRec.Location + anamRec.HeaderLength,
                    sub);
            }
        }

        CleanEmptyQuestGroups(
            stream,
            formKey,
            fileOffset);

        FixVMADFormIDs(
            majorFrame,
            fileOffset,
            out var vmad,
            out var objectFormat,
            out var processedLen);
        if (vmad != null)
        {
            fileOffset += majorFrame.Header.HeaderLength + vmad.Value.Location + vmad.Value.Header.HeaderLength;
            var stream2 = new MutagenMemoryReadStream(vmad.Value.Content, Bundle)
            {
                Position = processedLen - vmad.Value.HeaderLength
            };
            if (stream2.Complete) return;
            // skip unknown
            stream2.Position += 1;
            var fragCount = stream2.ReadUInt16();
            FixVMADScriptIDs(stream2, fileOffset, objectFormat);
            for (int i = 0; i < fragCount; i++)
            {
                stream2.Position += 9;
                // skip name
                var len = stream2.ReadUInt16();
                stream2.Position += len;
                // skip name
                len = stream2.ReadUInt16();
                stream2.Position += len;
            }

            var aliasCount = stream2.ReadUInt16();
            for (int i = 0; i < aliasCount; i++)
            {
                FixObjectPropertyIDs(stream2, fileOffset, objectFormat);
                // skip version
                stream2.Position += 2;
                objectFormat = stream2.ReadUInt16();
                var numScripts = stream2.ReadUInt16();
                for (int j = 0; j < numScripts; j++)
                {
                    FixVMADScriptIDs(stream2, fileOffset, objectFormat);
                }
            }
        }
    }

    private void FixObjectPropertyIDs(IMutagenReadStream stream, long fileOffset, ushort objectFormat)
    {
        switch (objectFormat)
        {
            case 2:
                {
                    stream.Position += 4;
                    long offset = fileOffset + stream.Position;
                    ProcessFormIDOverflow(stream.ReadSpan(4), ref offset);
                }
                break;
            case 1:
                {
                    long offset = fileOffset + stream.Position;
                    ProcessFormIDOverflow(stream.ReadSpan(4), ref offset);
                    stream.Position += 4;
                }
                break;
            default:
                throw new NotImplementedException();
        }
    }

    public void FixVMADFormIDs(
        MajorRecordFrame frame,
        long fileOffset,
        out SubrecordPinFrame? vmad,
        out ushort objectFormat,
        out int processed)
    {
        vmad = RecordSpanExtensions.TryFindSubrecord(frame.Content, Meta, RecordTypes.VMAD);
        if (vmad == null)
        {
            processed = 0;
            objectFormat = 0;
            return;
        }

        var stream = new MutagenMemoryReadStream(frame.HeaderAndContentData, Bundle)
        {
            Position = vmad.Value.Location + frame.HeaderLength
        };
        stream.Position += Meta.SubConstants.HeaderLength;
        // Skip version
        stream.Position += 2;
        objectFormat = stream.ReadUInt16();
        var scriptCt = stream.ReadUInt16();
        for (int i = 0; i < scriptCt; i++)
        {
            FixVMADScriptIDs(stream, fileOffset, objectFormat);
        }

        processed = (int)(stream.Position - vmad.Value.Location - frame.HeaderLength);
    }

    private void FixVMADScriptIDs(IMutagenReadStream stream, long fileOffset, ushort objectFormat)
    {
        // skip name
        var len = stream.ReadUInt16();
        stream.Position += len;
        if (len == 0) return;
        // Skip flags
        stream.Position += 1;
        FixScriptEntry(stream, fileOffset, objectFormat, isStruct: false);
    }

    private void FixScriptEntry(IMutagenReadStream stream, long fileOffset, ushort objectFormat, bool isStruct)
    {
        var propCount = isStruct ? stream.ReadUInt32() : stream.ReadUInt16();
        for (int propIndex = 0; propIndex < propCount; propIndex++)
        {
            // skip name
            var len = stream.ReadUInt16();
            stream.Position += len;
            var type = (ScriptProperty.Type)stream.ReadUInt8();
            // skip flags
            stream.Position += 1;
            // Going to cheat here, and use the autogenerated records
            ScriptProperty prop = type switch
            {
                ScriptProperty.Type.None => new ScriptProperty(),
                ScriptProperty.Type.Object => new ScriptObjectProperty(),
                ScriptProperty.Type.String => new ScriptStringProperty(),
                ScriptProperty.Type.Int => new ScriptIntProperty(),
                ScriptProperty.Type.Float => new ScriptFloatProperty(),
                ScriptProperty.Type.Bool => new ScriptBoolProperty(),
                ScriptProperty.Type.ArrayOfObject => new ScriptObjectListProperty(),
                ScriptProperty.Type.ArrayOfString => new ScriptStringListProperty(),
                ScriptProperty.Type.ArrayOfInt => new ScriptIntListProperty(),
                ScriptProperty.Type.ArrayOfFloat => new ScriptFloatListProperty(),
                ScriptProperty.Type.ArrayOfBool => new ScriptBoolListProperty(),
                ScriptProperty.Type.ArrayOfStruct => new ScriptStructListProperty(),
                ScriptProperty.Type.Struct => new ScriptStructProperty(),
                _ => throw new NotImplementedException(),
            };
            switch (prop)
            {
                case ScriptObjectProperty obj:
                    FixObjectPropertyIDs(stream, fileOffset, objectFormat);
                    break;
                case ScriptObjectListProperty objList:
                    var count = stream.ReadUInt32();
                    for (int i = 0; i < count; i++)
                    {
                        FixObjectPropertyIDs(stream, fileOffset, objectFormat);
                    }

                    break;
                case ScriptStructProperty structProp:
                    FixScriptEntry(stream, fileOffset, objectFormat, isStruct: true);
                    break;
                case ScriptStructListProperty structList:
                    var structListCount = stream.ReadUInt32();
                    for (int j = 0; j < structListCount; j++)
                    {
                        FixScriptEntry(stream, fileOffset, objectFormat, isStruct: true);
                    }
                    break;
                default:
                    prop.CopyInFromBinary(new MutagenFrame(stream));
                    break;
            }
        }
    }

    public void PerkStringHandler(
        IMutagenReadStream stream,
        MajorRecordHeader major,
        List<StringEntry> processedStrings,
        IStringsLookup overlay)
    {
        var majorCompletePos = stream.Position + major.ContentLength;
        long? lastepft = null;
        while (stream.Position < majorCompletePos)
        {
            var sub = stream.GetSubrecordHeader();
            switch (sub.RecordTypeInt)
            {
                case RecordTypeInts.FULL:
                case RecordTypeInts.EPF2:
                    AStringsAlignment.ProcessStringLink(stream, processedStrings, overlay);
                    break;
                case RecordTypeInts.EPFT:
                    lastepft = stream.Position;
                    break;
                case RecordTypeInts.EPFD:
                    var pos = stream.Position;
                    stream.Position = lastepft.Value;
                    var epftFrame = stream.ReadSubrecord();
                    if (epftFrame.Content[0] == (byte)APerkEntryPointEffect.ParameterType.LString)
                    {
                        stream.Position = pos;
                        AStringsAlignment.ProcessStringLink(stream, processedStrings, overlay);
                    }

                    stream.Position = pos;
                    break;
                default:
                    break;
            }

            stream.Position += sub.TotalLength;
        }
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
                    new RecordType[] { "ENCH", "FULL" },
                    new RecordType[] { "SPEL", "FULL" },
                    new RecordType[] { "MGEF", "FULL", "DNAM" },
                    new RecordType[] { "ACTI", "FULL", "ATTX" },
                    new RecordType[] { "RACE", "FULL", "TTGP", "MPPN", "FMRN" },
                    new RecordType[] { "TACT", "FULL" },
                    new RecordType[] { "ARMO", "FULL", "DESC" },
                    new RecordType[] { "BOOK", "FULL" },
                    new RecordType[] { "CMPO", "FULL" },
                    new RecordType[] { "CONT", "FULL" },
                    new RecordType[] { "DOOR", "FULL", "ONAM", "CNAM" },
                    new RecordType[] { "CLAS", "FULL" },
                    new RecordType[] { "INGR", "FULL" },
                    new RecordType[] { "LIGH", "FULL" },
                    new RecordType[] { "FACT", "FULL" },
                    new RecordType[] { "HDPT", "FULL" },
                    new RecordType[] { "MISC", "FULL" },
                    new RecordType[] { "STAT", "FULL" },
                    new RecordType[] { "SCOL", "FULL" },
                    new RecordType[] { "MSTT", "FULL" },
                    new RecordType[] { "FLOR", "FULL", "ATTX" },
                    new RecordType[] { "FURN", "FULL", "ATTX" },
                    new RecordType[] { "WEAP", "FULL" },
                    new RecordType[] { "AMMO", "FULL" },
                    new RecordType[] { "NPC_", "FULL", "ATTX" },
                    new RecordType[] { "KEYM", "FULL" },
                    new RecordType[] { "ALCH", "FULL" },
                    new RecordType[] { "NOTE", "FULL" },
                    new RecordType[] { "PROJ", "FULL" },
                    new RecordType[] { "HAZD", "FULL" },
                    new RecordType[] { "TERM", "FULL" },
                    new RecordType[] { "LVLI", "ONAM" },
                    new RecordType[] { "REGN", "RDMP" },
                    new RecordType[] { "CELL", "FULL" },
                    new RecordType[] { "REFR", "FULL" },
                    new RecordType[] { "WRLD", "FULL" },
                    new RecordType[] { "QUST", "FULL", "NNAM" },
                    new RecordType[] { "DIAL", "FULL" },
                    new RecordType[] { "WATR", "FULL" },
                    new RecordType[] { "EXPL", "FULL" },
                    new RecordType[] { "FLST", "FULL" },
                    new StringsAlignmentCustom("PERK", PerkStringHandler),
                    new RecordType[] { "BPTD", "BPTN" },
                    new RecordType[] { "AVIF", "FULL" },
                    new RecordType[] { "LCTN", "FULL" },
                    new RecordType[] { "MESG", "FULL", "NNAM", "ITXT" },
                    new RecordType[] { "SNCT", "FULL" },
                    new RecordType[] { "CLFM", "FULL" },
                    new RecordType[] { "OMOD", "FULL" },
                    new RecordType[] { "INNR", "WNAM" },
                };
            case StringsSource.DL:
                return new AStringsAlignment[]
                {
                    new RecordType[] { "RACE", "DESC" },
                    new RecordType[] { "SPEL", "DESC" },
                    new RecordType[] { "RACE", "DESC" },
                    new RecordType[] { "BOOK", "CNAM", "DESC" },
                    new RecordType[] { "WEAP", "DESC" },
                    new RecordType[] { "AMMO", "DESC" },
                    new RecordType[] { "ALCH", "DESC", "DNAM" },
                    new RecordType[] { "TERM", "WNAM", "NAM0", "ITXT", "RNAM", "UNAM", "BTXT" },
                    new RecordType[] { "QUST", "CNAM" },
                    new RecordType[] { "LSCR", "DESC" },
                    new RecordType[] { "PERK", "DESC" },
                    new RecordType[] { "AVIF", "DESC" },
                    new RecordType[] { "MESG", "DESC" },
                    new RecordType[] { "COLL", "DESC" },
                    new RecordType[] { "COBJ", "DESC" },
                    new RecordType[] { "OMOD", "DESC" },
                };
            case StringsSource.IL:
                return new AStringsAlignment[]
                {
                    new RecordType[] { "INFO", "NAM1", "RNAM" },
                };
            default:
                throw new NotImplementedException();
        }
    }

}