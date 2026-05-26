using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Processing;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Mutagen.Bethesda.Plugins.Binary.Translations;
using Mutagen.Bethesda.Plugins.Internals;
using Mutagen.Bethesda.Plugins.Meta;
using Mutagen.Bethesda.Plugins.Records.Internals;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Binary.Processing;

public class ModGroupMergerTests
{
    // Oblivion's layout is the simplest: 20-byte record/group headers, no extra
    // form-version fields.  Tests use empty/synthetic records so no game-specific
    // subrecord parsing happens.
    private static readonly GameConstants Constants = GameConstants.Oblivion;

    // --- Writer helpers ----------------------------------------------------

    private static void WriteModHeader(MutagenWriter w)
    {
        using (HeaderExport.Record(w, new RecordType("TES4")))
        {
            WriteMajorRecordStandardFields(w, formId: 0);
        }
    }

    private static void WriteMajorRecord(MutagenWriter w, string type, uint formId, int extraContentBytes = 0)
    {
        using (HeaderExport.Record(w, new RecordType(type)))
        {
            WriteMajorRecordStandardFields(w, formId);
            if (extraContentBytes > 0)
            {
                w.Write(new byte[extraContentBytes]);
            }
        }
    }

    private static void WriteTopLevelGroup(MutagenWriter w, string containedType, Action<MutagenWriter> content, uint stamp = 0)
    {
        using (HeaderExport.Group(w, RecordTypes.GRUP))
        {
            w.Write(new RecordType(containedType).TypeInt); // ContainedRecordType label
            w.Write(0);                                     // GroupType: top-level
            w.Write(stamp);                                 // LastModified stamp
            content(w);
        }
    }

    private static void WriteSubGroup(MutagenWriter w, int label, int groupType, Action<MutagenWriter> content, uint stamp = 0)
    {
        using (HeaderExport.Group(w, RecordTypes.GRUP))
        {
            w.Write(label);      // block / sub-block number
            w.Write(groupType);  // e.g. 2 = interior cell block, 3 = sub-block
            w.Write(stamp);
            content(w);
        }
    }

    private static void WriteMajorRecordStandardFields(MutagenWriter w, uint formId)
    {
        // Oblivion 20-byte major record header has 12 bytes after type+length:
        //   flags (uint32) + formId (uint32) + versionControl (uint32)
        w.Write((uint)0);
        w.Write(formId);
        w.Write((uint)0);
    }

    // --- Test driver -------------------------------------------------------

    private static byte[] RunMerger(byte[] input)
    {
        using var output = new MemoryStream();
        ModGroupMerger.MergeGroups(
            streamCreator: () => new MutagenBinaryReadStream(
                new MemoryStream(input, writable: false),
                new ParsingMeta(Constants, ModKey.Null, masterReferences: null!),
                dispose: true),
            outputStream: output);
        return output.ToArray();
    }

    // --- Tests: baseline / no-op ------------------------------------------

    [Fact]
    public void NoDuplicates_IsByteIdentical()
    {
        var inputBytes = new MemoryStream();
        using (var w = new MutagenWriter(inputBytes, Constants, dispose: false))
        {
            WriteModHeader(w);
            WriteTopLevelGroup(w, "NPC_", g => WriteMajorRecord(g, "NPC_", formId: 0x800));
            WriteTopLevelGroup(w, "WEAP", g => WriteMajorRecord(g, "WEAP", formId: 0x801));
        }

        var output = RunMerger(inputBytes.ToArray());

        Assert.Equal(inputBytes.ToArray(), output);
    }

    [Fact]
    public void EmptyMod_JustHeader()
    {
        var inputBytes = new MemoryStream();
        using (var w = new MutagenWriter(inputBytes, Constants, dispose: false))
        {
            WriteModHeader(w);
        }

        var output = RunMerger(inputBytes.ToArray());

        Assert.Equal(inputBytes.ToArray(), output);
    }

    // --- Tests: top-level duplicate merging --------------------------------

