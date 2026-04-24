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
}
