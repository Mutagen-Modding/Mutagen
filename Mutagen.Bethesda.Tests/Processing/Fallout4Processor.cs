using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Fallout4.Internals;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Strings;
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
            ProcessBool(frame, fileOffset, ref offset, 2, 1);
            offset = 20;
            ProcessBool(frame, fileOffset, ref offset, 4, 1);
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
            ProcessBool(xbsd, fileOffset, ref loc, 1, 1);
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
            ProcessBool(xown, fileOffset, ref offset, 4, 1);
        }
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
    }

    private void ProcessPlacedNpc(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        if (majorFrame.TryFindSubrecord(RecordTypes.XOWN, out var xown)
            && xown.ContentLength == 12)
        {
            int offset = 8;
            ProcessBool(xown, fileOffset, ref offset, 4, 1);
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
            ProcessBool(xown, fileOffset, ref offset, 4, 1);
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
            ProcessBool(xown, fileOffset, ref offset, 4, 1);
        }

        ProcessLengths(
            majorFrame,
            sizeChange,
            fileOffset);
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