    [Fact]
    public void TopLevelDuplicates_Merged()
    {
        Action<MutagenWriter> record1 = g => WriteMajorRecord(g, "NPC_", formId: 0x800);
        Action<MutagenWriter> record2 = g => WriteMajorRecord(g, "NPC_", formId: 0x801);
        Action<MutagenWriter> record3 = g => WriteMajorRecord(g, "NPC_", formId: 0x802);

        var inputBytes = new MemoryStream();
        using (var w = new MutagenWriter(inputBytes, Constants, dispose: false))
        {
            WriteModHeader(w);
            WriteTopLevelGroup(w, "NPC_", record1, stamp: 1);
            WriteTopLevelGroup(w, "NPC_", g => { record2(g); record3(g); }, stamp: 7);
        }

        var output = RunMerger(inputBytes.ToArray());

        // Expected: single NPC_ top-level group containing all three records,
        // using the LAST duplicate's header stamp (=7).
        var expectedBytes = new MemoryStream();
        using (var w = new MutagenWriter(expectedBytes, Constants, dispose: false))
        {
            WriteModHeader(w);
            WriteTopLevelGroup(w, "NPC_", g => { record1(g); record2(g); record3(g); }, stamp: 7);
        }

        Assert.Equal(expectedBytes.ToArray(), output);
    }

    [Fact]
    public void TopLevelDuplicates_OtherTypesUntouched()
    {
        Action<MutagenWriter> npc1 = g => WriteMajorRecord(g, "NPC_", formId: 0x800);
        Action<MutagenWriter> npc2 = g => WriteMajorRecord(g, "NPC_", formId: 0x801);
        Action<MutagenWriter> weap = g => WriteMajorRecord(g, "WEAP", formId: 0x900);

        var inputBytes = new MemoryStream();
        using (var w = new MutagenWriter(inputBytes, Constants, dispose: false))
        {
            WriteModHeader(w);
            WriteTopLevelGroup(w, "NPC_", npc1);
            WriteTopLevelGroup(w, "WEAP", weap);
            WriteTopLevelGroup(w, "NPC_", npc2);
        }

        var output = RunMerger(inputBytes.ToArray());

        // Expected: merged NPC_ is emitted at the first NPC_ position,
        // WEAP follows unchanged.
        var expectedBytes = new MemoryStream();
        using (var w = new MutagenWriter(expectedBytes, Constants, dispose: false))
        {
            WriteModHeader(w);
            WriteTopLevelGroup(w, "NPC_", g => { npc1(g); npc2(g); });
            WriteTopLevelGroup(w, "WEAP", weap);
        }

        Assert.Equal(expectedBytes.ToArray(), output);
    }

    // --- Tests: nested block / sub-block merging ---------------------------
    //
    // Layout exercised here (Oblivion cell hierarchy):
    //   Top-level CELL GRUP (type 0, label "CELL")
    //     └─ Interior Cell Block GRUP (type 2, label = block number)
    //           └─ Interior Cell Sub-Block GRUP (type 3, label = sub-block number)
    //                 └─ CELL major records (+ their own Cell Children, omitted here)

    [Fact]
    public void DuplicateInteriorCellBlocks_WithinSingleTopLevel_Merged()
    {
        // Two blocks with the same block number (0), each containing a different sub-block
        Action<MutagenWriter> subBlockA = g => WriteSubGroup(g, label: 0, groupType: 3,
            content: c => WriteMajorRecord(c, "CELL", formId: 0xA01));
        Action<MutagenWriter> subBlockB = g => WriteSubGroup(g, label: 1, groupType: 3,
            content: c => WriteMajorRecord(c, "CELL", formId: 0xA02));

        var inputBytes = new MemoryStream();
        using (var w = new MutagenWriter(inputBytes, Constants, dispose: false))
        {
            WriteModHeader(w);
            WriteTopLevelGroup(w, "CELL", g =>
            {
                WriteSubGroup(g, label: 0, groupType: 2, content: subBlockA);
                WriteSubGroup(g, label: 0, groupType: 2, content: subBlockB);
            });
        }

        var output = RunMerger(inputBytes.ToArray());

        var expectedBytes = new MemoryStream();
        using (var w = new MutagenWriter(expectedBytes, Constants, dispose: false))
        {
            WriteModHeader(w);
            WriteTopLevelGroup(w, "CELL", g =>
                WriteSubGroup(g, label: 0, groupType: 2, content: merged =>
                {
                    subBlockA(merged);
                    subBlockB(merged);
                }));
        }

        Assert.Equal(expectedBytes.ToArray(), output);
    }

