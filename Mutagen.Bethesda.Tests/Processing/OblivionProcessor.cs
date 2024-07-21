using Mutagen.Bethesda.Oblivion;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Headers;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using RecordTypes = Mutagen.Bethesda.Oblivion.Internals.RecordTypes;
using Noggog;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Strings;
using Mutagen.Bethesda.Strings.DI;

namespace Mutagen.Bethesda.Tests;

public class OblivionProcessor : Processor
{
    public override bool StrictStrings => true;
    
    public OblivionProcessor(bool multithread) : base(multithread)
    {
    }

    public override GameRelease GameRelease => GameRelease.Oblivion;

    #region Dynamic Processing
    /*
     * Some records that seem older have an odd record order.  Rather than accommodating, dynamically mark as exceptions
     */

    protected override void AddDynamicProcessorInstructions()
    {
        base.AddDynamicProcessorInstructions();
        AddDynamicProcessing(RecordTypes.NPC_, ProcessNPC);
        AddDynamicProcessing(RecordTypes.LVLI, ProcessLeveledItemDataFields);
        AddDynamicProcessing(RecordTypes.REGN, ProcessRegions);
        AddDynamicProcessing(RecordTypes.REFR, ProcessPlacedObject);
        AddDynamicProcessing(RecordTypes.ACRE, ProcessPlacedCreature);
        AddDynamicProcessing(RecordTypes.ACHR, ProcessPlacedNPC);
        AddDynamicProcessing(RecordTypes.CELL, ProcessCells);
        AddDynamicProcessing(RecordTypes.DIAL, ProcessDialogTopics);
        AddDynamicProcessing(RecordTypes.INFO, ProcessDialogItems);
        AddDynamicProcessing(RecordTypes.IDLE, ProcessIdleAnimations);
        AddDynamicProcessing(RecordTypes.PACK, ProcessAIPackages);
        AddDynamicProcessing(RecordTypes.CSTY, ProcessCombatStyle);
        AddDynamicProcessing(RecordTypes.WATR, ProcessWater);
        AddDynamicProcessing(RecordTypes.GMST, ProcessGameSettings);
        AddDynamicProcessing(RecordTypes.BOOK, ProcessBooks);
        AddDynamicProcessing(RecordTypes.LIGH, ProcessLights);
        AddDynamicProcessing(RecordTypes.SPEL, ProcessSpell);
    }

    protected override AStringsAlignment[] GetStringsFileAlignments(StringsSource source)
    {
        return Array.Empty<AStringsAlignment>();
    }

    private void ProcessNPC(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        DynamicMove(
            majorFrame,
            fileOffset,
            offendingIndices: new RecordType[]
            {
                new("CNTO"),
                new("SCRI"),
                new("AIDT")
            },
            offendingLimits: new RecordType[]
            {
                new("ACBS")
            },
            locationsToMove: new RecordType[]
            {
                new("CNAM")
            });
    }

    private void ProcessLeveledItemDataFields(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        if (!majorFrame.TryFindSubrecord(RecordTypes.DATA, out var dataFrame)) return;

        int amount = 0;
        var dataFlag = dataFrame.AsUInt8();
        if (dataFlag == 1)
        {
            var lvld = majorFrame.FindSubrecordHeader(RecordTypes.LVLD);
            var index = lvld.EndLocation + 1;
            Instructions.SetAddition(
                loc: index + fileOffset,
                addition: new byte[]
                {
                    (byte)'L',
                    (byte)'V',
                    (byte)'L',
                    (byte)'F',
                    0x1,
                    0x0,
                    0x2
                });
            amount += 7;
        }
        else
        {
            var existingLen = dataFrame.ContentLength;
            byte[] lenData = new byte[2];
            using (var writer = new MutagenWriter(new MemoryStream(lenData), GameRelease))
            {
                writer.Write((ushort)(existingLen - 7));
            }
            Instructions.SetSubstitution(
                loc: fileOffset + Mutagen.Bethesda.Plugins.Internals.Constants.HeaderLength,
                sub: lenData);
        }

        // Remove DATA
        var dataRange = new RangeInt64(dataFrame.Location + fileOffset, dataFrame.Location + fileOffset + 7 - 1);
        Instructions.SetRemove(dataRange);
        amount -= (int)dataRange.Width;

        ProcessLengths(
            majorFrame,
            amount,
            fileOffset);
    }

