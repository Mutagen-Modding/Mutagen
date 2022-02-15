using Mutagen.Bethesda.Archives;
using Mutagen.Bethesda.Fallout4.Internals;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Strings;
using Noggog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Mutagen.Bethesda.Strings.DI;

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
                    new RecordType[] { "KYWD", "FULL" }
                };
            //case StringsSource.DL:
            //    ProcessStringsFiles(
            //        modKey,
            //        dataFolder,
            //        language,
            //        StringsSource.DL,
            //        strict: true,
            //        RenumberStringsFileEntries(
            //            modKey,
            //            stream,
            //            dataFolder,
            //            language,
            //            StringsSource.DL,
            //            new RecordType[] { "SCRL", "DESC" },
            //        ));
            //    break;
            //case StringsSource.IL:
            //    ProcessStringsFiles(
            //        modKey,
            //        dataFolder,
            //        language,
            //        StringsSource.IL,
            //        strict: true,
            //        RenumberStringsFileEntries(
            //            modKey,
            //            stream,
            //            dataFolder,
            //            language,
            //            StringsSource.IL,
            //            new RecordType[] { "DIAL" },
            //            new RecordType[] { "INFO", "NAM1" }
            //        ));
            //    break;
            default:
                throw new NotImplementedException();
        }
    }

}