    [Fact]
    public void DuplicateSubBlocksWithinSameBlock_Merged()
    {
        // Two sub-blocks with the same sub-block number (4), each with a different CELL record
        Action<MutagenWriter> cellA = g => WriteMajorRecord(g, "CELL", formId: 0xB01);
        Action<MutagenWriter> cellB = g => WriteMajorRecord(g, "CELL", formId: 0xB02);

        var inputBytes = new MemoryStream();
        using (var w = new MutagenWriter(inputBytes, Constants, dispose: false))
        {
            WriteModHeader(w);
            WriteTopLevelGroup(w, "CELL", g =>
                WriteSubGroup(g, label: 2, groupType: 2, content: block =>
                {
                    WriteSubGroup(block, label: 4, groupType: 3, content: cellA);
                    WriteSubGroup(block, label: 4, groupType: 3, content: cellB);
                }));
        }

        var output = RunMerger(inputBytes.ToArray());

        var expectedBytes = new MemoryStream();
        using (var w = new MutagenWriter(expectedBytes, Constants, dispose: false))
        {
            WriteModHeader(w);
            WriteTopLevelGroup(w, "CELL", g =>
                WriteSubGroup(g, label: 2, groupType: 2, content: block =>
                    WriteSubGroup(block, label: 4, groupType: 3, content: sub =>
                    {
                        cellA(sub);
                        cellB(sub);
                    })));
        }

        Assert.Equal(expectedBytes.ToArray(), output);
    }

    [Fact]
    public void NestedDuplicates_AtMultipleDepths_AllMerged()
    {
        // Two top-level CELL GRUPs (duplicate at depth 0),
        // each with the same block number 7 (duplicate at depth 1),
        // and each of those blocks contains the same sub-block number 3 (duplicate at depth 2).
        Action<MutagenWriter> cellA = g => WriteMajorRecord(g, "CELL", formId: 0xC01);
        Action<MutagenWriter> cellB = g => WriteMajorRecord(g, "CELL", formId: 0xC02);

        var inputBytes = new MemoryStream();
        using (var w = new MutagenWriter(inputBytes, Constants, dispose: false))
        {
            WriteModHeader(w);
            WriteTopLevelGroup(w, "CELL", g =>
                WriteSubGroup(g, label: 7, groupType: 2, content: block =>
                    WriteSubGroup(block, label: 3, groupType: 3, content: cellA)));
            WriteTopLevelGroup(w, "CELL", g =>
                WriteSubGroup(g, label: 7, groupType: 2, content: block =>
                    WriteSubGroup(block, label: 3, groupType: 3, content: cellB)));
        }

        var output = RunMerger(inputBytes.ToArray());

        // Expected collapse: 1 top-level → 1 block (7) → 1 sub-block (3) → two CELL records
        var expectedBytes = new MemoryStream();
        using (var w = new MutagenWriter(expectedBytes, Constants, dispose: false))
        {
            WriteModHeader(w);
            WriteTopLevelGroup(w, "CELL", g =>
                WriteSubGroup(g, label: 7, groupType: 2, content: block =>
                    WriteSubGroup(block, label: 3, groupType: 3, content: sub =>
                    {
                        cellA(sub);
                        cellB(sub);
                    })));
        }

        Assert.Equal(expectedBytes.ToArray(), output);
    }

    [Fact]
    public void DifferentBlockNumbers_InsideTopLevel_NotMerged()
    {
        // Two blocks with DIFFERENT block numbers — both must be preserved, in order.
        var inputBytes = new MemoryStream();
        using (var w = new MutagenWriter(inputBytes, Constants, dispose: false))
        {
            WriteModHeader(w);
            WriteTopLevelGroup(w, "CELL", g =>
            {
                WriteSubGroup(g, label: 0, groupType: 2, content: block =>
                    WriteSubGroup(block, label: 0, groupType: 3, content: c =>
                        WriteMajorRecord(c, "CELL", formId: 0xD01)));
                WriteSubGroup(g, label: 1, groupType: 2, content: block =>
                    WriteSubGroup(block, label: 0, groupType: 3, content: c =>
                        WriteMajorRecord(c, "CELL", formId: 0xD02)));
            });
        }

        var input = inputBytes.ToArray();
        var output = RunMerger(input);

        Assert.Equal(input, output);
    }