    private void ProcessRegions(
        IMutagenReadStream stream,
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        if (!majorFrame.TryFindSubrecord(RecordTypes.RDAT, out var rdatFrame)) return;
        var rdatIndex = rdatFrame.Location;
        int amount = 0;
        SortedList<uint, RangeInt64> rdats = new SortedList<uint, RangeInt64>();
        bool foundNext = true;
        while (foundNext)
        {
            foundNext = majorFrame.TryFindSubrecord(RecordTypes.RDAT, offset: rdatIndex + rdatFrame.TotalLength, out var nextRdatFrame);
            var index = rdatFrame.Content.UInt32();
            rdats[index] =
                new RangeInt64(
                    rdatIndex + fileOffset,
                    foundNext ? nextRdatFrame.Location - 1 + fileOffset : fileOffset + majorFrame.TotalLength - 1);
            rdatFrame = nextRdatFrame;
            rdatIndex = nextRdatFrame.Location;
        }

        foreach (var item in rdats.Reverse())
        {
            if (item.Key == (int)RegionData.RegionDataType.Icon) continue;
            Instructions.SetMove(
                loc: fileOffset + majorFrame.TotalLength,
                section: item.Value);
        }

        if (rdats.ContainsKey((int)RegionData.RegionDataType.Icon))
        { // Need to create icon record
            if (!majorFrame.TryFindSubrecord("EDID", out var edidFrame))
            {
                throw new ArgumentException();
            }
            var locToPlace = fileOffset + edidFrame.Location + edidFrame.TotalLength;

            // Get icon string
            var iconLoc = rdats[(int)RegionData.RegionDataType.Icon];
            stream.Position = iconLoc.Min + 20;
            var iconStr = BinaryStringUtility.ToZString(stream.ReadMemory((int)(iconLoc.Max - stream.Position)), MutagenEncoding._1252);

            // Get icon bytes
            MemoryStream memStream = new MemoryStream();
            using (var writer = new MutagenWriter(memStream, GameRelease))
            {
                using (HeaderExport.Header(
                           writer,
                           RecordTypes.ICON,
                           ObjectType.Subrecord))
                {
                    StringBinaryTranslation.Instance.Write(writer, iconStr, StringBinaryType.NullTerminate);
                }
            }

            var arr = memStream.ToArray();
            Instructions.SetAddition(
                loc: locToPlace,
                addition: arr);
            Instructions.SetRemove(
                section: iconLoc);
            amount += arr.Length;
            amount -= (int)iconLoc.Width;
        }

        ProcessLengths(
            majorFrame,
            amount,
            fileOffset);
    }

    private void ProcessPlacedObject(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        int amount = 0;
        if (majorFrame.TryFindSubrecord(RecordTypes.XLOC, out var xlocFrame)
            && xlocFrame.ContentLength == 16)
        {
            ModifyLengthTracking(fileOffset, -4);
            var removeStart = fileOffset + xlocFrame.Location + xlocFrame.HeaderLength + 12;
            Instructions.SetSubstitution(
                loc: fileOffset + xlocFrame.Location + 4,
                sub: new byte[] { 12, 0 });
            Instructions.SetRemove(
                section: new RangeInt64(
                    removeStart,
                    removeStart + 3));
            amount -= 4;
        }
        if (majorFrame.TryFindSubrecord(RecordTypes.XSED, out var xsedFrame))
        {
            var len = xsedFrame.ContentLength;
            if (len == 4)
            {
                ModifyLengthTracking(fileOffset, -3);
                var removeStart = fileOffset + xsedFrame.Location + xsedFrame.HeaderLength + 1;
                Instructions.SetSubstitution(
                    loc: fileOffset + xsedFrame.Location + 4,
                    sub: new byte[] { 1, 0 });
                Instructions.SetRemove(
                    section: new RangeInt64(
                        removeStart,
                        removeStart + 2));
                amount -= 3;
            }
        }

        if (majorFrame.TryFindSubrecord(RecordTypes.DATA, out var dataRec))
        {
            ProcessZeroFloats(dataRec, fileOffset, 6);
        }

        if (majorFrame.TryFindSubrecord(RecordTypes.XTEL, out var xtelFrame))
        {
            ProcessZeroFloats(xtelFrame, fileOffset, 6);
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
        int amount = 0;
        if (majorFrame.TryFindSubrecord(RecordTypes.DATA, out var dataRec))
        {
            ProcessZeroFloats(dataRec, fileOffset, 6);
        }

        ProcessLengths(
            majorFrame,
            amount,
            fileOffset);
    }

    private void ProcessPlacedNPC(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        int amount = 0;
        if (majorFrame.TryFindSubrecord(RecordTypes.DATA, out var dataRec))
        {
            ProcessZeroFloats(dataRec, fileOffset, 6);
        }

        ProcessLengths(
            majorFrame,
            amount,
            fileOffset);
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
            numSubGroups: 3);
    }

