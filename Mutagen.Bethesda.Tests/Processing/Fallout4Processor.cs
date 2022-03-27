using Mutagen.Bethesda.Fallout4.Internals;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Strings;
using Noggog;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;

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
    }

    private void ProcessGameSettings(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        if (!majorFrame.TryLocateSubrecordFrame("EDID", out var edidFrame)) return;
        if ((char)edidFrame.Content[0] != 'f') return;

        if (!majorFrame.TryLocateSubrecordPinFrame(RecordTypes.DATA, out var dataRec)) return;
        ProcessZeroFloat(dataRec, fileOffset);
    }

    private void ProcessTransforms(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        if (!majorFrame.TryLocateSubrecordPinFrame(RecordTypes.DATA, out var dataRec)) return;
        int offset = 0;
        ProcessZeroFloats(dataRec, fileOffset, ref offset, 9);
    }

    private void ProcessRaces(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        if (!majorFrame.TryLocateSubrecordPinFrame(RecordTypes.MLSI, out var mlsi)) return;

        if (majorFrame.TryLocateSubrecord(RecordTypes.MSID, out _))
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
        if (majorFrame.TryLocateSubrecordPinFrame(RecordTypes.SNAM, out var frame))
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
        if (majorFrame.TryLocateSubrecordPinFrame(RecordTypes.DNAM, out var frame))
        {
            int offset = 4;
            ProcessZeroFloats(frame, fileOffset, ref offset, 8);
            offset += 19;
            ProcessZeroFloats(frame, fileOffset, ref offset, 2);
            offset += 19;
        }
    }

    public void GameSettingStringHandler(
        IMutagenReadStream stream,
        MajorRecordHeader major,
        List<StringEntry> processedStrings,
        IStringsLookup overlay)
    {
        stream.Position -= major.HeaderLength;
        var majorRec = stream.GetMajorRecordFrame();
        if (!majorRec.TryLocateSubrecordFrame("EDID", out var edidRec)) throw new ArgumentException();
        if (edidRec.Content[0] != (byte)'s') return;
        if (!majorRec.TryLocateSubrecordPinFrame("DATA", out var dataRec)) throw new ArgumentException();
        stream.Position += dataRec.Location;
        AStringsAlignment.ProcessStringLink(stream, processedStrings, overlay);
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
                    new RecordType[] { "RACE", "TTGP", "MPPN" },
                    new RecordType[] { "ACTI", "FULL", "ATTX" },
                    new RecordType[] { "TACT", "FULL" },
                    new RecordType[] { "ARMO", "FULL", "DESC" },
                    new RecordType[] { "BOOK", "FULL" },
                    new RecordType[] { "CONT", "FULL" },
                    new RecordType[] { "DOOR", "FULL" },
                    new RecordType[] { "INGR", "FULL" },
                    new RecordType[] { "LIGH", "FULL" },
                    new RecordType[] { "MISC", "FULL" },
                    new RecordType[] { "STAT", "FULL" },
                    new RecordType[] { "SCOL", "FULL" },
                    new RecordType[] { "MSTT", "FULL" },
                    new RecordType[] { "FLOR", "FULL", "ATTX" },
                    new RecordType[] { "FURN", "FULL", "ATTX" },
                    new RecordType[] { "WEAP", "FULL" },
                };
            case StringsSource.DL:
                return new AStringsAlignment[]
                {
                    new RecordType[] { "RACE", "DESC" },
                    new RecordType[] { "SPEL", "DESC" },
                    new RecordType[] { "BOOK", "CNAM", "DESC" },
                    new RecordType[] { "DOOR", "ONAM", "CNAM" },
                    new RecordType[] { "WEAP", "DESC" },
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