    [Fact]
    public void NestedMerge_VariableSizedRecords_BytewiseCorrect()
    {
        // Sub-blocks are merged and both parent GRUPs (block + top-level) need
        // their length fields recomputed.  Uses records of different sizes so an
        // off-by-one in length fixup would produce a mismatched byte sequence.
        Action<MutagenWriter> cellA = g => WriteMajorRecord(g, "CELL", formId: 0xE01, extraContentBytes: 4);
        Action<MutagenWriter> cellB = g => WriteMajorRecord(g, "CELL", formId: 0xE02, extraContentBytes: 8);

        var inputBytes = new MemoryStream();
        using (var w = new MutagenWriter(inputBytes, Constants, dispose: false))
        {
            WriteModHeader(w);
            WriteTopLevelGroup(w, "CELL", g =>
                WriteSubGroup(g, label: 1, groupType: 2, content: block =>
                {
                    WriteSubGroup(block, label: 5, groupType: 3, content: cellA);
                    WriteSubGroup(block, label: 5, groupType: 3, content: cellB);
                }));
        }

        var output = RunMerger(inputBytes.ToArray());

        var expectedBytes = new MemoryStream();
        using (var w = new MutagenWriter(expectedBytes, Constants, dispose: false))
        {
            WriteModHeader(w);
            WriteTopLevelGroup(w, "CELL", g =>
                WriteSubGroup(g, label: 1, groupType: 2, content: block =>
                    WriteSubGroup(block, label: 5, groupType: 3, content: sub =>
                    {
                        cellA(sub);
                        cellB(sub);
                    })));
        }

        Assert.Equal(expectedBytes.ToArray(), output);
    }

    [Fact]
    public void MixedContent_RecordsAndSubGroups_CopiedVerbatim()
    {
        // A top-level group whose children are a mix of GRUPs and records
        // (e.g. QUST in Fallout4: GRUP children interleaved with QUST records).
        // The merger must NOT attempt to recurse into such a group.
        var inputBytes = new MemoryStream();
        using (var w = new MutagenWriter(inputBytes, Constants, dispose: false))
        {
            WriteModHeader(w);
            WriteTopLevelGroup(w, "QUST", g =>
            {
                WriteSubGroup(g, label: 0x100, groupType: 10, content: c =>
                    WriteMajorRecord(c, "DIAL", formId: 0xF01));
                WriteMajorRecord(g, "QUST", formId: 0xF02);
                WriteSubGroup(g, label: 0x200, groupType: 10, content: c =>
                    WriteMajorRecord(c, "DIAL", formId: 0xF03));
            });
        }

        var output = RunMerger(inputBytes.ToArray());

        Assert.Equal(inputBytes.ToArray(), output);
    }

    [Fact]
    public void MixedContent_DuplicateTopLevel_MergedAsLeaf()
    {
        // Two duplicate top-level groups with mixed content — should concatenate
        // children without trying to merge sub-groups by key.
        Action<MutagenWriter> firstContent = g =>
        {
            WriteSubGroup(g, label: 0x100, groupType: 10, content: c =>
                WriteMajorRecord(c, "DIAL", formId: 0xF01));
            WriteMajorRecord(g, "QUST", formId: 0xF02);
        };
        Action<MutagenWriter> secondContent = g =>
        {
            WriteMajorRecord(g, "QUST", formId: 0xF03);
            WriteSubGroup(g, label: 0x200, groupType: 10, content: c =>
                WriteMajorRecord(c, "DIAL", formId: 0xF04));
        };

        var inputBytes = new MemoryStream();
        using (var w = new MutagenWriter(inputBytes, Constants, dispose: false))
        {
            WriteModHeader(w);
            WriteTopLevelGroup(w, "QUST", firstContent, stamp: 1);
            WriteTopLevelGroup(w, "QUST", secondContent, stamp: 5);
        }

        var output = RunMerger(inputBytes.ToArray());

        var expectedBytes = new MemoryStream();
        using (var w = new MutagenWriter(expectedBytes, Constants, dispose: false))
        {
            WriteModHeader(w);
            WriteTopLevelGroup(w, "QUST", g =>
            {
                firstContent(g);
                secondContent(g);
            }, stamp: 5);
        }

        Assert.Equal(expectedBytes.ToArray(), output);
    }
}