    private void ProcessDialogTopics(
        IMutagenReadStream stream,
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        var formKey = FormKey.Factory(stream.MetaData.MasterReferences, majorFrame.FormID, reference: false);
        CleanEmptyDialogGroups(
            stream,
            formKey,
            fileOffset);
    }

    private void ProcessDialogItems(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        int amount = 0;
        foreach (var ctdt in majorFrame.FindEnumerateSubrecords(RecordTypes.CTDT))
        {
            Instructions.SetSubstitution(
                loc: ctdt.Location + fileOffset + 3,
                sub: new byte[] { (byte)'A', 0x18 });
            Instructions.SetAddition(
                addition: new byte[4],
                loc: ctdt.Location + fileOffset + 0x1A);
            amount += 4;
        }

        foreach (var schd in majorFrame.FindEnumerateSubrecords(RecordTypes.SCHD))
        {
            var existingLen = schd.ContentLength;
            var diff = existingLen - 0x14;
            Instructions.SetSubstitution(
                loc: schd.Location + fileOffset + 3,
                sub: new byte[] { (byte)'R', 0x14 });
            if (diff == 0) continue;
            var locToRemove = fileOffset + schd.Location + schd.HeaderLength + 0x14;
            Instructions.SetRemove(
                section: new RangeInt64(
                    locToRemove,
                    locToRemove + diff - 1));
            amount -= diff;
        }

        ProcessLengths(
            majorFrame,
            amount,
            fileOffset);
    }

    private void ProcessIdleAnimations(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        int amount = 0;
        foreach (var ctdt in majorFrame.FindEnumerateSubrecords(RecordTypes.CTDT))
        {
            Instructions.SetSubstitution(
                loc: ctdt.Location + fileOffset + 3,
                sub: new byte[] { (byte)'A', 0x18 });
            Instructions.SetAddition(
                addition: new byte[4],
                loc: ctdt.Location + fileOffset + 0x1A);
            amount += 4;
        }

        ProcessLengths(
            majorFrame,
            amount,
            fileOffset);
    }

    private void ProcessAIPackages(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        int amount = 0;
        foreach (var ctdt in majorFrame.FindEnumerateSubrecords(RecordTypes.CTDT))
        {
            Instructions.SetSubstitution(
                loc: ctdt.Location + fileOffset + 3,
                sub: new byte[] { (byte)'A', 0x18 });
            Instructions.SetAddition(
                addition: new byte[4],
                loc: ctdt.Location + fileOffset + 0x1A);
            amount += 4;
        }

        foreach (var ctdt in majorFrame.FindEnumerateSubrecords(RecordTypes.PKDT))
        {
            if (ctdt.ContentLength != 4) continue;
            Instructions.SetSubstitution(
                loc: fileOffset + ctdt.Location + 4,
                sub: new byte[] { 0x8 });
            var first1 = ctdt.Content[0];
            var first2 = ctdt.Content[1];
            var second1 = ctdt.Content[2];
            var second2 = ctdt.Content[3];
            Instructions.SetSubstitution(
                loc: fileOffset + ctdt.Location + 6,
                sub: new byte[] { first1, first2, 0, 0 });
            Instructions.SetAddition(
                loc: fileOffset + ctdt.Location + 10,
                addition: new byte[] { second1, 0, 0, 0 });
            amount += 4;
        }

        ProcessLengths(
            majorFrame,
            amount,
            fileOffset);
    }

    private void ProcessCombatStyle(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        if (majorFrame.TryFindSubrecordHeader(RecordTypes.CSTD, out var ctsd))
        {
            var len = ctsd.ContentLength;
            var move = 2;
            Instructions.SetSubstitution(
                sub: new byte[] { 0, 0 },
                loc: fileOffset + ctsd.EndLocation + move);
            move = 38;
            if (len < 2 + move) return;
            Instructions.SetSubstitution(
                sub: new byte[] { 0, 0 },
                loc: fileOffset + ctsd.EndLocation + move);
            move = 53;
            if (len < 3 + move) return;
            Instructions.SetSubstitution(
                sub: new byte[] { 0, 0, 0 },
                loc: fileOffset + ctsd.EndLocation + 53);
            move = 69;
            if (len < 3 + move) return;
            Instructions.SetSubstitution(
                sub: new byte[] { 0, 0, 0 },
                loc: fileOffset + ctsd.EndLocation + 69);
            move = 82;
            if (len < 2 + move) return;
            Instructions.SetSubstitution(
                sub: new byte[] { 0, 0 },
                loc: fileOffset + ctsd.EndLocation + 82);
            move = 113;
            if (len < 3 + move) return;
            Instructions.SetSubstitution(
                sub: new byte[] { 0, 0, 0 },
                loc: fileOffset + ctsd.EndLocation + 113);
        }
    }

    private void ProcessWater( 
        MajorRecordFrame majorFrame, 
        long fileOffset) 
    { 
        var amount = 0; 
        if (majorFrame.TryFindSubrecordHeader(RecordTypes.DATA, out var dataRec)) 
        { 
            var len = dataRec.ContentLength; 
            if (len == 0x02) 
            { 
                Instructions.SetSubstitution( 
                    loc: fileOffset + dataRec.Location + Mutagen.Bethesda.Plugins.Internals.Constants.HeaderLength, 
                    sub: new byte[] { 0, 0 }); 
                Instructions.SetRemove( 
                    section: RangeInt64.FromLength( 
                        loc: fileOffset + dataRec.EndLocation, 
                        length: 2)); 
                amount -= 2; 
            } 
 
            if (len == 0x56) 
            { 
                Instructions.SetSubstitution( 
                    loc: fileOffset + dataRec.Location + Mutagen.Bethesda.Plugins.Internals.Constants.HeaderLength, 
                    sub: new byte[] { 0x54, 0 }); 
                Instructions.SetRemove( 
                    section: RangeInt64.FromLength( 
                        loc: fileOffset + dataRec.EndLocation + 0x54, 
                        length: 2)); 
                amount -= 2; 
            } 
 
            if (len == 0x2A) 
            { 
                Instructions.SetSubstitution( 
                    loc: fileOffset + dataRec.Location + Mutagen.Bethesda.Plugins.Internals.Constants.HeaderLength, 
                    sub: new byte[] { 0x28, 0 }); 
                Instructions.SetRemove( 
                    section: RangeInt64.FromLength( 
                        loc: fileOffset + dataRec.EndLocation + 0x28, 
                        length: 2)); 
                amount -= 2; 
            } 
 
            if (len == 0x3E) 
            { 
                Instructions.SetSubstitution( 
                    loc: fileOffset + dataRec.Location + Mutagen.Bethesda.Plugins.Internals.Constants.HeaderLength, 
                    sub: new byte[] { 0x3C, 0 }); 
                Instructions.SetRemove( 
                    section: RangeInt64.FromLength( 
                        loc: fileOffset + dataRec.EndLocation + 0x3C, 
                        length: 2)); 
                amount -= 2; 
            } 
 
            var move = 0x39; 
            if (len >= 3 + move) 
            { 
                Instructions.SetSubstitution( 
                    sub: new byte[] { 0, 0, 0 }, 
                    loc: fileOffset + dataRec.EndLocation + move); 
            } 
        } 
 
        ProcessLengths( 
            majorFrame, 
            amount, 
            fileOffset); 
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

    private void ProcessBooks(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        if (majorFrame.TryFindSubrecord(RecordTypes.DATA, out var dataRec))
        {
            var offset = 2;
            ProcessZeroFloats(dataRec, fileOffset, ref offset, 2);
        }
    }

    private void ProcessLights(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        if (majorFrame.TryFindSubrecord(RecordTypes.DATA, out var dataRec))
        {
            var offset = 16;
            ProcessZeroFloats(dataRec, fileOffset, ref offset, 2);
            offset += 4;
            ProcessZeroFloat(dataRec, fileOffset, ref offset);
        }
    }

    private void ProcessSpell(
        MajorRecordFrame majorFrame,
        long fileOffset)
    {
        foreach (var scit in majorFrame.FindEnumerateSubrecords(RecordTypes.SCIT))
        {
            ProcessFormIDOverflow(scit, fileOffset);
        }
    }
    #endregion
}